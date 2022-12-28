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
using System.Linq;
using System.Threading.Tasks;

namespace DotNetify.WebApi
{
   /// <summary>
   /// Persists Web API connection info.
   /// </summary>
   public interface IWebApiConnectionCache
   {
      TimeSpan? CacheExpiration { get; set; }

      Task AddVMAsync(string connectionId, string vmId, string vmArgs);

      Task RemoveVMAsync(string connectionId, string vmId);

      Task RefreshAsync(string connectionId);

      Task RemoveAsync(string connectionId);

      Task<List<Connection>> GetConnectionsAsync();

      Task<ConnectionGroup> GetGroupAsync(string groupName);

      Task SaveGroupAsync(ConnectionGroup group);
   }

   public class Connection
   {
      public string Id { get; set; }
      public List<ConnectionVMInfo> VMInfo { get; set; } = new List<ConnectionVMInfo>();
   }

   public class ConnectionGroup
   {
      public string Name { get; set; }
      public HashSet<string> ConnectionIds { get; set; } = new HashSet<string>();
   }

   public class ConnectionVMInfo
   {
      public string VMId { get; set; }
      public string VMArgs { get; set; }
   }

   /// <summary>
   /// Stores Web API connection info.
   /// </summary>
   public class WebApiConnectionCache : IWebApiConnectionCache
   {
      private readonly IMemoryCache _cache;

      protected const string CONNECTION_KEY_PREFIX = "$dotnetifyConn__";
      protected const string GROUP_KEY_PREFIX = "$dotnetifyGroup__";
      internal const string ACTIVE_GROUP = "$active";

      public TimeSpan? CacheExpiration { get; set; } = TimeSpan.FromHours(2);

      public WebApiConnectionCache(IMemoryCache cache)
      {
         _cache = cache;
      }

      public Task AddVMAsync(string connectionId, string vmId, string vmArgs)
      {
         var connection = (_cache.Get(CONNECTION_KEY_PREFIX + connectionId) as Connection) ?? new Connection { Id = connectionId };
         if (!connection.VMInfo.Any(x => x.VMId == vmId))
         {
            connection.VMInfo.Add(new ConnectionVMInfo { VMId = vmId, VMArgs = vmArgs });
            _cache.Set(CONNECTION_KEY_PREFIX + connectionId, connection);
         }
         return Task.CompletedTask;
      }

      public Task RemoveVMAsync(string connectionId, string vmId)
      {
         var connection = _cache.Get(CONNECTION_KEY_PREFIX + connectionId) as Connection;
         if (connection?.VMInfo.Any(x => x.VMId == vmId) == true)
         {
            connection.VMInfo = connection.VMInfo.Where(x => x.VMId != vmId).ToList();
            _cache.Set(CONNECTION_KEY_PREFIX + connectionId, connection);
         }
         return Task.CompletedTask;
      }

      public Task RefreshAsync(string connectionId)
      {
         _cache.Get(CONNECTION_KEY_PREFIX + connectionId);
         return Task.CompletedTask;
      }

      public Task RemoveAsync(string connectionId)
      {
         _cache.Remove(CONNECTION_KEY_PREFIX + connectionId);
         return Task.CompletedTask;
      }

      public Task<ConnectionGroup> GetGroupAsync(string groupName)
      {
         var group = _cache.Get(GROUP_KEY_PREFIX + groupName) as ConnectionGroup;
         return Task.FromResult(group ?? new ConnectionGroup { Name = groupName });
      }

      public Task SaveGroupAsync(ConnectionGroup group)
      {
         _cache.Set(GROUP_KEY_PREFIX + group.Name, group);
         return Task.CompletedTask;
      }

      public async Task<List<Connection>> GetConnectionsAsync()
      {
         var connections = new List<Connection>();

         var active = await GetGroupAsync(ACTIVE_GROUP);
         foreach (var connectionId in active.ConnectionIds)
         {
            var connection = _cache.Get(CONNECTION_KEY_PREFIX + connectionId) as Connection;
            if (connection != null)
               connections.Add(connection);
         }

         return connections;
      }
   }
}