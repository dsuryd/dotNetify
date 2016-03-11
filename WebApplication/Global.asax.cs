using System;
using System.Web.Mvc;
using System.Web.Routing;
using DotNetify;
using ViewModels;

namespace WebApplication
{
   public class WebApplication : System.Web.HttpApplication
   {
      protected void Application_Start()
      {
         AreaRegistration.RegisterAllAreas();
         RouteConfig.RegisterRoutes(RouteTable.Routes);

         // Find the DEMO assembly "ViewModels" and register it to DotNetify.VMController.
         // This will cause all the classes inside the assembly that inherits from DotNetify.BaseVM to be known as view models.
         var vmAssembly = System.Reflection.Assembly.Load("ViewModels");
         if (vmAssembly != null)
            VMController.RegisterAssembly(vmAssembly);

         // The following demonstrates dotNetify's capability to support dependency injection.
         // In this example, the responsibility of constructing the model object is delegated 
         // away from its view model.  You can use your favorite IoC container to provide 
         // view model instances here.
         VMController.CreateInstance = (type, args) =>
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
   }
}
