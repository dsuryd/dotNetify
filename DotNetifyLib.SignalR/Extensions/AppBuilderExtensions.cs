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
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetify
{
   public static class AppBuilderExtensions
   {
      private static List<Type> _middlewareTypes = new List<Type>();

      public static IApplicationBuilder UseDotNetify(this IApplicationBuilder appBuilder, Action<IDotNetifyConfiguration> config = null)
      {
         var provider = appBuilder.ApplicationServices;

         // Make sure all the required services are there.
         if (provider.GetService<IMemoryCache>() == null)
            throw new InvalidOperationException("No service of type IMemoryCache has been registered. Please add the service by calling 'IServiceCollection.AddMemoryCache()' in the startup class.");

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

         // If no view model assembly has been registered, default to the entry assembly.
         if (!dotNetifyConfig.HasAssembly)
            dotNetifyConfig.RegisterEntryAssembly();

         // Sets how long to keep a view model controller in memory after it hasn't been accessed for a while.
         if (dotNetifyConfig.VMControllerCacheExpiration.HasValue)
            vmControllerFactory.CacheExpiration = dotNetifyConfig.VMControllerCacheExpiration;

         // Add middleware factories to the hub.
         var middlewareFactories = provider.GetService<IList<Func<IMiddleware>>>();
         _middlewareTypes.ForEach(t => middlewareFactories?.Add(() => (IMiddleware)factoryMethod(t, null)));

         return appBuilder;
      }

      public static void UseMiddleware<T>(this IDotNetifyConfiguration dotNetifyConfig) where T : IMiddleware => _middlewareTypes.Add(typeof(T));
   }
}
