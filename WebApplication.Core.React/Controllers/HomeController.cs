using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using DotNetify.Routing;

namespace ReactWebApp.Controllers
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

      [Route("demo/{*id}")]
      public IActionResult Demo(string id) => this.File( _hostingEnv, "index.html");
   }
}
