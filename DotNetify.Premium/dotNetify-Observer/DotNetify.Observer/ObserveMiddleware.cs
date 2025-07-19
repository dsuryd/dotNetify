using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetify.Forwarding;
using DotNetify.Security;
using DotNetify.Util;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;

namespace DotNetify.Observer
{
   /// <summary>
   /// The middleware used for observing local hub messages.
   /// </summary>
   public class ObserveMiddleware : IForwardingMiddleware
   {
      private static readonly string[] _internalVMs = new string[]
      {
         nameof(ObserverAppVM), nameof(ConnectionGraphVM), nameof(HubInfoVM), nameof(NodeInfoVM)
      };

      private readonly IConnectionTracker _connectionTracker;

      public ObserveMiddleware(IConnectionTracker connectionTracker)
      {
         _connectionTracker = connectionTracker;
      }

      public async Task Invoke(DotNetifyHubContext hubContext, NextDelegate next)
      {
         if (!_internalVMs.Contains(hubContext.VMId))
         {
            var context = hubContext.CallerContext.GetConnectionContext();
            if (context != null)
               _connectionTracker.ReceiveHubMessage(context, hubContext);
         }

         await next(hubContext);
      }

      public Task OnDisconnected(HubCallerContext callerContext)
      {
         var context = callerContext.GetConnectionContext();
         if (context != null)
            _connectionTracker.ReceiveDisconnection(context);

         return Task.CompletedTask;
      }
   }
}