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
using System.ComponentModel;
using System.Threading.Tasks;

namespace DotNetify.Client
{
   /// <summary>
   /// Defines a dotNetify client.
   /// </summary>
   public interface IDotNetifyClient : IDisposable
   {
      Task ConnectAsync(string vmId, INotifyPropertyChanged view, RequestVMOptions options = null);

      Task ConnectAsync(string vmId, IViewState viewState, RequestVMOptions options = null);

      Task DisposeAsync();

      Task DispatchAsync(Dictionary<string, object> propertyValues);
   }

   /// <summary>
   /// Provides connection to a dotNetify hub server.
   /// </summary>
   public class DotNetifyClient : IDotNetifyClient
   {
      private readonly IDotNetifyHubProxy _hubProxy;
      private string _vmId;
      private IViewState _viewState;

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="hubProxy">DotNetify hub server proxy.</param>
      public DotNetifyClient(IDotNetifyHubProxy hubProxy)
      {
         _hubProxy = hubProxy;
      }

      public void Dispose()
      {
         Task.Run(() => DisposeAsync());
      }

      /// <summary>
      /// Disposes this instance.
      /// </summary>
      public async Task DisposeAsync()
      {
         if (!string.IsNullOrEmpty(_vmId))
         {
            _hubProxy.Response_VM -= OnResponseReceived;
            await _hubProxy.Dispose_VM(_vmId);
            _vmId = null;
         }
      }

      /// <summary>
      /// Connects to the dotNetify hub server.
      /// </summary>
      /// <param name="vmId">Identifies the view model to request.</param>
      /// <param name="view">The connecting view; must implement changed notification.</param>
      /// <param name="options">View model initialization options.</param>
      public async Task ConnectAsync(string vmId, INotifyPropertyChanged view, RequestVMOptions options = null)
      {
         await ConnectAsync(vmId, new ViewState(view), options);
      }

      /// <summary>
      /// Connects to the dotNetify hub server.
      /// </summary>
      /// <param name="vmId">Identifies the view model to request.</param>
      /// <param name="viewState">View state manager.</param>
      /// <param name="options">View model initialization options.</param>
      public async Task ConnectAsync(string vmId, IViewState viewState, RequestVMOptions options = null)
      {
         if (!string.IsNullOrEmpty(_vmId))
            throw new ApplicationException($"The instance was connected to '{_vmId}'. Call Dispose to disconnect.");

         _vmId = vmId;
         _viewState = viewState;
         await _hubProxy.StartAsync();
         _hubProxy.Response_VM += OnResponseReceived;
         await _hubProxy.Request_VM(vmId, options);
      }

      /// <summary>
      /// Dispatches view model update to the dotNetify hub server.
      /// </summary>
      /// <param name="propertyValues">Dictionary of property names and updated values.</param>
      public async Task DispatchAsync(Dictionary<string, object> propertyValues)
      {
         await _hubProxy.Update_VM(_vmId, propertyValues);
      }

      private void OnResponseReceived(object sender, ResponseVMEventArgs e)
      {
         if (e.VMId == _vmId)
         {
            e.Handled = true;
            _viewState.Set(e.Data);
         }
      }
   }
}