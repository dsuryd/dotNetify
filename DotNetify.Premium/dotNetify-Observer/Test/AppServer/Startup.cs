using System;
using DotNetify;
using DotNetify.LoadTester;
using DotNetify.Observer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppServer
{
   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddCors();
         services.AddSignalR();//.AddMessagePackProtocol();
         services.AddDotNetify()
            .AddDotNetifyObserverClient()
            .AddDotNetifyObserver();
      }

      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }

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
            config.RegisterLoadProfiles();
            config.ConfigureObserver();
            config.ConfigureObserverClient(Environment.GetEnvironmentVariable("OBSERVER_URL") ?? "https://localhost:9001",
               options => options.ConnectionPoolSize = 5);
         });

         app.UseStaticFiles();
         app.UseRouting();

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapHub<DotNetifyHub>("/dotnetify");
            endpoints.MapObserver("/admin/observer");
            endpoints.MapGet("/", async context => await context.Response.WriteAsync("App Server"));
         });
      }
   }
}