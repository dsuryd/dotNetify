using System.Collections.Generic;
using DotNetify.Routing;

namespace DotNetify.Observer
{
   public class ObserverAppVM : BaseVM, IRoutable
   {
      private enum Route
      {
         Home,
         Connections
      }

      public RoutingState RoutingState { get; set; }

      public static string RouteRoot = "observer";

      public ObserverAppVM()
      {
         this.RegisterRoutes(RouteRoot, new List<RouteTemplate>
         {
            new RouteTemplate(nameof(Route.Home))        { UrlPattern = "", ViewUrl = nameof(Route.Connections) },
            new RouteTemplate(nameof(Route.Connections)) { UrlPattern = "connections" },
        });

         AddProperty("NavMenu", new NavMenu(
            new NavMenuItem[]
            {
               new NavRoute("Connections",   this.GetRoute(nameof(Route.Connections)))
            }));
      }
   }
}