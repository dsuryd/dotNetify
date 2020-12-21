/*
Copyright 2020 Dicky Suryadi

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
using DotNetify.Client;
using Microsoft.AspNetCore.SignalR;

namespace DotNetify.Forwarding
{
   /// <summary>
   /// Forwards hub messages to another hub server.
   /// </summary>
   public class DotNetifyHubForwarder : IDotNetifyHubHandler
   {
      private const string CONNECTION_ID_TOKEN = "$fwdConnId";
      private const string CONNECTION_CONTEXT_TOKEN = "$fwdConnContext";

      private readonly IDotNetifyHubProxy _hubProxy;
      private IDotNetifyHubResponse _hubResponse;

      /// <summary>
      /// Sets the current caller context.
      /// </summary>
      public HubCallerContext CallerContext { get; set; }

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="hubProxy">Provides connection to the other hub server.</param>
      /// <param name="hubResponse">Provides methods to send responses back to the client.</param>
      public DotNetifyHubForwarder(IDotNetifyHubProxy hubProxy, IDotNetifyHubResponse hubResponse)
      {
         _hubProxy = hubProxy;
         _hubResponse = hubResponse;

         _hubProxy.Response_VM += OnResponse_VM;
         _hubProxy.StartAsync();
      }

      /// <summary>
      /// Forwards Dispose_VM message through the Invoke method.
      /// </summary>
      public async Task DisposeVMAsync(string vmId)
      {
         await _hubProxy.Invoke(nameof(IDotNetifyHubMethod.Dispose_VM), new object[] { vmId }, BuildMetadata());
      }

      /// <summary>
      /// Forwards disconnection.
      /// </summary>
      public async Task OnDisconnectedAsync(Exception ex)
      {
         await _hubProxy.Invoke(nameof(DotNetifyHub.OnDisconnectedAsync), new object[] { ex }, BuildMetadata());
      }

      /// <summary>
      /// Forwards Request_VM message through the Invoke method.
      /// </summary>
      public async Task RequestVMAsync(string vmId, object vmArg)
      {
         // Need to do this because nested JObject values get lost by converted to JsonElement.
         vmArg = vmArg != null ? JsonSerializer.Deserialize<Dictionary<string, object>>(vmArg.ToString()) : new Dictionary<string, object>();

         await _hubProxy.Invoke(nameof(IDotNetifyHubMethod.Request_VM), new object[] { vmId, vmArg }, BuildMetadata());
      }

      /// <summary>
      /// Forwards Update_VM message through the Invoke method.
      /// </summary>
      public async Task UpdateVMAsync(string vmId, Dictionary<string, object> vmData)
      {
         await _hubProxy.Invoke(nameof(IDotNetifyHubMethod.Update_VM), new object[] { vmId, vmData }, BuildMetadata());
      }

      /// <summary>
      /// Handles Invoke method responses received from the other hub server.
      /// </summary>
      protected void OnResponse_VM(object sender, ResponseVMEventArgs e)
      {
         var eventArgs = e as InvokeResponseEventArgs;
         if (eventArgs != null)
         {
            var args = new List<object> { eventArgs.Metadata[CONNECTION_ID_TOKEN] };
            args.AddRange(eventArgs.MethodArgs);

            _hubResponse.GetType().GetMethod(eventArgs.MethodName).Invoke(_hubResponse, args.ToArray());

            e.Handled = true;
         }
      }

      /// <summary>
      /// Builds metadata to forward to the other hub server.
      /// </summary>
      /// <returns></returns>
      private Dictionary<string, object> BuildMetadata()
      {
         return new Dictionary<string, object>
         {
            { CONNECTION_CONTEXT_TOKEN, CallerContext.GetConnectionContext() }
         };
      }

      /// <summary>
      /// Returns the origin connection context from a dictionary.
      /// </summary>
      /// <param name="items">Dictionary.</param>
      /// <returns>Connection context.</returns>
      static public ConnectionContext GetOriginConnectionContext(IDictionary<string, object> items)
      {
         return items.ContainsKey(CONNECTION_CONTEXT_TOKEN) ? JsonSerializer.Deserialize<ConnectionContext>(items[CONNECTION_CONTEXT_TOKEN].ToString()) : null;
      }

      /// <summary>
      /// Builds metadata to be included in the response to the forwarded messages.
      /// </summary>
      /// <param name="connectionId">Identifies the origin connection.</param>
      /// <returns>Metadata.</returns>
      internal static Dictionary<string, object> BuildResponseMetadata(string connectionId)
      {
         return new Dictionary<string, object> { { CONNECTION_ID_TOKEN, connectionId } };
      }
   }
}