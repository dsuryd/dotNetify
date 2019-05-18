using System.Linq;
using DotNetify;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevApp.Blazor.Server
{
   public class Startup
   {
      // This method gets called by the runtime. Use this method to add services to the container.
      // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
      public void ConfigureServices( IServiceCollection services )
      {
         services.AddMemoryCache();
         services.AddSignalR();
         services.AddDotNetify();

         services.AddMvc().AddNewtonsoftJson();
         services.AddResponseCompression( opts =>
         {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                   new[] { "application/octet-stream" } );
         } );

         services.AddTransient<ILiveDataService, MockLiveDataService>();
         services.AddScoped<ICustomerRepository, CustomerRepository>();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
      {
         app.UseWebSockets();
         app.UseSignalR( routes => routes.MapDotNetifyHub() );
         app.UseDotNetify( config =>
         {
            config.UseDeveloperLogging();
         } );

         app.UseResponseCompression();

         if ( env.IsDevelopment() )
         {
            app.UseDeveloperExceptionPage();
            app.UseBlazorDebugging();
         }

         app.UseRouting();

         app.UseEndpoints( endpoints =>
         {
            endpoints.MapDefaultControllerRoute();
         } );

         app.UseBlazor<Client.Startup>();
      }
   }
}