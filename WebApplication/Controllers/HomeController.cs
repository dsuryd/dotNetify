using System;
using System.Web.Mvc;
using DotNetify.Routing;

namespace WebApplication.Controllers
{
   public class HomeController : Controller
   {
      [Route("{*id}")]
      public ActionResult Index( string id )
      {
         return Demo("index", null);
      }

      [Route("Demo/{*id}")]
      public ActionResult Demo( string id, object iModel )
      {
         if ( String.IsNullOrEmpty(id) )
            id = "index";

         if ( id.EndsWith("_cshtml") )
            return View("/Views/" + id.Replace("_cshtml", ".cshtml"), iModel != null && iModel.GetType() != typeof(object) ? iModel : null);
         return File(Server.MapPath("/Views/" + ( id.EndsWith(".html") ? id : id + ".html" )), "text/html");
      }

      [Route("WebStore/{*id}")]
      public ActionResult WebStore( string id )
      {
         return Routing(new RoutingViewData("/webstore/" + id, "WebStore/Index_cshtml", typeof(ViewModels.WebStore.NavBarVM)));
      }

      public ActionResult Routing( RoutingViewData iViewData )
      {
         IRoutable model;
         var viewId = RoutableExtension.Route(ref iViewData, out model);
         if ( viewId != null )
         {
            ViewData["Routing"] = iViewData;
            return Demo(viewId.Replace("/Demo/", ""), model);
         }
         return new EmptyResult();
      }
   }
}