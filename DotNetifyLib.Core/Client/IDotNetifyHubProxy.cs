/*
Copyright 2018 Dicky Suryadi

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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetify.Client
{
   /// <summary>
   /// Defines a proxy of the dotNetify server hub.
   /// </summary>
   public interface IDotNetifyHubProxy
   {
      /// <summary>
      /// Occurs on incoming Response_VM message from the server.
      /// </summary>
      event EventHandler<ResponseVMEventArgs> Response_VM;

      /// <summary>
      /// Occurs the when the connection state changed.
      /// </summary>
      event EventHandler<HubConnectionState> StateChanged;

      /// <summary>
      /// Occurs when the connection is disconnected.
      /// </summary>
      event EventHandler Disconnected;

      /// <summary>
      /// Initializes the proxy.
      /// </summary>
      /// <param name="hubPath">Hub server relative path.</param>
      /// <param name="serverUrl">Hub server URL.</param>
      void Init(string hubPath, string serverUrl);

      /// <summary>
      /// Starts the connection with the server.
      /// </summary>
      Task StartAsync();

      /// <summary>
      /// Sends a Request_VM message to the server.
      /// </summary>
      /// <param name="vmId">Identifies the view model being requested.</param>
      /// <param name="options">DotNetify connection options.</param>
      Task Request_VM(string vmId, Dictionary<string, object> options);

      /// <summary>
      /// Sends an Update_VM message to the server.
      /// </summary>
      /// <param name="vmId">Identifies the view model to send the update to.</param>
      /// <param name="propertyValues">Dictionary of property names and updated values.</param>
      Task Update_VM(string vmId, Dictionary<string, object> propertyValues);

      /// <summary>
      /// Sends a Dispose_VM message to the server.
      /// </summary>
      /// <param name="vmId">Identifies the view model to dispose.</param>
      Task Dispose_VM(string vmId);
   }

   /// <summary>
   /// Possible SignalR hub connection states.
   /// </summary>
   public enum HubConnectionState
   {
      Connecting = 0,
      Connected = 1,
      Reconnecting = 2,
      Disconnected = 4,
      Terminated = 99
   }

   /// <summary>
   /// Event arguments of the incoming Response_VM message.
   /// </summary>
   public class ResponseVMEventArgs : EventArgs
   {
      public string VMId { get; set; }
      public Dictionary<string, object> Data { get; set; }
      public bool Handled { get; set; }
   }
}