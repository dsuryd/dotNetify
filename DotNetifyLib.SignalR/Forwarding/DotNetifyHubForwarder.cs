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
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetify.Client;
using Microsoft.AspNetCore.SignalR;
using DotNetify.Util;

namespace DotNetify.Forwarding
{
   public class DotNetifyHubForwarder : IDotNetifyHubHandler
   {
      public const string CONNECTION_ID_TOKEN = "$fwdConnId";

      private readonly IDotNetifyHubProxy _hubProxy;
      private HubCallerContext _context;
      private IDotNetifyHubResponse _hubResponse;

      public HubCallerContext CallerContext { set => _context = value; }
      public IDotNetifyHubResponse HubResponse { set => _hubResponse = value; }

      public DotNetifyHubForwarder(IDotNetifyHubProxy hubProxy, IDotNetifyHubResponse hubResponse)
      {
         _hubProxy = hubProxy;
         _hubResponse = hubResponse;

         _hubProxy.Response_VM += OnResponse_VM;
         _hubProxy.StartAsync();
      }

      public async Task DisposeVMAsync(string vmId)
      {
         await _hubProxy.Invoke(nameof(IDotNetifyHubMethod.Dispose_VM), new object[] { vmId }, BuildMetadata());
      }

      public Task OnDisconnectedAsync(Exception _)
      {
         return Task.CompletedTask;
      }

      public async Task RequestVMAsync(string vmId, object vmArg)
      {
         // Need to do this because nested JObject values get lost by converted to JsonElement.
         vmArg = vmArg != null ? JsonSerializer.Deserialize<Dictionary<string, object>>(vmArg.ToString()) : new Dictionary<string, object>();

         await _hubProxy.Invoke(nameof(IDotNetifyHubMethod.Request_VM), new object[] { vmId, vmArg }, BuildMetadata());
      }

      public async Task UpdateVMAsync(string vmId, Dictionary<string, object> vmData)
      {
         await _hubProxy.Invoke(nameof(IDotNetifyHubMethod.Update_VM), new object[] { vmId, vmData }, BuildMetadata());
      }

      private void OnResponse_VM(object sender, ResponseVMEventArgs e)
      {
         var eventArgs = e as InvokeResponseEventArgs;
         if (eventArgs != null)
         {
            var args = new List<object> { eventArgs.Metadata[CONNECTION_ID_TOKEN] };
            args.AddRange(eventArgs.MethodArgs);

            _hubResponse.GetType().GetMethod(nameof(IDotNetifyHubResponse.SendAsync)).Invoke(_hubResponse, args.ToArray());

            e.Handled = true;
         }
      }

      private Dictionary<string, object> BuildMetadata()
      {
         return new Dictionary<string, object> { { CONNECTION_ID_TOKEN, _context.ConnectionId } };
      }
   }
}