/*
Copyright 2018 Dicky Suryadi

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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace DotNetify
{
   /// <summary>
   /// Provides view models.
   /// </summary>
   public class VMFactory : IVMFactory
   {
      /// <summary>
      /// Registered view model types.
      /// </summary>
      private IEnumerable<TypeHelper> VMTypes => VMController.VMTypes;

      /// <summary>
      /// For caching multicast view models.
      /// </summary>
      private readonly IMemoryCache _memoryCache;

      /// <summary>
      /// Used for synchronizing key-based access to the memory cache.
      /// </summary>
      private readonly ConcurrentDictionary<string, object> _keyBasedLock = new ConcurrentDictionary<string, object>();

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="memoryCache">Memory cache for caching multicast view models.</param>
      public VMFactory(IMemoryCache memoryCache)
      {
         _memoryCache = memoryCache;
      }

      /// <summary>
      /// Creates a view model instance from a list of registered types.
      /// </summary>
      /// <param name="vmTypeName">View model type name.</param>
      /// <param name="vmInstanceId">Optional view model instance identifier.</param>
      /// <param name="vmNamespace">Optional view model type namespace.</param>
      /// <returns></returns>
      public BaseVM GetInstance(string vmTypeName, string vmInstanceId = null, string vmNamespace = null)
      {
         var vmType = GetVMTypeHelper(vmTypeName, vmNamespace);
         if (vmType == null)
            return null;

         return vmType.Is<MulticastVM>() ? GetMulticastInstance(vmType, vmInstanceId) : Create(vmType, vmInstanceId);
      }

      /// <summary>
      /// Creates a view model instance.
      /// </summary>
      /// <param name="vmType">View model type helper.</param>
      /// <param name="vmInstanceId">Optional view model instance identifier.</param>
      /// <returns>View model instance.</returns>
      private BaseVM Create(TypeHelper vmType, string vmInstanceId)
      {
         object[] arg = vmInstanceId != null ? new object[] { vmInstanceId } : null;
         try
         {
            if (vmType.CreateInstance(arg) is INotifyPropertyChanged instance)
               return instance is BaseVM ? instance as BaseVM : new BaseVM(instance);
         }
         catch (MissingMethodException)
         {
            if (arg != null)
               Trace.Fail($"[dotNetify] ERROR: '{vmType.Name}' has no constructor accepting instance ID.");
            else
               Trace.Fail($"[dotNetify] ERROR: '{vmType.Name}' has no parameterless constructor.");
         }

         return null;
      }

      /// <summary>
      /// Gets a multicast instance or creates a new one.
      /// </summary>
      /// <param name="vmType">View model type helper.</param>
      /// <param name="vmInstanceId">Optional view model instance identifier.</param>
      /// <returns>View model instance.</returns>
      private BaseVM GetMulticastInstance(TypeHelper vmType, string vmInstanceId)
      {
         string key = vmType.FullName;
         MulticastVM vm = null;

         lock (_keyBasedLock.GetOrAdd(key, _ => new object()))
         {
            if (!_memoryCache.TryGetValue(key, out HashSet<MulticastVM> vmCollections))
            {
               vm = CreateMulticastInstance(vmType, vmInstanceId, key);
               vmCollections = new HashSet<MulticastVM> { vm };
               _memoryCache.Set(key, vmCollections);
            }
            else
            {
               vm = vmCollections.FirstOrDefault(x => x.IsMember);
               if (vm != null)
                  vm.AddRef();
               else
               {
                  vm = CreateMulticastInstance(vmType, vmInstanceId, key);
                  vmCollections.Add(vm);
               }
            }
         }
         return vm;
      }

      /// <summary>
      /// Returns a helper object that can create instances of a given view model type.
      /// </summary>
      /// <param name="vmTypeName">View model type name.</param>
      /// <param name="vmNamespace">Optional view model type namespace.</param>
      /// <returns>View model type helper.</returns>
      private TypeHelper GetVMTypeHelper(string vmTypeName, string vmNamespace)
      {
         return vmNamespace != null ?
            VMTypes.FirstOrDefault(i => i.FullName == $"{vmNamespace}.{vmTypeName}") :
            VMTypes.FirstOrDefault(i => i.Name == vmTypeName);
      }

      /// <summary>
      /// Creates a multicast view model instance.
      /// </summary>
      /// <param name="vmType">View model type.</param>
      /// <param name="vmInstanceId">Optional view model instance identifier.</param>
      /// <param name="key">Key to the cache collection, which is the view model's type name.</param>
      /// <returns>Multicast view model.</returns>
      private MulticastVM CreateMulticastInstance(Type vmType, string vmInstanceId, string key)
      {
         var vm = Create(vmType, vmInstanceId) as MulticastVM;
         vm.Disposed += (sender, e) => RemoveMulticastInstance(vm, key);

         vm.Initialize();
         return vm;
      }

      /// <summary>
      /// Removes multicast view model instance from the cache.
      /// </summary>
      /// <param name="vm">Multicast view model.</param>
      /// <param name="key">Key to the cache collection, which is the view model's type name.</param>
      private void RemoveMulticastInstance(MulticastVM vm, string key)
      {
         lock (_keyBasedLock.GetOrAdd(key, _ => new object()))
         {
            if (_memoryCache.TryGetValue(key, out HashSet<MulticastVM> vmCollections))
               vmCollections.Remove(vm);
         }
      }
   }
}