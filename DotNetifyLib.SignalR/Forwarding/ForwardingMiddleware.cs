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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetify.Security;
using Microsoft.AspNetCore.SignalR;

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
      private static readonly ConcurrentDictionary<DotNetifyHubForwarder, SemaphoreSlim> _semaphores = new ConcurrentDictionary<DotNetifyHubForwarder, SemaphoreSlim>();
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
         var hubForwarder = _hubForwarderFactory.GetInstance(_serverUrl, _config);
         var semaphore = _semaphores.GetOrAdd(hubForwarder, key => new SemaphoreSlim(1));

         await semaphore.WaitAsync();

         try
         {
            hubForwarder.CallerContext = context.CallerContext;

            if (context.CallType == nameof(IDotNetifyHubMethod.Request_VM))
               await hubForwarder.RequestVMAsync(context.VMId, context.Data);
            else if (context.CallType == nameof(IDotNetifyHubMethod.Update_VM))
            {
               var data = (Dictionary<string, object>) context.Data;
               await hubForwarder.UpdateVMAsync(context.VMId, data);
            }
            else if (context.CallType == nameof(IDotNetifyHubMethod.Dispose_VM))
               await hubForwarder.DisposeVMAsync(context.VMId);
            else if (context.CallType == nameof(IDotNetifyHubMethod.Response_VM))
               await hubForwarder.ResponseVMAsync(context.VMId, context.Data);
         }
         catch (Exception ex)
         {
            throw new ForwardingException(ex.Message, ex);
         }
         finally
         {
            semaphore.Release();
         }

         if (!_config.HaltPipeline)
            await next(context);
      }

      public async Task OnDisconnected(HubCallerContext context)
      {
         var hubForwarder = _hubForwarderFactory.GetInstance(_serverUrl, _config);
         var semaphore = _semaphores.GetOrAdd(hubForwarder, key => new SemaphoreSlim(1));

         await semaphore.WaitAsync();

         try
         {
            hubForwarder.CallerContext = context;
            await hubForwarder.OnDisconnectedAsync(null);
         }
         finally
         {
            semaphore.Release();
         }
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
      public static IDotNetifyConfiguration UseForwarding(this IDotNetifyConfiguration config, string serverUrl, Action<ForwardingOptions> options = null)
      {
         return config.UseForwarding(new string[] { serverUrl }, options);
      }

      /// <summary>
      /// The middleware used for forwarding incoming hub messages to other servers.
      /// </summary>
      /// <param name="config">DotNetify configuration.</param>
      /// <param name="serverUrls">Array of URLs of the servers to forward messages to.</param>
      /// <param name="haltPipeline">Whether to prevent further processing in this server after forwarding messages.</param>
      public static IDotNetifyConfiguration UseForwarding(this IDotNetifyConfiguration config, string[] serverUrls, Action<ForwardingOptions> options = null)
      {
         for (int i = 0; i < serverUrls.Length; i++)
         {
            var forwardingConfig = new ForwardingOptions();
            options?.Invoke(forwardingConfig);

            if (i < serverUrls.Length - 1)
               forwardingConfig.HaltPipeline = false;

            config.UseMiddleware<ForwardingMiddleware>(serverUrls[i], forwardingConfig);
         }

         // Add the extract headers middleware to prevent this same middleware get automatically inserted at the top.
         config.UseMiddleware<ExtractHeadersMiddleware>();
         return config;
      }

      /// <summary>
      /// Enables message pack in the forwarding options.
      /// </summary>
      public static ForwardingOptions UseMessagePack(this ForwardingOptions options)
      {
         options.UseMessagePack = true;
         return options;
      }

      /// <summary>
      /// Returns the origin connection context if the hub message was forwarded from a server.
      /// </summary>
      /// <param name="callerContext">Hub caller context.</param>
      /// <returns>Hub connection context.</returns>
      public static ConnectionContext GetOriginConnectionContext(this HubCallerContext callerContext)
      {
         return DotNetifyHubForwarder.GetOriginConnectionContext(callerContext.Items.ToDictionary(x => x.Key.ToString(), x => x.Value));
      }
   }
}