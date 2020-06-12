using DotNetify;
using DotNetify.Elements;

namespace DevApp.Blazor.Server
{
   public class MainNav : BaseVM
   {
      public MainNav()
      {
         AddProperty("NavMenu", new NavMenu(
            new NavMenuItem[]
            {
               new NavRoute("Overview", ""),
               new NavGroup
               {
                  Label = "Basic Examples",
                  Routes = new NavRoute[]
                  {
                     new NavRoute("Counter", "counter"),
                  },
                  IsExpanded = true
               },
               new NavGroup
               {
                  Label = "Further Examples",
                  Routes = new NavRoute[]
                  {
                     new NavRoute("Dashboard", "dashboard"),
                     new NavRoute("Form", "form"),
                  },
                  IsExpanded = true
               },
               new NavGroup
               {
                  Label = "API Reference",
                  Routes = new NavRoute[]
                  {
                     new NavRoute (".NET Client", "dotnetclient"),
                     //new NavRoute ("Connection Management", this.GetRoute (nameof (Route.Connection))),
                     new NavRoute ("CRUD", "crud"),
                     new NavRoute ("Dependency Injection", "di"),
                     new NavRoute ("Filter", "filter"),
                     //new NavRoute ("Local Mode", this.GetRoute (nameof (Route.LocalMode))),
                     new NavRoute ("Middleware", "middleware"),
                     new NavRoute ("Multicast","multicast"),
                     //new NavRoute ("Routing", this.GetRoute (nameof (Route.Routing))),
                     //new NavRoute ("Security", this.GetRoute (nameof (Route.Security))),
                     //new NavRoute ("Web API Mode", this.GetRoute (nameof (Route.WebApiMode))),
                  },
                  IsExpanded = true
               }
            })
         );
      }
   }
}