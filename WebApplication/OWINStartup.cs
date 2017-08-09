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
            .AddTransient<EmployeeModel>()
            .AddTransient<AFITop100Model>()
            .AddTransient<AFITop100VM>()
            .AddTransient<TreeViewVM>()
            .AddTransient<GridViewVM>()
            .AddTransient<CompositeViewVM>();
         
         app.MapSignalR();
         app.UseDotNetify(config =>
         {
            // Register the DEMO assembly "ViewModels". All subclasses of DotNetify.BaseVM 
            // inside that assembly will be known as view models.
            config.RegisterAssembly("ViewModels");

            // Override default factory method to provide custom objects.
            config.SetFactoryMethod((type, args) =>
            {
               if (type == typeof(SimpleListVM))
                  return new SimpleListVM(new EmployeeModel(7));
               else if (type == typeof(BetterListVM))
                  return new BetterListVM(new EmployeeModel(7));

               return ActivatorUtilities.CreateInstance(provider, type, args);
            });

         }, provider);
      }
   }
}
