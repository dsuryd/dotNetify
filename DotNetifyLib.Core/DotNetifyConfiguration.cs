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
using System.ComponentModel;
using System.Reflection;

namespace DotNetify
{
   /// <summary>
   /// Provides startup configuration options.
   /// </summary>
   public class DotNetifyConfiguration : IDotNetifyConfiguration
   {
      /// <summary>
      /// How long to keep a view model controller in memory after it hasn't been accessed for a while. Default to never expire.
      /// </summary>
      public TimeSpan? VMControllerCacheExpiration { get; set; }

      /// <summary>
      /// Provides a factory method to create view model instances.
      /// The method accepts a class type and constructor arguments, and returns an instance of that type.
      /// </summary>
      public void SetFactoryMethod(Func<Type, object[], object> factoryMethod) => VMController.CreateInstance = (type, args) => factoryMethod(type, args);

      /// <summary>
      /// Register view model classes in an assembly that are subtypes of BaseVM.
      /// </summary>
      public void RegisterAssembly(Assembly assembly) => VMController.RegisterAssembly(assembly);

      /// <summary>
      /// Register view model classes in an assembly that are subtypes of BaseVM.
      /// </summary>
      public void RegisterAssembly(string assemblyName) => VMController.RegisterAssembly(Assembly.Load(new AssemblyName(assemblyName)));

      /// <summary>
      /// Register view model classes in an assembly that are subtypes of a certain type.
      /// </summary>
      public void RegisterAssembly<T>(Assembly assembly) where T : INotifyPropertyChanged => VMController.RegisterAssembly<T>(assembly);

      /// <summary>
      /// Register view model classes in an assembly that are subtypes of a certain type.
      /// </summary>
      public void RegisterAssembly<T>(string assemblyName) where T : INotifyPropertyChanged => VMController.RegisterAssembly<T>(Assembly.Load(new AssemblyName(assemblyName)));

      /// <summary>
      /// Register a specific view model class type.
      /// </summary>
      public IDotNetifyConfiguration Register<T>() where T : INotifyPropertyChanged
      {
         VMController.Register<T>();
         return this;
      }

      /// <summary>
      /// Register a specific runtime view model class type.
      /// </summary>
      public IDotNetifyConfiguration Register(string typeName, Func<object[], INotifyPropertyChanged> factory)
      {
         VMController.Register(typeName, factory);
         return this;
      }

      #region Methods for internal use

      /// <summary>
      /// Registers view model classes in the entry assembly.
      /// </summary>
      public void RegisterEntryAssembly()
      {
         var entryAssembly = Assembly.GetEntryAssembly();
         if (entryAssembly != null)
            VMController.RegisterAssembly(entryAssembly);
      }

      /// <summary>
      /// Whether anything has been registered.
      /// </summary>
      public bool HasAssembly => VMController._registeredAssemblies.Count > 0;

      #endregion
   }
}
