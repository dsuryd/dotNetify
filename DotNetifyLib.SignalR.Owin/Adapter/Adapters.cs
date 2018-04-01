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

using Owin;
using System;
using System.Runtime.Caching;

namespace DotNetify
{
   public static class AdapterExtension
   {
      public static IAppBuilder UseDotNetify(this IAppBuilder appBuilder, Action<IDotNetifyConfiguration> config = null, IServiceProvider serviceProvider = null)
      {
         serviceProvider = serviceProvider ?? new TinyIoCServiceProvider();
         var builder = new ApplicationBuilder(appBuilder, serviceProvider);

         // Register dotNetify dependencies to the provided DI container.
         builder.ApplicationServices.AddSingleton<IMemoryCache, MemoryCacheAdapter>();
         builder.ApplicationServices.AddSingleton<IVMServiceScopeFactory>(_ => new ServiceScopeFactoryAdapter(serviceProvider));
         builder.ApplicationServices.AddDotNetify();

         // Configure dotNetify.
         builder.UseDotNetify(config);
         return appBuilder;
      }

      public static IAppBuilder UseDotNetify(this IAppBuilder appBuilder, IServiceProvider serviceProvider) => appBuilder.UseDotNetify(null, serviceProvider);
   }

   public interface IApplicationBuilder
   {
      IServiceProvider ApplicationServices { get; }
   }

   public class ApplicationBuilder : IApplicationBuilder
   {
      private readonly IAppBuilder _appBuilder;
      private readonly IServiceProvider _serviceProvider;

      public IServiceProvider ApplicationServices => _serviceProvider;

      public ApplicationBuilder(IAppBuilder appBuilder, IServiceProvider serviceProvider)
      {
         _appBuilder = appBuilder;
         _serviceProvider = serviceProvider;
         ActivatorUtilities.ServiceProvider = serviceProvider;
      }
   }

   /// <summary>
   /// Implements memory cache.
   /// </summary>
   public class MemoryCacheAdapter : IMemoryCache
   {
      private MemoryCache _cache = new MemoryCache("DotNetify");

      public event EventHandler<string> Removed;

      public bool TryGetValue<T>(string key, out T cachedValue) where T : class
      {
         cachedValue = null;
         var cacheItem = _cache.GetCacheItem(key);
         if (cacheItem != null)
            cachedValue = cacheItem.Value as T;
         return cachedValue != null;
      }

      public object Get(string key) => _cache.Get(key);

      public void Set<T>(string key, T cachedValue, MemoryCacheEntryOptions options = null) where T : class
      {
         var policy = new CacheItemPolicy();
         if (options != null)
         {
            policy.RemovedCallback = i => options.Callback(i.CacheItem.Key, i.CacheItem.Value, null, null);
            if (options.SlidingExpiration.HasValue)
               policy.SlidingExpiration = options.SlidingExpiration.Value;
         }

         _cache.Set(key, cachedValue, policy);
      }

      public void Remove(string key)
      {
         _cache.Remove(key);
         Removed?.Invoke(this, key);
      }
   }

   /// <summary>
   /// Implements the dependency injection service scope.
   /// </summary>
   public class ServiceScopeFactoryAdapter : IVMServiceScopeFactory
   {
      private readonly System.IServiceProvider _serviceProvider;

      internal class ServiceScope : IVMServiceScope
      {
         private readonly System.IServiceProvider _serviceProvider;

         public ServiceScope(System.IServiceProvider serviceProvider)
         {
            _serviceProvider = serviceProvider;
         }

         public System.IServiceProvider ServiceProvider => _serviceProvider;

         public void Dispose()
         {
         }
      }

      public ServiceScopeFactoryAdapter(System.IServiceProvider serviceProvider)
      {
         _serviceProvider = serviceProvider;
      }

      public IVMServiceScope CreateScope() => new ServiceScope(_serviceProvider);
   }
}