/*
Copyright 2018-2020 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetify.Client
{
   /// <summary>
   /// Class that serves as a proxy of the dotNetify server hub.
   /// </summary>
   public class DotNetifyHubProxy : IDotNetifyHubProxy, IDisposable
   {
      private string _hubPath;
      private string _serverUrl;
      private HubConnection _connection;
      private HubConnectionState _connectionState;
      private List<IDisposable> _subs = new List<IDisposable>();

      /// <summary>
      /// DotNetify hub path.
      /// </summary>
      public static string HubPath { get; set; } = "/dotnetify";

      /// <summary>
      /// DotNetify hub server URL.
      /// </summary>
      public static string ServerUrl { get; set; } = "http://localhost:5000";

      /// <summary>
      /// Occurs when the connection is disconnected.
      /// </summary>
      public event EventHandler Disconnected;

      /// <summary>
      /// Occurs on incoming Response_VM message from the server.
      /// </summary>
      public event EventHandler<ResponseVMEventArgs> Response_VM;

      /// <summary>
      /// Occurs the when the connection state changed.
      /// </summary>
      public event EventHandler<HubConnectionState> StateChanged;

      /// <summary>
      /// Disposes this proxy.
      /// </summary>
      public void Dispose()
      {
         _subs.ForEach(sub => sub.Dispose());
         _subs.Clear();

         _connection?.DisposeAsync();
         _connection.Closed -= OnConnectionClosed;
         _connection = null;
      }

      /// <summary>
      /// Initializes the proxy.
      /// </summary>
      /// <param name="hubPath">Hub server relative path.</param>
      /// <param name="serverUrl">Hub server URL.</param>
      public void Init(string hubPath = null, string serverUrl = null)
      {
         if (_connection != null)
            Dispose();

         _hubPath = string.IsNullOrWhiteSpace(hubPath) ? HubPath : hubPath;
         _serverUrl = string.IsNullOrWhiteSpace(serverUrl) ? ServerUrl + _hubPath : serverUrl + _hubPath;

         var hubConnectionBuilder = new HubConnectionBuilder();
         hubConnectionBuilder.Services.AddSingleton(BuildHubProtocol());

         _connection = hubConnectionBuilder
             .WithUrl(_serverUrl)
             .Build();
         _connection.Closed += OnConnectionClosed;

         _subs.Add(_connection.On<object>(nameof(IDotNetifyHubMethod.Response_VM), OnResponse_VM));
      }

      /// <summary>
      /// Starts the connection with the server.
      /// </summary>
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

      /// <summary>
      /// Sends a Request_VM message to the server.
      /// </summary>
      /// <param name="vmId">Identifies the view model being requested.</param>
      /// <param name="options">DotNetify connection options.</param>
      public async Task Request_VM(string vmId, Dictionary<string, object> options) => await _connection?.SendCoreAsync(nameof(IDotNetifyHubMethod.Request_VM), new object[] { vmId, options });

      /// <summary>
      /// Sends an Update_VM message to the server.
      /// </summary>
      /// <param name="vmId">Identifies the view model to send the update to.</param>
      /// <param name="propertyValues">Dictionary of property names and updated values.</param>
      public async Task Update_VM(string vmId, Dictionary<string, object> propertyValues) => await _connection?.SendCoreAsync(nameof(IDotNetifyHubMethod.Update_VM), new object[] { vmId, propertyValues });

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
               PayloadSerializerSettings = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() }
            }));
      }

      /// <summary>
      /// Handles connection being closed.
      /// </summary>
      private Task OnConnectionClosed(Exception arg)
      {
         SetStateChanged(HubConnectionState.Disconnected);
         return Task.CompletedTask;
      }

      /// <summary>
      /// Handles incoming Response_VM message.
      /// </summary>
      private void OnResponse_VM(object payload)
      {
         if (payload is JArray == false)
            return;

         // SignalR .NET Core is sending an array of arguments.
         var vmId = $"{(payload as JArray)[0]}";
         var rawData = (payload as JArray)[1].ToString();
         var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(rawData);

         var eventArgs = new ResponseVMEventArgs { VMId = vmId, Data = data };
         var args = new object[] { this, eventArgs };

         foreach (Delegate d in Response_VM?.GetInvocationList())
         {
            d.DynamicInvoke(args);
            if (eventArgs.Handled)
               break;
         }

         // If we get to this point, that means the server holds a view model instance
         // whose view no longer existed.  So, tell the server to dispose the view model.
         if (!eventArgs.Handled)
         {
            _ = Dispose_VM(vmId);
         }
      }

      /// <summary>
      /// Changes the state.
      /// </summary>
      /// <param name="state">State to change.</param>
      private void SetStateChanged(HubConnectionState state)
      {
         _connectionState = state;
         StateChanged?.Invoke(this, state);
         if (state == HubConnectionState.Disconnected)
            Disconnected?.Invoke(this, null);
      }
   }
}