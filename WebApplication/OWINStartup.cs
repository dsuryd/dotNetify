using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebApplication.OWINStartup))]

namespace WebApplication
{
    public class OWINStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
