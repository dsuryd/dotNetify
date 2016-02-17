/* 
Copyright 2016 Dicky Suryadi

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
using System.Runtime.Caching;

namespace DotNetify
{
   /// <summary>
   /// Provides view model controllers.
   /// </summary>
   public class VMControllerFactory : IVMControllerFactory
   {
      /// <summary>
      /// View model controllers by the client connection Ids.
      /// </summary>
      private readonly Lazy<MemoryCache> _controllersCache = new Lazy<MemoryCache>(() => new MemoryCache("DotNetify"));

      /// <summary>
      /// How long to keep a view model controller in memory after it hasn't been accessed for a while.
      /// </summary>
      public TimeSpan CacheExpiration { get; set; } = new TimeSpan(0, 20, 0);

      /// <summary>
      /// Singleton; used for when there is no dependency injection.
      /// </summary>
      public static VMControllerFactory Singleton { get; } = new VMControllerFactory();

      /// <summary>
      /// Creates a view model controller and assigns it a key. 
      /// On subsequent calls, use the same key to return the same object.
      /// </summary>
      /// <param name="key">Identifies the object.</param>
      /// <returns>View model controller.</returns>
      public VMController GetInstance(string key)
      {
         var cache = _controllersCache.Value;
         var newValue = new Lazy<VMController>(() => new VMController(DotNetifyHub.Response_VM));
         var cachedValue = cache.AddOrGetExisting(key, newValue, GetCacheItemPolicy()) as Lazy<VMController>;

         return cachedValue == null ? newValue.Value : cachedValue.Value;
      }

      /// <summary>
      /// Removes an existing view model controller.
      /// </summary>
      /// <param name="key">Identifies the object.</param>
      /// <returns>True if the object was removed.</returns>
      public bool Remove(string key)
      {
         var cache = _controllersCache.Value;
         if (cache.Contains(key))
         {
            cache.Remove(key);
            return true;
         }
         return false;
      }

      /// <summary>
      /// Returns cached item policy for view model controllers.
      /// </summary>
      /// <returns>Cache item policy.</returns>
      private CacheItemPolicy GetCacheItemPolicy()
      {
         return new CacheItemPolicy
         {
            SlidingExpiration = CacheExpiration,
            RemovedCallback = i => ((i.CacheItem.Value as Lazy<VMController>).Value as IDisposable).Dispose()
         };
      }
   }
}
