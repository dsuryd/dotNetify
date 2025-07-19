using System.Threading.Tasks;
using DotNetify;
using DotNetify.Observer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ObserverServer
{
   public class Startup
   {
      // This method gets called by the runtime. Use this method to add services to the container.
      // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddCors();
         services.AddSignalR();//.AddMessagePackProtocol();
         services.AddDotNetify().AddDotNetifyObserver();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }

         app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
         app.UseCors(builder => builder.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(_ => true).AllowCredentials());

         app.UseWebSockets();
         app.UseDotNetify(config =>
         {
            config.ConfigureObserverHost();
         });

         app.UseRouting();
         app.UseStaticFiles();

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapHub<DotNetifyHub>("/dotnetify");
            endpoints.MapObserver("/");
         });
      }
   }
}