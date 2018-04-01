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

namespace DotNetify
{
   public interface IServiceCollection
   {
      IServiceCollection AddSingleton<TInterface, TImpl>() where TInterface : class where TImpl : class, TInterface;

      IServiceCollection AddSingleton<TInterface>(Func<object, TInterface> factory) where TInterface : class;

      IServiceCollection AddSingleton<TImpl>() where TImpl : class;

      IServiceCollection AddTransient<TInterface, TImpl>() where TInterface : class where TImpl : class, TInterface;

      IServiceCollection AddTransient<TImpl>() where TImpl : class;

      IServiceCollection AddScoped<TInterface, TImpl>() where TInterface : class where TImpl : class, TInterface;

      IServiceCollection AddScoped<TImpl>() where TImpl : class;
   }

   public interface IServiceProvider : IServiceCollection, System.IServiceProvider
   {
      T GetService<T>() where T : class;
   }

   public class ActivatorUtilities
   {
      public static IServiceProvider ServiceProvider { get; internal set; }

      public static object CreateInstance(IServiceProvider provider, Type type, object[] args)
      {
         try
         {
            object instance = null;
            if (args?.Length > 0)
               instance = Activator.CreateInstance(type, args);
            return instance ?? provider.GetService(type);
         }
         catch (Exception)
         {
            return Activator.CreateInstance(type, args);
         }
      }
   }
}