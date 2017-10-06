using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Core.React.Controllers
{
   public static class ControllerExtensions
   {
      /// <summary>
      /// Returns the content of a static file.
      /// </summary>
      public static FileStreamResult FileResult(this Controller controller, IHostingEnvironment hostingEnv, string fileName)
      {
         string path = fileName;
         path = Path.Combine(hostingEnv.ContentRootPath, "wwwroot/" + fileName);

         var mimeType = "text/plain";
         if (fileName.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
            mimeType = "text/js";
         else if (fileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
            mimeType = "text/html";

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

      public static string File( this Controller controller, IHostingEnvironment hostingEnv, string fileName )
      {
         var stream = controller.FileResult(hostingEnv, fileName).FileStream;
         using ( var streamReader = new StreamReader(stream) )
            return streamReader.ReadToEnd();
      }
   }
}
