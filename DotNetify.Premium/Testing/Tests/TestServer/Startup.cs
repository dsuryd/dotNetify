using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DotNetify;
using DevApp.ViewModels;
using Microsoft.Extensions.Logging;

namespace TestServer
{
   public class Startup
   {
      // This method gets called by the runtime. Use this method to add services to the container.
      // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddLogging();
         services.AddSignalR();
         services.AddDotNetify();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();

         app.UseRouting();
         app.UseDotNetify(config =>
         {
            config.RegisterAssembly(typeof(HelloWorldVM).Assembly);
            config.UseDeveloperLogging(log => logger.LogInformation(log));
         });

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapHub<DotNetifyHub>("/dotnetify");
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Test server is alive!");
            });
         });
      }
   }
}