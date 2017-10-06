using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using DotNetify.Routing;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace WebApplication.Core.Controllers
{
   public class HomeController : Controller
   {
      private readonly IHostingEnvironment _hostingEnvironment;

      public HomeController(IHostingEnvironment hostingEnvironment)
      {
         _hostingEnvironment = hostingEnvironment;
      }

      public IActionResult Index(string id) => Demo("index");

      [Route("Demo/{*id}")]
      public IActionResult Demo(string id)
      {
         if (String.IsNullOrEmpty(id))
            id = "index";

         if (id.EndsWith("_cshtml"))
            return View("/Views/" + id.Replace("_cshtml", ".cshtml"));

         // If not ending with .js or .map, assume it's a request for static html file.
         if (!id.EndsWith(".js") && !id.EndsWith(".map"))
            id = Path.Combine(_hostingEnvironment.ContentRootPath, "Views/" + (id.EndsWith(".html") ? id : id + ".html"));
         return File(id);
      }

      [Route("WebStore/{*id}")]
      public IActionResult WebStore(string id)
      {
         var initialRoutingData = new RoutingViewData("/webstore/" + id, "WebStore/Index_cshtml", typeof(ViewModels.WebStore.NavBarVM));
         return ViewComponent("Routing", new { iViewData = initialRoutingData });
      }

      private FileStreamResult File(string path)
      {
         var mimeType = "text/plain";
         if (path.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
            mimeType = "text/js";
         else if (path.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
            mimeType = "text/html";
         return File(System.IO.File.OpenRead(path), mimeType);
      }
   }

   /// <summary>
   /// View component to build content for server-side routing.
   /// </summary>
   public class RoutingViewComponent : ViewComponent
   {
      private readonly IHostingEnvironment _hostingEnvironment;

      public RoutingViewComponent(IHostingEnvironment hostingEnvironment)
      {
         _hostingEnvironment = hostingEnvironment;
      }

      public IViewComponentResult Invoke(RoutingViewData iViewData)
      {
         IRoutable model;
         var viewId = RoutableExtension.Route(ref iViewData, out model);
         if (viewId != null)
         {
            ViewData["Routing"] = iViewData;
            var id = viewId.Replace("/Demo/", "");
            if (id.EndsWith("_cshtml"))
               return View("/Views/" + id.Replace("_cshtml", ".cshtml"), model?.GetType() != typeof(object) ? model : null);

            var htmlFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Views/" + (id.EndsWith(".html") ? id : id + ".html"));
            return new HtmlResult(htmlFilePath);
         }
         return null;
      }
   }

   /// <summary>
   /// View component result that returns HTML content given its file path.
   /// </summary>
   public class HtmlResult : IViewComponentResult
   {
      private string _html;

      public HtmlResult(string htmlFilePath)
      {
         var fileStream = File.OpenRead(htmlFilePath);
         using (StreamReader reader = new StreamReader(fileStream))
         {
            _html = reader.ReadToEnd();
         }
      }

      public void Execute(ViewComponentContext context) => context.Writer.Write(_html);
      public Task ExecuteAsync(ViewComponentContext context) => context.Writer.WriteAsync(_html);
   }
}
