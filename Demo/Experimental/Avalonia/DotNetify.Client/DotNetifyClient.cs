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
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
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
      private readonly IUIThreadDispatcher _dispatcher;
      private string _vmId;
      private Dictionary<string, string> _itemKeys;
      private IViewState _viewState;

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="hubProxy">DotNetify hub server proxy.</param>
      /// <param name="dispatcher">UI thread dispatcher.</param>
      public DotNetifyClient(IDotNetifyHubProxy hubProxy, IUIThreadDispatcher dispatcher)
      {
         _hubProxy = hubProxy;
         _dispatcher = dispatcher;
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
         await ConnectAsync(vmId, new ViewState(view, _dispatcher), options);
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

      /// <summary>
      /// Preprocess view model update from the server before we set the state.
      /// </summary>
      /// <param name="data">Dictionary of property names and values.</param>
      /// <returns>Preprocessed data.</returns>
      protected virtual Dictionary<string, object> Preprocess(Dictionary<string, object> data)
      {
         foreach (var kvp in data.ToList())
         {
            string prop = kvp.Key;
            Match match;

            // Look for property that end with '_add'. Interpret the value as a list item to be added
            // to an existing list whose property name precedes that suffix.
            match = Regex.Match(prop, @"(.*)_add");
            if (match.Success && kvp.Value != null)
            {
               var listName = match.Groups[1].Value;
               if (_viewState.HasProperty(listName))
               {
                  string itemKey = null;
                  if (_itemKeys != null)
                     _itemKeys.TryGetValue(listName, out itemKey);

                  try
                  {
                     _viewState.AddList(listName, kvp.Value, itemKey);
                  }
                  catch (Exception ex)
                  {
                     Trace.TraceWarning($"[{_vmId}] {ex.Message} {ex.InnerException?.Message}");
                  }
               }
               else
                  Trace.TraceWarning($"[{_vmId}] Unable to resolve `${prop}`");
               data.Remove(prop);
               continue;
            }

            // Look for property that end with '_itemKey'. Interpret the value as the property name that will
            // uniquely identify items in the list.
            match = Regex.Match(prop, @"(.*)_itemKey");
            if (match.Success && kvp.Value != null)
            {
               var listName = match.Groups[1].Value;
               SetItemKey(listName, kvp.Value.ToString());
               data.Remove(prop);
               continue;
            }
         }
         return data;
      }

      /// <summary>
      /// Handles response received from the server.
      /// </summary>
      private void OnResponseReceived(object sender, ResponseVMEventArgs e)
      {
         if (e.VMId == _vmId)
         {
            e.Handled = true;
            _viewState.Set(Preprocess(e.Data));
         }
      }

      /// <summary>
      /// Sets items key to identify individual items in a list.
      /// </summary>
      /// <param name="listName">Property name of a list.</param>
      /// <param name="itemKey">Item property name that identify items in the list.</param>
      private void SetItemKey(string listName, string itemKey)
      {
         if (_itemKeys == null)
            _itemKeys = new Dictionary<string, string>();
         _itemKeys.Add(listName, itemKey);
      }
   }
}