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

using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetify.Client;
using DotNetify.Security;
using Microsoft.AspNetCore.SignalR;

namespace DotNetify.Forwarding
{
   public class ForwardingMiddleware : IMiddleware
   {
      private static readonly Dictionary<string, DotNetifyHubForwarder> _hubForwarders = new Dictionary<string, DotNetifyHubForwarder>();

      private readonly DotNetifyHubForwarder _hubForwarder;
      private readonly bool _haltPipeline;

      public ForwardingMiddleware(IHubContext<DotNetifyHub> globalHubContext, string serverUrl, bool haltPipeline)
      {
         if (!_hubForwarders.ContainsKey(serverUrl))
            _hubForwarders.Add(serverUrl, new DotNetifyHubForwarder(new DotNetifyHubProxy(serverUrl), new DotNetifyHubResponse(globalHubContext)));

         _hubForwarder = _hubForwarders[serverUrl];
         _haltPipeline = haltPipeline;
      }

      public async Task Invoke(DotNetifyHubContext context, NextDelegate next)
      {
         _hubForwarder.CallerContext = context.CallerContext;

         if (context.CallType == nameof(IDotNetifyHubMethod.Request_VM))
            await _hubForwarder.RequestVMAsync(context.VMId, context.Data);
         else if (context.CallType == nameof(IDotNetifyHubMethod.Update_VM))
         {
            var data = (Dictionary<string, object>) context.Data;
            await _hubForwarder.UpdateVMAsync(context.VMId, data);
         }
         else if (context.CallType == nameof(IDotNetifyHubMethod.Dispose_VM))
            await _hubForwarder.DisposeVMAsync(context.VMId);

         if (!_haltPipeline)
            await next(context);
      }
   }

   public static class ForwardingMiddlewareExtensions
   {
      public static void UseForwarding(this IDotNetifyConfiguration config, string serverUrl, bool haltPipeline = true)
      {
         config.UseMiddleware<ForwardingMiddleware>(serverUrl, haltPipeline);

         // Add the extract headers middleware to prevent this same middleware get automatically inserted at the top.
         config.UseMiddleware<ExtractHeadersMiddleware>();
      }

      public static string GetForwardConnectionId(this HubCallerContext callerContext)
      {
         if (callerContext.Items.ContainsKey(DotNetifyHubForwarder.CONNECTION_ID_TOKEN))
            return callerContext.Items[DotNetifyHubForwarder.CONNECTION_ID_TOKEN].ToString();
         return null;
      }
   }
}