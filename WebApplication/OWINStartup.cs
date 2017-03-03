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
         app.MapSignalR();
         app.UseDotNetify(config =>
         {
            // Register the DEMO assembly "ViewModels". All subclasses of DotNetify.BaseVM 
            // inside that assembly will be known as view models.
            config.RegisterAssembly("ViewModels");

            // The following demonstrates dotNetify's capability to support dependency injection.
            // In this example, the responsibility of constructing the model object is delegated 
            // away from its view model.  You can use your favorite IoC container to provide 
            // view model instances here.
            config.SetFactoryMethod((type, args) =>
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
            });
         });
      }
   }
}
