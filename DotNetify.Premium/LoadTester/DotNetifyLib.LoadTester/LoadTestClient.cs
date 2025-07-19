using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DotNetify.LoadTester
{
   public interface IClientVM
   {
      string ClientId { get; }
      string VMId { get; }

      void Dispatch(object args);

      void Destroy();
   }

   internal class LoadTestClient : IDisposable
   {
      private readonly string _serverUrl;
      private readonly CancellationTokenSource _cancelTokenSource;
      private readonly CancellationTokenSource _stopTokenSource;
      private readonly AutoResetEvent _actionEvent = new AutoResetEvent(false);
      private readonly ConcurrentQueue<Func<LoadTestClient, Task>> _actions = new ConcurrentQueue<Func<LoadTestClient, Task>>();
      private readonly Dictionary<string, ServerResponseDelegate> _serverResponseHandlers = new Dictionary<string, ServerResponseDelegate>();
      private readonly Dictionary<string, Action<IClientVM>> _destroyedHandlers = new Dictionary<string, Action<IClientVM>>();
      private readonly Dictionary<string, ClientConnection> _connections = new Dictionary<string, ClientConnection>();
      private readonly ILogger _logger;

      private class ClientConnection : IDisposable
      {
         private IDisposable _responseSubs;

         public IDotNetifyClient Client { get; }
         public bool IsDisposed { get; private set; }

         public ClientConnection(IDotNetifyClient client)
         {
            Client = client;
         }

         public void OnServerResponse(Action<object[]> onNext)
         {
            _responseSubs = Client.ResponseStream.Subscribe(x => onNext(x));
         }

         public void Dispose()
         {
            if (!IsDisposed)
            {
               IsDisposed = true;
               _responseSubs.Dispose();
               Client.DestroyAsync().GetAwaiter().GetResult();
               Client.Dispose();
            }
         }
      }

      public class ClientVM : IClientVM
      {
         private readonly LoadTestClient _client;

         public string ClientId => _client.Id;
         public string VMId { get; }

         public ClientVM(LoadTestClient client, string vmId)
         {
            _client = client;
            VMId = vmId;
         }

         public void Dispatch(object args)
         {
            _client.AddAction(x => x.DispatchAsync(VMId, args));
         }

         public void Destroy() => _client.Destroy(VMId);
      }

      public string Id { get; set; }
      public CancellationToken StopToken => _stopTokenSource.Token;

      public LoadTestClient(string clientId, string serverUrl, CancellationTokenSource cancelTokenSource, ILogger logger)
      {
         Id = clientId;
         _serverUrl = serverUrl;
         _cancelTokenSource = cancelTokenSource;
         _stopTokenSource = new CancellationTokenSource();
         _logger = logger;
      }

      public void Dispose()
      {
         _connections.Values.ToList().ForEach(x => x.Dispose());
      }

      public void AddAction(Func<LoadTestClient, Task> action)
      {
         _actions.Enqueue(action);
         _actionEvent.Set();
      }

      public void HandleServerResponse(string vmId, ServerResponseDelegate handler) => _serverResponseHandlers[vmId] = handler;

      public void HandleDestroyed(string vmId, Action<IClientVM> handler) => _destroyedHandlers[vmId] = handler;

      public async Task StartAsync()
      {
         bool isCancelled() => _cancelTokenSource.IsCancellationRequested || _stopTokenSource.IsCancellationRequested;

         do
         {
            while (_actions.Count > 0 && !isCancelled())
            {
               if (_actions.TryDequeue(out Func<LoadTestClient, Task> action))
               {
                  try
                  {
                     await action.Invoke(this);
                  }
                  catch (TaskCanceledException) { }
               }
            }

            WaitHandle.WaitAny(new[] { _actionEvent, _cancelTokenSource.Token.WaitHandle, _stopTokenSource.Token.WaitHandle });
         }
         while (!isCancelled());

         foreach (var connection in _connections)
         {
            var vmId = connection.Key;
            Destroy(vmId);
            if (_destroyedHandlers.ContainsKey(vmId))
               _destroyedHandlers[vmId](new ClientVM(this, vmId));
         }
      }

      public void Stop()
      {
         _stopTokenSource.Cancel();
      }

      public async Task ConnectAsync(string vmId, VMConnectOptions options)
      {
         if (!_connections.ContainsKey(vmId))
         {
            try
            {
               var client = await DotNetifyHubClient.CreateClientAsync(_serverUrl);

               _connections.Add(vmId, new ClientConnection(client));
               var connection = _connections[vmId];

               if (_serverResponseHandlers.ContainsKey(vmId))
               {
                  var clientVM = new ClientVM(this, vmId);
                  connection.OnServerResponse(x =>
                  {
                     if (!connection.IsDisposed)
                        _serverResponseHandlers[vmId](clientVM, new ServerResponse(x));
                  });
               }

               await connection.Client.ConnectAsync(vmId, options);
            }
            catch (Exception ex)
            {
               string message = ex.Message;
               if (ex.InnerException != null)
                  message += $" - {ex.InnerException.Message}";
               _logger?.LogError($"Client {Id} failed to connect to {_serverUrl}: {message}");
            }
         }
      }

      public async Task DispatchAsync(string vmId, object dispatchArgs)
      {
         if (!_cancelTokenSource.IsCancellationRequested && !_stopTokenSource.IsCancellationRequested && _connections.ContainsKey(vmId))
         {
            var connection = _connections[vmId];
            if (!connection.IsDisposed)
            {
               if (dispatchArgs is Func<IClientVM, object>)
               {
                  var clientVM = new ClientVM(this, vmId);
                  var data = (dispatchArgs as Func<IClientVM, object>)(clientVM);
                  await connection.Client.DispatchAsync(data);
               }
               else
                  await connection.Client.DispatchAsync(dispatchArgs);
            }
         }
      }

      public void Destroy(string vmId)
      {
         if (_connections.ContainsKey(vmId))
            _connections[vmId].Dispose();
      }
   }
}