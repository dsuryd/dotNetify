using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetify
{
   public static class AppBuilderExtensions
   {
      public static IApplicationBuilder UseDotNetify(this IApplicationBuilder appBuilder, Action<IDotNetifyConfiguration> config = null)
      {
         var provider = appBuilder.ApplicationServices;

         // Make sure all the required services are there.
         if (provider.GetService<IMemoryCache>() == null)
            throw new InvalidOperationException("No service of type IMemoryCache has been registered. Please add the service by calling 'IServiceCollection.AddMemoryCache()' in the startup class.");
         if ( provider.GetService<IVMControllerFactory>() == null )
            throw new InvalidOperationException("Please call 'IServiceCollection.AddDotNetify()' inside the ConfigureServices() of the startup class.");

         // Use ASP.NET Core DI to provide view model instances by default.
         var dotNetifyConfig = new DotNetifyConfiguration();
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
