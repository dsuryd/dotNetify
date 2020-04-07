using System.Collections.Generic;
using DotNetify;
using DotNetify.Routing;

namespace LazyLoadRouting
{
   public class Page1 : BaseVM, IRoutable
   {
      public string Title => "Page 1";

      public RoutingState RoutingState { get; set; }

      public List<object> Links => new List<object>()
        {
          new { Title = "Page 1A", Route = this.GetRoute("Page1A") },
          new { Title = "Page 1B", Route = this.GetRoute("Page1B") }
        };

      public Page1()
      {
         this.RegisterRoutes("Page1", new List<RouteTemplate>
         {
            new RouteTemplate("Page1Home") { UrlPattern = "", ViewUrl = "Page1A" },
            new RouteTemplate("Page1A")  { UrlPattern = "Page1A" },
            new RouteTemplate("Page1B")  { UrlPattern = "Page1B" }
         });
      }
   }
}