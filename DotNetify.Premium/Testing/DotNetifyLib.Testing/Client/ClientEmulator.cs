using DotNetify.Client;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DotNetify.Testing
{
   /// <summary>
   /// Emulates a dotNetify client.
   /// </summary>
   public class ClientEmulator : IClientEmulator, IClientProxy
   {
      private readonly IDotNetifyHubEmulatorProxy _dotNetifyHub;
      private readonly ClientContext _clientContext;
      private readonly Action _dispose;
      private IDisposable _unsubscribeResponse;
      private string _vmId;
      private ClientViewState _viewState;

      /// <summary>
      /// DotNetify hub.
      /// </summary>
      public DotNetifyHub Hub => _dotNetifyHub.Hub;

      /// <summary>
      /// Mock SignalR connection ID.
      /// </summary>
      public string ConnectionId => _clientContext.ConnectionId;

      /// <summary>
      /// Maximum number of responses to record.
      /// </summary>
      public int MaxResponses { get; set; } = ClientSession.MAX_RESPONSES;

      /// <summary>
      /// Timeout in milliseconds waiting for responses.
      /// </summary>
      public int ResponseTimeout { get; set; } = ClientSession.WAIT_TIMEOUT;

      /// <summary>
      /// Accumulated responses from the server.
      /// </summary>
      public EmulationResponses ResponseHistory { get; } = new EmulationResponses();

      /// <summary>
      /// Stream of every server response received by the client.
      /// </summary>
      public IObservable<EmulationResponse> ResponseStream => _clientContext.Response.Select(x => new EmulationResponse(x));

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="dotNetifyHub">DotNetify hub instance for this client.</param>
      /// <param name="clientContext">Client context, which includes signalR connection ID.</param>
      /// <param name="dispose">Delegate to discard the client context.</param>
      internal ClientEmulator(IDotNetifyHubEmulatorProxy dotNetifyHub, ClientContext clientContext, Action dispose)
      {
         _dotNetifyHub = dotNetifyHub;
         _clientContext = clientContext;
         _dispose = dispose;

         _unsubscribeResponse = _clientContext.Response.Subscribe(args => HandleResponse(new EmulationResponse(args)));
      }

      /// <summary>
      /// Disposes resources.
      /// </summary>
      public void Dispose()
      {
         _dispose();
         _vmId = null;
         _unsubscribeResponse.Dispose();
      }

      /// <summary>
      /// Emulates client connection to a server view model.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="options">Connection options.</param>
      /// <returns>Server responses.</returns>
      public EmulationResponses Connect(string vmId, VMConnectOptions options)
      {
         return Connect(vmId, (JObject) options);
      }

      /// <summary>
      /// Emulates client connection to a server view model.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="connectOptions">Connection options.</param>
      /// <returns>Server responses.</returns>
      public EmulationResponses Connect(string vmId, JObject connectOptions = null)
      {
         if (!string.IsNullOrEmpty(_vmId))
            throw new InvalidOperationException($"The client is already connected to '{_vmId}'.");

         _vmId = vmId;

         var options = connectOptions?.ToObject<Dictionary<string, object>>();
         var result = new ClientSession(() => _dotNetifyHub.Request_VM(vmId, options), _clientContext.Response)
            .Run(MaxResponses, ResponseTimeout);

         if (result.Exception != null)
            throw result.Exception;
         return result;
      }

      /// <summary>
      /// Emulates client dispatching data to the connected view model.
      /// </summary>
      /// <param name="vmData">Data to dispatch.</param>
      /// <returns>Server responses.</returns>
      public EmulationResponses Dispatch(object vmData)
      {
         if (string.IsNullOrEmpty(_vmId))
            throw new InvalidOperationException("The client hasn't been connected to any view model.");

         var result = new ClientSession(() => _dotNetifyHub.Update_VM(_vmId, vmData), _clientContext.Response)
            .Run(MaxResponses, ResponseTimeout);

         if (result.Exception != null)
            throw result.Exception;
         return result;
      }

      /// <summary>
      /// Emulates client destroys the connection to the view model.
      /// </summary>
      /// <returns>Server responses.</returns>
      public EmulationResponses Destroy()
      {
         if (string.IsNullOrEmpty(_vmId))
            throw new InvalidOperationException("The client hasn't been connected to any view model.");

         var result = new ClientSession(() => _dotNetifyHub.Dispose_VM(_vmId), _clientContext.Response)
            .Run(MaxResponses, ResponseTimeout);

         _vmId = null;
         if (result.Exception != null)
            throw result.Exception;
         return result;
      }

      public T GetState<T>() => _viewState != null ? _viewState.As<T>() : default;

      /// <summary>
      /// Listens to server responses.
      /// </summary>
      /// <param name="action">Action delegate to execute.</param>
      /// <param name="duration">Listening duration in milliseconds.</param>
      /// <returns>Server responses.</returns>
      public EmulationResponses Listen(Func<Task> action, int? duration = null)
      {
         return new ClientSession(action, _clientContext.Response)
            .Run(duration != null ? int.MaxValue : MaxResponses, duration ?? ResponseTimeout);
      }

      /// <summary>
      /// Listens to server responses.
      /// </summary>
      /// <param name="action">Action delegate to execute.</param>
      /// <param name="duration">Listening duration in milliseconds.</param>
      /// <returns>Server responses.</returns>
      public EmulationResponses Listen(Action action, int? duration = null)
      {
         return new ClientSession(() =>
         {
            action?.Invoke();
            return Task.CompletedTask;
         }, _clientContext.Response)
            .Run(duration != null ? int.MaxValue : MaxResponses, duration ?? ResponseTimeout);
      }

      /// <summary>
      /// Listens to server responses.
      /// </summary>
      /// <param name="duration">Listening duration in milliseconds.</param>
      /// <returns>Server responses.</returns>
      public EmulationResponses Listen(int? duration = null)
      {
         return new ClientSession(() => Task.CompletedTask, _clientContext.Response)
            .Run(duration != null ? int.MaxValue : MaxResponses, duration ?? ResponseTimeout);
      }

      /// <summary>
      /// Listens to server responses.
      /// </summary>
      /// <param name="duration">Listening duration in milliseconds.</param>
      /// <returns>Server responses.</returns>
      public Task<EmulationResponses> ListenAsync(int? duration = null)
      {
         return new ClientSession(() => Task.CompletedTask, _clientContext.Response)
            .RunAsync(duration != null ? int.MaxValue : MaxResponses, duration ?? ResponseTimeout);
      }

      /// <summary>
      /// Simulates hub termination.
      /// </summary>
      public void TerminateHubConnection()
      {
         (_dotNetifyHub as HubEmulator.DotNetifyHubEmulatorProxy)?.TerminateHubConnection();
      }

      /// <summary>
      /// Records and builds a state object from responses.
      /// </summary>
      /// <param name="response">Emulation response.</param>
      private void HandleResponse(EmulationResponse response)
      {
         ResponseHistory.Add(response);
         if (response.VMId != null)
         {
            if (_viewState == null)
               _viewState = new ClientViewState(response);
            else
               _viewState.Set(response);
         }
      }

      public Task SendCoreAsync(string method, object[] args, CancellationToken cancellationToken = default)
      {
         return _clientContext.ClientProxy.SendCoreAsync(method, args, cancellationToken);
      }
   }
}