using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using DotNetify;
using DotNetify.Routing;

namespace WebApplication.Core.React.Controllers
{
   public class HomeController : Controller
   {
      private IHostingEnvironment _hostingEnv;

      public HomeController(IHostingEnvironment hostingEnv)
      {
         _hostingEnv = hostingEnv;
      }

      // The single-page app's entry point - always starts from index.html.
      [Route("{*id}")]
      public IActionResult Index(string id) => this.File(_hostingEnv, "index.html");
      
      // Returns JS files for Composite View example, including initial view model states for faster client-side rendering.
      [Route("module/get/CompositeView")]
      public IActionResult AFITop100(string view, string vm)
      {
         var js = this.GetEmbeddedResource("PaginatedTable.js") + this.GetEmbeddedResource("CompositeView.js");
         string vmState = BuildStateString("AFITop100VM")
            + BuildStateString("AFITop100VM.FilterableMovieTableVM.MovieTableVM")
            + BuildStateString("AFITop100VM.MovieDetailsVM");
         return Content(vmState + js, "text/js");
      }

      // Returns JS file of a view, including initial view model states for faster client-side rendering.
      [Route("module/get/{view}/{vm?}")]
      public IActionResult Module(string view, string vm)
      {
         var js = this.GetEmbeddedResource($"{view}.js");
         string vmState = vm != null ? BuildStateString(vm) : null;
         return Content(vmState + js, "text/js");
      }

      private string BuildStateString(string vm) => $"window.vmStates = window.vmStates || {{}}; window.vmStates['{vm}'] = {VMController.GetInitialState(vm) ?? "{}"};";
   }
}
