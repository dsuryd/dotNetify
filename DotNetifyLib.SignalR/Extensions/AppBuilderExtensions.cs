using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetify
{
   public static class AppBuilderExtensions
   {
      public static IApplicationBuilder UseDotNetify(this IApplicationBuilder appBuilder)
      {
         // Use ASP.NET Core DI to provide view model instances by default, but you can always use your favorite IoC container.
         var provider = appBuilder.ApplicationServices;
         VMController.CreateInstance = (type, args) => ActivatorUtilities.CreateInstance(provider, type, args ?? new object[] { });

         return appBuilder;
      }
   }
}
