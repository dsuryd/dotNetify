using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.DependencyInjection;
using DotNetify;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace app1
{
   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {       
         services.AddMemoryCache();
         services.AddSignalR();
         services.AddDotNetify();

         services.AddTransient<ILiveDataService, MockLiveDataService>();
         services.AddScoped<ICustomerRepository, CustomerRepository>();
      }

      public void Configure(IApplicationBuilder app, IHostingEnvironment env)
      {
         app.UseWebSockets();
         app.UseSignalR(routes => routes.MapDotNetifyHub());
         app.UseDotNetify(config =>
         {
            config.UseDeveloperLogging();
         });

         if (env.IsDevelopment())
         {
            app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
            {
               HotModuleReplacement = true,
               HotModuleReplacementClientOptions = new Dictionary<string, string> { { "reload", "true" } },
            });
         }

         app.UseFileServer();
         app.Run(async (context) =>
         {
            using (var reader = new StreamReader(File.OpenRead("wwwroot/index.html")))
               await context.Response.WriteAsync(reader.ReadToEnd());
         });
      }
   }
}