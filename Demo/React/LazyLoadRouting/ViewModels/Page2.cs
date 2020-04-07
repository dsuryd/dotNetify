using System.Collections.Generic;
using System.Linq;
using DotNetify;
using DotNetify.Routing;

namespace LazyLoadRouting
{
   public class Page2 : BaseVM, IRoutable
   {
      public string Title => "Page 2";
      public RoutingState RoutingState { get; set; }

      public IEnumerable<object> Links => Enumerable.Range(1, 12).Select(i => new
      {
         Title = $"Item {i}",
         Route = this.GetRoute("Page2Item", $"item/{i}")
      });

      public Page2()
      {
         this.RegisterRoutes("Page2", new List<RouteTemplate>
         {
            new RouteTemplate("Page2Home") { UrlPattern = "" },
            new RouteTemplate("Page2Item") { UrlPattern = "item(/:id)", VMType = typeof(Page2Item) }
         });
      }
   }

   public class Page2Item : BaseVM, IRoutable
   {
      public string Title { get; set; }
      public RoutingState RoutingState { get; set; }

      public Page2Item()
      {
         this.OnRouted((sender, e) =>
         {
            Title = $"Item {e.From.Replace("item/", "")}";
            Changed(nameof(Title));
         });
      }
   }
}