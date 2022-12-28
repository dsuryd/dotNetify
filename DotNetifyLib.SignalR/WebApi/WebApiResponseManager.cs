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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DotNetify.WebApi
{
   public interface IWebApiResponseManager : IDotNetifyHubResponseManager
   {
      event EventHandler<WebApiResponseManagerSendEventArgs> Sending;
   }

   public class WebApiResponseManagerSendEventArgs : EventArgs
   {
      public List<string> ConnectionIds { get; set; }
      public string VMId { get; set; }
      public string VMData { get; set; }
      public bool Handled { get; set; }
   }

   /// <summary>
   /// This class manages sending responses to the web socket server that has forwarded view model requests/updates from its clients
   /// to this server through web API. Responses may be sent to a particular connection, or broadcasted to a group of connections.
   /// </summary>
   public class WebApiResponseManager : IWebApiResponseManager
   {
      private readonly IHttpClientFactory _httpClientFactory;
      private readonly IWebApiConnectionCache _cache;

      private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
      {
         PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
         Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
      };

      /// <summary>
      /// Maximum number of parallel HTTP requests when broadcasting a response.
      /// </summary>
      public static int MaxParallelHttpRequests { get; set; } = 1000;

      /// <summary>
      /// Invoked before sending or broadcasting a response to clients.
      /// </summary>
      public event EventHandler<WebApiResponseManagerSendEventArgs> Sending;

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="httpClientFactory">Http client factory for making callback to the websocket server that handles client connections.</param>
      /// <param name="cache">Cache to persist active connection groups.</param>
      public WebApiResponseManager(IHttpClientFactory httpClientFactory, IWebApiConnectionCache cache)
      {
         _httpClientFactory = httpClientFactory;
         _cache = cache;
      }

      /// <summary>
      /// Add a connection to a group.
      /// </summary>
      /// <param name="connectionId">WebSocket connection.</param>
      /// <param name="groupName">Group name.</param>
      public async Task AddToGroupAsync(string connectionId, string groupName)
      {
         if (string.IsNullOrWhiteSpace(connectionId) || string.IsNullOrWhiteSpace(groupName))
            return;

         var group = await _cache.GetGroupAsync(groupName);
         if (!group.ConnectionIds.Contains(connectionId))
         {
            group.ConnectionIds.Add(connectionId);
            await _cache.SaveGroupAsync(group);
         }
      }

      /// <summary>
      /// This method is called on new connection.
      /// </summary>
      /// <param name="context">Connection context.</param>
      public void CreateInstance(HubCallerContext context)
      {
         _ = AddToGroupAsync(context.ConnectionId, WebApiConnectionCache.ACTIVE_GROUP);
      }

      /// <summary>
      /// This method is not applicable in the Web API context.
      /// </summary>
      public HubCallerContext GetCallerContext(string connectionId) => null;

      /// <summary>
      /// Removes a connection from a group.
      /// </summary>
      /// <param name="connectionId">WebSocket connection.</param>
      /// <param name="groupName">Group name.</param>
      public async Task RemoveFromGroupAsync(string connectionId, string groupName)
      {
         if (string.IsNullOrWhiteSpace(connectionId) || string.IsNullOrWhiteSpace(groupName))
            return;

         var group = await _cache.GetGroupAsync(groupName);
         if (group.ConnectionIds.Contains(connectionId))
         {
            group.ConnectionIds.Remove(connectionId);
            await _cache.SaveGroupAsync(group);
         }
      }

      /// <summary>
      /// This method is called on disconnection.
      /// </summary>
      /// <param name="connectionId">WebSocket connection.</param>
      public void RemoveInstance(string connectionId)
      {
         _ = RemoveFromGroupAsync(connectionId, WebApiConnectionCache.ACTIVE_GROUP);
      }

      /// <summary>
      /// Sends a view model response to the connection client via the WebSocket servers' callback HTTP URL.
      /// </summary>
      /// <param name="connectionId">WebSocket connection.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public async Task SendAsync(string connectionId, string vmId, string vmData)
      {
         await SendConnectionsAsync(new string[] { connectionId }, vmId, vmData);
      }

      /// <summary>
      /// Sends a view model response to a group of connection clients via the WebSocket servers' callback HTTP URL.
      /// </summary>
      /// <param name="groupName">Group name.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public Task SendToGroupAsync(string groupName, string vmId, string vmData)
      {
         return SendToGroupExceptAsync(groupName, new List<string>(), vmId, vmData);
      }

      /// <summary>
      /// Sends a view model response to a group of connection clients except certain connections via the WebSocket servers' callback HTTP URL.
      /// </summary>
      /// <param name="groupName">Group name.</param>
      /// <param name="excludedConnectionIds">Excluded SignalR connections.
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public async Task SendToGroupExceptAsync(string groupName, IReadOnlyList<string> excludedIds, string vmId, string vmData)
      {
         var active = await _cache.GetGroupAsync(WebApiConnectionCache.ACTIVE_GROUP);

         var group = await _cache.GetGroupAsync(groupName);
         var activeConnectionIds = group.ConnectionIds.Where(id => active.ConnectionIds.Contains(id)).ToHashSet();
         if (activeConnectionIds.Count != group.ConnectionIds.Count)
         {
            group.ConnectionIds = activeConnectionIds;
            await _cache.SaveGroupAsync(group);
         }

         await SendConnectionsAsync(group.ConnectionIds.Except(excludedIds), vmId, vmData);
      }

      /// <summary>
      /// Sends a view model response to a list of connections.
      /// </summary>
      /// <param name="connectionIds">List of WebSocket connections.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public async Task SendToManyAsync(IReadOnlyList<string> connectionIds, string vmId, string vmData)
      {
         var active = await _cache.GetGroupAsync(WebApiConnectionCache.ACTIVE_GROUP);

         await SendConnectionsAsync(connectionIds.Where(id => active.ConnectionIds.Contains(id)), vmId, vmData);
      }

      /// <summary>
      /// Not implemented.
      /// </summary>
      public Task SendToUsersAsync(IReadOnlyList<string> userIds, string vmId, string vmData)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Sends a view model response to the connection client via the WebSocket servers' callback HTTP URL.
      /// </summary>
      /// <param name="connectionId">WebSocket connection.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      private async Task SendConnectionsAsync(IEnumerable<string> connectionIds, string vmId, string vmData)
      {
         // Give a chance for any external party to handle client broadcast.
         var eventArgs = new WebApiResponseManagerSendEventArgs
         {
            ConnectionIds = connectionIds.ToList(),
            VMId = vmId,
            VMData = vmData
         };
         Sending?.Invoke(this, eventArgs);
         if (eventArgs.Handled)
            return;

         var response = new DotNetifyWebApi.IntegrationResponse { VMId = vmId, Data = vmData };
         var content = new StringContent(JsonSerializer.Serialize(response, _jsonSerializerOptions), Encoding.UTF8, "application/json");

         var semaphore = new SemaphoreSlim(MaxParallelHttpRequests);

         await Task.WhenAll(connectionIds.Select(connectionId => DoHttpHrequestAsync(connectionId, content, semaphore)));
      }

      private async Task DoHttpHrequestAsync(string connectionId, StringContent content, SemaphoreSlim semaphore)
      {
         await semaphore.WaitAsync();

         try
         {
            using var httpClient = _httpClientFactory.CreateClient(nameof(DotNetifyWebApi));
            if (httpClient != null)
            {
               var result = await httpClient.PostAsync($"{connectionId}", content);
               if (!result.IsSuccessStatusCode)
                  RemoveInstance(connectionId);
            }
            else
               throw new Exception("Missing HttpClient. Include 'services.AddDotNetifyHttpClient()' in the startup.");
         }
         finally
         {
            semaphore.Release();
         }
      }
   }
}