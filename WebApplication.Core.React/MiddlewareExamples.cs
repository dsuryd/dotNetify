using System.Diagnostics;
using DotNetify;
using Newtonsoft.Json;

namespace WebApplication.Core.React
{
   public class LogRequestMiddleware : IMiddleware
    {
      public void Invoke(IDotNetifyHubContext hubContext)
      {
         Trace.WriteLine($"{hubContext.MethodName} {hubContext.VMId}, {JsonConvert.SerializeObject(hubContext.Data)}");
      }
    }

   public class LogUserMiddleware : IMiddleware
   {
      public void Invoke(IDotNetifyHubContext hubContext)
      {
         Trace.WriteLine(JsonConvert.SerializeObject(hubContext.CallerContext.User));
         throw new System.UnauthorizedAccessException();
      }
   }
}
