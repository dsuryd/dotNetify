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

using Microsoft.Extensions.Caching.Memory;
using ms = Microsoft.Extensions.Caching.Memory;

namespace DotNetify
{
   /// <summary>
   /// Implements IDotnetify.IMemoryCache with the ASP.NET Core memory cache object.
   /// </summary>
   internal class MemoryCacheAdapter : IMemoryCache
   {
      private readonly ms.IMemoryCache _memoryCache;

      public MemoryCacheAdapter(ms.IMemoryCache memoryCache)
      {
         _memoryCache = memoryCache;
      }

      public object Get(string key) => _memoryCache.Get(key);

      public void Remove(string key) => _memoryCache.Remove(key);

      public void Set<T>(string key, T cachedValue, MemoryCacheEntryOptions options = null) where T : class
      {
         var entryOptions = new ms.MemoryCacheEntryOptions();
         if (options != null)
         {
            entryOptions.SlidingExpiration = options.SlidingExpiration;
            entryOptions.RegisterPostEvictionCallback((entryKey, value, reason, substate) => options.Callback?.Invoke(entryKey.ToString(), value, reason, substate));
         }

         _memoryCache.Set(key, cachedValue, entryOptions);
      }

      public bool TryGetValue<T>(string key, out T cachedValue) where T : class => _memoryCache.TryGetValue(key, out cachedValue);
   }
}
