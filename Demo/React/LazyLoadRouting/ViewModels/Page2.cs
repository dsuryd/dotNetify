using System.Collections.Generic;
using System.Linq;
using DotNetify;
using DotNetify.Routing;

namespace LazyLoadRouting
{
   public class Page2 : BaseVM, IRoutable
   {
      public RoutingState RoutingState { get; set; }

      public string Title => "Page 2";

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
      public RoutingState RoutingState { get; set; }

      public string Title { get; set; }
      public string Content { get; set; }

      public Page2Item()
      {
         this.OnRouted((sender, e) =>
         {
            Title = $"Item {e.From.Replace("item/", "")}";
            Content = $@"{Title}: Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore
        magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo
        consequat.Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla
        pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est
        laborum.";
            Changed(nameof(Title));
            Changed(nameof(Content));
         });
      }
   }
}