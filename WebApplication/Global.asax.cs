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
         // In this example, the responsibility of constructing the AFITop100 model instance 
         // is delegated away from its view model.  You can use your favorite IoC container 
         // to provide view model instances here.
         VMController.CreateInstance = (type, args) =>
         {
            if (type == typeof(AFITop100VM))
               return new AFITop100VM(new AFITop100Model());
            return Activator.CreateInstance(type, args);
         };
      }
   }
}
