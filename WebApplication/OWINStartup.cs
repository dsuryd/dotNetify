using System;
using Microsoft.Owin;
using Owin;
using DotNetify;
using ViewModels;

[assembly: OwinStartup(typeof(WebApplication.OWINStartup))]

namespace WebApplication
{
   public class OWINStartup
   {
      public void Configuration(IAppBuilder app)
      {
         // Register the view models to a Dependency Injection container through IServiceProvider interface.
         // The default uses TinyIoC (https://github.com/grumpydev/TinyIoC). To override, pass your own 
         // implementation of IServiceProvider.
         var provider = new TinyIoCServiceProvider();
         provider
            .AddScoped(arg => new SimpleListVM(new EmployeeModel(7)))
            .AddScoped(arg => new BetterListVM(new EmployeeModel(7)))
            .AddScoped(arg => new EmployeeModel())
            .AddScoped<AFITop100Model>()
            .AddScoped<AFITop100VM>()
            .AddScoped<TreeViewVM>()
            .AddScoped<GridViewVM>()
            .AddScoped<CompositeViewVM>();
         
         app.MapSignalR();
         app.UseDotNetify(config =>
         {
            // Register the DEMO assembly "ViewModels". All subclasses of DotNetify.BaseVM 
            // inside that assembly will be known as view models.
            config.RegisterAssembly("ViewModels");
         }, provider);
      }
   }
}
