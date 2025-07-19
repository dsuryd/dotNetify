using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetify.Forwarding;
using DotNetify.Util;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;

namespace DotNetify.Observer
{
   /// <summary>
   /// The middleware used for observing forwarded hub messages by other servers.
   /// </summary>
   public class ObserveForwardingMiddleware : IForwardingMiddleware
   {
      private readonly IConnectionTracker _connectionTracker;

      public ObserveForwardingMiddleware(IConnectionTracker connectionTracker)
      {
         License.CheckAtLeastTeam();

         _connectionTracker = connectionTracker;
      }

      public async Task Invoke(DotNetifyHubContext hubContext, NextDelegate next)
      {
         // Origin context is the connection context on the forwarding hub when it received the message.
         var originContext = hubContext.CallerContext.GetOriginConnectionContext();
         if (originContext != null)
         {
            _connectionTracker.ReceiveHubMessage(originContext, hubContext);
         }
         // Hub that uses the Observer.Client package will send telemetry data every second.
         else if (hubContext.CallType == nameof(IConnectionTracker.ReceiveTelemetry) && hubContext.Data is object[])
         {
            if (hubContext.CallerContext.Items.ContainsKey(nameof(ConnectionContext.HubId)))
            {
               object info = (hubContext.Data as object[])[0];
               string hubId = hubContext.CallerContext.Items[nameof(ConnectionContext.HubId)]?.ToString();
               string hubName = hubContext.CallerContext.GetConnectionContext().HttpConnection.RemoteIpAddressString;

               _connectionTracker.ReceiveTelemetry(hubId, hubName, ToDictionary(info));
            }
         }
         else
            await next(hubContext);
      }

      public Task OnDisconnected(HubCallerContext callerContext)
      {
         var originContext = callerContext.GetOriginConnectionContext();
         if (originContext != null)
            _connectionTracker.ReceiveDisconnection(originContext);

         return Task.CompletedTask;
      }

      private Dictionary<string, object> ToDictionary(object info)
      {
         if (info is JsonElement || info is JObject)
            return info.ToString().ConvertFromString(typeof(Dictionary<string, object>)) as Dictionary<string, object>;
         else if (info is Dictionary<object, object>) // MessagePack
            return (info as Dictionary<object, object>).ToDictionary(x => (string) x.Key, x => x.Value);
         return info as Dictionary<string, object>;
      }
   }
}