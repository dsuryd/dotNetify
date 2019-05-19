/*
Copyright 2019 Dicky Suryadi

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

namespace DotNetify.Client.Blazor
{
   public static class StartupExtension
   {
      public static IServiceCollection AddDotNetifyClient(this IServiceCollection services, string serverUrl = null)
      {
         if (serverUrl != null)
            DotNetifyHubProxy.ServerUrl = serverUrl;

         services.AddSingleton<IDotNetifyHubProxy, DotNetifyHubProxy>();
         services.AddTransient<IDotNetifyClient, DotNetifyClient>();
         services.AddSingleton<IUIThreadDispatcher, DefaultUIThreadDispatcher>();
         return services;
      }
   }
}