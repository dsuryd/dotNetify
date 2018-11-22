using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace DotNetify.Client
{
   public abstract class VMProxy<T> : INotifyPropertyChanged, IDisposable
   {
      private readonly IDotNetifyClient _dotNetify;
      private readonly string _vmId;

      public event PropertyChangedEventHandler PropertyChanged;

      public VMProxy(IDotNetifyClient dotnetify)
      {
         _dotNetify = dotnetify;
         _vmId = typeof(T).Name;
         Task task = InitializeAsync();
      }

      public void Dispose()
      {
         _dotNetify.Dispose();
      }

      public void Changed(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

      public void Changed<TProp>(string propName, TProp value)
      {
         GetType().GetProperty(propName)?.SetValue(this, value);
         Changed(propName);
      }

      public async Task DispatchAsync(string key, object value) => await DispatchAsync(new Dictionary<string, object> { { key, value } });

      public async Task DispatchAsync(Dictionary<string, object> propertyValues) => await _dotNetify.DispatchAsync(propertyValues);

      private async Task InitializeAsync()
      {
         await _dotNetify.ConnectAsync(_vmId, this);
      }
   }
}