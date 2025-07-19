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
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DotNetify.Security;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetify.WebApi
{
   internal static class RestoreManager
   {
      /// <summary>
      /// Recreates view model instances that belong to active connections.
      /// </summary>
      /// <param name="services"></param>
      public static void RestoreActiveConnectionVMs(IServiceProvider services)
      {
         var cache = services.GetService<IWebApiConnectionCache>();
         if (cache == null)
            return;

         var vmControllerFactory = services.GetRequiredService<IWebApiVMControllerFactory>();
         var hubServiceProvider = services.GetRequiredService<IHubServiceProvider>();
         var principalAccessor = services.GetRequiredService<IPrincipalAccessor>();
         var hubPipeline = services.GetRequiredService<IHubPipeline>();
         var responseManager = services.GetRequiredService<IWebApiResponseManager>();
         var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();

         // Intercept any send response event that occurs when a new view model is recreated.
         responseManager.Sending += ResponseManager_Sending;

         foreach (var connection in GetActiveConnectionsAsync(cache, httpClientFactory).GetAwaiter().GetResult())
         {
            License.CheckUsage();

            foreach (var vmInfo in connection.VMInfo)
            {
               var httpCallerContext = new DotNetifyWebApi.HttpCallerContext(connection.Id);
               new DotNetifyHubHandler(vmControllerFactory, hubServiceProvider, principalAccessor, hubPipeline, responseManager) { CallerContext = httpCallerContext }
                  .RequestVMAsync(vmInfo.VMId, JsonSerializer.Deserialize<object>(vmInfo.VMArgs)).GetAwaiter().GetResult();
            }
         }

         responseManager.Sending -= ResponseManager_Sending;
      }

      /// <summary>
      /// Recreates a view model instance.
      /// </summary>
      internal static async Task RestoreVMAsync(IServiceProvider services, string connectionId, string vmId, string vmArgs)
      {
         var vmControllerFactory = services.GetRequiredService<IWebApiVMControllerFactory>();
         var hubServiceProvider = services.GetRequiredService<IHubServiceProvider>();
         var principalAccessor = services.GetRequiredService<IPrincipalAccessor>();
         var hubPipeline = services.GetRequiredService<IHubPipeline>();
         var responseManager = services.GetRequiredService<IWebApiResponseManager>();
         var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();

         // Intercept any send response event that occurs when a new view model is recreated.
         responseManager.Sending += ResponseManager_Sending;

         var httpCallerContext = new DotNetifyWebApi.HttpCallerContext(connectionId);
         var handler = new DotNetifyHubHandler(vmControllerFactory, hubServiceProvider, principalAccessor, hubPipeline, responseManager) { CallerContext = httpCallerContext };
         await handler.RequestVMAsync(vmId, JsonSerializer.Deserialize<object>(vmArgs));

         responseManager.Sending -= ResponseManager_Sending;
      }

      /// <summary>
      /// Handles sending event from the response manager object.
      /// </summary>
      private static void ResponseManager_Sending(object sender, WebApiResponseManagerSendEventArgs e)
      {
         e.Handled = true;
      }

      /// <summary>
      /// Reads cached connections and ping them to find connections that are still active.
      /// </summary>
      /// <param name="cache">Distributed cache.</param>
      /// <param name="httpClientFactory">Http client factory.</param>
      /// <returns>List of active connections.</returns>
      private static async Task<List<Connection>> GetActiveConnectionsAsync(IWebApiConnectionCache cache, IHttpClientFactory httpClientFactory)
      {
         // Get connections from the cache and ping them.
         var connections = (await cache.GetConnectionsAsync()) ?? new List<Connection>();
         var semaphore = new SemaphoreSlim(WebApiResponseManager.MaxParallelHttpRequests);
         var activeConnectionIds = (await Task.WhenAll(connections.Select(connection => PingConnectionAsync(httpClientFactory, connection.Id, semaphore))))
            .Where(x => x.Item2 == true)
            .Select(x => x.Item1)
            .ToList();

         // Remove non-responding connections from the cache.
         foreach (var connection in connections.Where(x => !activeConnectionIds.Contains(x.Id)))
         {
            await cache.RemoveAsync(connection.Id);

            var group = cache.GetGroupAsync(ResilientWebApiConnectionCache.ACTIVE_GROUP).GetAwaiter().GetResult();
            if (group.ConnectionIds.Contains(connection.Id))
            {
               group.ConnectionIds.Remove(connection.Id);
               _ = cache.SaveGroupAsync(group);
            }
         }

         return connections.Where(x => activeConnectionIds.Contains(x.Id)).ToList();
      }

      /// <summary>
      /// Pings a connection.
      /// </summary>
      /// <param name="httpClientFactory">Http client factory.</param>
      /// <param name="connectionId">Identifies a connection.</param>
      /// <param name="semaphore">Semaphore used to control number of parallel HTTP requests.</param>
      /// <returns>Tuple of connection id and the ping result.</returns>
      private static async Task<Tuple<string, bool>> PingConnectionAsync(IHttpClientFactory httpClientFactory, string connectionId, SemaphoreSlim semaphore)
      {
         await semaphore.WaitAsync();

         try
         {
            using var httpClient = httpClientFactory.CreateClient(nameof(DotNetifyWebApi));
            return Tuple.Create(connectionId, (await httpClient.PostAsync($"{connectionId}", new StringContent("{}"))).IsSuccessStatusCode);
         }
         finally
         {
            semaphore.Release();
         }
      }
   }
}