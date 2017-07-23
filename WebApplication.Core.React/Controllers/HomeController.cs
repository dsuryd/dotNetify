using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using DotNetify;
using DotNetify.Routing;

namespace WebApplication.Core.React.Controllers
{
   public class HomeController : Controller
   {
      private readonly IHostingEnvironment _hostingEnv;

      public HomeController(IHostingEnvironment hostingEnv)
      {
         _hostingEnv = hostingEnv;
      }

      // The single-page app's entry point - always starts from index.html.
      [Route("{*id}")]
      public IActionResult Index(string id) => this.FileResult(_hostingEnv, "index.html");

      // Returns JS files for Composite View example, including initial view model states for faster client-side rendering.
      [Route("module/get/CompositeView")]
      public IActionResult CompositeView(string view, string vm) => Content(string.Concat(
         GetInitialState(new List<string>
         {
            "AFITop100VM",
            "AFITop100VM.FilterableMovieTableVM.MovieTableVM",
            "AFITop100VM.MovieDetailsVM"
         }),
         GetJavascript(new List<string>
         {
            "PaginatedTable",
            "CompositeView"
         })), "text/js");

      // Returns JS file of a view, including initial view model states for faster client-side rendering.
      [Route("module/get/{view}/{vm?}")]
      public IActionResult Module(string view, string vm) => Content(string.Concat(GetInitialState(vm), GetJavascript(view)), "text/js");

      #region Private Methods

      private string GetJavascript(string view) => this.File(_hostingEnv, $"/js/{view}.js");

      private string GetInitialState(string vm) => vm == null ? null : $@"
         window.vmStates = window.vmStates || {{}}; 
         window.vmStates['{vm}'] = {VMController.GetInitialState(vm) ?? "{}"};";

      private string GetJavascript(List<string> views) => views.Select(view => GetJavascript(view)).Aggregate((s1, s2) => string.Concat(s1, s2));
      private string GetInitialState(List<string> viewModels) => viewModels.Select(vm => GetInitialState(vm)).Aggregate((s1, s2) => string.Concat(s1, s2));

      #endregion
   }
}
