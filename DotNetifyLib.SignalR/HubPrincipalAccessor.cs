/*
Copyright 2016-2018 Dicky Suryadi

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
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Principal;
using System.Threading;

namespace DotNetify
{
   /// <summary>
   /// Implements ambient data store for the current principal and connection context of the SignalR hub.
   /// </summary>
   internal class HubPrincipalAccessor : IPrincipalAccessor, IHubCallerContextAccessor, IConnectionContext
   {
      private readonly static AsyncLocal<IPrincipal> _asyncLocalPrincipal = new AsyncLocal<IPrincipal>();
      private readonly static AsyncLocal<HubCallerContext> _asyncLocalCallerContext = new AsyncLocal<HubCallerContext>();

      /// <summary>
      /// Current principal.
      /// </summary>
      public IPrincipal Principal
      {
         get { return _asyncLocalPrincipal.Value; }
         set { _asyncLocalPrincipal.Value = value; }
      }

      /// <summary>
      /// SignalR hub caller context.
      /// </summary>
      public HubCallerContext CallerContext
      {
         get { return _asyncLocalCallerContext.Value; }
         set { _asyncLocalCallerContext.Value = value; }
      }

      /// <summary>
      /// HTTP request headers.
      /// </summary>
      public HttpRequestHeaders HttpRequestHeaders
      {
         get
         {
            var httpContext = CallerContext?.Features.Get<IHttpContextFeature>();
            return httpContext != null ? new HttpRequestHeaders(
               allHeaders: JObject.Parse(JsonConvert.SerializeObject(httpContext.HttpContext?.Request?.Headers)),
               userAgent: httpContext.HttpContext?.Request?.Headers["User-Agent"]
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
            var feature = CallerContext?.Features.Get<IHttpConnectionFeature>();
            return feature != null ? new HttpConnection(
               connectionId: feature.ConnectionId,
               localIpAddress: feature.LocalIpAddress,
               remoteIpAddress: feature.RemoteIpAddress,
               localPort: feature.LocalPort,
               remotePort: feature.RemotePort
               ) : null;
         }
      }
   }
}