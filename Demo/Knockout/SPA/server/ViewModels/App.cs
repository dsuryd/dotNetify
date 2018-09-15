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
         public string Path { get; set; }
         public string Caption { get; set; }
      }

      public RoutingState RoutingState { get; set; }

      public List<Link> Links => new List<Link>
      {
         new Link { Route = this.GetRoute("SimpleList"), Path = "/SimpleList", Caption = "Simple List" },
         new Link { Route = this.GetRoute("LiveChart"), Path = "/LiveChart", Caption = "Live Chart" }
      };

      public App()
      {
         this.RegisterRoutes("", new List<RouteTemplate>
         {
            new RouteTemplate("Home") { UrlPattern = "", ViewUrl = "SimpleList.html" },
            new RouteTemplate("SimpleList") { ViewUrl = "SimpleList.html" },
            new RouteTemplate("LiveChart") { ViewUrl = "LiveChart.html" }
         });
      }
   }
}
