/* 
Copyright 2016-2017 Dicky Suryadi

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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Hubs;

namespace DotNetify
{
   /// <summary>
   /// Provides management of the invocation of pipeline middlewares and view model filters for the dotNetify hub.
   /// </summary>
   public interface IHubPipeline
   {
      T RunMiddlewares<T>(HubCallerContext context, string callType, string vmId, T data, NextDelegate finalAction) where T : class;

      void RunDisconnectionMiddlewares(HubCallerContext context);

      void RunVMFilters<T>(HubCallerContext context, string callType, string vmId, BaseVM vm, ref T data, IPrincipal principal) where T : class;

      Exception RunExceptionMiddleware(HubCallerContext context, Exception exception);
   }

   public class HubPipeline : IHubPipeline
   {
      private readonly IList<Tuple<Type, Func<IMiddlewarePipeline>>> _middlewareFactories;
      private readonly IDictionary<Type, Func<IVMFilter>> _vmFilterFactories;

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="vmControllerFactory">Factory of view model controllers.</param>
      /// <param name="principalAccessor">Allow to pass the hub principal.</param>
      /// <param name="middlewareFactories">Middlewares to intercept incoming view model requests and updates.</param>
      public HubPipeline(IList<Tuple<Type, Func<IMiddlewarePipeline>>> middlewareFactories, IDictionary<Type, Func<IVMFilter>> vmFilterFactories)
      {
         _middlewareFactories = middlewareFactories;
         _vmFilterFactories = vmFilterFactories;
      }

      private List<IMiddlewarePipeline> GetMiddlewares<TMiddleware>() where TMiddleware : IMiddlewarePipeline
      {
         return _middlewareFactories?
            .Where(tuple => typeof(TMiddleware).IsAssignableFrom(tuple.Item1))
            .Select(tuple => tuple.Item2())
            .ToList();
      }

      /// <summary>
      /// Run the middlewares on the data.
      /// </summary>
      /// <param name="callType">Call type: Request_VM, Update_VM or Response_VM.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="data">Call data.</param>
      /// <param name="exceptionSent">Whether the exception should be sent to the client.</param>
      /// <returns>Hub context data.</returns>
      public T RunMiddlewares<T>(HubCallerContext context, string callType, string vmId, T data, NextDelegate finalAction) where T : class
      {
         var nextActions = new Stack<NextDelegate>();
         nextActions.Push(finalAction);
         foreach (IMiddleware middleware in GetMiddlewares<IMiddleware>().Reverse<IMiddlewarePipeline>())
            nextActions.Push(ctx => middleware.Invoke(ctx, nextActions.Pop()));

         var hubContext = new DotNetifyHubContext(context, callType, vmId, data, null, context.User);
         if (nextActions.Count > 0)
            nextActions.Pop()(hubContext);

         return hubContext.Data as T;
      }

      /// <summary>
      /// Runs the middlewares that hook to client disconnected event.
      /// </summary>
      /// <param name="context">SignalR hub context.</param>
      public void RunDisconnectionMiddlewares(HubCallerContext context)
      {
         try
         {
            GetMiddlewares<IDisconnectionMiddleware>().ToList().ForEach(middleware => (middleware as IDisconnectionMiddleware).OnDisconnected(context));
         }
         catch(Exception ex)
         {
            Trace.Fail(ex.ToString());
         }
      }

      /// <summary>
      /// Runs the middlewares that hook to exception thrown event.
      /// </summary>
      /// <param name="context">SignalR hub context.</param>
      public Exception RunExceptionMiddleware(HubCallerContext context, Exception exception)
      {
         Exception finalException = exception;
         try
         {
            foreach (IExceptionMiddleware middleware in GetMiddlewares<IExceptionMiddleware>())
               finalException = middleware.OnException(context, finalException).Result;
         }
         catch (Exception ex)
         {
            finalException = ex;
         }
         return finalException;
      }

      /// <summary>
      /// Runs the view model filter.
      /// </summary>
      /// <param name="callType">Call type: Request_VM, Update_VM or Response_VM.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vm">View model instance.</param>
      /// <param name="data">Call data.</param>
      public void RunVMFilters<T>(HubCallerContext context, string callType, string vmId, BaseVM vm, ref T data, IPrincipal principal) where T : class
      {
         var vmContext = new VMContext(new DotNetifyHubContext(context, callType, vmId, data, null, principal), vm);

         // Find and execute the filter that matches each view model class attribute.
         foreach (var attr in vm.GetType().GetTypeInfo().GetCustomAttributes())
         {
            var vmFilterType = typeof(IVMFilter<>).MakeGenericType(attr.GetType());
            if (_vmFilterFactories.Keys.Any(t => vmFilterType.IsAssignableFrom(t)))
            {
               var vmFilter = _vmFilterFactories.FirstOrDefault(kvp => vmFilterType.IsAssignableFrom(kvp.Key)).Value();
               vmFilterType.GetMethod(nameof(IVMFilter<Attribute>.OnExecuting))?.Invoke(vmFilter, new object[] { attr, vmContext });
               data = vmContext.HubContext.Data as T;
            }
         }
      }
   }
}
