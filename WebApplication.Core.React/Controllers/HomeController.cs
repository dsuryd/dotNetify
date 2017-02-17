using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using DotNetify;
using DotNetify.Routing;

namespace WebApplication.Core.React.Controllers
{
   public class HomeController : Controller
   {
      private IHostingEnvironment _hostingEnv;

      public HomeController(IHostingEnvironment hostingEnv )
      {
         _hostingEnv = hostingEnv;
      }

      [Route("{*id}")]
      public IActionResult Index(string id) => string.IsNullOrEmpty(id) ? (IActionResult) RedirectToAction(nameof(Demo), "Home") : this.File(_hostingEnv, id);

      // The single-page app's entry point - always starts from index.html.
      [Route("demo/{*id}")]
      public IActionResult Demo(string id) => this.File( _hostingEnv, "index.html");

      // Provides initial view model states for faster client-side rendering.
      [Route("state/get/{*id}")]
      public IActionResult State(string id) => Content( $"window.vmStates = window.vmStates || {{}}; window.vmStates.{id} = {new VMController().GetInitialState(id)};", "text/js" );
   }
}
