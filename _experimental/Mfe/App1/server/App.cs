using System.Collections.Generic;
using DotNetify;
using DotNetify.Elements;
using DotNetify.Routing;
using DotNetify.Security;

namespace app1
{
   public class App : BaseVM, IRoutable
   {
      private enum Route
      {
         Home,
         Dashboard,
         Form
      }

      public RoutingState RoutingState { get; set; }

      public App()
      {
         this.RegisterRoutes("", new List<RouteTemplate>
         {
            new RouteTemplate(nameof(Route.Home))        { UrlPattern = "", ViewUrl = nameof(Route.Dashboard) },
            new RouteTemplate(nameof(Route.Dashboard))   { UrlPattern = "dashboard" },
            new RouteTemplate(nameof(Route.Form))        { UrlPattern = "form" },
        });

         AddProperty("NavMenu", new NavMenu(
            new NavMenuItem[]
            {
               new NavRoute("Dashboard",  this.GetRoute(nameof(Route.Dashboard))),
               new NavRoute("Form",       this.GetRoute(nameof(Route.Form))),
            }));
      }
   }
}