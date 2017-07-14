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
using Microsoft.Extensions.Caching.Memory;

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
      private readonly IMemoryCache _controllersCache;

      /// <summary>
      /// How long to keep a view model controller in memory after it hasn't been accessed for a while. Default to never expire.
      /// </summary>
      public TimeSpan? CacheExpiration { get; set; }

      /// <summary>
      /// Delegate to return the response back to the client.
      /// </summary>
      public VMController.VMResponseDelegate ResponseDelegate { get; set; }

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="memoryCache">Memory cache for storing the view model controllers.</param>
      public VMControllerFactory(IMemoryCache memoryCache)
      {
         if (memoryCache == null)
            throw new ArgumentNullException("No service of type IMemoryCache has been registered.");

         _controllersCache = memoryCache;
      }

      /// <summary>
      /// Creates a view model controller and assigns it a key. 
      /// On subsequent calls, use the same key to return the same object.
      /// </summary>
      /// <param name="key">Identifies the object.</param>
      /// <returns>View model controller.</returns>
      public VMController GetInstance(string key)
      {
         var cache = _controllersCache;

         Lazy<VMController> cachedValue;
         if (!cache.TryGetValue(key, out cachedValue))
         {
            cachedValue = new Lazy<VMController>(() => new VMController(ResponseDelegate));
            cache.Set(key, cachedValue, GetCacheEntryOptions());
         }
         return cachedValue?.Value;
      }

      /// <summary>
      /// Removes an existing view model controller.
      /// </summary>
      /// <param name="key">Identifies the object.</param>
      /// <returns>True if the object was removed.</returns>
      public bool Remove(string key)
      {
         var cache = _controllersCache;
         object value;
         if (cache.TryGetValue(key, out value))
         {
            cache.Remove(key);
            return true;
         }
         return false;
      }

      /// <summary>
      /// Returns cached entry options for view model controllers.
      /// </summary>
      /// <returns>Cache entry options.</returns>
      private MemoryCacheEntryOptions GetCacheEntryOptions()
      {
         var options = new MemoryCacheEntryOptions()
            .RegisterPostEvictionCallback((key, value, reason, substate) => ((value as Lazy<VMController>).Value as IDisposable).Dispose());

         if (CacheExpiration.HasValue)
            options.SetSlidingExpiration(CacheExpiration.Value);

         return options;
      }
   }
}
