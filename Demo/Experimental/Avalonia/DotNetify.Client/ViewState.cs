using DotNetify;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace DotNetify.Client
{
   public interface IViewState
   {
      T Get<T>(string name);

      void Set(Dictionary<string, object> states);
   }

   public class ViewState : IViewState
   {
      private readonly INotifyPropertyChanged _view;
      private readonly IDeserializer _deserializer = new VMSerializer();

      public ViewState(INotifyPropertyChanged view)
      {
         _view = view;
      }

      public T Get<T>(string name)
      {
         var propInfo = _view.GetType().GetProperty(name);
         return (T)propInfo?.GetValue(_view);
      }

      public void Set(Dictionary<string, object> states)
      {
         foreach (string name in states.Keys)
         {
            var value = states[name]?.ToString() ?? string.Empty;
            _deserializer.Deserialize(_view, name, value);

            var eventArgs = new object[] { this, new PropertyChangedEventArgs(name) };

            MulticastDelegate propChangedEvent = null;
            Type viewType = _view.GetType();
            while (propChangedEvent == null && viewType != null)
            {
               propChangedEvent = (MulticastDelegate)viewType
                 .GetField(nameof(INotifyPropertyChanged.PropertyChanged), BindingFlags.Instance | BindingFlags.NonPublic)?
                 .GetValue(_view);
               viewType = viewType.BaseType;
            };

            if (propChangedEvent != null)
               foreach (var d in propChangedEvent.GetInvocationList())
                  d.DynamicInvoke(eventArgs);
         }
      }
   }
}