using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNetify.Forwarding;
using Microsoft.Extensions.Hosting;

namespace DotNetify.Observer
{
   internal class TelemetryCollector : IHostedService
   {
      private const int CONNECT_RETRY_DELAY_SEC = 60;

      private static readonly IList<string> _serverUrls = new List<string>();
      private static readonly Dictionary<string, DateTime?> _lastFailedConnect = new Dictionary<string, DateTime?>();

      private readonly MetricsReader _metricsReader = new MetricsReader();
      private readonly IDotNetifyHubForwarderFactory _hubForwarderFactory;
      private readonly ForwardingOptions _forwardingOptions;
      private Timer _timer;

      public TelemetryCollector(IDotNetifyHubForwarderFactory hubForwarderFactory)
      {
         _hubForwarderFactory = hubForwarderFactory;
         _forwardingOptions = new ForwardingOptions
         {
            HaltPipeline = false,
            ConnectionPoolSize = 1
         };
      }

      public Task StartAsync(CancellationToken cancellationToken)
      {
         var closestSecond = 1000 - DateTime.UtcNow.Millisecond;
         _timer = new Timer(_ => DoWork(), null, closestSecond, 1000);

         return Task.CompletedTask;
      }

      public Task StopAsync(CancellationToken cancellationToken)
      {
         _timer.Dispose();
         return Task.CompletedTask;
      }

      public static void AddServerUrl(string url)
      {
         _serverUrls.Add(url);
         _lastFailedConnect.Add(url, null);
      }

      private IDictionary<string, object> GetMetrics()
      {
         return new Dictionary<string, object>
         {
            { "proc_cpu", _metricsReader.CpuUsage },
            { "total_cpu", _metricsReader.TotalCpuUsage },
            { "proc_mem", _metricsReader.MemoryUsage },
            { "total_mem", _metricsReader.TotalMemoryUsage },
         };
      }

      private void DoWork()
      {
         var context = new ConnectionContext();

         foreach (var serverUrl in _serverUrls)
         {
            DateTime? lastFailedConnect = _lastFailedConnect[serverUrl];

            // Set delay before retrying failed connection.
            if (lastFailedConnect != null && (DateTime.Now - lastFailedConnect.Value).TotalSeconds < CONNECT_RETRY_DELAY_SEC)
               return;

            _lastFailedConnect[serverUrl] = DateTime.Now;
            _hubForwarderFactory.InvokeInstanceAsync(serverUrl, _forwardingOptions, async hubForwarder =>
            {
               if (hubForwarder.IsConnected)
               {
                  _lastFailedConnect[serverUrl] = null;

                  await hubForwarder.InvokeAsync("ReceiveTelemetry", new object[] { GetMetrics() },
                     new Dictionary<string, object>
                     {
                        { nameof(ConnectionContext.HubId), context.HubId }
                     });
               }
            });
         }
      }
   }
}