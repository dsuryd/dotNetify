using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DotNetify;

namespace WebApplication.Core.React
{
   public class Startup
   {
      // This method gets called by the runtime. Use this method to add services to the container.
      // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
      public void ConfigureServices( IServiceCollection services )
      {
         services.AddMvc();
         services.AddLocalization();

         services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();  // Required by ReactJS.NET.
         services.AddSignalR();  // Required by dotNetify.

         services.AddDotNetify();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure( IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory )
      {
         app.UseStaticFiles();
         app.UseAuthServer(); // Provide auth tokens.

         app.UseWebSockets();
         app.UseSignalR(); // Required by dotNetify.
         app.UseDotNetify(config =>
         {
            config.UseMiddleware<LogRequestMiddleware>();
            config.UseMiddleware<JwtBearerAuthenticationMiddleware>();
         });

         app.UseMvc(routes =>
         {
            routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");
         });
      }
   }
}
