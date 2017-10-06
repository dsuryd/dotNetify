using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication
{
   public class WebApplication : System.Web.HttpApplication
   {
      protected void Application_Start()
      {
         AreaRegistration.RegisterAllAreas();
         RouteConfig.RegisterRoutes(RouteTable.Routes);
      }
   }
}
