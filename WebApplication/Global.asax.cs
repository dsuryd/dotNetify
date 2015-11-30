using System.Web.Mvc;
using System.Web.Routing;
using DotNetify;

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
      }
   }
}
