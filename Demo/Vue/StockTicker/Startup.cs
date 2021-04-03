using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using BrunoLau.SpaServices.Webpack;
using DotNetify;

namespace StockTicker
{
   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddMemoryCache();
         services.AddSignalR();
         services.AddDotNetify();

         services.AddTransient<IStockTickerService, StockTickerService>();
      }

      public void Configure(IApplicationBuilder app)
      {
         app.UseWebSockets();
         app.UseDotNetify();

         app.UseWebpackDevMiddlewareEx(new WebpackDevMiddlewareOptions
         {
            HotModuleReplacement = true,
            HotModuleReplacementClientOptions = new Dictionary<string, string> { { "reload", "true" } },
         });

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