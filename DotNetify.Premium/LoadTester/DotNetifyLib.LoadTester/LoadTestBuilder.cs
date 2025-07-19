using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DotNetify.LoadTester
{
   public class LoadTestBuilder
   {
      private static readonly int MIN_DELAY_BETWEEN_CLIENTS = 100;

      private ILogger _logger;
      private readonly List<LoadTestClient> _clients = new List<LoadTestClient>();
      private readonly CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
      private readonly string _serverUrl;
      private TimeSpan? _rampUpPeriod;
      private TimeSpan? _rampDownPeriod;
      private Action _completedHandler;

      private static uint _totalClients = 0;

      public LoadTestBuilder(string serverUrl)
      {
         try
         {
            License.CheckOnlyInstance();
         }
         catch (Exception ex)
         {
            throw ex;
         }

         _serverUrl = serverUrl;
      }

      public LoadTestBuilder AddClient(string clientPrefix, uint numberOfClients, Action<ILoadTestClientBuilder, uint> configureClient)
      {
         _totalClients += numberOfClients;

         clientPrefix = clientPrefix ?? Guid.NewGuid().ToString().Substring(0, 4);

         uint index = 0;
         while (index++ < numberOfClients)
         {
            var clientBuilder = new LoadTestClientBuilder($"{clientPrefix}.{index}", _serverUrl, _cancelTokenSource, _logger);
            configureClient(clientBuilder, index);
            _clients.Add(clientBuilder.Build());
         }
         return this;
      }

      public LoadTestBuilder AddClient(uint numberOfClients, Action<ILoadTestClientBuilder, uint> configureClient) => AddClient(null, numberOfClients, configureClient);

      public LoadTestBuilder AddLogger(ILogger logger)
      {
         _logger = logger;
         return this;
      }

      public LoadTestBuilder SetRampUpPeriod(TimeSpan period)
      {
         _rampUpPeriod = period;
         return this;
      }

      public LoadTestBuilder SetRampDownPeriod(TimeSpan period)
      {
         _rampDownPeriod = period;
         return this;
      }

      public LoadTestBuilder OnCompleted(Action handler)
      {
         _completedHandler = handler;
         return this;
      }

      public async Task RunAsync(TimeSpan duration)
      {
         if (_clients.Count == 0)
            throw new InvalidOperationException("No load test client was defined!");

         int rampUpDelay = _rampUpPeriod.HasValue ? (int) (_rampUpPeriod.Value.TotalMilliseconds / _clients.Count) : MIN_DELAY_BETWEEN_CLIENTS;
         int rampDownDelay = _rampDownPeriod.HasValue ? (int) (_rampDownPeriod.Value.TotalMilliseconds / _clients.Count) : MIN_DELAY_BETWEEN_CLIENTS;

         _logger?.Log(LogLevel.Information, $"Clients={_clients.Count}, Duration={duration.TotalSeconds}s, Ramp Up Interval={rampUpDelay}ms, Ramp Down Interval={rampDownDelay}ms");

         var tasks = new List<Task>();

         ThreadPool.GetMinThreads(out _, out int minIOThreads);
         ThreadPool.SetMinThreads((int) _totalClients, minIOThreads);

         foreach (var client in _clients)
         {
            string getError(Exception ex)
            {
               string error = ex.Message;
               if (ex.InnerException != null)
                  error += $" - {ex.InnerException.Message}";
               return error;
            }

            tasks.Add(Task.Run(async () =>
             {
                try
                {
                   _logger?.LogTrace($"Starting client {client.Id}");
                   await client.StartAsync();

                   client.Dispose();
                   _logger?.LogTrace($"Client {client.Id} disposed");
                }
                catch (Exception ex)
                {
                   _logger?.Log(LogLevel.Error, $"Failed to run client {client.Id}: {getError(ex)}");
                }
             }));

            await Task.Delay(rampUpDelay);
         }

         await Task.Delay(duration);

         foreach (var client in _clients)
         {
            client.Stop();
            await Task.Delay(rampDownDelay);
         }

         _cancelTokenSource.Cancel();
         await Task.WhenAll(tasks.ToArray());

         _completedHandler?.Invoke();
      }
   }
}