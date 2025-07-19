using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetify.Observer
{
   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddSignalR().AddMessagePackProtocol();
         services.AddDotNetify().AddDotNetifyObserver();
      }

      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         if (env.IsDevelopment())
         {
#pragma warning disable 618
            app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
            {
               HotModuleReplacement = true,
               HotModuleReplacementClientOptions = new Dictionary<string, string> { { "reload", "true" } },
            });
#pragma warning restore 618
         }

         app.UseDotNetify(config =>
         {
            config.ConfigureObserverHost();
         });

         app.UseStaticFiles();
         app.UseRouting();

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapHub<DotNetifyHub>("/dotnetify");
            endpoints.MapControllers();
            endpoints.MapObserver();
            endpoints.MapGet("/", context =>
            {
               context.Response.Redirect("/observer");
               return Task.CompletedTask;
            });
         });
      }
   }
}