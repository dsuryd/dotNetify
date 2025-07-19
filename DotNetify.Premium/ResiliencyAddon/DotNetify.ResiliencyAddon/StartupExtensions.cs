/*
Copyright 2023 Dicky Suryadi

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

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetify.WebApi
{
   public static class StartupExtensions
   {
      public static IServiceCollection AddDotNetifyResiliencyAddon(this IServiceCollection services)
      {
         services.AddSingleton<IWebApiConnectionCache, ResilientWebApiConnectionCache>();
         services.AddTransient<CachedConnection>();
         services.AddTransient<CachedConnectionGroup>();

         return services;
      }

      public static IApplicationBuilder UseDotNetifyResiliencyAddon(this IApplicationBuilder app)
      {
         RestoreManager.RestoreActiveConnectionVMs(app.ApplicationServices);

         app.UseMiddleware<EnsureVMExistsOnUpdateMiddleware>();
         return app;
      }
   }
}