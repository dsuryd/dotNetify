using System.Collections.Generic;
using System.Reactive.Linq;
using System.Linq;
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
            Home,
            Overview,
            DataFlow,
            Reactive,
            GetStarted,
            HelloWorld,
            ControlTypes,
            SimpleList,
            CompositeView,
            LiveChart,
            BookStore,
            SecurePage,
            Connection,
            CRUD,
            DI,
            Filter,
            Middleware,
            Routing,
            Security
        }
        
        private NavMenuItem[] _navMenuItems;

        public RoutingState RoutingState { get; set; }

        public App()
        {
            this.RegisterRoutes("", new List<RouteTemplate>
            {
                new RouteTemplate(nameof(Route.Home))           { UrlPattern = "", ViewUrl = nameof(Route.Overview) },
                new RouteTemplate(nameof(Route.Overview))       { UrlPattern = "overview" },
                new RouteTemplate(nameof(Route.DataFlow))       { UrlPattern = "dataflow" },
                new RouteTemplate(nameof(Route.Reactive))       { UrlPattern = "reactive" },
                new RouteTemplate(nameof(Route.GetStarted))     { UrlPattern = "getstarted" },
                new RouteTemplate(nameof(Route.HelloWorld))     { UrlPattern = "examples/helloworld" },
                new RouteTemplate(nameof(Route.ControlTypes))   { UrlPattern = "examples/controltypes" },
                new RouteTemplate(nameof(Route.SimpleList))     { UrlPattern = "examples/simplelist" },
                new RouteTemplate(nameof(Route.CompositeView))  { UrlPattern = "examples/compositeview" },
                new RouteTemplate(nameof(Route.LiveChart))      { UrlPattern = "examples/livechart" },
                new RouteTemplate(nameof(Route.BookStore))      { UrlPattern = "examples/bookstore" },
                new RouteTemplate(nameof(Route.SecurePage))     { UrlPattern = "examples/securepage" },
                new RouteTemplate(nameof(Route.Connection))     { UrlPattern = "api/connection" },
                new RouteTemplate(nameof(Route.CRUD))           { UrlPattern = "api/crud" },
                new RouteTemplate(nameof(Route.DI))             { UrlPattern = "api/di" },
                new RouteTemplate(nameof(Route.Filter))         { UrlPattern = "api/filter" },
                new RouteTemplate(nameof(Route.Middleware))     { UrlPattern = "api/middleware" },
                new RouteTemplate(nameof(Route.Routing))        { UrlPattern = "api/routing" },
                new RouteTemplate(nameof(Route.Security))       { UrlPattern = "api/security" },
            });

            _navMenuItems = new NavMenuItem[]
            {
                new NavRoute("Overview",               this.GetRoute(nameof(Route.Overview))),
                new NavRoute("Data Flow Pattern",      this.GetRoute(nameof(Route.DataFlow))),
                new NavRoute("Reactive Programming",   this.GetRoute(nameof(Route.Reactive))),
                new NavRoute("Get Started",            this.GetRoute(nameof(Route.GetStarted))),

                new NavGroup
                {
                    Label = "Basic Examples",
                    Routes = new NavRoute[]
                    {
                        new NavRoute("Hello World",   this.GetRoute(nameof(Route.HelloWorld))),
                        new NavRoute("Control Types", this.GetRoute(nameof(Route.ControlTypes))),
                        new NavRoute("Simple List",   this.GetRoute(nameof(Route.SimpleList))),
                        new NavRoute("Live Chart",       this.GetRoute(nameof(Route.LiveChart)))
                    },
                    IsExpanded = true
                },
                new NavGroup
                {
                    Label = "Further Examples",
                    Routes = new NavRoute[]
                    {
                        new NavRoute("Composite View",   this.GetRoute(nameof(Route.CompositeView))),
                        new NavRoute("Book Store",       this.GetRoute(nameof(Route.BookStore))),
                        new NavRoute("Secure Page",      this.GetRoute(nameof(Route.SecurePage))),
                    },
                    IsExpanded = false
                },
                new NavGroup
                {
                    Label = "API Reference",
                    Routes = new NavRoute[]
                    {
                        new NavRoute("Connection Management",  this.GetRoute(nameof(Route.Connection))),
                        new NavRoute("CRUD",                   this.GetRoute(nameof(Route.CRUD))),
                        new NavRoute("Dependency Injection",   this.GetRoute(nameof(Route.DI))),
                        new NavRoute("Filter",                 this.GetRoute(nameof(Route.Filter))),
                        new NavRoute("Middleware",             this.GetRoute(nameof(Route.Middleware))),
                        new NavRoute("Routing",                this.GetRoute(nameof(Route.Routing))),
                        new NavRoute("Security",               this.GetRoute(nameof(Route.Security))),
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
                navMenuItems[5] = new NavGroup
                {
                    Label = "Further Examples",
                    Routes = new NavRoute[]
                    {
                        new NavRoute("Composite View",   this.GetRoute(nameof(Route.CompositeView))),
                        new NavRoute("Book Store",       this.GetRoute(nameof(Route.BookStore)))
                    },
                    IsExpanded = false
                };
                navMenuItems.RemoveAt(1);  // Remove "Data Flow Pattern".
            }
            return new NavMenu(navMenuItems.ToArray());
        }
    }
}