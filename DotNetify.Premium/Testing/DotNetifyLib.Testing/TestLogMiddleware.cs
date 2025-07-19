using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace DotNetify.Testing
{
   /// <summary>
   /// Middleware to log test outputs.
   /// </summary>
   public class TestLogMiddleware : IMiddleware, IDisconnectionMiddleware, IExceptionMiddleware
   {
      private readonly LogTraceDelegate _trace;

      public TestLogMiddleware(LogTraceDelegate trace)
      {
         _trace = trace;
      }

      public Task Invoke(DotNetifyHubContext hubContext, NextDelegate next)
      {
         object data = hubContext.Data;
         if (data is string)
            data = JsonConvert.DeserializeObject<dynamic>(hubContext.Data.ToString());

         var type = $"[{hubContext.CallType}]    ".Substring(0, 13);

         var log = $@"{type} connId={hubContext.CallerContext.ConnectionId}
              vmId={hubContext.VMId}
              data={JsonConvert.SerializeObject(data ?? string.Empty, Formatting.None)}";

         if (hubContext.Headers != null)
            log += $@"
              headers={JsonConvert.SerializeObject(hubContext.Headers, Formatting.None)}";

         _trace(log);
         return next(hubContext);
      }

      public Task OnDisconnected(HubCallerContext context)
      {
         _trace($"[Disconnected] connId={context.ConnectionId} type=OnDisconnected");
         return Task.CompletedTask;
      }

      public Task<Exception> OnException(HubCallerContext context, Exception exception)
      {
         _trace($"[Exception] connId={context.ConnectionId} {exception.GetType().Name}={exception.Message}");
         return Task.FromResult(exception);
      }
   }
}