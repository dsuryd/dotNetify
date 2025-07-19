/*
Copyright 2023 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemoryPack;
using Microsoft.Extensions.Caching.Distributed;

namespace DotNetify.WebApi
{
   [MemoryPackable]
   public partial class CachedConnection
   {
      public string Id { get; set; }
      public List<CachedConnectionVMInfo> VMInfo { get; set; } = new List<CachedConnectionVMInfo>();
   }

   [MemoryPackable]
   public partial class CachedConnectionGroup
   {
      public string Name { get; set; }
      public HashSet<string> ConnectionIds { get; set; } = new HashSet<string>();
   }

   [MemoryPackable]
   public partial class CachedConnectionVMInfo
   {
      public string VMId { get; set; }
      public string VMArgs { get; set; }
   }

   /// <summary>
   /// Persists Web API connection info on a distributed cache.
   /// </summary>
   public class ResilientWebApiConnectionCache : IWebApiConnectionCache
   {
      private readonly IDistributedCache _cache;

      private const string CONNECTION_KEY_PREFIX = "$dotnetifyConn__";
      private const string GROUP_KEY_PREFIX = "$dotnetifyGroup__";
      internal const string ACTIVE_GROUP = "$active";

      public TimeSpan? CacheExpiration { get; set; } = TimeSpan.FromHours(2);

      public ResilientWebApiConnectionCache(IDistributedCache cache)
      {
         _cache = cache;
      }

      public async Task AddVMAsync(string connectionId, string vmId, string vmArgs)
      {
         var bytes = await _cache.GetAsync(CONNECTION_KEY_PREFIX + connectionId);
         var connection = bytes != null ? MemoryPackSerializer.Deserialize<CachedConnection>(bytes) : new CachedConnection { Id = connectionId };
         if (!connection.VMInfo.Any(x => x.VMId == vmId))
         {
            connection.VMInfo.Add(new CachedConnectionVMInfo { VMId = vmId, VMArgs = vmArgs });
            await _cache.SetAsync(CONNECTION_KEY_PREFIX + connectionId, MemoryPackSerializer.Serialize(connection), new DistributedCacheEntryOptions
            {
               SlidingExpiration = CacheExpiration
            });
         }
      }

      public async Task RemoveVMAsync(string connectionId, string vmId)
      {
         var bytes = await _cache.GetAsync(CONNECTION_KEY_PREFIX + connectionId);
         var connection = bytes != null ? MemoryPackSerializer.Deserialize<CachedConnection>(bytes) : null;
         if (connection?.VMInfo.Any(x => x.VMId == vmId) == true)
         {
            connection.VMInfo = connection.VMInfo.Where(x => x.VMId != vmId).ToList();
            await _cache.SetAsync(CONNECTION_KEY_PREFIX + connectionId, MemoryPackSerializer.Serialize(connection), new DistributedCacheEntryOptions
            {
               SlidingExpiration = CacheExpiration
            });
         }
      }

      public Task RefreshAsync(string connectionId)
      {
         return _cache.GetAsync(CONNECTION_KEY_PREFIX + connectionId);
      }

      public Task RemoveAsync(string connectionId)
      {
         return _cache.RemoveAsync(CONNECTION_KEY_PREFIX + connectionId);
      }

      public async Task<ConnectionGroup> GetGroupAsync(string groupName)
      {
         var bytes = await _cache.GetAsync(GROUP_KEY_PREFIX + groupName);
         var cachedGroup = bytes != null ? MemoryPackSerializer.Deserialize<CachedConnectionGroup>(bytes) : new CachedConnectionGroup { Name = groupName };
         return new ConnectionGroup
         {
            ConnectionIds = cachedGroup.ConnectionIds.ToList(),
            Name = cachedGroup.Name
         };
      }

      public Task SaveGroupAsync(ConnectionGroup group)
      {
         var cachedGroup = new CachedConnectionGroup
         {
            Name = group.Name,
            ConnectionIds = group.ConnectionIds.ToHashSet()
         };
         return _cache.SetAsync(GROUP_KEY_PREFIX + group.Name, MemoryPackSerializer.Serialize(cachedGroup), new DistributedCacheEntryOptions
         {
            SlidingExpiration = CacheExpiration
         });
      }

      public async Task<List<Connection>> GetConnectionsAsync()
      {
         var connections = new List<Connection>();

         var active = await GetGroupAsync(ACTIVE_GROUP);
         foreach (var connectionId in active.ConnectionIds)
         {
            var bytes = await _cache.GetAsync(CONNECTION_KEY_PREFIX + connectionId);
            var connection = bytes != null ? MemoryPackSerializer.Deserialize<CachedConnection>(bytes) : null;
            if (connection != null)
               connections.Add(new Connection
               {
                  Id = connection.Id,
                  VMInfo = connection.VMInfo.Select(x => new ConnectionVMInfo { VMId = x.VMId, VMArgs = x.VMArgs }).ToList()
               });
         }

         return connections;
      }
   }
}