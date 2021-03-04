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
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DotNetify.Forwarding
{
   public class ForwardingException : Exception
   {
      public ForwardingException(string message, Exception innerException) : base(message, innerException)
      {
      }
   }

   /// <summary>
   /// The middleware used for forwarding incoming hub messages to another server.
   /// </summary>
   public class ForwardingMiddleware : IMiddleware, IDisconnectionMiddleware
   {
      private readonly IDotNetifyHubForwarderFactory _hubForwarderFactory;
      private readonly string _serverUrl;
      private readonly ForwardingOptions _config;

      /// <summary>
      /// Class constructor.
      /// </summary>
      /// <param name="hubForwarderFactory">Factory of hub message forwarder objects.</param>
      /// <param name="serverUrl">URL of the server to forward messages to.</param>
      /// <param name="config">Forwarding configuration.</param>
      public ForwardingMiddleware(IDotNetifyHubForwarderFactory hubForwarderFactory, string serverUrl, ForwardingOptions config)
      {
         _hubForwarderFactory = hubForwarderFactory;
         _serverUrl = serverUrl;
         _config = config;
      }

      public async Task Invoke(DotNetifyHubContext context, NextDelegate next)
      {
         // Make it so the unawaited async method below has its own copy of data to avoid mutation by the middleware that follows.
         var contextData = context.Data;

         // Don't await to avoid blocking if there's connectivity failure.
         _ = _hubForwarderFactory.InvokeInstanceAsync(_serverUrl, _config, async hubForwarder =>
         {
            if (hubForwarder.IsConnected)
            {
               hubForwarder.CallerContext = context.CallerContext;

               if (context.CallType == nameof(IDotNetifyHubMethod.Request_VM))
                  await hubForwarder.RequestVMAsync(context.VMId, contextData);
               else if (context.CallType == nameof(IDotNetifyHubMethod.Update_VM))
               {
                  var data = (Dictionary<string, object>) contextData;
                  await hubForwarder.UpdateVMAsync(context.VMId, data);
               }
               else if (context.CallType == nameof(IDotNetifyHubMethod.Dispose_VM))
                  await hubForwarder.DisposeVMAsync(context.VMId);
               else if (context.CallType == nameof(IDotNetifyHubMethod.Response_VM))
               {
                  context.PipelineData.TryGetValue(DotNetifyHubHandler.GROUP_NAME_TOKEN, out object groupName);
                  await hubForwarder.ResponseVMAsync(context.VMId, contextData, groupName?.ToString());
               }
            }
            else
               Logger.LogError($"Failed to forward {context.CallType} message to {_serverUrl}: server is not connected.");
         });

         if (!_config.HaltPipeline)
            await next(context);
      }

      public async Task OnDisconnected(HubCallerContext context)
      {
         await _hubForwarderFactory.InvokeInstanceAsync(_serverUrl, _config, async hubForwarder =>
         {
            if (hubForwarder.IsConnected)
            {
               hubForwarder.CallerContext = context;
               await hubForwarder.OnDisconnectedAsync(null);
            }
            else
               Logger.LogError($"Failed to forward OnDisconnected message to {_serverUrl}: server is not connected.");
         });
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
      public static IDotNetifyConfiguration UseForwarding(this IDotNetifyConfiguration config, string serverUrl, Action<ForwardingOptions> optionsAccessor = null)
      {
         return config.UseForwarding(new string[] { serverUrl }, optionsAccessor);
      }

      /// <summary>
      /// The middleware used for forwarding incoming hub messages to other servers.
      /// </summary>
      /// <param name="config">DotNetify configuration.</param>
      /// <param name="serverUrls">Array of URLs of the servers to forward messages to.</param>
      /// <param name="haltPipeline">Whether to prevent further processing in this server after forwarding messages.</param>
      public static IDotNetifyConfiguration UseForwarding(this IDotNetifyConfiguration config, string[] serverUrls, Action<ForwardingOptions> optionsAccessor = null)
      {
         for (int i = 0; i < serverUrls.Length; i++)
         {
            var forwardingOptions = new ForwardingOptions();
            optionsAccessor?.Invoke(forwardingOptions);

            if (i < serverUrls.Length - 1)
               forwardingOptions.HaltPipeline = false;

            config.UseMiddleware<ForwardingMiddleware>(serverUrls[i], forwardingOptions);
         }
         return config;
      }

      /// <summary>
      /// Returns the origin connection context from a forwarded hub message.
      /// </summary>
      /// <param name="callerContext">Hub caller context.</param>
      public static ConnectionContext GetOriginConnectionContext(this HubCallerContext callerContext)
      {
         return DotNetifyHubForwarder.GetOriginConnectionContext(callerContext.Items);
      }

      /// <summary>
      /// Returns the group send info from a forwarded hub message of a multicast view model.
      /// </summary>
      /// <param name="callerContext">Hub caller context.</param>
      public static VMController.GroupSend GetGroupSend(this HubCallerContext callerContext)
      {
         return DotNetifyHubForwarder.GetGroupSend(callerContext.Items);
      }

      /// <summary>
      /// Returns the group name from a forwarded hub message of a multicast view model.
      /// </summary>
      /// <param name="callerContext">Hub caller context.</param>
      public static string GetGroupName(this HubCallerContext callerContext)
      {
         return DotNetifyHubForwarder.GetGroupName(callerContext.Items);
      }
   }
}