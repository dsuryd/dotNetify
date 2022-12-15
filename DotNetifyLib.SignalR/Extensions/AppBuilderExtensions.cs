/*
Copyright 2017-2022 Dicky Suryadi

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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DotNetify.Forwarding;
using DotNetify.Routing;
using DotNetify.Security;
using DotNetify.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNetify
{
   public static class AppBuilderExtensions
   {
      private static readonly List<Tuple<Type, object[]>> _middlewareTypes = new List<Tuple<Type, object[]>>();
      private static readonly List<Tuple<Type, object[]>> _filterTypes = new List<Tuple<Type, object[]>>();
      private static bool _useDotNetify;

      /// <summary>
      /// Includes dotNetify in the application request pipeline.
      /// </summary>
      /// <param name="appBuilder">Application builder.</param>
      /// <param name="config">Delegate to provide custom configuration.</param>
      public static IApplicationBuilder UseDotNetify(this IApplicationBuilder appBuilder, Action<IDotNetifyConfiguration> config = null)
      {
         var provider = appBuilder.ApplicationServices;

         Logger.Init(provider.GetService<ILoggerFactory>());

         var vmControllerFactory = provider.GetService<IVMControllerFactory>();
         if (vmControllerFactory == null)
            throw new InvalidOperationException("Please add the required service by calling 'IServiceCollection.AddDotNetify()' in the application startup.");

         var scopedServiceProvider = provider.GetService<IHubServiceProvider>();

         // Use ASP.NET Core DI to provide view model instances by default.
         Func<Type, object[], object> factoryMethod = (type, args) =>
         {
            try
            {
               return scopedServiceProvider.FactoryMethod?.Invoke(type, args) ?? ActivatorUtilities.CreateInstance(provider, type, args ?? new object[] { });
            }
            catch (Exception ex)
            {
               Logger.LogError($"Failed to create instance of '{type}': {ex.Message}");
               throw ex;
            }
         };

         var dotNetifyConfig = new DotNetifyConfiguration();
         dotNetifyConfig.SetFactoryMethod(factoryMethod);

         config?.Invoke(dotNetifyConfig);

         // Sets how long to keep a view model controller in memory after it hasn't been accessed for a while.
         if (dotNetifyConfig.VMControllerCacheExpiration.HasValue)
         {
            vmControllerFactory.CacheExpiration = dotNetifyConfig.VMControllerCacheExpiration;

            var webApiVMControllerFactory = provider.GetService<IWebApiVMControllerFactory>();
            webApiVMControllerFactory.CacheExpiration = dotNetifyConfig.VMControllerCacheExpiration;
         }

         // Add middleware to extract headers from incoming requests.
         if (!_middlewareTypes.Exists(t => t.Item1 == typeof(ExtractHeadersMiddleware)))
         {
            // Place the middleware after any forwarding middleware to ensure it forwards unprocessed data.
            int pos = _middlewareTypes.FindLastIndex(x => typeof(IForwardingMiddleware).IsAssignableFrom(x.Item1)) + 1;
            _middlewareTypes.Insert(pos, Tuple.Create(typeof(ExtractHeadersMiddleware), new object[] { }));
         }

         // Add middleware factories to the hub.
         var middlewareFactories = provider.GetService<IList<Tuple<Type, Func<IMiddlewarePipeline>>>>();
         _middlewareTypes.ForEach(t => middlewareFactories?.Add(Tuple.Create<Type, Func<IMiddlewarePipeline>>(t.Item1, () => (IMiddlewarePipeline) factoryMethod(t.Item1, t.Item2))));

         // Add filter factories to the hub.
         var filterFactories = provider.GetService<IDictionary<Type, Func<IVMFilter>>>();
         _filterTypes.ForEach(t => filterFactories?.Add(t.Item1, () => (IVMFilter) factoryMethod(t.Item1, t.Item2)));

         _useDotNetify = true;
         return appBuilder;
      }

      /// <summary>
      /// Includes a middleware to the dotNetify's pipeline.
      /// </summary>
      /// <param name="dotNetifyConfig">DotNetify configuration.</param>
      /// <param name="args">Middleware arguments.</param>
      public static void UseMiddleware<T>(this IDotNetifyConfiguration _, params object[] args) where T : IMiddlewarePipeline
      {
         if (!_middlewareTypes.Any(x => x.Item1 == typeof(T) && x.Item2 == args))
            _middlewareTypes.Add(Tuple.Create(typeof(T), args));
      }

      /// <summary>
      /// Includes a view model filter to the dotNetify's pipeline.
      /// </summary>
      /// <param name="dotNetifyConfig">DotNetify configuration.</param>
      /// <param name="args">View model filter arguments.</param>
      public static void UseFilter<T>(this IDotNetifyConfiguration _, params object[] args) where T : IVMFilter
      {
         if (!_filterTypes.Any(x => x.Item1 == typeof(T) && x.Item2 == args))
            _filterTypes.Add(Tuple.Create(typeof(T), args));
      }

      /// <summary>
      /// Includes server-side rendering in the application request pipeline.
      /// </summary>
      /// <param name="appBuilder">Application builder.</param>
      /// <param name="mainVMType">Main application view model type.</param>
      /// <param name="nodeJSInvokeAsync">Function to invoke javascript in NodeJS.</param>
      /// <param name="onError">Request delegate to handle failure.</param>
      public static IApplicationBuilder UseSsr(this IApplicationBuilder appBuilder, Type mainVMType, Func<string[], Task<string>> nodeJSInvokeAsync, RequestDelegate onError)
      {
         var vmFactory = appBuilder.ApplicationServices.GetRequiredService<IVMFactory>();
         appBuilder.UseWhen(context => !Path.HasExtension(context.Request.Path.Value), app =>
         {
            app.Run(async context =>
            {
               try
               {
                  // Server-side rendering.
                  string path = context.Request.Path.Value;
                  string ssrStates = ServerSideRender.GetInitialStates(vmFactory, ref path, mainVMType);

                  string userAgent = context.Request.Headers["User-Agent"];
                  string result = await nodeJSInvokeAsync(new string[] { userAgent, path, ssrStates });
                  await context.Response.WriteAsync(result);
               }
               catch (Exception ex)
               {
                  Logger.LogError($"SSR exception: {ex.Message}");
                  await onError(context);
               }
            });
         });
         return appBuilder;
      }

      /// <summary>
      /// Adds a dynamic dotNetify view model.
      /// </summary>
      /// <param name="app">Application builder.</param>
      /// <param name="vmName">View model name.</param>
      /// <param name="propertyBuilder">Delegate that returns an object for building the view model's properties.</param>
      public static IApplicationBuilder MapVM(this IApplicationBuilder app, string vmName, Delegate propertyBuilder) => MapVM<BaseVM>(app, vmName, propertyBuilder);

      /// <summary>
      /// Adds a dynamic dotNetify multicast view model.
      /// </summary>
      /// <param name="app">Application builder.</param>
      /// <param name="vmName">View model name.</param>
      /// <param name="propertyBuilder">Delegate that returns an object for building the view model's properties.</param>
      public static IApplicationBuilder MapMulticastVM(this IApplicationBuilder app, string vmName, Delegate propertyBuilder) => MapVM<MulticastVM>(app, vmName, propertyBuilder);

      /// <summary>
      /// Adds a dynamic dotNetify view model.
      /// </summary>
      /// <param name="app">Application builder.</param>
      /// <param name="vmName">View model name.</param>
      /// <param name="propertyBuilder">Delegate that returns an object for building the view model's properties.</param>
      private static IApplicationBuilder MapVM<T>(this IApplicationBuilder app, string vmName, Delegate propertyBuilder) where T : BaseVM, new()
      {
         vmName = !string.IsNullOrWhiteSpace(vmName) ? vmName : throw new ArgumentNullException(nameof(vmName));
         propertyBuilder = propertyBuilder ?? throw new ArgumentNullException(nameof(propertyBuilder));

         // Make sure "UseDotNetify()" is called first.
         if (!_useDotNetify)
         {
            app.UseWebSockets();
            app.UseDotNetify(config => config.UseFilter<AuthorizeFilter>());
         }

         VMController.Register(vmName, _ =>
         {
            using (var scope = app.ApplicationServices.CreateScope())
            {
               var args = new List<object>();
               foreach (var parameter in propertyBuilder.Method.GetParameters())
               {
                  var instance = scope.ServiceProvider.GetRequiredService(parameter.ParameterType);
                  args.Add(instance);
               }

               var propertySource = propertyBuilder.DynamicInvoke(args.ToArray());
               if (propertySource is Task)
               {
                  var resultProperty = propertySource.GetType().GetProperty("Result");
                  propertySource = resultProperty.GetValue(propertySource);
               }

               return VMBuilder.Build<T>(propertySource, propertyBuilder.Method.GetCustomAttributes());
            }
         });

         return app;
      }
   }
}