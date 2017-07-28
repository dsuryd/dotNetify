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
      /// Register an assembly that has the view model classes.
      /// </summary>
      void RegisterAssembly(Assembly assembly);
      void RegisterAssembly(string assemblyName);
   }

   public class DotNetifyConfiguration : IDotNetifyConfiguration
   {
      public TimeSpan? VMControllerCacheExpiration { get; set; }

      public void SetFactoryMethod(Func<Type, object[], object> factoryMethod) => VMController.CreateInstance = (type, args) => factoryMethod(type, args);

      public void RegisterAssembly(Assembly assembly) => VMController.RegisterAssembly(assembly);

      public void RegisterAssembly(string assemblyName) => VMController.RegisterAssembly(Assembly.Load(new AssemblyName(assemblyName)));

      public void RegisterEntryAssembly() => VMController.RegisterAssembly(Assembly.GetEntryAssembly());

      public bool HasAssembly => VMController._registeredAssemblies.Count > 0;
   }
}
