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
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.AspNet.SignalR.Hubs;

namespace DotNetify
{
   /// <summary>
   /// Provides request context for a middleware.
   /// </summary>
   public class DotNetifyHubContext
   {
      private Lazy<Dictionary<string, object>> _pipelineData = new Lazy<Dictionary<string, object>>();

      public HubCallerContext CallerContext { get; }
      public string CallType { get; }
      public string VMId { get; }
      public object Data { get; set; }
      public object Headers { get; set; }
      public IPrincipal Principal { get; set; }
      public IDictionary<string, object> PipelineData => _pipelineData.Value;

      internal DotNetifyHubContext(HubCallerContext callerContext, string callType, string vmId, object data, object headers, IPrincipal principal)
      {
         CallerContext = callerContext;
         CallType = callType;
         VMId = vmId;
         Data = data;
         Headers = headers;
         Principal = principal;
      }
   }
}
