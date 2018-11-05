using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace HelloWorld
{
   public enum HubConnectionState
   {
      Connecting = 0,
      Connected = 1,
      Reconnecting = 2,
      Disconnected = 4,
      Terminated = 99
   }

   public interface IDotNetifyHubProxy
   {
      void Init(string hubPath, string serverUrl);

      Task StartAsync();

      void Request_VM(string vmId, RequestVMOptions options);

      void Dispose_VM(string vmId);
   }

   public class DotNetifyHubProxy : IDotNetifyHubProxy, IDisposable
   {
      private static readonly string HUB_PATH = "/dotnetify";
      private static readonly string DEFAULT_URL = "http://localhost:5000";

      private string _hubPath;
      private string _serverUrl;
      private HubConnection _connection;
      private HubConnectionState _connectionState;
      private List<IDisposable> _subs = new List<IDisposable>();

      public event EventHandler Disconnected;

      public event EventHandler<HubConnectionState> StateChanged;

      public void Dispose()
      {
         _subs.ForEach(sub => sub.Dispose());
         _subs.Clear();

         _connection?.DisposeAsync();
         _connection.Closed -= OnConnectionClosed;
         _connection = null;
      }

      public void Init(string hubPath = null, string serverUrl = null)
      {
         if (_connection != null)
            Dispose();

         _hubPath = string.IsNullOrWhiteSpace(hubPath) ? HUB_PATH : hubPath;
         _serverUrl = string.IsNullOrWhiteSpace(serverUrl) ? DEFAULT_URL + _hubPath : serverUrl + _hubPath;

         _connection = new HubConnectionBuilder()
             .WithUrl(_serverUrl)
             .Build();

         _connection.Closed += OnConnectionClosed;

         _subs.Add(_connection.On<string>("Response_VM", OnResponse_VM));
      }

      public async Task StartAsync()
      {
         if (_connection == null)
            Init();

         if (_connectionState == HubConnectionState.Connected)
            return;

         SetStateChanged(HubConnectionState.Connecting);

         await _connection.StartAsync();
         SetStateChanged(HubConnectionState.Connected);
      }

      public void Request_VM(string vmId, RequestVMOptions options)
      {
         _connection?.SendCoreAsync("Request_VM", new object[] { vmId, options });
      }

      public void Dispose_VM(string vmId)
      {
         _connection?.SendCoreAsync("Dispose_VM", new object[] { vmId });
      }

      private Task OnConnectionClosed(Exception arg)
      {
         SetStateChanged(HubConnectionState.Disconnected);
         return Task.CompletedTask;
      }

      private void OnResponse_VM(string payload)
      {
         System.Diagnostics.Trace.WriteLine(payload);
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