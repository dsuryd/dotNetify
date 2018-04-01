/*
Copyright 2016-2018 Dicky Suryadi

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

using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotNetify
{
   /// <summary>
   /// Provides scoped dependency injection service provider.
   /// </summary>
   internal class VMServiceScope : IVMServiceScope
   {
      private readonly IServiceScope _serviceScope;

      public IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;

      public VMServiceScope(IServiceScope serviceScope)
      {
         _serviceScope = serviceScope;
      }

      public void Dispose() => _serviceScope.Dispose();
   }

   /// <summary>
   /// Provides view model service scopes.
   /// </summary>
   internal class VMServiceScopeFactory : IVMServiceScopeFactory
   {
      private readonly IServiceProvider _serviceProvider;

      public VMServiceScopeFactory(IServiceProvider serviceProvider)
      {
         _serviceProvider = serviceProvider;
      }

      public IVMServiceScope CreateScope() => new VMServiceScope(_serviceProvider.CreateScope());
   }
}