/*
Copyright 2016-2020 Dicky Suryadi

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

using System.Security.Principal;
using System.Threading;
using DotNetify.Forwarding;
using DotNetify.Security;
using Microsoft.AspNetCore.SignalR;

namespace DotNetify
{
   /// <summary>
   /// Implements ambient data store for the current principal and connection context of the SignalR hub.
   /// </summary>
   internal class HubInfoAccessor : IPrincipalAccessor, IHubCallerContextAccessor, IDotNetifyHubContextAccessor, IConnectionContext
   {
      private readonly static AsyncLocal<IPrincipal> _asyncLocalPrincipal = new AsyncLocal<IPrincipal>();
      private readonly static AsyncLocal<DotNetifyHubContext> _asyncLocalContext = new AsyncLocal<DotNetifyHubContext>();

      /// <summary>
      /// Current principal.
      /// </summary>
      public IPrincipal Principal
      {
         get { return _asyncLocalPrincipal.Value; }
         set { _asyncLocalPrincipal.Value = value; }
      }

      /// <summary>
      /// Hub context.
      /// </summary>
      public DotNetifyHubContext Context
      {
         get { return _asyncLocalContext.Value; }
         set { _asyncLocalContext.Value = value; }
      }

      /// <summary>
      /// SignalR hub caller context.
      /// </summary>
      public HubCallerContext CallerContext => Context?.CallerContext;

      /// <summary>
      /// Identifies the SignalR connection.
      /// </summary>
      public string ConnectionId => Context?.ConnectionId;

      /// <summary>
      /// HTTP request headers.
      /// </summary>
      public HttpRequestHeaders HttpRequestHeaders => CallerContext.GetOriginConnectionContext()?.HttpRequestHeaders ?? CallerContext.GetHttpRequestHeaders();

      /// <summary>
      /// HTTP connection info.
      /// </summary>
      public HttpConnection HttpConnection => CallerContext.GetOriginConnectionContext()?.HttpConnection ?? CallerContext.GetHttpConnection();
   }
}