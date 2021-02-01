/*
Copyright 2016-2020 Dicky Suryadi

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
using System.Threading;

namespace DotNetify
{
   /// <summary>
   /// Provides an object factory method that uses the scoped service provider of the active VMController instance.
   /// </summary>
   public interface IHubServiceProvider
   {
      Func<Type, object[], object> FactoryMethod { get; }
   }

   /// <summary>
   /// Implements ambient data store for the object factory method associated with the active VMController instance.
   /// </summary>
   internal class HubServiceProvider : IHubServiceProvider
   {
      private readonly static AsyncLocal<Func<Type, object[], object>> _asyncLocal = new AsyncLocal<Func<Type, object[], object>>();

      public Func<Type, object[], object> FactoryMethod
      {
         get { return _asyncLocal.Value; }
         set { _asyncLocal.Value = value; }
      }
   }
}