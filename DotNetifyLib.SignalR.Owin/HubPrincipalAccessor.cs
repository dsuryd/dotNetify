/*
Copyright 2017-2018 Dicky Suryadi

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

using DotNetify.Security;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Security.Principal;
using System.Threading;

namespace DotNetify
{
   internal class HubPrincipalAccessor : IPrincipalAccessor, IHubCallerContextAccessor, IConnectionContext
   {
      private readonly static AsyncLocal<HubCallerContext> _asyncLocalCallerContext = new AsyncLocal<HubCallerContext>();

      public IPrincipal Principal { get; set; } = Thread.CurrentPrincipal;

      /// <summary>
      /// SignalR hub caller context.
      /// </summary>
      public HubCallerContext CallerContext
      {
         get { return _asyncLocalCallerContext.Value; }
         set { _asyncLocalCallerContext.Value = value; }
      }

      /// <summary>
      /// Identifies the SignalR connection.
      /// </summary>
      public string ConnectionId => CallerContext?.ConnectionId;

      /// <summary>
      /// HTTP request headers.
      /// </summary>
      public HttpRequestHeaders HttpRequestHeaders
      {
         get
         {
            var headers = CallerContext.Request?.Headers;
            if (headers == null)
               return null;

            var expandoObj = new ExpandoObject();
            var expandoObjCollection = (ICollection<KeyValuePair<string, object>>)expandoObj;
            foreach (var kvp in headers)
               expandoObjCollection.Add(new KeyValuePair<string, object>(kvp.Key, kvp.Value));

            return headers != null ? new HttpRequestHeaders(
               allHeaders: expandoObj,
               userAgent: headers["User-Agent"]
               ) : null;
         }
      }

      /// <summary>
      /// HTTP connection info.
      /// </summary>
      public HttpConnection HttpConnection
      {
         get
         {
            var env = CallerContext.Request?.Environment;
            if (env == null)
               return null;

            return new HttpConnection(
               connectionId: null,
               localIpAddress: IPAddress.Parse(env["server.LocalIpAddress"].ToString()),
               remoteIpAddress: IPAddress.Parse(env["server.RemoteIpAddress"].ToString()),
               localPort: int.Parse(env["server.LocalPort"].ToString()),
               remotePort: int.Parse(env["server.RemotePort"].ToString())
               );
         }
      }
   }
}