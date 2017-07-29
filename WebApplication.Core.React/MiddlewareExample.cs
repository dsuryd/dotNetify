using DotNetify;
using Newtonsoft.Json;
using System.Diagnostics;

namespace WebApplication.Core.React
{
   public class LogRequestMiddleware : IMiddleware
   {
      public void Invoke(DotNetifyHubContext hubContext)
      {
         Trace.WriteLine($"{hubContext.CallType} {hubContext.VMId}, {JsonConvert.SerializeObject(hubContext.Data)}");
      }
   }

}
