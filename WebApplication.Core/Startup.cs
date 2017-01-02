using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
         services.AddSignalR(options => options.Hubs.EnableDetailedErrors = true);
         services.AddMemoryCache();

         // Find the assembly "ViewModels.Examples" and register it to DotNetify.VMController.
         // This will cause all the classes inside the assembly that inherits from DotNetify.BaseVM to be known as view models.
         var vmAssembly = Assembly.Load(new AssemblyName("ViewModels.Examples"));
         if (vmAssembly != null)
            DotNetify.VMController.RegisterAssembly(vmAssembly);

         // Use ASP.NET Core DI to inject the dotNetify's view model controller factory to the dotNetify's SignalR hub.
         services.AddSingleton<DotNetify.IVMControllerFactory, DotNetify.VMControllerFactory>();

         // Use ASP.NET Core DI to provide view model instances, but you can always use your favorite IoC container.
         DotNetify.VMController.CreateInstance = (type, args) =>
         {
            if (type == typeof(SimpleListVM) || type == typeof(BetterListVM))
               args = new object[] { new EmployeeModel(7) };
            else if (type == typeof(TreeViewVM) || type == typeof(GridViewVM) || type == typeof(CompositeViewVM))
               args = new object[] { new EmployeeModel() };
            else if (type == typeof(AFITop100VM))
               args = new object[] { new AFITop100Model() };

            return ActivatorUtilities.CreateInstance(services.BuildServiceProvider(), type, args ?? new object[] { });
         };
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
      {
         loggerFactory.AddConsole(Configuration.GetSection("Logging"));
         loggerFactory.AddDebug();

         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
            //app.UseBrowserLink();
         }
         else
         {
            app.UseExceptionHandler("/Home/Error");
         }

         app.UseStaticFiles();

         app.UseMvc(routes =>
         {
            routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");
         });

         // Required by dotNetify.
         app.UseWebSockets();
         app.UseSignalR();
      }
   }
}
