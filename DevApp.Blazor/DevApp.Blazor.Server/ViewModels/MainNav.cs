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
            })
         );
      }
   }
}