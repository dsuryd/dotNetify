using System;
using System.Collections.Generic;
using DotNetify;
using DotNetify.Routing;

namespace SPA
{
   public class App : BaseVM, IRoutable
   {
      public class Link
      {
         public string Id => Route.TemplateId;
         public Route Route { get; set; }
         public string Caption { get; set; }
      }

      public RoutingState RoutingState { get; set; }

      public List<Link> Links => new List<Link>
      {
         new Link { Route = this.GetRoute("SimpleList"), Caption = "Simple List" },
         new Link { Route = this.GetRoute("LiveChart"), Caption = "Live Chart" },
         new Link { Route = this.GetRoute("CompositeView"), Caption = "Composite View" },
      };

      public App()
      {
         this.RegisterRoutes("", new List<RouteTemplate>
         {
            new RouteTemplate("Home") { UrlPattern = "", ViewUrl = "SimpleList" },
            new RouteTemplate("SimpleList"),
            new RouteTemplate("LiveChart"),
            new RouteTemplate("CompositeView")
         });
      }
   }
}
