using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using Owin;
using TinyIoC;
using DotNetify.Security;

namespace DotNetify
{
   public class ServiceProvider
   {
      private static readonly IList<Tuple<Type, Func<IMiddlewarePipeline>>> middlewares = new List<Tuple<Type, Func<IMiddlewarePipeline>>>();
      private static readonly IDictionary<Type, Func<IVMFilter>> filters = new Dictionary<Type, Func<IVMFilter>>();

      static ServiceProvider()
      {
         var container = TinyIoCContainer.Current;

         container.Register<IMemoryCache, MemoryCacheAdapter>().AsSingleton();
         container.Register<IVMControllerFactory, VMControllerFactory>().AsSingleton();
         container.Register<IPrincipalAccessor, HubPrincipalAccessor>().AsMultiInstance();
         container.Register<IHubPipeline, HubPipeline>().AsMultiInstance();
         container.Register<IList<Tuple<Type, Func<IMiddlewarePipeline>>>>(middlewares);
         container.Register<IDictionary<Type, Func<IVMFilter>>>(filters);
      }

      public T GetService<T>() where T : class => Get<T>();

      public static T Get<T>() where T : class
      {
         var container = TinyIoCContainer.Current;
         return container.Resolve<T>();
      }
   }

   public class ActivatorUtilities
   {
      public static object CreateInstance(ServiceProvider provider, Type type, object[] args) => CreateInstance(type, args);

      public static object CreateInstance(Type type, object[] args)
      {
         var container = TinyIoCContainer.Current;
         return container.Resolve(type) ?? Activator.CreateInstance(type, args);
      }
   }

   public static class AdapterExtension
   {
      public static IAppBuilder UseDotNetify(this IAppBuilder appBuilder, Action<IDotNetifyConfiguration> config)
      {
         var builder = new IApplicationBuilder(appBuilder);
         builder.UseDotNetify(config);
         return appBuilder;
      }
   }

   public interface IMemoryCache
   {
      bool TryGetValue<T>(string key, out T cachedValue) where T : class;
      object Get(string key);
      void Set<T>(string key, T cachedValue, MemoryCacheEntryOptions options = null) where T : class;
      void Remove(string key);
   }

   public class MemoryCacheEntryOptions
   {
      private TimeSpan _slidingExpiration;
      private Action<string, object, object, object> _callback;

      public void SetSlidingExpiration(TimeSpan value) => _slidingExpiration = value;

      public MemoryCacheEntryOptions RegisterPostEvictionCallback(Action<string, object, object, object> callback)
      {
         _callback = callback;
         return this;
      }

      public CacheItemPolicy GetCacheItemPolicy()
      {
         return new CacheItemPolicy
         {
            SlidingExpiration = _slidingExpiration,
            RemovedCallback = i => _callback(i.CacheItem.Key, i.CacheItem.Value, null, null)
         };
      }
   }

   internal class MemoryCacheAdapter : IMemoryCache
   {
      private MemoryCache _cache = new MemoryCache("DotNetify");

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
         _cache.AddOrGetExisting(key, cachedValue, options?.GetCacheItemPolicy());
      }

      public void Remove(string key) => _cache.Remove(key);
   }

   public class IApplicationBuilder
   {
      private readonly IAppBuilder _appBuilder;

      public ServiceProvider ApplicationServices => new ServiceProvider();

      public IApplicationBuilder(IAppBuilder appBuilder)
      {
         _appBuilder = appBuilder;
      }
   }
}
