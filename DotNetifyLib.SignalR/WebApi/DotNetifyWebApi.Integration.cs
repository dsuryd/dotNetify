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
using System.Text.Json;
using System.Threading.Tasks;
using DotNetify.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetify.WebApi
{
   public partial class DotNetifyWebApi
   {
      public class IntegrationPayload
      {
         public string CallType { get; set; }
         public string VMId { get; set; }
         public string VMArgs { get; set; }
         public string Value { get; set; }
      }

      public class IntegrationRequest
      {
         public string ConnectionId { get; set; }
         public string State { get; set; }
         public IntegrationPayload Payload { get; set; }
      }

      public class IntegrationResponse
      {
         public string CallType => "response_vm";
         public string VMId { get; set; }
         public object Data { get; set; }
      }

      /// <summary>
      ///  This method is intended for integrating dotNetify with non-SignalR websocket server such as AWS Websocket API Gateway.
      ///  In this use case, clients make connections to the websocket server, and it passes the messages to the dotNetify server through HTTP calls.
      /// </summary>
      /// <param name="request">Integration request.</param>
      /// <param name="vmControllerFactory">Factory of view model controllers.</param>
      /// <param name="serviceProvider">Allows to provide scoped service provider for the view models.</param>
      /// <param name="principalAccessor">Allows to pass the hub principal.</param>
      /// <param name="hubPipeline">Manages middlewares and view model filters.</param>
      /// <param name="hubResponseFactory">Factory of objects to send responses back to hub clients.</param>
      [HttpPost()]
      public async Task IntegrationEndpoint(
         [FromBody] IntegrationRequest request,
         [FromServices] IWebApiVMControllerFactory vmControllerFactory,
         [FromServices] IHubServiceProvider hubServiceProvider,
         [FromServices] IPrincipalAccessor principalAccessor,
         [FromServices] IHubPipeline hubPipeline,
         [FromServices] IWebApiResponseManager responseManager
         )
      {
         if (string.IsNullOrWhiteSpace(request.ConnectionId))
            throw new ArgumentNullException(nameof(request.ConnectionId));

         HttpContext.Items.Add(nameof(HttpCallerContext.ConnectionId), request.ConnectionId);

         if (request.Payload?.CallType != null)
         {
            var vmId = request.Payload.VMId;

            if (request.Payload.CallType.Equals("request_vm", StringComparison.OrdinalIgnoreCase))
            {
               var hub = CreateHubHandler(vmControllerFactory, hubServiceProvider, principalAccessor, hubPipeline, responseManager, null, nameof(IDotNetifyHubMethod.Request_VM), vmId, request.Payload.VMArgs);
               await hub.RequestVMAsync(vmId, request.Payload.VMArgs);
            }
            else if (request.Payload.CallType.Equals("update_vm", StringComparison.OrdinalIgnoreCase))
            {
               var vmData = JsonSerializer.Deserialize<Dictionary<string, object>>(request.Payload.Value);
               var hub = CreateHubHandler(vmControllerFactory, hubServiceProvider, principalAccessor, hubPipeline, responseManager, null, nameof(IDotNetifyHubMethod.Update_VM), vmId, vmData);
               await hub.UpdateVMAsync(vmId, vmData);
            }
            else if (request.Payload.CallType.Equals("dispose_vm", StringComparison.OrdinalIgnoreCase))
            {
               var hub = CreateHubHandler(vmControllerFactory, hubServiceProvider, principalAccessor, hubPipeline, responseManager, null, nameof(IDotNetifyHubMethod.Dispose_VM), vmId);
               await hub.DisposeVMAsync(vmId);
            }
            else
               throw new InvalidOperationException("Type not recognized: " + request.Payload.CallType);
         }
         else
         {
            if (request.State.Equals("closed", StringComparison.OrdinalIgnoreCase))
            {
               var hub = CreateHubHandler(vmControllerFactory, hubServiceProvider, principalAccessor, hubPipeline, responseManager, null, null, null);
               await hub.OnDisconnectedAsync(null);
            }
         }
      }
   }
}