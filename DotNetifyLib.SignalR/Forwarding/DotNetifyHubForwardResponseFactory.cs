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

using Microsoft.AspNetCore.SignalR;

namespace DotNetify.Forwarding
{
   /// <summary>
   /// Provides objects for sending view model responses to clients.
   /// </summary>
   public interface IDotNetifyHubForwardResponseFactory
   {
      DotNetifyHubForwardResponse GetInstance(string connectionId);
   }

   /// <summary>
   /// This class produces objects for sending view model responses to clients.
   /// </summary>
   public class DotNetifyHubForwardResponseFactory : IDotNetifyHubForwardResponseFactory
   {
      private readonly IHubContext<DotNetifyHub> _globalHubContext;

      public DotNetifyHubForwardResponseFactory(IHubContext<DotNetifyHub> globalHubContext)
      {
         _globalHubContext = globalHubContext;
      }

      public DotNetifyHubForwardResponse GetInstance(string connectionId) => new DotNetifyHubForwardResponse(_globalHubContext, connectionId);
   }
}