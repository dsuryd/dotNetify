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
      private IMemoryCache _memoryCache;

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

         return vmType.IsMulticast ? GetMulticastInstance(vmType, vmInstanceId) : Create(vmType, vmInstanceId);
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
         return Create(vmType, vmInstanceId);
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
   }
}