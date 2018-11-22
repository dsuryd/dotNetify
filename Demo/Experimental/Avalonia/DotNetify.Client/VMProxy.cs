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
   /// Proxy to the server-side view model.
   /// </summary>
   /// <typeparam name="T">Server-side view model type.</typeparam>
   public abstract class VMProxy<T> : INotifyPropertyChanged, IDisposable
   {
      private readonly IDotNetifyClient _dotNetify;
      private readonly string _vmId;

      /// <summary>
      /// Occurs when the property value changed.
      /// </summary>
      public event PropertyChangedEventHandler PropertyChanged;

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="dotnetify">DotNetify client instance.</param>
      public VMProxy(IDotNetifyClient dotnetify)
      {
         _dotNetify = dotnetify;
         _vmId = typeof(T).Name;
         Task task = InitializeAsync();
      }

      /// <summary>
      /// Disposes the instance.
      /// </summary>
      public void Dispose() => _dotNetify.Dispose();

      /// <summary>
      /// Raises the PropertyChanged event.
      /// </summary>
      /// <param name="propName">Property name.</param>
      public void Changed(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

      /// <summary>
      /// Dispatches a property value to the server.
      /// </summary>
      /// <param name="propertyName">Property name.</param>
      /// <param name="value">Property value.</param>
      public async Task DispatchAsync(string propertyName, object value) => await DispatchAsync(new Dictionary<string, object> { { propertyName, value } });

      /// <summary>
      /// Dispatches a set of property values to the server.
      /// </summary>
      /// <param name="propertyValues">Dictionary of property names and values.</param>
      public async Task DispatchAsync(Dictionary<string, object> propertyValues) => await _dotNetify.DispatchAsync(propertyValues);

      /// <summary>
      /// Initializes the proxy.
      /// </summary>
      private async Task InitializeAsync() => await _dotNetify.ConnectAsync(_vmId, this);
   }
}