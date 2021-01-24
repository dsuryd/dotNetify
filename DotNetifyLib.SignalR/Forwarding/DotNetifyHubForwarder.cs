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
      private const string GROUP_SEND_TOKEN = "$fwdGroupSend";

      private readonly IDotNetifyHubProxy _hubProxy;
      private IDotNetifyHubResponse _hubResponse;

      /// <summary>
      /// Sets the current caller context.
      /// </summary>
      public HubCallerContext CallerContext { get; set; }

      /// <summary>
      /// Whether the target server is connected.
      /// </summary>
      public bool IsConnected => _hubProxy.ConnectionState == HubConnectionState.Connected;

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
      }

      /// <summary>
      /// Starts the connection to the target server.
      /// </summary>
      public async Task StartAsync()
      {
         if (_hubProxy.ConnectionState == HubConnectionState.Disconnected)
            await _hubProxy.StartAsync();
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
         string data = vmArg?.ToString();
         await _hubProxy.Invoke(nameof(IDotNetifyHubMethod.Request_VM), new object[] { vmId, data }, BuildMetadata());
      }

      /// <summary>
      /// Forwards Response_VM message through the Invoke method.
      /// </summary>
      public async Task ResponseVMAsync(string vmId, object vmData)
      {
         var groupSend = vmData as VMController.GroupSend;
         if (groupSend?.IsEmpty() == true)
            return;

         await _hubProxy.Invoke(nameof(IDotNetifyHubMethod.Response_VM), new object[] { vmId, groupSend?.Data ?? vmData }, BuildResponseMetadata(groupSend));
      }

      /// <summary>
      /// Forwards Update_VM message through the Invoke method.
      /// </summary>
      public async Task UpdateVMAsync(string vmId, Dictionary<string, object> vmData)
      {
         string data = JsonSerializer.Serialize(vmData);
         await _hubProxy.Invoke(nameof(IDotNetifyHubMethod.Update_VM), new object[] { vmId, data }, BuildMetadata());
      }

      /// <summary>
      /// Forwards a message through the Invoke method.
      /// </summary>
      /// <param name="methodName">Hub method name.</param>
      /// <param name="methodArgs">Method arguments.</param>
      /// <param name="metadata">Message metadata.</param>
      public async Task InvokeAsync(string methodName, object[] methodArgs, IDictionary<string, object> metadata)
      {
         await _hubProxy.Invoke(methodName, methodArgs, metadata);
      }

      /// <summary>
      /// Handles Invoke method responses received from the other hub server.
      /// </summary>
      protected void OnResponse_VM(object sender, ResponseVMEventArgs e)
      {
         var eventArgs = e as InvokeResponseEventArgs;
         if (eventArgs != null)
         {
            object connectionId = eventArgs.MethodName == nameof(IDotNetifyHubResponse.SendToManyAsync) ?
               (object) JsonSerializer.Deserialize<List<string>>(eventArgs.Metadata[CONNECTION_ID_TOKEN]) : eventArgs.Metadata[CONNECTION_ID_TOKEN];

            var args = new List<object> { connectionId };
            args.AddRange(eventArgs.MethodArgs);

            _hubResponse.GetType().GetMethod(eventArgs.MethodName).Invoke(_hubResponse, args.ToArray());

            e.Handled = true;
         }
      }

      /// <summary>
      /// Returns the origin connection context from a dictionary.
      /// </summary>
      /// <param name="items">Dictionary.</param>
      public static ConnectionContext GetOriginConnectionContext(IDictionary<object, object> items)
      {
         var connectionContext = items.ContainsKey(CONNECTION_CONTEXT_TOKEN) ? items[CONNECTION_CONTEXT_TOKEN] : null;
         return connectionContext != null ? JsonSerializer.Deserialize<ConnectionContext>(connectionContext.ToString()) : null;
      }

      /// <summary>
      /// Returns the multicast group send info from a dictionary.
      /// </summary>
      /// <param name="items">Dictonary</param>
      public static VMController.GroupSend GetGroupSend(IDictionary<object, object> items)
      {
         return items.ContainsKey(GROUP_SEND_TOKEN) ? JsonSerializer.Deserialize<VMController.GroupSend>(items[GROUP_SEND_TOKEN].ToString()) : null;
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

      /// <summary>
      /// Builds metadata to be included in the response to the forwarded messages.
      /// </summary>
      /// <param name="connectionId">Identifies the origin connection.</param>
      /// <returns>Metadata.</returns>
      internal static Dictionary<string, object> BuildResponseMetadata(IReadOnlyList<string> connectionIds)
      {
         return new Dictionary<string, object> { { CONNECTION_ID_TOKEN, JsonSerializer.Serialize(connectionIds) } };
      }

      /// <summary>
      /// Builds metadata to forward to the other hub server.
      /// </summary>
      /// <returns></returns>
      private Dictionary<string, object> BuildMetadata()
      {
         return new Dictionary<string, object>
         {
            { CONNECTION_CONTEXT_TOKEN, JsonSerializer.Serialize(CallerContext.GetConnectionContext()) }
         };
      }

      /// <summary>
      /// Builds response metadata to forward to the other hub server.
      /// </summary>
      private Dictionary<string, object> BuildResponseMetadata(VMController.GroupSend groupSend)
      {
         var metadata = new Dictionary<string, object>
         {
            { CONNECTION_CONTEXT_TOKEN, JsonSerializer.Serialize( CallerContext.GetConnectionContext()) }
         };

         // If multicast message, include the metadata.
         if (groupSend != null)
            metadata.Add(GROUP_SEND_TOKEN, JsonSerializer.Serialize(groupSend));

         return metadata;
      }
   }
}