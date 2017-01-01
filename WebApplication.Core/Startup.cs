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
         services.AddSignalR(options => options.Hubs.EnableDetailedErrors = true);

         services.AddMemoryCache();

         services.AddSingleton<DotNetify.IVMControllerFactory, DotNetify.VMControllerFactory>();

         // Find the DEMO assembly "ViewModels" and register it to DotNetify.VMController.
         // This will cause all the classes inside the assembly that inherits from DotNetify.BaseVM to be known as view models.
         var vmAssembly = typeof(SimpleListVM).GetTypeInfo().Assembly;
         if (vmAssembly != null)
            DotNetify.VMController.RegisterAssembly(vmAssembly);

         // The following demonstrates dotNetify's capability to support dependency injection.
         // In this example, the responsibility of constructing the model object is delegated 
         // away from its view model.  You can use your favorite IoC container to provide 
         // view model instances here.
         DotNetify.VMController.CreateInstance = (type, args) =>
         {
            if (type == typeof(SimpleListVM))
               return new SimpleListVM(new EmployeeModel(7));
            else if (type == typeof(BetterListVM))
               return new BetterListVM(new EmployeeModel(7));
            else if (type == typeof(AFITop100VM))
               return new AFITop100VM(new AFITop100Model());
            else if (type == typeof(TreeViewVM))
               return new TreeViewVM(new EmployeeModel());
            else if (type == typeof(GridViewVM))
               return new GridViewVM(new EmployeeModel());
            else if (type == typeof(CompositeViewVM))
               return new CompositeViewVM(new EmployeeModel());

            return Activator.CreateInstance(type, args);
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

         app.UseWebSockets();
         app.UseSignalR();
      }
   }
}
