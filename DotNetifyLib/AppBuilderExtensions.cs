using System;
using Owin;

namespace DotNetify
{
   public static class AppBuilderExtensions
   {
      public static IAppBuilder UseDotNetify(this IAppBuilder appBuilder, Action<IDotNetifyConfiguration> config = null)
      {
         var dotNetifyConfig = new DotNetifyConfiguration();
         config?.Invoke(dotNetifyConfig);

         // If no view model assembly has been registered, default to the entry assembly.
         if (!dotNetifyConfig.HasAssembly)
            dotNetifyConfig.RegisterEntryAssembly();

         return appBuilder;
      }
   }
}
