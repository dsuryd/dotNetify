using System;
using DotNetify.Forwarding;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetify.Observer
{
   public static class StartupExtensions
   {
      public static IServiceCollection AddDotNetifyObserverClient(this IServiceCollection services)
      {
         services.AddHostedService<TelemetryCollector>();
         return services;
      }

      public static IDotNetifyConfiguration ConfigureObserverClient(this IDotNetifyConfiguration config, string serverUrl, Action<ObserverClientOptions> optionsAccessor = null)
      {
         TelemetryCollector.AddServerUrl(serverUrl);

         var options = new ObserverClientOptions();
         optionsAccessor?.Invoke(options);

         config.UseForwarding(serverUrl, forwardingOptions =>
         {
            forwardingOptions.HaltPipeline = false;
            forwardingOptions.ConnectionPoolSize = options.ConnectionPoolSize;
            forwardingOptions.UseMessagePack = options.UseMessagePack;
         });
         return config;
      }
   }
}