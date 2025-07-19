using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DotNetify.Testing
{
   /// <summary>
   /// Emulates a dotNetify client.
   /// </summary>
   public interface IClientEmulator : IDisposable
   {
      // DotNetify hub.
      DotNetifyHub Hub { get; }

      /// <summary>
      /// Emulated connection ID.
      /// </summary>
      string ConnectionId { get; }

      /// <summary>
      /// Maximum number of responses to record.
      /// </summary>
      int MaxResponses { get; set; }

      /// <summary>
      /// Timeout in milliseconds waiting for responses.
      /// </summary>
      int ResponseTimeout { get; set; }

      /// <summary>
      /// Accumulated responses from the server.
      /// </summary>
      EmulationResponses ResponseHistory { get; }

      /// <summary>
      /// Stream of every server response received by the client.
      /// </summary>
      IObservable<EmulationResponse> ResponseStream { get; }

      /// <summary>
      /// Emulates client connection to a server view model.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="options">Connection options.</param>
      /// <returns>Server responses.</returns>
      EmulationResponses Connect(string vmId, VMConnectOptions options);

      /// <summary>
      /// Emulates client connection to a server view model.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="options">Connection options.</param>
      /// <returns>Server responses.</returns>
      EmulationResponses Connect(string vmId, JObject options = null);

      /// <summary>
      /// Emulates client dispatching data to the connected view model.
      /// </summary>
      /// <param name="vmData">Data to dispatch.</param>
      /// <returns>Server responses.</returns>
      EmulationResponses Dispatch(object vmData);

      /// <summary>
      /// Emulates client destroys the connection to the view model.
      /// </summary>
      /// <returns>Server responses.</returns>
      EmulationResponses Destroy();

      /// <summary>
      /// Gets latest client state.
      /// </summary>
      T GetState<T>();

      /// <summary>
      /// Listens to server responses.
      /// </summary>
      /// <param name="action">Action delegate to execute.</param>
      /// <param name="duration">Listening duration in milliseconds.</param>
      /// <returns>Server responses.</returns>
      EmulationResponses Listen(Func<Task> action, int? duration = null);

      /// <summary>
      /// Listens to server responses.
      /// </summary>
      /// <param name="action">Action delegate to execute.</param>
      /// <param name="duration">Listening duration in milliseconds.</param>
      /// <returns>Server responses.</returns>
      EmulationResponses Listen(Action action, int? duration = null);

      /// <summary>
      /// Listens to server responses.
      /// </summary>
      /// <param name="duration">Listening duration in milliseconds.</param>
      /// <returns>Server responses.</returns>
      EmulationResponses Listen(int? duration = null);

      /// <summary>
      /// Listens to server responses.
      /// </summary>
      /// <param name="duration">Listening duration in milliseconds.</param>
      /// <returns>Server responses.</returns>
      Task<EmulationResponses> ListenAsync(int? duration = null);

      /// <summary>
      /// Emulates terminating connection.
      /// </summary>
      void TerminateHubConnection();
   }
}