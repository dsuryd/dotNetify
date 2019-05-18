using DotNetify;
using DotNetify.Elements;
using DotNetify.Routing;

namespace Blazor.Server.ViewModels
{
    public class MainNav : BaseVM, IRoutable
    {
        public RoutingState RoutingState { get; set; }

        public MainNav()
        {
            AddProperty("NavMenu", new NavMenu(
            new NavMenuItem[]
            {
                new NavRoute("Home", this.Redirect("/", "")),
                new NavRoute("Counter", this.Redirect("/", "counter")),
                new NavRoute("Fetch", this.Redirect("/", "fetchdata")),
                new NavRoute("Dashboard", this.Redirect("/", "dashboard")),
                new NavRoute("Form", this.Redirect("/", "form")),
            }));
        }
    }
}