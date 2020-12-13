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

using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetify
{
   public static class HubCallerContextExtensions
   {
      public static HttpRequestHeaders GetHttpRequestHeaders(this HubCallerContext context)
      {
         var httpContext = context?.Features.Get<IHttpContextFeature>();
         return httpContext != null ? new HttpRequestHeaders(
            allHeaders: JObject.Parse(JsonConvert.SerializeObject(httpContext.HttpContext?.Request?.Headers)),
            userAgent: httpContext.HttpContext?.Request?.Headers["User-Agent"]
            ) : null;
      }

      public static HttpConnection GetHttpConnection(this HubCallerContext context)
      {
         var feature = context?.Features.Get<IHttpConnectionFeature>();
         return feature != null ? new HttpConnection(
            connectionId: feature.ConnectionId,
            localIpAddress: feature.LocalIpAddress,
            remoteIpAddress: feature.RemoteIpAddress,
            localPort: feature.LocalPort,
            remotePort: feature.RemotePort
            ) : null;
      }

      public static IConnectionContext GetConnectionContext(this HubCallerContext context)
      {
         return new ConnectionContext
         {
            ConnectionId = context.ConnectionId,
            HttpConnection = context.GetHttpConnection(),
            HttpRequestHeaders = context.GetHttpRequestHeaders()
         };
      }
   }
}