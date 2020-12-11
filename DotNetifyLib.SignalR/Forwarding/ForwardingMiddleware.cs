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
using DotNetify.Security;
using Microsoft.AspNetCore.SignalR;

namespace DotNetify.Forwarding
{
   /// <summary>
   /// The middleware used for forwarding incoming hub messages to another server.
   /// </summary>
   public class ForwardingMiddleware : IMiddleware
   {
      private readonly DotNetifyHubForwarder _hubForwarder;
      private readonly bool _haltPipeline;

      /// <summary>
      /// Class constructor.
      /// </summary>
      /// <param name="hubForwarderFactory">Factory of hub message forwarder objects.</param>
      /// <param name="serverUrl">URL of the server to forward messages to.</param>
      /// <param name="haltPipeline">Whether to prevent further processing in this server after forwarding messages.</param>
      public ForwardingMiddleware(IDotNetifyHubForwarderFactory hubForwarderFactory, string serverUrl, bool haltPipeline)
      {
         _hubForwarder = hubForwarderFactory.GetInstance(serverUrl);
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
      /// <summary>
      /// The middleware used for forwarding incoming hub messages to another server.
      /// </summary>
      /// <param name="config">DotNetify configuration.</param>
      /// <param name="serverUrl">URL of the server to forward messages to.</param>
      /// <param name="haltPipeline">Whether to prevent further processing in this server after forwarding messages.</param>
      public static void UseForwarding(this IDotNetifyConfiguration config, string serverUrl, bool haltPipeline = true)
      {
         config.UseMiddleware<ForwardingMiddleware>(serverUrl, haltPipeline);

         // Add the extract headers middleware to prevent this same middleware get automatically inserted at the top.
         config.UseMiddleware<ExtractHeadersMiddleware>();
      }

      /// <summary>
      /// The middleware used for forwarding incoming hub messages to other servers.
      /// </summary>
      /// <param name="config">DotNetify configuration.</param>
      /// <param name="serverUrls">Array of URLs of the servers to forward messages to.</param>
      /// <param name="haltPipeline">Whether to prevent further processing in this server after forwarding messages.</param>
      public static void UseForwarding(this IDotNetifyConfiguration config, string[] serverUrls, bool haltPipeline = true)
      {
         for (int i = 0; i < serverUrls.Length; i++)
            config.UseMiddleware<ForwardingMiddleware>(serverUrls[i], i == serverUrls.Length - 1 && haltPipeline);

         // Add the extract headers middleware to prevent this same middleware get automatically inserted at the top.
         config.UseMiddleware<ExtractHeadersMiddleware>();
      }

      /// <summary>
      /// Returns the origin connection ID if the hub message was forwarded from a server.
      /// </summary>
      /// <param name="callerContext">Hub caller context.</param>
      /// <returns>Hub connection ID.</returns>
      public static string GetForwardConnectionId(this HubCallerContext callerContext)
      {
         if (callerContext.Items.ContainsKey(DotNetifyHubForwarder.CONNECTION_ID_TOKEN))
            return callerContext.Items[DotNetifyHubForwarder.CONNECTION_ID_TOKEN].ToString();
         return null;
      }
   }
}