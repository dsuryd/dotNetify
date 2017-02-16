using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetify
{
   public static class AppBuilderExtensions
   {
      public static IApplicationBuilder UseDotNetify(this IApplicationBuilder appBuilder, Action<IDotNetifyConfiguration> config = null)
      {
         var dotNetifyConfig = new DotNetifyConfiguration();

         // Use ASP.NET Core DI to provide view model instances by default.
         var provider = appBuilder.ApplicationServices;
         dotNetifyConfig.SetFactoryMethod((type, args) =>
         {
            try
            {
               return ActivatorUtilities.CreateInstance(provider, type, args ?? new object[] { });
            }
            catch (Exception ex)
            {
               Trace.Fail(ex.Message);
               throw ex;
            }
         });

         config?.Invoke(dotNetifyConfig);

         // If no view model assembly has been registered, default to the entry assembly.
         if (!dotNetifyConfig.HasAssembly)
            dotNetifyConfig.RegisterEntryAssembly();

         return appBuilder;
      }
   }
}
