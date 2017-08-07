/* 
Copyright 2017 Dicky Suryadi

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
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Hubs;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

namespace DotNetify.Security
{
   /// <summary>
   /// Middleware to extract headers from the incoming view model data.
   /// </summary>
   public class ExtractHeadersMiddleware : IMiddleware, IDisconnectionMiddleware
   {
      private const string JTOKEN_VMARG = "$vmArg";
      private const string JTOKEN_HEADERS = "$headers";

      private readonly IMemoryCache _headersCache;
      private readonly Func<string, string> _headersKey = (string connectionId) => JTOKEN_HEADERS + connectionId;

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="headersCache">Memory cache to store headers per connection.</param>
      public ExtractHeadersMiddleware(IMemoryCache headersCache)
      {
         _headersCache = headersCache;
      }

      /// <summary>
      /// Invokes middleware.
      /// </summary>
      /// <param name="context">DotNetify hub context.</param>
      /// <param name="next">Next middleware delegate.</param>
      public Task Invoke(DotNetifyHubContext context, NextDelegate next)
      {       
         // Set initial headers from previously cached headers.
         context.Headers = _headersCache.Get(_headersKey(context.CallerContext.ConnectionId));

         var tuple = ExtractHeaders(context.Data);
         if (tuple.Item1 != null)
         {
            context.Headers = tuple.Item1;
            _headersCache.Set(_headersKey(context.CallerContext.ConnectionId), context.Headers);
         }
         context.Data = tuple.Item2;

         // If the incoming message is only for updating the headers, no need to continue down the pipeline.
         if (context.Data == null && context.CallType == nameof(DotNetifyHub.Update_VM))
            return Task.CompletedTask;

         return next(context);
      }

      /// <summary>
      /// Handles disconnected event by removing the headers associated with the connection from the cache.
      /// </summary>
      /// <param name="context">SignalR hub context.</param>
      public Task OnDisconnected(HubCallerContext context)
      {
         _headersCache.Remove(_headersKey(context.ConnectionId));
         return Task.CompletedTask;
      }

      /// <summary>
      /// Extract headers from view model data.
      /// </summary>
      /// <param name="data">Data that comes from Request_VM or Update_VM.</param>
      /// <returns>Tuple consisting of headers and data.</returns>
      private Tuple<object, T> ExtractHeaders<T>(T data) where T : class
      {
         object headers = null;

         if (typeof(T) == typeof(Dictionary<string, object>))
         {
            var vmData = data as Dictionary<string, object>;
            if (vmData.ContainsKey(JTOKEN_HEADERS))
            {
               headers = vmData[JTOKEN_HEADERS];
               vmData.Remove(JTOKEN_HEADERS);
               if (vmData.Count == 0)
                  data = null;
            }
         }
         else
         {
            JObject arg = data as JObject;
            if (arg?.Property(JTOKEN_HEADERS) != null)
            {
               headers = arg[JTOKEN_HEADERS];
               arg.Remove(JTOKEN_HEADERS);
            }
            if (arg?.Property(JTOKEN_VMARG) != null)
               data = arg[JTOKEN_VMARG] as T;
         }

         return Tuple.Create(headers, data);
      }
   }
}
