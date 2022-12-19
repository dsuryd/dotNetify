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
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using MemoryPack;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;

namespace DotNetify.WebApi
{
   public interface IWebApiResponseManager : IDotNetifyHubResponseManager
   { }

   [MemoryPackable]
   public partial class ConnectionGroup
   {
      public string Name { get; set; }
      public HashSet<string> ConnectionIds { get; set; } = new HashSet<string>();
   }

   public class WebApiResponseManager : IWebApiResponseManager
   {
      private readonly IHttpClientFactory _httpClientFactory;
      private readonly IDistributedCache _cache;

      private const string GROUP_KEY_PREFIX = "$dotnetifyGroup__";
      private const string ACTIVE_GROUP = "$dotNetifyActiveGroup";

      private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
      {
         PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
         Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
      };

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="httpClientFactory">Http client factory for making callback to the websocket server that handles client connections.</param>
      public WebApiResponseManager(IHttpClientFactory httpClientFactory, IDistributedCache cache)
      {
         _httpClientFactory = httpClientFactory;
         _cache = cache;
      }

      public async Task AddToGroupAsync(string connectionId, string groupName)
      {
         if (string.IsNullOrWhiteSpace(connectionId) || string.IsNullOrWhiteSpace(groupName))
            return;

         var group = await LoadGroupAsync(groupName);
         if (!group.ConnectionIds.Contains(connectionId))
         {
            group.ConnectionIds.Add(connectionId);
            await SaveGroupAsync(group);
         }
      }

      public void CreateInstance(HubCallerContext context)
      {
         _ = AddToGroupAsync(context.ConnectionId, ACTIVE_GROUP);
      }

      public HubCallerContext GetCallerContext(string connectionId) => null;

      public async Task RemoveFromGroupAsync(string connectionId, string groupName)
      {
         if (string.IsNullOrWhiteSpace(connectionId) || string.IsNullOrWhiteSpace(groupName))
            return;

         var group = await LoadGroupAsync(groupName);
         if (group.ConnectionIds.Contains(connectionId))
         {
            group.ConnectionIds.Remove(connectionId);
            await SaveGroupAsync(group);
         }
      }

      public void RemoveInstance(string connectionId)
      {
         _ = RemoveFromGroupAsync(connectionId, ACTIVE_GROUP);
      }

      public async Task SendAsync(string connectionId, string vmId, string vmData)
      {
         var response = new DotNetifyWebApi.IntegrationResponse { VMId = vmId, Data = vmData };

         var httpClient = _httpClientFactory.CreateClient(nameof(DotNetifyWebApi));
         if (httpClient != null)
         {
            var result = await httpClient.PostAsync($"{connectionId}", new StringContent(JsonSerializer.Serialize(response, _jsonSerializerOptions), Encoding.UTF8, "application/json"));
            if (!result.IsSuccessStatusCode)
               RemoveInstance(connectionId);
         }
         else
            throw new Exception("Missing HttpClient. Include 'services.AddDotNetifyHttpClient()' in the startup.");
      }

      public Task SendToGroupAsync(string groupName, string vmId, string vmData)
      {
         return SendToGroupExceptAsync(groupName, new List<string>(), vmId, vmData);
      }

      public async Task SendToGroupExceptAsync(string groupName, IReadOnlyList<string> excludedIds, string vmId, string vmData)
      {
         var active = await LoadGroupAsync(ACTIVE_GROUP);

         var group = await LoadGroupAsync(groupName);
         var activeConnectionIds = group.ConnectionIds.Where(id => active.ConnectionIds.Contains(id)).ToHashSet();
         if (activeConnectionIds.Count != group.ConnectionIds.Count)
         {
            group.ConnectionIds = activeConnectionIds;
            await SaveGroupAsync(group);
         }

         foreach (var connectionId in group.ConnectionIds.Except(excludedIds))
            _ = SendAsync(connectionId, vmId, vmData);
      }

      public async Task SendToManyAsync(IReadOnlyList<string> connectionIds, string vmId, string vmData)
      {
         var active = await LoadGroupAsync(ACTIVE_GROUP);

         foreach (var connectionId in connectionIds.Where(id => active.ConnectionIds.Contains(id)))
            _ = SendAsync(connectionId, vmId, vmData);
      }

      public Task SendToUsersAsync(IReadOnlyList<string> userIds, string vmId, string vmData)
      {
         throw new NotImplementedException();
      }

      private async Task<ConnectionGroup> LoadGroupAsync(string groupName)
      {
         var bytes = await _cache.GetAsync(GROUP_KEY_PREFIX + groupName);
         return bytes != null ? MemoryPackSerializer.Deserialize<ConnectionGroup>(bytes) : new ConnectionGroup { Name = groupName };
      }

      private Task SaveGroupAsync(ConnectionGroup group) => _cache.SetAsync(GROUP_KEY_PREFIX + group.Name, MemoryPackSerializer.Serialize(group));
   }
}