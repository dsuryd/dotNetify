using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetify.Client;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

using client = DotNetify.Client;

namespace DotNetify.Testing
{
   /// <summary>
   /// Class that serves as a proxy of the dotNetify server hub.
   /// </summary>
   public class LiveHub : IDotNetifyHubEmulatorProxy
   {
      private string _hubPath;
      private string _serverUrl;
      private HubConnection _connection;
      private client.HubConnectionState _connectionState;
      private List<IDisposable> _subs = new List<IDisposable>();

      public DotNetifyHub Hub { get; }

      /// <summary>
      /// DotNetify hub path.
      /// </summary>
      public string HubPath { get; set; } = "/dotnetify";

      /// <summary>
      /// DotNetify hub server URL.
      /// </summary>
      public string ServerUrl { get; set; } = "http://localhost:5000";

      /// <summary>
      /// Connection state.
      /// </summary>
      public client.HubConnectionState ConnectionState { get; }

      /// <summary>
      /// Connecting client.
      /// </summary>
      internal ClientContext ClientContext { get; }

      /// <summary>
      /// Occurs when the connection is disconnected.
      /// </summary>
      public event EventHandler Disconnected;

      /// <summary>
      /// Occurs on incoming Response_VM message from the server.
      /// </summary>
      public event EventHandler<ResponseVMEventArgs> Response_VM;

      /// <summary>
      /// Occurs on incoming Response_VM message from the server.
      /// </summary>
      public event EventHandler<object[]> Response_VM_Raw;

      /// <summary>
      /// Occurs the when the connection state changed.
      /// </summary>
      public event EventHandler<client.HubConnectionState> StateChanged;

      public static async Task<IClientEmulator> CreateClientAsync(string serverUrl = null)
      {
         var hubProxy = new LiveHub(new ClientContext());
         hubProxy.Init(null, serverUrl);
         await hubProxy.StartAsync();

         return new ClientEmulator(hubProxy, hubProxy.ClientContext, async () => await hubProxy.DisposeAsync())
         {
            ResponseTimeout = 2000
         };
      }

      public void Dispose()
      {
      }

      internal LiveHub(ClientContext clientContext)
      {
         ClientContext = clientContext;
         Response_VM_Raw += SendRawResponse;
      }

      /// <summary>
      /// Disposes this proxy.
      /// </summary>
      public async Task DisposeAsync()
      {
         Response_VM_Raw -= SendRawResponse;
         _subs.ForEach(sub => sub.Dispose());
         _subs.Clear();

         if (_connection != null)
         {
            _connection.Closed -= OnConnectionClosed;
            await _connection.StopAsync();
            await _connection.DisposeAsync();
            _connection = null;
         }
      }

      /// <summary>
      /// Initializes the proxy.
      /// </summary>
      /// <param name="hubPath">Hub server relative path.</param>
      /// <param name="serverUrl">Hub server URL.</param>
      public void Init(string hubPath = null, string serverUrl = null)
      {
         if (_connection != null)
            DisposeAsync().GetAwaiter().GetResult();

         _hubPath = string.IsNullOrWhiteSpace(hubPath) ? HubPath : hubPath;
         _serverUrl = string.IsNullOrWhiteSpace(serverUrl) ? ServerUrl + _hubPath : serverUrl + _hubPath;

         var hubConnectionBuilder = new HubConnectionBuilder();
         hubConnectionBuilder.Services.AddSingleton(BuildHubProtocol());

         _connection = hubConnectionBuilder
             .WithUrl(_serverUrl)
             .Build();
         _connection.Closed += OnConnectionClosed;

         _subs.Add(_connection.On<object[]>(nameof(IDotNetifyHubMethod.Response_VM), OnResponse_VM));
      }

      /// <summary>
      /// Starts the connection with the server.
      /// </summary>
      public async Task StartAsync()
      {
         if (_connection == null)
            Init();

         if (_connectionState == client.HubConnectionState.Connected)
            return;

         SetStateChanged(client.HubConnectionState.Connecting);

         await _connection.StartAsync();
         SetStateChanged(client.HubConnectionState.Connected);
      }

      /// <summary>
      /// Sends a Request_VM message to the server.
      /// </summary>
      /// <param name="vmId">Identifies the view model being requested.</param>
      /// <param name="options">DotNetify connection options.</param>
      public async Task Request_VM(string vmId, Dictionary<string, object> options)
      {
         if (options == null)
            await _connection?.SendCoreAsync(nameof(IDotNetifyHubMethod.Request_VM), new object[] { vmId, new JObject() });
         else
            await _connection?.SendCoreAsync(nameof(IDotNetifyHubMethod.Request_VM), new object[] { vmId, options });
      }

      public async Task Request_VM(string vmId, object options) => await _connection?.SendCoreAsync(nameof(IDotNetifyHubMethod.Request_VM), new object[] { vmId, options });

      /// <summary>
      /// Sends an Update_VM message to the server.
      /// </summary>
      /// <param name="vmId">Identifies the view model to send the update to.</param>
      /// <param name="propertyValues">Dictionary of property names and updated values.</param>
      public async Task Update_VM(string vmId, Dictionary<string, object> propertyValues) => await Update_VM(vmId, (object) propertyValues);

      public async Task Update_VM(string vmId, object propertyValues) => await _connection?.SendCoreAsync(nameof(IDotNetifyHubMethod.Update_VM), new object[] { vmId, propertyValues });

      /// <summary>
      /// Sends a Dispose_VM message to the server.
      /// </summary>
      /// <param name="vmId">Identifies the view model to dispose.</param>
      public async Task Dispose_VM(string vmId) => await _connection?.SendCoreAsync(nameof(IDotNetifyHubMethod.Dispose_VM), new object[] { vmId });

      /// <summary>
      /// Builds SignalR hub protocol.
      /// </summary>
      /// <returns>Hub protocol.</returns>
      protected virtual IHubProtocol BuildHubProtocol()
      {
         // Override JSON serializer to retain original case (don't force to camel case).
         return new JsonHubProtocol(Options.Create(
            new JsonHubProtocolOptions
            {
               PayloadSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = null }
            }));
      }

      /// <summary>
      /// Builds the event arguments from the incoming Response_VM payload.
      /// </summary>
      /// <param name="payload"></param>
      /// <returns></returns>
      internal static ResponseVMEventArgs BuildResponseVMEventArgs(object[] payload)
      {
         object[] payloadArray = payload;

         // Payload with 3 arguments is the response to the Invoke message.
         if (payloadArray.Length == 3)
         {
            string[] methodArgs = null;
            if (payloadArray[1] is JsonElement || payloadArray[1] is JObject)
               methodArgs = JsonSerializer.Deserialize<string[]>(payloadArray[1].ToString());
            else if (payloadArray[1] is object[])
               methodArgs = (payloadArray[1] as object[]).Select(x => (string) x).ToArray();

            IDictionary<string, string> metadata = null;
            if (payloadArray[2] is JsonElement || payloadArray[2] is JObject)
               metadata = JsonSerializer.Deserialize<IDictionary<string, string>>(payloadArray[2].ToString());
            else if (payloadArray[2] is Dictionary<object, object>)  // MessagePack
               metadata = (payloadArray[2] as Dictionary<object, object>).ToDictionary(x => (string) x.Key, x => (string) x.Value);

            return new InvokeResponseEventArgs
            {
               MethodName = payloadArray[0].ToString(),
               MethodArgs = methodArgs,
               Metadata = metadata
            };
         }
         else
         {
            var vmId = payloadArray[0].ToString();

            Dictionary<string, object> data = null;
            if (payloadArray[1] is JsonElement || payloadArray[1] is JObject)
               data = JsonSerializer.Deserialize<Dictionary<string, object>>(payloadArray[1].ToString());
            else if (payloadArray[1] is Dictionary<object, object>) // MessagePack
               data = (payloadArray[1] as Dictionary<object, object>).ToDictionary(x => (string) x.Key, x => x.Value);

            return new ResponseVMEventArgs
            {
               VMId = vmId,
               Data = data
            };
         }
      }

      /// <summary>
      /// Handles connection being closed.
      /// </summary>
      private Task OnConnectionClosed(Exception arg)
      {
         SetStateChanged(client.HubConnectionState.Disconnected);
         return Task.CompletedTask;
      }

      /// <summary>
      /// Handles incoming Response_VM message.
      /// </summary>
      private void OnResponse_VM(object[] payload)
      {
         var eventArgs = BuildResponseVMEventArgs(payload);
         var args = new object[] { this, eventArgs };

         Response_VM_Raw?.Invoke(this, new object[] { eventArgs.VMId, JsonSerializer.Serialize(eventArgs.Data) });

         if (Response_VM == null)
            return;

         foreach (Delegate d in Response_VM.GetInvocationList())
         {
            d.DynamicInvoke(args);
            if (eventArgs.Handled)
               break;
         }

         // If we get to this point, that means the server holds a view model instance
         // whose view no longer existed.  So, tell the server to dispose the view model.
         if (!eventArgs.Handled)
         {
            _ = Dispose_VM(eventArgs.VMId);
         }
      }

      /// <summary>
      /// Sends response to the client.
      /// </summary>
      private void SendRawResponse(object sender, object[] e)
      {
         ClientContext.ClientProxy.SendCoreAsync(nameof(IDotNetifyHubMethod.Response_VM), new object[] { e });
      }

      /// <summary>
      /// Changes the state.
      /// </summary>
      /// <param name="state">State to change.</param>
      private void SetStateChanged(client.HubConnectionState state)
      {
         _connectionState = state;
         StateChanged?.Invoke(this, state);
         if (state == client.HubConnectionState.Disconnected)
            Disconnected?.Invoke(this, null);
      }
   }
}