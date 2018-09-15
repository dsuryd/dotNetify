using System;
using System.Collections.Generic;
using DotNetify;
using DotNetify.Routing;

namespace AllExamples
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

      public List<Link> ExampleLinks => new List<Link>
      {
         new Link { Route = this.GetRoute("HelloWorld"), Caption = "Hello World"  },
         new Link { Route = this.GetRoute("ControlTypes"), Caption = "Control Types"  },
         new Link { Route = this.GetRoute("SimpleList"), Caption = "Simple List" },
         new Link { Route = this.GetRoute("CompositeView"), Caption = "Composite View" },
         new Link { Route = this.GetRoute("LiveChart"), Caption = "Live Chart" },
         new Link { Route = this.GetRoute("BookStore"), Caption = "Book Store"  },
         new Link { Route = this.GetRoute("SecurePage"), Caption = "Secure Page"  },
      };

      public App()
      {
         this.RegisterRoutes("index", new List<RouteTemplate>
         {
            new RouteTemplate("Home",           "/module/get/HelloWorld/HelloWorldVM") { UrlPattern = "", ViewUrl = "HelloWorld" },
            new RouteTemplate("HelloWorld",     "/module/get/HelloWorld/HelloWorldVM"),
            new RouteTemplate("ControlTypes",   "/module/get/ControlTypes/ControlTypesVM"),
            new RouteTemplate("SimpleList",     "/module/get/SimpleList/SimpleListVM" ),
            new RouteTemplate("CompositeView",  "/module/get/CompositeView" ),
            new RouteTemplate("LiveChart",      "/module/get/LiveChart/LiveChartVM"),
            new RouteTemplate("BookStore",      "/module/get/BookStore/BookStoreVM"),
            new RouteTemplate("SecurePage",     "/module/get/SecurePage/SecurePageVM"),
         });
      }
   }
}
