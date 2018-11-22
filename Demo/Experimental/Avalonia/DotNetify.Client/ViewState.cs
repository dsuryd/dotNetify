using DotNetify;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DotNetify.Client
{
   public interface IViewState
   {
      void AddList(string listName, object data, string itemKey);

      bool HasProperty(string name);

      T Get<T>(string name);

      void Set(Dictionary<string, object> states);
   }

   public class ViewState : IViewState
   {
      private readonly INotifyPropertyChanged _view;
      private readonly IUIThreadDispatcher _dispatcher;
      private readonly IDeserializer _deserializer = new VMSerializer();

      public ViewState(INotifyPropertyChanged view, IUIThreadDispatcher dispatcher)
      {
         _view = view;
         _dispatcher = dispatcher;
      }

      public object Get(string name) => Get<object>(name);

      public T Get<T>(string name)
      {
         var propInfo = _view.GetType().GetProperty(name);
         return (T)propInfo?.GetValue(_view);
      }

      public bool HasProperty(string name) => _view.GetType().GetProperty(name) != null;

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

      /// <summary>
      /// Adds a new item to a list.
      /// </summary>
      /// <param name="listName">Property name of the list.</param>
      /// <param name="data">Item to add to the list.</param>
      /// <param name="itemKey">Item property name that identify items in the list.</param>
      public virtual void AddList(string listName, object data, string itemKey)
      {
         var list = Get(listName);
         Type itemType = GetCollectionItemType(list);
         if (itemType != null)
         {
            var newItem = ConvertObjectToType(data, itemType);

            if (!string.IsNullOrEmpty(itemKey))
            {
               var itemKeyProp = itemType.GetProperty(itemKey);
               if (itemKeyProp == null)
                  throw new Exception($"couldn't add item to '{listName}' due to missing property '{itemKey}'.");

               var key = itemKeyProp.GetValue(newItem);
               foreach (var x in (IList)list)
                  if (itemKeyProp.GetValue(x) == key)
                     throw new Exception($"couldn't add item to '{listName}' because the key already exists.");
            }

            _dispatcher.InvokeAsync(() => (list as IList).Add(newItem));
         }
      }

      private object ConvertObjectToType(object data, Type type)
      {
         return data is JObject ? (data as JObject).ToObject(type) : JsonConvert.DeserializeObject(data?.ToString(), type);
      }

      private Type GetCollectionItemType(object list)
      {
         if (list == null)
            return null;

         var genericTypes = list.GetType().GetGenericArguments();
         if (genericTypes?.Length > 0)
         {
            var listType = typeof(ICollection<>).MakeGenericType(genericTypes[0]);
            if (listType.IsAssignableFrom(list.GetType()))
               return genericTypes[0];
         }
         return null;
      }
   }
}