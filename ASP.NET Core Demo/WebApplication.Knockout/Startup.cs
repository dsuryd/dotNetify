using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DotNetify;
using ViewModels;

namespace WebApplication.Core
{
   public class Startup
   {
      public Startup(IHostingEnvironment env)
      {
         var builder = new ConfigurationBuilder()
             .SetBasePath(env.ContentRootPath)
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
             .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
             .AddEnvironmentVariables();
         Configuration = builder.Build();
      }

      public IConfigurationRoot Configuration { get; }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services)
      {
         // Add framework services.
         services.AddMvc();

         // SignalR and Memory Cache are required by dotNetify.
         services.AddSignalR();
         services.AddMemoryCache();
         services.AddDotNetify();

         // Register the view models to ASP.NET Core DI container.
         services.AddScoped<EmployeeModel>();
         services.AddScoped<AFITop100Model>();
         services.AddScoped(p => new SimpleListVM(new EmployeeModel(7)));
         services.AddScoped(p => new BetterListVM(new EmployeeModel(7)));
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
      {
         loggerFactory.AddConsole(Configuration.GetSection("Logging"));
         loggerFactory.AddDebug();

         app.UseStaticFiles();

         app.UseMvc(routes =>
         {
            routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");
         });

         // Required by dotNetify.
         app.UseWebSockets();
         app.UseSignalR(routes => routes.MapHub<DotNetifyHub>("/dotnetify"));

         app.UseDotNetify(config => {

            // Find the assembly "ViewModels" and register it to DotNetify.VMController.
            // This will cause all the classes inside the assembly that inherits from DotNetify.BaseVM to be known as view models.
            config.RegisterAssembly("ViewModels");
         });
      }
   }
}
