using System.Collections.Generic;
using DotNetify.Routing;

namespace DotNetify.Observer
{
   internal class NavMenu : List<NavMenuItem>
   {
      public NavMenu(NavMenuItem[] navMenuItems) : base(navMenuItems)
      {
      }
   }

   internal abstract class NavMenuItem
   {
      public string Label { get; set; }
      public string Icon { get; set; }
   }

   internal class NavGroup : NavMenuItem
   {
      public bool IsExpanded { get; set; } = true;
      public NavRoute[] Routes { get; set; }
   }

   internal class NavRoute : NavMenuItem
   {
      public Route Route { get; set; }

      public NavRoute()
      {
      }

      public NavRoute(string label, Route route, string icon = null)
      {
         Label = label;
         Route = route;
         Icon = icon;
      }

      public NavRoute(string label, string url, string icon = null) : this(label, new Route { RedirectRoot = "/", Path = url }, icon)
      {
      }
   }
}