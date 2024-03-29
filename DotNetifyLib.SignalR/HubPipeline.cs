﻿/*
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DotNetify
{
   /// <summary>
   /// Provides invocation of middlewares and view model filters.
   /// </summary>
   public interface IHubPipeline
   {
      void RunMiddlewares(DotNetifyHubContext hubContext, NextDelegate finalAction);

      Task RunMiddlewaresAsync(DotNetifyHubContext hubContext, NextDelegate finalAction);

      void RunDisconnectionMiddlewares(HubCallerContext context);

      Task RunDisconnectionMiddlewaresAsync(HubCallerContext context);

      void RunVMFilters(DotNetifyHubContext hubContext, BaseVM vm, NextFilterDelegate finalFilter);

      Task RunVMFiltersAsync(DotNetifyHubContext hubContext, BaseVM vm, NextFilterDelegate finalFilter);

      Exception RunExceptionMiddleware(HubCallerContext context, Exception exception);

      Task<Exception> RunExceptionMiddlewareAsync(HubCallerContext context, Exception exception);
   }

   public class HubPipeline : IHubPipeline
   {
      private readonly IList<Tuple<Type, Func<IMiddlewarePipeline>>> _middlewareFactories;
      private readonly IDictionary<Type, Func<IVMFilter>> _vmFilterFactories;

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="middlewareFactories">Middlewares to intercept view model actions on a global level.</param>
      /// /// <param name="vmFilterFactories">View model filters to intercept view model actions on a view model level.</param>
      public HubPipeline(IList<Tuple<Type, Func<IMiddlewarePipeline>>> middlewareFactories, IDictionary<Type, Func<IVMFilter>> vmFilterFactories)
      {
         _middlewareFactories = middlewareFactories;
         _vmFilterFactories = vmFilterFactories;
      }

      private List<IMiddlewarePipeline> GetMiddlewares<TMiddleware>() where TMiddleware : IMiddlewarePipeline
      {
         return _middlewareFactories?
            .Where(tuple => typeof(TMiddleware).GetTypeInfo().IsAssignableFrom(tuple.Item1))
            .Select(tuple => tuple.Item2())
            .ToList();
      }

      /// <summary>
      /// Run the middlewares on the data.
      /// </summary>
      /// <param name="hubContext">Hub context.</param>
      /// <param name="finalAction">The last action to do after running all the middlewares.</param>
      [Obsolete]
      public void RunMiddlewares(DotNetifyHubContext hubContext, NextDelegate finalAction)
      {
         _ = RunMiddlewaresAsync(hubContext, finalAction);
      }

      /// <summary>
      /// Run the middlewares on the data.
      /// </summary>
      /// <param name="hubContext">Hub context.</param>
      /// <param name="finalAction">The last action to do after running all the middlewares.</param>
      public async Task RunMiddlewaresAsync(DotNetifyHubContext hubContext, NextDelegate finalAction)
      {
         var nextMiddlewares = new Stack<NextDelegate>();
         nextMiddlewares.Push(finalAction);
         foreach (IMiddleware middleware in GetMiddlewares<IMiddleware>().Reverse<IMiddlewarePipeline>())
            nextMiddlewares.Push(ctx => middleware.Invoke(ctx, nextMiddlewares.Pop()));

         if (nextMiddlewares.Count > 0)
            await nextMiddlewares.Pop()(hubContext);
      }

      /// <summary>
      /// Runs the middlewares that hook to client disconnected event.
      /// </summary>
      /// <param name="context">SignalR hub context.</param>
      [Obsolete]
      public void RunDisconnectionMiddlewares(HubCallerContext context)
      {
         _ = RunDisconnectionMiddlewaresAsync(context);
      }

      /// <summary>
      /// Runs the middlewares that hook to client disconnected event.
      /// </summary>
      /// <param name="context">SignalR hub context.</param>
      public async Task RunDisconnectionMiddlewaresAsync(HubCallerContext context)
      {
         try
         {
            foreach (IDisconnectionMiddleware middleware in GetMiddlewares<IDisconnectionMiddleware>())
               await middleware.OnDisconnected(context);
         }
         catch (Exception ex)
         {
            Logger.LogError($"Disconnection middleware exception: {ex.Message}");
         }
      }

      /// <summary>
      /// Runs the middlewares that hook to exception thrown event.
      /// </summary>
      /// <param name="context">SignalR hub context.</param>
      [Obsolete]
      public Exception RunExceptionMiddleware(HubCallerContext context, Exception exception)
      {
         return RunExceptionMiddlewareAsync(context, exception).ConfigureAwait(true).GetAwaiter().GetResult();
      }

      /// <summary>
      /// Runs the middlewares that hook to exception thrown event.
      /// </summary>
      /// <param name="context">SignalR hub context.</param>
      public async Task<Exception> RunExceptionMiddlewareAsync(HubCallerContext context, Exception exception)
      {
         Exception finalException = exception;
         try
         {
            foreach (IExceptionMiddleware middleware in GetMiddlewares<IExceptionMiddleware>())
               finalException = await middleware.OnException(context, finalException);
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
      /// <param name="hubContext">Hub context.</param>
      /// <param name="vm">View model instance.</param>
      [Obsolete]
      public void RunVMFilters(DotNetifyHubContext hubContext, BaseVM vm, NextFilterDelegate finalFilter)
      {
         _ = RunVMFiltersAsync(hubContext, vm, finalFilter);
      }

      /// <summary>
      /// Runs the view model filter.
      /// </summary>
      /// <param name="hubContext">Hub context.</param>
      /// <param name="vm">View model instance.</param>
      public async Task RunVMFiltersAsync(DotNetifyHubContext hubContext, BaseVM vm, NextFilterDelegate finalFilter)
      {
         var nextFilters = new Stack<NextFilterDelegate>();
         nextFilters.Push(finalFilter);

         // Find and execute the filter that matches each view model class attribute.
         foreach (var attr in vm.CustomAttributes.Reverse())
         {
            var vmFilterType = typeof(IVMFilter<>).GetTypeInfo().MakeGenericType(attr.GetType());
            if (_vmFilterFactories.Keys.Any(t => vmFilterType.GetTypeInfo().IsAssignableFrom(t)))
            {
               var vmFilter = _vmFilterFactories.FirstOrDefault(kvp => vmFilterType.GetTypeInfo().IsAssignableFrom(kvp.Key)).Value();
               var vmFilterInvokeMethod = vmFilterType.GetTypeInfo().GetMethod(nameof(IVMFilter<Attribute>.Invoke));
               if (vmFilterInvokeMethod != null)
                  nextFilters.Push(ctx => (Task) vmFilterInvokeMethod.Invoke(vmFilter, new object[] { attr, ctx, nextFilters.Pop() }));
            }
         }

         var vmContext = new VMContext(hubContext, vm);
         if (nextFilters.Count > 0)
            await nextFilters.Pop()(vmContext);
      }
   }
}