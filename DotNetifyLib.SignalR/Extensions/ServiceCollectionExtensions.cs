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
using Microsoft.Extensions.DependencyInjection;
using DotNetify.Security;

namespace DotNetify
{
   public static class ServiceCollectionExtensions
   {
      public static IServiceCollection AddDotNetify(this IServiceCollection services)
      {
         // Add memory cache.
         services.AddSingleton<IMemoryCache, MemoryCacheAdapter>();

         // Add view model controller factory, to be injected to dotNetify's signalR hub.
         services.AddSingleton<IVMControllerFactory, VMControllerFactory>();

         // Add the dependency injection service scope factory for view model controllers.
         services.AddSingleton<IVMServiceScopeFactory, VMServiceScopeFactory>();

         // Add service to get the hub principal.
         services.AddSingleton<IPrincipalAccessor, HubPrincipalAccessor>();

         // Add service to get the service provider for the view models.
         services.AddSingleton<IHubServiceProvider, HubServiceProvider>();

         // Add service to run middleware and filters.
         services.AddTransient<IHubPipeline, HubPipeline>();

         // Add middleware and filter factories.
         services.AddSingleton<IList<Tuple<Type, Func<IMiddlewarePipeline>>>>(p => new List<Tuple<Type, Func<IMiddlewarePipeline>>>());
         services.AddSingleton<IDictionary<Type, Func<IVMFilter>>>(p => new Dictionary<Type, Func<IVMFilter>>());

         return services;
      }
   }
}