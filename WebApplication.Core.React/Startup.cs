using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using React.AspNet;
using DotNetify;

namespace ReactWebApp
{
   public class Startup
   {
      // This method gets called by the runtime. Use this method to add services to the container.
      // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
      public void ConfigureServices( IServiceCollection services )
      {
         services.AddMvc();
         services.AddLocalization();

         services.AddMemoryCache(); // Required by dotNetify and ReactJS.NET.
         services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();  // Required by ReactJS.NET.
         services.AddSignalR();  // Required by dotNetify.

         services.AddReact();
         services.AddDotNetify(); 
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure( IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory )
      {
         app.UseReact( config => { } );
         app.UseStaticFiles();

         app.UseWebSockets();
         app.UseSignalR(); // Required by dotNetify.
         app.UseDotNetify();

         app.UseMvc(routes =>
         {
            routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");
         });
      }
   }
}
