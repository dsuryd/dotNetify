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
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DotNetify
{
   /// <summary>
   /// This class manages instantiations and updates of view models as requested by browser clients.
   /// </summary>
   public partial class VMController
   {
      /// <summary>
      /// List of known view model classes.
      /// </summary>
      internal static List<TypeHelper> _vmTypes = new List<TypeHelper>();

      internal static AggregateException _registrationException;

      /// <summary>
      /// List of registered assemblies.
      /// </summary>
      protected internal static List<string> _registeredAssemblies = new List<string>();

      #region Factory Method

      /// <summary>
      /// Delegate to override default mechanism used for creating view model instances.
      /// </summary>
      public static CreateInstanceDelegate CreateInstance { get; set; } = (type, args) => Activator.CreateInstance(type, args);

      /// <summary>
      /// Creates a view model instance of type T.
      /// </summary>
      public static T Create<T>(object[] args = null) where T : class => CreateInstance(typeof(T), args) as T;

      #endregion Factory Method

      /// <summary>
      /// Gets known view model types.
      /// </summary>
      internal static List<TypeHelper> VMTypes
      {
         get
         {
            // If there's deferred exception on registering assemblies, throw it now.
            if (_registrationException != null)
               throw _registrationException;
            return _vmTypes;
         }
      }

      /// <summary>
      /// Registers all view model types in an assembly.
      /// </summary>
      /// <param name="vmAssembly">Assembly.</param>
      public static void RegisterAssembly(Assembly vmAssembly) => RegisterAssembly<BaseVM>(vmAssembly);

      /// <summary>
      /// Registers all view model types in an assembly.
      /// </summary>
      /// <param name="vmAssembly">Assembly.</param>
      public static void RegisterAssembly<T>(Assembly vmAssembly) where T : INotifyPropertyChanged
      {
         if (vmAssembly == null)
            throw new ArgumentNullException();

         if (_registeredAssemblies.Exists(i => i == vmAssembly.FullName))
            return;

         _registeredAssemblies.Add(vmAssembly.FullName);

         // Check the assembly for two things:
         // 1) it doesn't contain duplicate view models fom existing registered assemblies.
         // 2) it actually has a view model.
         // If there's exception, it will be deferred until the first view model is accessed.
         List<Exception> exceptions = new List<Exception>();
         bool hasVMTypes = false;
         foreach (Type vmType in vmAssembly.GetExportedTypes().Where(i => typeof(T).GetTypeInfo().IsAssignableFrom(i)))
         {
            hasVMTypes = true;
            if (!_vmTypes.Any(i => i == vmType))
               _vmTypes.Add(vmType);
            else
               exceptions.Add(new Exception($"ERROR: View model '{vmType.Name}' was already registered by another assembly!"));
         }

         if (!hasVMTypes)
            exceptions.Add(new Exception($"ERROR: Assembly '{vmAssembly.GetName().Name}' does not define any view model!"));

         if (exceptions.Count > 0)
            _registrationException = new AggregateException(exceptions);
      }

      /// <summary>
      /// Registers a view model type.
      /// </summary>
      public static void Register<T>() where T : INotifyPropertyChanged
      {
         Type vmType = typeof(T);
         if (!_vmTypes.Any(i => i == vmType))
            _vmTypes.Add(vmType);
      }

      /// <summary>
      /// Registers a runtime view model type.
      /// </summary>
      public static void Register(string typeName, Func<object[], INotifyPropertyChanged> factory)
      {
         if (!_vmTypes.Any(i => i.FullName == typeName))
            _vmTypes.Add(new TypeHelper(typeName, factory));
      }
   }
}