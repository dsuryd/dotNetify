using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace DotNetify.LoadTester
{
   public interface IDotNetifyClient : IDisposable
   {
      ISubject<object[]> ResponseStream { get; }

      Task ConnectAsync(string vmId, VMConnectOptions connectOptions = null);

      Task DispatchAsync(object vmData);

      Task DestroyAsync();
   }

   public class DotNetifyHubClient : IDotNetifyClient
   {
      private const string DOTNETIFY_HUB_PATH = "/dotnetify";

      private string _serverUrl;
      private HubConnection _connection;
      private HubConnectionState _connectionState;
      private IDisposable _responseSubs;
      private string _vmId;

      public event EventHandler Disconnected;

      public event EventHandler<HubConnectionState> StateChanged;

      public ISubject<object[]> ResponseStream { get; } = new Subject<object[]>();

      public static async Task<IDotNetifyClient> CreateClientAsync(string serverUrl)
      {
         var hubClient = new DotNetifyHubClient();
         hubClient.Init(serverUrl);
         await hubClient.StartAsync();

         return hubClient;
      }

      public void Init(string serverUrl)
      {
         if (_connection != null)
            DisposeAsync().GetAwaiter().GetResult();

         _serverUrl = serverUrl;
         var hubConnectionBuilder = new HubConnectionBuilder();
         hubConnectionBuilder.Services.AddSingleton(BuildHubProtocol());

         _connection = hubConnectionBuilder
             .WithUrl(serverUrl + DOTNETIFY_HUB_PATH)
             .AddNewtonsoftJsonProtocol(configure => configure.PayloadSerializerSettings = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() })
             .Build();
         _connection.Closed += OnConnectionClosed;

         _responseSubs = _connection.On<object>("Response_VM", OnResponse_VM);
      }

      public async Task StartAsync()
      {
         if (_connection == null)
            Init(_serverUrl);

         if (_connectionState == HubConnectionState.Connected)
            return;

         SetStateChanged(HubConnectionState.Connecting);

         await _connection.StartAsync();
         SetStateChanged(HubConnectionState.Connected);
      }

      public async Task DisposeAsync()
      {
         if (_connection != null)
         {
            _responseSubs.Dispose();
            await _connection.StopAsync();
            await _connection.DisposeAsync();
            _connection = null;
         }
      }

      public async Task Request_VM(string vmId, Dictionary<string, object> options) => await _connection?.SendCoreAsync("Request_VM", new object[] { vmId, options });

      public async Task Update_VM(string vmId, object propertyValues) => await _connection?.SendCoreAsync("Update_VM", new object[] { vmId, propertyValues });

      public async Task Dispose_VM(string vmId) => await _connection?.SendCoreAsync("Dispose_VM", new object[] { vmId });

      #region IDotNetifyClient

      public Task ConnectAsync(string vmId, VMConnectOptions connectOptions = null)
      {
         if (!string.IsNullOrEmpty(_vmId))
            throw new InvalidOperationException($"The client is already connected to '{_vmId}'.");

         _vmId = vmId;

         var options = ((JObject) connectOptions)?.ToObject<Dictionary<string, object>>();
         return Request_VM(vmId, options);
      }

      public Task DispatchAsync(object vmData)
      {
         if (string.IsNullOrEmpty(_vmId))
            throw new InvalidOperationException("The client hasn't been connected to any view model.");

         return Update_VM(_vmId, vmData);
      }

      public async Task DestroyAsync()
      {
         if (string.IsNullOrEmpty(_vmId))
            throw new InvalidOperationException("The client hasn't been connected to any view model.");

         await Dispose_VM(_vmId);

         _vmId = null;
      }

      public void Dispose()
      {
         _vmId = null;
         DisposeAsync().GetAwaiter().GetResult();
      }

      #endregion IDotNetifyClient

      private IHubProtocol BuildHubProtocol()
      {
         // Override JSON serializer to retain original case (don't force to camel case).
         return new JsonHubProtocol(Options.Create(
            new JsonHubProtocolOptions
            {
               PayloadSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = null }
            }));
      }

      private Task OnConnectionClosed(Exception arg)
      {
         SetStateChanged(HubConnectionState.Disconnected);
         return Task.CompletedTask;
      }

      private void OnResponse_VM(object payload)
      {
         if (payload is JArray)
            ResponseStream.OnNext((payload as JArray).ToObject<object[]>());
      }

      private void SetStateChanged(HubConnectionState state)
      {
         _connectionState = state;
         StateChanged?.Invoke(this, state);
         if (state == HubConnectionState.Disconnected)
            Disconnected?.Invoke(this, null);
      }
   }
}