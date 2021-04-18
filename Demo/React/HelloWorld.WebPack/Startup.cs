using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BrunoLau.SpaServices.Webpack;
using DotNetify;

namespace HelloWorld.WebPack
{
   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddSignalR();
         services.AddDotNetify(); 
      }

      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         app.UseWebSockets();
         app.UseDotNetify();

         if (env.IsDevelopment())
            app.UseWebpackDevMiddlewareEx(new WebpackDevMiddlewareOptions { HotModuleReplacement = true });

         app.UseStaticFiles();
         app.UseRouting();
         app.UseEndpoints(endpoints =>
         { 
            endpoints.MapHub<DotNetifyHub>("/dotnetify"); 
            endpoints.MapFallbackToFile("index.html");
         });
      }
   }
}