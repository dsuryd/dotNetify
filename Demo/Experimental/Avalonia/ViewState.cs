using DotNetify;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace HelloWorld
{
   public interface IViewState
   {
      T Get<T>(string name);

      void Set(Dictionary<string, string> states);
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

      public void Set(Dictionary<string, string> states)
      {
         foreach (string name in states.Keys)
         {
            _deserializer.Deserialize(_view, name, states[name]);

            var eventArgs = new object[] { this, new PropertyChangedEventArgs(name) };
            var propChangedEvent = (MulticastDelegate)_view
               .GetType()
               .GetField(nameof(INotifyPropertyChanged.PropertyChanged), BindingFlags.Instance | BindingFlags.NonPublic)?
               .GetValue(_view);

            if (propChangedEvent != null)
               foreach (var d in propChangedEvent.GetInvocationList())
                  d.DynamicInvoke(eventArgs);
         }
      }
   }
}