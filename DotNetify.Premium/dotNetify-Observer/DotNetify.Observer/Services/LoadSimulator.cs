using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;

namespace DotNetify.Observer.Test
{
   internal static class LoadSimulator
   {
      private class MockHubCallerContext : HubCallerContext
      {
         public string _connectionId { get; set; }
         public override string ConnectionId => _connectionId;
         public override string UserIdentifier => throw new NotImplementedException();
         public override ClaimsPrincipal User => throw new NotImplementedException();
         public override IDictionary<object, object> Items { get; } = new Dictionary<object, object>();
         public override IFeatureCollection Features => throw new NotImplementedException();
         public override CancellationToken ConnectionAborted => throw new NotImplementedException();

         public override void Abort() => throw new NotImplementedException();
      }

      internal static void SimulateHubMessage(IConnectionTracker tracker, int suffix, int limit, int interval = 100)
      {
         int count = -10;

         var hubCallerContext = new MockHubCallerContext
         {
            _connectionId = $"hub-{suffix}"
         };

         Observable
            .Interval(TimeSpan.FromMilliseconds(interval))
               .Subscribe(_ =>
               {
                  count++;
                  if (count < 1)
                     return;
                  else if (count <= limit)
                  {
                     var connectionContext = new ConnectionContext
                     {
                        ConnectionId = $"{count}-{suffix}",
                        HttpConnection = new HttpConnection($"http-{count}-{suffix}", $"::{count}-{suffix}", $"::{count}-{suffix}", 1, 1),
                        HttpRequestHeaders = new HttpRequestHeaders(new Dictionary<string, object>() { { "Host", $"[\"1-{suffix}\"]" } }, "UserAgent"),
                        Items = new Dictionary<string, object>()
                     };
                     var hubContext = new DotNetifyHubContext(hubCallerContext, nameof(IDotNetifyHubMethod.Request_VM), "HelloVM", null, null, new ClaimsPrincipal());
                     tracker.ReceiveHubMessage(connectionContext, hubContext);
                  }
                  else if (count <= limit * 2)
                  {
                     var countReverse = limit * 2 + 1 - count;

                     var connectionContext = new ConnectionContext
                     {
                        ConnectionId = $"{countReverse}-{suffix}",
                        HttpConnection = new HttpConnection($"http-{countReverse}-{suffix}", $"::{countReverse}-{suffix}", $"::{countReverse}-{suffix}", 1, 1),
                        HttpRequestHeaders = new HttpRequestHeaders(new Dictionary<string, object>() { { "Host", $"[\"1-{suffix}\"]" } }, "UserAgent"),
                        Items = new Dictionary<string, object>()
                     };

                     tracker.ReceiveDisconnection(connectionContext);
                  }
                  else
                     count = 0;
               });
      }
   }
}