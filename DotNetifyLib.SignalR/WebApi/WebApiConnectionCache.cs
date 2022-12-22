/*
Copyright 2019-2023 Dicky Suryadi

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
using System.Threading.Tasks;
using MemoryPack;
using Microsoft.Extensions.Caching.Distributed;

namespace DotNetify.WebApi
{
   /// <summary>
   /// Persists Web API connection info.
   /// </summary>
   public interface IWebApiConnectionCache
   {
      TimeSpan? CacheExpiration { get; set; }

      Task AddVMAsync(string connectionId, string vmId);

      Task RemoveVMAsync(string connectionId, string vmId);

      Task RefreshAsync(string connectionId);

      Task RemoveAsync(string connectionId);

      Task<List<Connection>> GetConnections();

      Task<ConnectionGroup> GetGroupAsync(string groupName);

      Task SaveGroupAsync(ConnectionGroup group);
   }

   [MemoryPackable]
   public partial class Connection
   {
      public string Id { get; set; }
      public List<string> VMIds { get; set; } = new List<string>();
   }

   [MemoryPackable]
   public partial class ConnectionGroup
   {
      public string Name { get; set; }
      public HashSet<string> ConnectionIds { get; set; } = new HashSet<string>();
   }

   /// <summary>
   /// Persists Web API connection info on a distributed cache.
   /// </summary>
   public class WebApiConnectionCache : IWebApiConnectionCache
   {
      private readonly IDistributedCache _cache;

      private const string CONNECTION_KEY_PREFIX = "$dotnetifyConn__";
      private const string GROUP_KEY_PREFIX = "$dotnetifyGroup__";
      internal const string ACTIVE_GROUP = "$active";

      public TimeSpan? CacheExpiration { get; set; } = TimeSpan.FromHours(2);

      public WebApiConnectionCache(IDistributedCache cache)
      {
         _cache = cache;
      }

      public async Task AddVMAsync(string connectionId, string vmId)
      {
         var bytes = await _cache.GetAsync(CONNECTION_KEY_PREFIX + connectionId);
         var connection = bytes != null ? MemoryPackSerializer.Deserialize<Connection>(bytes) : new Connection { Id = connectionId };
         if (!connection.VMIds.Contains(vmId))
         {
            connection.VMIds.Add(vmId);
            await _cache.SetAsync(CONNECTION_KEY_PREFIX + connectionId, MemoryPackSerializer.Serialize(connection));
         }
      }

      public async Task RemoveVMAsync(string connectionId, string vmId)
      {
         var bytes = await _cache.GetAsync(CONNECTION_KEY_PREFIX + connectionId);
         var connection = bytes != null ? MemoryPackSerializer.Deserialize<Connection>(bytes) : null;
         if (connection?.VMIds.Contains(vmId) == true)
         {
            connection.VMIds.Remove(vmId);
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
         return bytes != null ? MemoryPackSerializer.Deserialize<ConnectionGroup>(bytes) : new ConnectionGroup { Name = groupName };
      }

      public Task SaveGroupAsync(ConnectionGroup group)
      {
         return _cache.SetAsync(GROUP_KEY_PREFIX + group.Name, MemoryPackSerializer.Serialize(group), new DistributedCacheEntryOptions
         {
            SlidingExpiration = CacheExpiration
         });
      }

      public async Task<List<Connection>> GetConnections()
      {
         var connections = new List<Connection>();

         var active = await GetGroupAsync(ACTIVE_GROUP);
         foreach (var connectionId in active.ConnectionIds)
         {
            var bytes = await _cache.GetAsync(CONNECTION_KEY_PREFIX + connectionId);
            var connection = bytes != null ? MemoryPackSerializer.Deserialize<Connection>(bytes) : null;
            if (connection != null)
               connections.Add(connection);
         }

         return connections;
      }
   }
}