using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ReactWebApp.Controllers
{
   public static class ControllerExtensions
   {
      /// <summary>
      /// Returns the content of a static file.
      /// </summary>
      public static FileStreamResult File(this Controller controller, IHostingEnvironment hostingEnv, string fileName)
      {
         string path = fileName;
         var mimeType = "text/plain";
         if (fileName.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
            mimeType = "text/js";
         else if (fileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase) || !fileName.Contains("."))
         {
            fileName = fileName.EndsWith(".html") ? fileName : fileName + ".html";
            path = Path.Combine(hostingEnv.ContentRootPath, "wwwroot\\" + fileName);
            mimeType = "text/html";
         }

         try
         {
            return controller.File(System.IO.File.OpenRead(path), mimeType);
         }
         catch (Exception)
         {
            Trace.WriteLine($"{path} not found");
            return null;
         }
      }
   }
}
