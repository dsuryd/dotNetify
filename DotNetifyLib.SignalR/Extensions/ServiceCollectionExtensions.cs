using Microsoft.Extensions.DependencyInjection;
using DotNetify.Security;

namespace DotNetify
{
   public static class ServiceCollectionExtensions
   {
      public static IServiceCollection AddDotNetify(this IServiceCollection services)
      {
         // Use ASP.NET Core DI to inject the dotNetify's view model controller factory to the dotNetify's SignalR hub.
         services.AddSingleton<IVMControllerFactory, VMControllerFactory>();

         // Add service to get the hub principal.
         services.AddScoped<IPrincipalAccessor>(p => new HubPrincipalAccessor());

         return services;
      }
   }
}
