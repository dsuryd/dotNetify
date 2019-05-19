/*
Copyright 2019 Dicky Suryadi

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

namespace DotNetify.Client.Blazor
{
   /// <summary>
   /// Defines a dotNetify client.
   /// </summary>
   public interface IDotNetifyClient : IDisposable
   {
      /// <summary>
      /// Connects to the dotNetify hub server.
      /// </summary>
      /// <param name="vmId">Identifies the view model to request.</param>
      /// <param name="view">The connecting view; must implement changed notification.</param>
      /// <param name="options">View model initialization options.</param>
      Task ConnectAsync(string vmId, object view, VMConnectOptions options = null);

      /// <summary>
      /// Connects to the dotNetify hub server.
      /// </summary>
      /// <param name="vmId">Identifies the view model to request.</param>
      /// <param name="viewState">View state manager.</param>
      /// <param name="options">View model initialization options.</param>
      Task ConnectAsync(string vmId, IViewState viewState, VMConnectOptions options = null);

      /// <summary>
      /// Dispatches a property value to the dotNetify hub server.
      /// </summary>
      /// <param name="propertyName">Property name.</param>
      /// <param name="value">Property value.</param>
      Task DispatchAsync(string propertyName, object value);

      /// <summary>
      /// Dispatches view model update to the dotNetify hub server.
      /// </summary>
      /// <param name="propertyValues">Dictionary of property names and updated values.</param>
      Task DispatchAsync(Dictionary<string, object> propertyValues);

      /// <summary>
      /// Disposes the instance.
      /// </summary>
      Task DisposeAsync();
   }

   /// <summary>
   /// Provides connection to a dotNetify hub server.
   /// </summary>
   public class DotNetifyClient : IDotNetifyClient
   {
      public const string TOKEN_VMARG = "$vmArg";
      public const string TOKEN_HEADERS = "$headers";

      private readonly IDotNetifyHubProxy _hubProxy;
      private readonly IUIThreadDispatcher _dispatcher;
      private string _vmId;
      private Dictionary<string, string> _itemKeys = new Dictionary<string, string>();
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

      /// <summary>
      /// Disposes this instance synchronously.
      /// </summary>
      public void Dispose()
      {
         DisposeAsync().GetAwaiter().GetResult();
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
      public async Task ConnectAsync(string vmId, object view, VMConnectOptions options = null)
      {
         await ConnectAsync(vmId, new ViewState(view, _dispatcher), options);
      }

      /// <summary>
      /// Connects to the dotNetify hub server.
      /// </summary>
      /// <param name="vmId">Identifies the view model to request.</param>
      /// <param name="viewState">View state manager.</param>
      /// <param name="options">View model initialization options.</param>
      public async Task ConnectAsync(string vmId, IViewState viewState, VMConnectOptions options = null)
      {
         if (!string.IsNullOrEmpty(_vmId))
            throw new Exception($"The instance was connected to '{_vmId}'. Call Dispose to disconnect.");

         _vmId = vmId;
         _viewState = viewState;

         await _hubProxy.StartAsync();
         _hubProxy.Response_VM += OnResponseReceived;

         Dictionary<string, object> data = null;
         if (options?.VMArg != null)
         {
            data = new Dictionary<string, object>();
            data.Add(TOKEN_VMARG, options.VMArg);
            if (options?.Headers != null)
               data.Add(TOKEN_HEADERS, options.Headers);
         }

         await _hubProxy.Request_VM(vmId, data);
      }

      /// <summary>
      /// Dispatches a property value to the dotNetify hub server.
      /// </summary>
      /// <param name="propertyName">Property name.</param>
      /// <param name="value">Property value.</param>
      public async Task DispatchAsync(string propertyName, object value = null) => await DispatchAsync(new Dictionary<string, object> { { propertyName, value } });

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
            string propName = kvp.Key;
            Match match;

            try
            {
               // Look for property that end with '_add'. Interpret the value as a list item to be added
               // to an existing list whose property name precedes that suffix.
               match = Regex.Match(propName, @"(.*)_add");
               if (match.Success && kvp.Value != null)
               {
                  var listName = match.Groups[1].Value;
                  if (_viewState.HasProperty(listName))
                  {
                     _itemKeys.TryGetValue(listName, out string itemKey);
                     _viewState.AddList(listName, kvp.Value, itemKey);
                  }
                  else
                     throw new Exception($"Unable to resolve `${propName}`");

                  data.Remove(propName);
                  continue;
               }

               // Look for property that end with '_update'. Interpret the value as a list item to be updated
               // to an existing list whose property name precedes that suffix.
               match = Regex.Match(propName, @"(.*)_update");
               if (match.Success && kvp.Value != null)
               {
                  var listName = match.Groups[1].Value;
                  if (_viewState.HasProperty(listName))
                  {
                     _itemKeys.TryGetValue(listName, out string itemKey);
                     _viewState.UpdateList(listName, kvp.Value, itemKey);
                  }
                  else
                     throw new Exception($"Unable to resolve `${propName}`");
                  data.Remove(propName);
                  continue;
               }

               // Look for property that end with '_remove'. Interpret the value as a list item key to remove
               // from an existing list whose property name precedes that suffix.
               match = Regex.Match(propName, @"(.*)_remove");
               if (match.Success && kvp.Value != null)
               {
                  var listName = match.Groups[1].Value;
                  if (_viewState.HasProperty(listName))
                  {
                     _itemKeys.TryGetValue(listName, out string itemKey);
                     _viewState.RemoveList(listName, kvp.Value, itemKey);
                  }
                  else
                     throw new Exception($"Unable to resolve `${propName}`");
                  data.Remove(propName);
                  continue;
               }

               // Look for property that end with '_itemKey'. Interpret the value as the property name that will
               // uniquely identify items in the list.
               match = Regex.Match(propName, @"(.*)_itemKey");
               if (match.Success && kvp.Value != null)
               {
                  var listName = match.Groups[1].Value;
                  SetItemKey(listName, kvp.Value.ToString());

                  data.Remove(propName);
                  continue;
               }
            }
            catch (Exception ex)
            {
               Trace.TraceWarning($"[{_vmId}] {ex.Message} {ex.InnerException?.Message}");
               data.Remove(propName);
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
         if (!_itemKeys.ContainsKey(listName))
            _itemKeys.Add(listName, itemKey);
         else
            throw new Exception($"[{_vmId}] Item key for {listName} was already set.");
      }
   }
}