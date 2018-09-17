using System;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
    public class Routing : BaseVM
    {
        public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.Routing.md");
    }

    public class RoutingKO : BaseVM
    {
        public string Content 
        {
            get 
            {
                var content = new Markdown("DotNetify.DevApp.Docs.APIs.Routing.md");
                var contentKO = new Markdown("DotNetify.DevApp.Docs.Knockout.APIs.Routing.md");
                return  contentKO.GetSection(null, "Setting Up Route Links")
                    + content.GetSection("Defining the Routes", "Setting Up Route Links")
                    + contentKO.GetSection("Setting Up Route Links")
                    + content.GetSection("Redirection", "Getting Initial State");
            }
        } 
    } 
}