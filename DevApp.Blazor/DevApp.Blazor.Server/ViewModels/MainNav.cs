using DotNetify;
using DotNetify.Elements;

namespace DevApp.Blazor.Server.ViewModels
{
   public class MainNav : BaseVM
   {
      public MainNav()
      {
         AddProperty( "NavMenu", new NavMenu(
            new NavMenuItem[]
            {
                   new NavRoute("Home", ""),
                   new NavRoute("Counter", "counter"),
                   new NavRoute("Dashboard", "dashboard"),
                   new NavRoute("Form", "form"),
            } ) );
      }
   }
}