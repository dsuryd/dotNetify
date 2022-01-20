using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DotNetify.Elements;
using DotNetify.Routing;

namespace DotNetify.DevApp
{
   public class App : BaseVM, IRoutable
   {
      private enum Route
      {
         Home,
         Overview,
         FromScratchWebPack,
         FromScratchScriptTag,
         FromScratchCRA,
         FromScratchCLI,
         DataFlow,
         Reactive,
         RealtimePostgres,
         MicroFrontend,
         Scaleout,
         GetStarted,
         HelloWorld,
         ControlTypes,
         SimpleList,
         CompositeView,
         LiveChart,
         BookStore,
         SecurePage,
         ChatRoom,
         Connection,
         CRUD,
         DI,
         DotNetClient,
         Filter,
         LocalMode,
         Middleware,
         MinimalApi,
         Multicast,
         Routing,
         Security,
         WebApiMode,
         DotNetifyTesting,
         DotNetifyLoadTester,
         DotNetifyObserver,
         NotFound404
      }

      private NavMenuItem[] _navMenuItems;

      public RoutingState RoutingState { get; set; }

      public App()
      {
         this.RegisterRoutes("core", new List<RouteTemplate>
         {
           new RouteTemplate (nameof (Route.Home)) { UrlPattern = "", ViewUrl = nameof (Route.Overview) },

           new RouteTemplate (nameof (Route.Overview)) { UrlPattern = "overview" },
           new RouteTemplate (nameof (Route.DataFlow)) { UrlPattern = "dataflow" },
           new RouteTemplate (nameof (Route.Reactive)) { UrlPattern = "reactive" },
           new RouteTemplate (nameof (Route.MicroFrontend)) { UrlPattern = "mfe" },
           new RouteTemplate (nameof (Route.Scaleout)) { UrlPattern = "scaleout" },
           new RouteTemplate (nameof (Route.RealtimePostgres)) { UrlPattern = "postgres" },
           new RouteTemplate (nameof (Route.GetStarted)) { UrlPattern = "getstarted" },

           new RouteTemplate (nameof (Route.HelloWorld)) { UrlPattern = "examples/helloworld" },
           new RouteTemplate (nameof (Route.ControlTypes)) { UrlPattern = "examples/controltypes" },
           new RouteTemplate (nameof (Route.SimpleList)) { UrlPattern = "examples/simplelist" },
           new RouteTemplate (nameof (Route.CompositeView)) { UrlPattern = "examples/compositeview" },
           new RouteTemplate (nameof (Route.LiveChart)) { UrlPattern = "examples/livechart" },
           new RouteTemplate (nameof (Route.BookStore)) { UrlPattern = "examples/bookstore" },
           new RouteTemplate (nameof (Route.SecurePage)) { UrlPattern = "examples/securepage" },
           new RouteTemplate (nameof (Route.ChatRoom)) { UrlPattern = "examples/chatroom" },
           new RouteTemplate (nameof (Route.Connection)) { UrlPattern = "api/connection" },
           new RouteTemplate (nameof (Route.CRUD)) { UrlPattern = "api/crud" },
           new RouteTemplate (nameof (Route.DI)) { UrlPattern = "api/di" },
           new RouteTemplate (nameof (Route.DotNetClient)) { UrlPattern = "api/dotnetclient" },
           new RouteTemplate (nameof (Route.Filter)) { UrlPattern = "api/filter" },
           new RouteTemplate (nameof (Route.LocalMode)) { UrlPattern = "api/localmode" },
           new RouteTemplate (nameof (Route.Middleware)) { UrlPattern = "api/middleware" },
           new RouteTemplate (nameof (Route.MinimalApi)) { UrlPattern = "api/minimalapi" },
           new RouteTemplate (nameof (Route.Multicast)) { UrlPattern = "api/multicast" },
           new RouteTemplate (nameof (Route.Routing)) { UrlPattern = "api/routing" },
           new RouteTemplate (nameof (Route.Security)) { UrlPattern = "api/security" },
           new RouteTemplate (nameof (Route.WebApiMode)) { UrlPattern = "api/webapimode" },

           new RouteTemplate (nameof (Route.FromScratchWebPack)) { UrlPattern = "fromscratch-webpack" },
           new RouteTemplate (nameof (Route.FromScratchScriptTag)) { UrlPattern = "fromscratch-scripttag" },
           new RouteTemplate (nameof (Route.FromScratchCRA)) { UrlPattern = "fromscratch-cra" },
           new RouteTemplate (nameof (Route.FromScratchCLI)) { UrlPattern = "fromscratch-cli" },

           new RouteTemplate (nameof (Route.DotNetifyTesting)) { UrlPattern = "dotnetify-testing" },
           new RouteTemplate (nameof (Route.DotNetifyLoadTester)) { UrlPattern = "dotnetify-loadtester" },
           new RouteTemplate (nameof (Route.DotNetifyObserver)) { UrlPattern = "dotnetify-observer" },

           new RouteTemplate (nameof (Route.NotFound404)) { UrlPattern = "*", ViewUrl = "NotFound404" },
         });

         _navMenuItems = new NavMenuItem[]
         {
            new NavRoute ("Overview", this.GetRoute (nameof (Route.Overview))),
            new NavRoute ("Get Started", this.GetRoute (nameof (Route.GetStarted))),

            new NavGroup
            {
               Label = "Topics",
               Routes = new NavRoute[]
               {
                  new NavRoute ("Data Flow Pattern", this.GetRoute (nameof (Route.DataFlow))),
                  new NavRoute ("Micro-Frontend", this.GetRoute (nameof (Route.MicroFrontend))),
                  new NavRoute ("Reactive Programming", this.GetRoute (nameof (Route.Reactive))),
                  new NavRoute ("Realtime PostgreSQL", this.GetRoute (nameof (Route.RealtimePostgres))),
                  new NavRoute ("Scale-Out", this.GetRoute (nameof (Route.Scaleout))),
               },
               IsExpanded = false
            },

            new NavGroup
            {
               Label = "Basic Examples",
               Routes = new NavRoute[]
               {
                  new NavRoute ("Hello World", this.GetRoute (nameof (Route.HelloWorld))),
                  new NavRoute ("Control Types", this.GetRoute (nameof (Route.ControlTypes))),
                  new NavRoute ("Simple List", this.GetRoute (nameof (Route.SimpleList))),
                  new NavRoute ("Live Chart", this.GetRoute (nameof (Route.LiveChart)))
               },
               IsExpanded = true
            },
           new NavGroup
           {
              Label = "Further Examples",
              Routes = new NavRoute[]
              {
                 new NavRoute ("Composite View", this.GetRoute (nameof (Route.CompositeView))),
                 new NavRoute ("Book Store", this.GetRoute (nameof (Route.BookStore))),
                 new NavRoute ("Secure Page", this.GetRoute (nameof (Route.SecurePage))),
                 new NavRoute ("Chat Room", this.GetRoute (nameof (Route.ChatRoom))),
              },
              IsExpanded = false
           },
           new NavGroup
           {
              Label = "API Reference",
              Routes = new NavRoute[]
              {
                 new NavRoute (".NET Client", this.GetRoute (nameof (Route.DotNetClient))),
                 new NavRoute ("Connection Management", this.GetRoute (nameof (Route.Connection))),
                 new NavRoute ("CRUD", this.GetRoute (nameof (Route.CRUD))),
                 new NavRoute ("Dependency Injection", this.GetRoute (nameof (Route.DI))),
                 new NavRoute ("Filter", this.GetRoute (nameof (Route.Filter))),
                 new NavRoute ("Local Mode", this.GetRoute (nameof (Route.LocalMode))),
                 new NavRoute ("Middleware", this.GetRoute (nameof (Route.Middleware))),
                 new NavRoute ("Minimal API", this.GetRoute (nameof (Route.MinimalApi))),
                 new NavRoute ("Multicast", this.GetRoute (nameof (Route.Multicast))),
                 new NavRoute ("Routing", this.GetRoute (nameof (Route.Routing))),
                 new NavRoute ("Security", this.GetRoute (nameof (Route.Security))),
                 new NavRoute ("Web API Mode", this.GetRoute (nameof (Route.WebApiMode))),
              },
              IsExpanded = false
           },
           new NavGroup
           {
              Label = "For Sponsors",
              Icon = "material-icons volunteer_activism",
               Routes = new NavRoute[]
              {
                 new NavRoute ("DotNetify-Observer", this.GetRoute (nameof (Route.DotNetifyObserver))),
                 new NavRoute ("DotNetify-LoadTester", this.GetRoute (nameof (Route.DotNetifyLoadTester))),
                 new NavRoute ("DotNetify-Testing", this.GetRoute (nameof (Route.DotNetifyTesting))),
              },
              IsExpanded = false
           }
         };

         AddProperty("NavMenu", new NavMenu(_navMenuItems))
           .SubscribeTo(AddProperty<string>("Framework").Select(framework => GetNavMenu(framework)));
      }

      private NavMenu GetNavMenu(string framework)
      {
         var navMenuItems = new List<NavMenuItem>(_navMenuItems);
         if (framework == "Knockout")
         {
            navMenuItems.RemoveAt(navMenuItems.FindIndex(x => (x as NavRoute)?.Route.TemplateId == nameof(Route.DataFlow)));

            int idx = navMenuItems.FindIndex(x => x.Label == "Further Examples");
            navMenuItems[idx] = new NavGroup
            {
               Label = "Further Examples",
               Routes = new NavRoute[]
               {
                  new NavRoute ("Composite View", this.GetRoute (nameof (Route.CompositeView))),
                  new NavRoute ("Book Store", this.GetRoute (nameof (Route.BookStore)))
               },
               IsExpanded = false
            };

            idx = navMenuItems.FindIndex(x => x.Label == "API Reference");
            navMenuItems[idx] = new NavGroup
            {
               Label = "API Reference",
               Routes = (navMenuItems[idx] as NavGroup).Routes.Where(x => x.Label != "Local Mode").ToArray(),
               IsExpanded = false
            };
         }
         else if (framework == "Vue")
         {
            int idx = navMenuItems.FindIndex(x => x.Label == "API Reference");
            navMenuItems[idx] = new NavGroup
            {
               Label = "API Reference",
               Routes = (navMenuItems[idx] as NavGroup).Routes.Where(x => x.Label != "Local Mode").ToArray(),
               IsExpanded = false
            };
         }

         return new NavMenu(navMenuItems.ToArray());
      }
   }
}