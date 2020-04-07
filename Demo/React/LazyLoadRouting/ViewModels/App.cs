using System.Collections.Generic;
using DotNetify;
using DotNetify.Routing;

namespace LazyLoadRouting
{
   public class App : BaseVM, IRoutable
   {
      public RoutingState RoutingState { get; set; }

      public List<object> Links => new List<object>()
        {
          new { Title = "Home", Route = this.GetRoute("Home") },
          new { Title = "Page 1", Route = this.GetRoute("Page1") },
          new { Title = "Page 2", Route = this.GetRoute("Page2") }
        };

      public App()
      {
         this.RegisterRoutes("", new List<RouteTemplate>
          {
              new RouteTemplate("Home")   { UrlPattern = "" },
              new RouteTemplate("Page1")  { UrlPattern = "Page1" },
              new RouteTemplate("Page2")  { UrlPattern = "Page2" }
          });
      }
   }
}