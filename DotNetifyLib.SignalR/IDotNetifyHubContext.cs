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

using Microsoft.AspNetCore.SignalR.Hubs;

namespace DotNetify
{
   /// <summary>
   /// Provides request context for a middleware.
   /// </summary>
   public interface IDotNetifyHubContext
   {
      HubCallerContext CallerContext { get; }
      string MethodName { get; }
      string VMId { get; }
      object Data { get; }
   }

   internal class DotNetifyHubContext : IDotNetifyHubContext
   {
      public HubCallerContext CallerContext { get; }
      public string MethodName { get; }
      public string VMId { get; }
      public object Data { get; }

      internal DotNetifyHubContext(HubCallerContext callerContext, string methodName, string vmId, object data)
      {
         CallerContext = callerContext;
         MethodName = methodName;
         VMId = vmId;
         Data = data;
      }
   }
}
