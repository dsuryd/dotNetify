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
using Microsoft.AspNet.SignalR.Hubs;

namespace DotNetify
{
   /// <summary>
   /// Marker interface for a variety of middleware pipelines.
   /// </summary>
   public interface IMiddlewarePipeline
   {
   }

   /// <summary>
   /// Delegate to invoke the next middleware in the pipeline.
   /// </summary>
   /// <param name="context">DotNetify hub context.</param>
   public delegate Task NextDelegate(DotNetifyHubContext context);

   /// <summary>
   /// Provides interception of incoming view model actions.
   /// If the middleware throws an exception, the exception type and message will be sent to the client.
   /// </summary>
   public interface IMiddleware : IMiddlewarePipeline
   {
      Task Invoke(DotNetifyHubContext context, NextDelegate next);
   }

   /// <summary>
   /// Provides interception of client disconnected event.
   /// </summary>
   public interface IDisconnectionMiddleware : IMiddlewarePipeline
   {
      Task OnDisconnected(HubCallerContext context);
   }

   /// <summary>
   /// Provides interception when an exception is thrown either from a view model or middlewares.
   /// </summary>
   public interface IExceptionMiddleware : IMiddlewarePipeline
   {
      Task<Exception> OnException(HubCallerContext context, Exception exception);
   }
}
