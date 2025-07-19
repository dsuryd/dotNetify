using System;
using DotNetify;
using DotNetify.Forwarding;
using DotNetify.Observer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;

namespace ProxyServer
{
   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddCors();
         services.AddSignalR();
         services.AddDotNetify().AddDotNetifyObserverClient();
      }

      public void Configure(IApplicationBuilder app)
      {
         app.UseForwardedHeaders(new ForwardedHeadersOptions
         {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
         });

         app.UseCors(builder => builder
          .AllowAnyMethod()
          .AllowAnyHeader()
          .SetIsOriginAllowed(_ => true)
          .AllowCredentials());

         app.UseWebSockets();
         app.UseDotNetify(config =>
         {
            config.ConfigureObserverClient(Environment.GetEnvironmentVariable("OBSERVER_URL") ?? "http://localhost:9000", options => options.ConnectionPoolSize = 5);
            config.UseForwarding(Environment.GetEnvironmentVariable("APPSERVER_URL") ?? "http://localhost:6100", options =>
            {
               //options.UseMessagePack = true;
               options.ConnectionPoolSize = 10;
            });
         });

         app.UseRouting();
         app.UseEndpoints(endpoints => endpoints.MapHub<DotNetifyHub>("/dotnetify"));
      }
   }
}