using Microsoft.Owin;
using Owin;
using System.Reflection;
using DotNetify;

[assembly: OwinStartup(typeof(HelloWorld.Startup))]

namespace HelloWorld
{
   public class Startup
   {
      public void Configuration(IAppBuilder app)
      {
         var vmAssembly = Assembly.GetExecutingAssembly();
         app.MapSignalR();
         app.UseDotNetify(config => config.RegisterAssembly(vmAssembly));
      }
   }
}