/*
Copyright 2017-2020 Dicky Suryadi

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
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using DotNetify.Client;
using DotNetify.Forwarding;
using DotNetify.Security;
using DotNetify.WebApi;

namespace DotNetify
{
   public static class ServiceCollectionExtensions
   {
      public static IServiceCollection AddDotNetifyCore(this IServiceCollection services)
      {
         // Add memory cache.
         if (!services.Any(x => x.ServiceType == typeof(Microsoft.Extensions.Caching.Memory.IMemoryCache)))
            services.AddMemoryCache();
         services.AddSingleton<IMemoryCache, MemoryCacheAdapter>();

         // Add view model controller factory, to be injected to dotNetify's signalR hub.
         services.AddSingleton<IVMControllerFactory, VMControllerFactory>();

         // Add view model factory.
         services.AddSingleton<IVMTypesAccessor, VMTypesAccessor>();
         services.AddSingleton<IVMFactory, VMFactory>();

         // Add the dependency injection service scope factory for view model controllers.
         services.AddSingleton<IVMServiceScopeFactory, VMServiceScopeFactory>();

         // Add service to handle hub messages.
         services.AddTransient<IDotNetifyHubHandler, DotNetifyHubHandler>();
         services.AddTransient<IDotNetifyHubResponse, DotNetifyHubResponse>();
         services.AddSingleton<IDotNetifyHubResponseManager, DotNetifyHubResponseManager>();

         // Add service to get the hub principal and the associated connection context.
         services.AddSingleton<IPrincipalAccessor, HubInfoAccessor>();
         services.AddSingleton(x => x.GetService<IPrincipalAccessor>() as IDotNetifyHubContextAccessor);
         services.AddSingleton(x => x.GetService<IPrincipalAccessor>() as IHubCallerContextAccessor);
         services.AddSingleton(x => x.GetService<IPrincipalAccessor>() as IConnectionContext);

         // Add service to get the service provider for the view models.
         services.AddSingleton<IHubServiceProvider, HubServiceProvider>();

         // Add service to run middleware and filters.
         services.AddTransient<IHubPipeline, HubPipeline>();

         // Add middleware and filter factories.
         services.AddSingleton<IList<Tuple<Type, Func<IMiddlewarePipeline>>>>(p => new List<Tuple<Type, Func<IMiddlewarePipeline>>>());
         services.AddSingleton<IDictionary<Type, Func<IVMFilter>>>(p => new Dictionary<Type, Func<IVMFilter>>());

         // Add factories used for hub forwarding.
         services.AddSingleton<IDotNetifyHubProxyFactory, DotNetifyHubProxyFactory>();
         services.AddSingleton<IDotNetifyHubForwarderFactory, DotNetifyHubForwarderFactory>();
         services.AddSingleton<IDotNetifyHubForwardResponseFactory, DotNetifyHubForwardResponseFactory>();

         return services;
      }

      public static IServiceCollection AddDotNetify(this IServiceCollection services)
      {
         services.AddDotNetifyCore()
            // Add web API support.
            .AddMvcCore().AddApplicationPart(typeof(DotNetifyWebApi).Assembly).AddControllersAsServices();
         services.AddTransient<WebApiVMControllerFactory>();
         return services;
      }

      public static IServiceCollection AddDotNetifyClient(this IServiceCollection services)
      {
         services.AddSingleton<IDotNetifyHubProxyFactory, DotNetifyHubProxyFactory>();
         services.AddSingleton<IDotNetifyHubProxy, DotNetifyHubProxy>();
         services.AddTransient<IDotNetifyClient, DotNetifyClient>();
         services.AddSingleton<IUIThreadDispatcher, DefaultUIThreadDispatcher>();

         return services;
      }
   }
}