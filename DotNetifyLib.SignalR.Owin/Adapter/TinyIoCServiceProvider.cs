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
using TinyIoC;

namespace DotNetify
{
   public class TinyIoCServiceProvider : IServiceProvider
   {
      public IServiceCollection AddSingleton<TInterface, TImpl>()
               where TInterface : class
               where TImpl : class, TInterface
      {
         TinyIoCContainer.Current.Register<TInterface, TImpl>().AsSingleton();
         return this;
      }

      public IServiceCollection AddSingleton<TInterface>(Func<object, TInterface> factory) where TInterface : class
      {
         TinyIoCContainer.Current.Register(factory(null));
         return this;
      }

      public IServiceCollection AddSingleton<TImpl>() where TImpl : class
      {
         TinyIoCContainer.Current.Register<TImpl>().AsSingleton();
         return this;
      }

      public IServiceCollection AddTransient<TInterface, TImpl>() where TInterface : class where TImpl : class, TInterface
      {
         TinyIoCContainer.Current.Register<TInterface, TImpl>().AsMultiInstance();
         return this;
      }

      public IServiceCollection AddTransient<TImpl>() where TImpl : class
      {
         TinyIoCContainer.Current.Register<TImpl>().AsMultiInstance();
         return this;
      }

      public virtual IServiceCollection AddScoped<TInterface, TImpl>() where TInterface : class where TImpl : class, TInterface
      {
         TinyIoCContainer.Current.Register<TInterface, TImpl>().AsPerThread();
         return this;
      }

      public virtual IServiceCollection AddScoped<TImpl>() where TImpl : class
      {
         TinyIoCContainer.Current.Register<TImpl>().AsPerThread();
         return this;
      }

      public T GetService<T>() where T : class => TinyIoCContainer.Current.Resolve<T>();
      public object GetService(Type type) => TinyIoCContainer.Current.Resolve(type);
   }
}
