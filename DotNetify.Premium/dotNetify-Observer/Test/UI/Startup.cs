using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.DependencyInjection;

namespace UI
{
   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {
      }

      public void Configure(IApplicationBuilder app)
      {
#pragma warning disable 618
         app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
         {
            HotModuleReplacement = true,
            HotModuleReplacementClientOptions = new Dictionary<string, string> { { "reload", "true" } },
         });
#pragma warning restore 618

         app.UseStaticFiles();
         app.UseRouting();

         app.UseEndpoints(endpoints => endpoints.MapFallbackToFile("index.html"));
      }
   }
}