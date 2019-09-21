using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.DependencyInjection;
using DotNetify;

namespace SPA
{
   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddMemoryCache();
         services.AddSignalR();
         services.AddDotNetify();

         services.AddSingleton<IMovieService, MovieService>();
         services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
      }

      public void Configure(IApplicationBuilder app)
      {
         app.UseWebSockets();
         app.UseSignalR(routes => routes.MapDotNetifyHub());
         app.UseDotNetify();

         app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
         {
            HotModuleReplacement = true,
            HotModuleReplacementClientOptions = new Dictionary<string, string> { { "reload", "true" } },
         });

         app.UseStaticFiles();
         app.Run(async (context) =>
         {
            using (var reader = new StreamReader(File.OpenRead("wwwroot/index.html")))
               await context.Response.WriteAsync(reader.ReadToEnd());
         });
      }
   }
}