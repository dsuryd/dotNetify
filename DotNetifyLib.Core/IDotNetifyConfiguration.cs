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
   public interface IDotNetifyConfiguration
   {
      /// <summary>
      /// How long to keep a view model controller in memory after it hasn't been accessed for a while. Default to never expire.
      /// </summary>
      TimeSpan? VMControllerCacheExpiration { get; set; }

      /// <summary>
      /// Provides a factory method to create view model instances.
      /// The method accepts a class type and constructor arguments, and returns an instance of that type.
      /// </summary>
      void SetFactoryMethod(Func<Type, object[], object> factoryMethod);

      /// <summary>
      /// Register view model classes in an assembly that are subtypes of BaseVM.
      /// </summary>
      void RegisterAssembly(Assembly assembly);
      void RegisterAssembly(string assemblyName);

      /// <summary>
      /// Register view model classes in an assembly that are subtypes of a certain type.
      /// </summary>
      void RegisterAssembly<T>(Assembly assembly) where T : INotifyPropertyChanged;
      void RegisterAssembly<T>(string assemblyName) where T : INotifyPropertyChanged;

      /// <summary>
      /// Register a specific view model class type.
      /// </summary>
      IDotNetifyConfiguration Register<T>() where T : INotifyPropertyChanged;

      /// <summary>
      /// Register a specific runtime view model class type.
      /// </summary>
      IDotNetifyConfiguration Register(string typeName, Func<object[], INotifyPropertyChanged> factory);
   }
}
