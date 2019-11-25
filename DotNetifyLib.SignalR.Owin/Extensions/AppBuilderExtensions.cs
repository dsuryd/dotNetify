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
using System.Diagnostics;
using DotNetify.Security;

namespace DotNetify
{
   public static class AppBuilderExtensions
   {
      private readonly static List<Tuple<Type, object[]>> _middlewareTypes = new List<Tuple<Type, object[]>>();
      private readonly static List<Tuple<Type, object[]>> _filterTypes = new List<Tuple<Type, object[]>>();

      public static IApplicationBuilder UseDotNetify(this IApplicationBuilder appBuilder, Action<IDotNetifyConfiguration> config = null)
      {
         var provider = appBuilder.ApplicationServices;

         var vmControllerFactory = provider.GetService<IVMControllerFactory>();
         if (vmControllerFactory == null)
            throw new InvalidOperationException("Please call 'IServiceCollection.AddDotNetify()' inside the ConfigureServices() of the startup class.");

         // Use ASP.NET Core DI to provide view model instances by default.
         Func<Type, object[], object> factoryMethod = (type, args) =>
         {
            try
            {
               return ActivatorUtilities.CreateInstance(provider, type, args ?? new object[] { });
            }
            catch (Exception ex)
            {
               Trace.Fail(ex.Message);
               throw ex;
            }
         };

         var dotNetifyConfig = new DotNetifyConfiguration();
         dotNetifyConfig.SetFactoryMethod(factoryMethod);

         config?.Invoke(dotNetifyConfig);

         // Sets how long to keep a view model controller in memory after it hasn't been accessed for a while.
         if (dotNetifyConfig.VMControllerCacheExpiration.HasValue)
            vmControllerFactory.CacheExpiration = dotNetifyConfig.VMControllerCacheExpiration;

         // Add middleware to extract headers from incoming requests.
         _middlewareTypes.Insert(0, Tuple.Create(typeof(ExtractHeadersMiddleware), new object[] { }));

         // Add middleware factories to the hub.
         var middlewareFactories = provider.GetService<IList<Tuple<Type, Func<IMiddlewarePipeline>>>>();
         _middlewareTypes.ForEach(t => middlewareFactories?.Add(Tuple.Create<Type, Func<IMiddlewarePipeline>>(t.Item1, () => (IMiddlewarePipeline) factoryMethod(t.Item1, t.Item2))));

         // Add filter factories to the hub.
         var filterFactories = provider.GetService<IDictionary<Type, Func<IVMFilter>>>();
         _filterTypes.ForEach(t => filterFactories?.Add(t.Item1, () => (IVMFilter) factoryMethod(t.Item1, t.Item2)));

         return appBuilder;
      }

      public static void UseMiddleware<T>(this IDotNetifyConfiguration dotNetifyConfig, params object[] args) where T : IMiddlewarePipeline => _middlewareTypes.Add(Tuple.Create(typeof(T), args));

      public static void UseFilter<T>(this IDotNetifyConfiguration dotNetifyConfig, params object[] args) where T : IVMFilter => _filterTypes.Add(Tuple.Create(typeof(T), args));
   }
}