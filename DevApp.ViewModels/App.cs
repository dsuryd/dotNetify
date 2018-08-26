using System.Collections.Generic;
using DotNetify;
using DotNetify.Elements;
using DotNetify.Routing;
using DotNetify.Security;

namespace DotNetify.DevApp
{
   public class App : BaseVM, IRoutable
   {
      private enum Route
      {
         Overview
      }

      public RoutingState RoutingState { get; set; }

      public App()
      {
         this.RegisterRoutes("", new List<RouteTemplate>
         {
            new RouteTemplate(nameof(Route.Overview)) { UrlPattern = "", ViewUrl = nameof(Route.Overview) }
        });

         AddProperty("NavMenu", new NavMenu(
            new NavMenuItem[]
            {
               new NavRoute("Overview",  this.GetRoute(nameof(Route.Overview)))
            }));
      }
   }
}