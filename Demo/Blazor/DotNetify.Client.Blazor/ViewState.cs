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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace DotNetify.Client.Blazor
{
   /// <summary>
   /// Provides methods that access property values of a view object and raise PropertyChanged events when changed.
   /// </summary>
   public class ViewState : IViewState
   {
      private readonly object _view;
      private readonly IUIThreadDispatcher _dispatcher;
      private readonly IDeserializer _deserializer = new VMSerializer();
      private MulticastDelegate _propChangedEvent;

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="view">View object.</param>
      /// <param name="dispatcher">UI thread dispatcher.</param>
      public ViewState(object view, IUIThreadDispatcher dispatcher)
      {
         _view = view;
         _dispatcher = dispatcher;
      }

      /// <summary>
      /// Returns the value of a property.
      /// </summary>
      /// <typeparam name="T">Property type.</typeparam>
      /// <param name="name">Property name.</param>
      /// <returns>Property value.</returns>
      public T Get<T>(string name)
      {
         var propInfo = PropertyInfoHelper.Find(_view, name);
         return (T)propInfo?.GetValue(_view);
      }

      /// <summary>
      /// Returns whether the view has the specified property.
      /// </summary>
      /// <param name="name">Property name.</param>
      /// <returns>True if the view has the property.</returns>
      public virtual bool HasProperty(string name) => PropertyInfoHelper.Find(_view, name) != null;

      /// <summary>
      /// Sets the view's properties given a dictionary of property names and values, and raises PropertyChanged events.
      /// </summary>
      /// <param name="states">Dictionary of property names and values.</param>
      public virtual void Set(Dictionary<string, object> states)
      {
         foreach (string name in states.Keys)
         {
            var value = states[name]?.ToString() ?? string.Empty;
            _deserializer.Deserialize(_view, name, value);
            RaisePropertyChanged(name);
         }
      }

      /// <summary>
      /// Adds a new item to a list.
      /// </summary>
      /// <param name="listName">Property name of the list.</param>
      /// <param name="data">Item to add to the list.</param>
      /// <param name="itemKeyName">Item property name that identify items in the list.</param>
      public virtual void AddList(string listName, object data, string itemKeyName)
      {
         var list = Get<object>(listName);
         Type itemType = GetCollectionItemType(list);
         if (itemType != null)
         {
            var newItem = ConvertObjectToType(data, itemType);

            if (!string.IsNullOrEmpty(itemKeyName))
            {
               var itemKeyProp = GetProperty(itemType, itemKeyName);
               if (itemKeyProp == null)
                  throw new Exception($"couldn't add item to '{listName}' due to missing property '{itemKeyName}'.");

               var key = itemKeyProp.GetValue(newItem);
               foreach (var x in (IList)list)
                  if (itemKeyProp.GetValue(x).Equals(key))
                     throw new Exception($"couldn't add item to '{listName}' because the key already exists.");
            }

            _dispatcher.InvokeAsync(() => (list as IList).Add(newItem));
         }
      }

      /// <summary>
      /// Updates an item on a list.
      /// </summary>
      /// <param name="listName">Property name of the list.</param>
      /// <param name="data">Item to update on the list.</param>
      /// <param name="itemKeyName">Item property name that identify items in the list.</param>
      public virtual void UpdateList(string listName, object data, string itemKeyName)
      {
         var list = Get<object>(listName);
         Type itemType = GetCollectionItemType(list);
         if (itemType != null)
         {
            var newItem = ConvertObjectToType(data, itemType);

            if (!string.IsNullOrEmpty(itemKeyName))
            {
               var itemKeyProp = GetProperty(itemType, itemKeyName);
               if (itemKeyProp == null)
                  throw new Exception($"couldn't update item to '{listName}' due to missing property '{itemKeyName}'.");

               var key = itemKeyProp.GetValue(newItem);
               var listIface = list as IList;
               for (int i = 0; i < listIface.Count; i++)
               {
                  if (itemKeyProp.GetValue(listIface[i]).Equals(key))
                  {
                     _dispatcher.InvokeAsync(() =>
                     {
                        listIface.Insert(i, newItem);
                        listIface.RemoveAt(i + 1);
                     });
                     return;
                  }
               }

               throw new Exception($"couldn't update item to '{listName}' because the key '{key}' doesn't exist.");
            }
            else
               throw new Exception($"missing item key for '{listName}'; add '{listName}_itemKey' property to the view model.");
         }
      }

      /// <summary>
      /// Removes an item from a list.
      /// </summary>
      /// <param name="listName">Property name of the list.</param>
      /// <param name="key">Identifies the item to remove.</param>
      /// <param name="itemKeyName">Item property name that identify items in the list.</param>
      public virtual void RemoveList(string listName, object key, string itemKeyName)
      {
         var list = Get<object>(listName);
         Type itemType = GetCollectionItemType(list);
         if (itemType != null)
         {
            if (!string.IsNullOrEmpty(itemKeyName))
            {
               var itemKeyProp = GetProperty(itemType, itemKeyName);
               if (itemKeyProp == null)
                  throw new Exception($"couldn't remove item to '{listName}' due to missing property '{itemKeyName}'.");

               key = ConvertObjectToType(key, itemKeyProp.PropertyType);

               var listIface = list as IList;
               for (int i = 0; i < listIface.Count; i++)
               {
                  if (itemKeyProp.GetValue(listIface[i]).Equals(key))
                  {
                     _dispatcher.InvokeAsync(() => listIface.RemoveAt(i));
                     return;
                  }
               }

               throw new Exception($"couldn't remove item to '{listName}' because the key '{key}' doesn't exist.");
            }
            else
               throw new Exception($"missing item key for '{listName}'; add '{listName}_itemKey' property to the view model.");
         }
      }

      #region Private Methods

      /// <summary>
      /// Converts a data object to a given type.
      /// </summary>
      /// <param name="data">Data to convert.</param>
      /// <param name="type">Type to convert to.</param>
      /// <returns>Converted data.</returns>
      private object ConvertObjectToType(object data, Type type)
      {
         return data is JObject ? (data as JObject).ToObject(type) : JsonConvert.DeserializeObject(data?.ToString(), type);
      }

      /// <summary>
      /// Returns the generic ICollection type of a list.
      /// </summary>
      /// <param name="list">List object.</param>
      /// <returns>Generic collection type, or null if the list isn't a subclass.</returns>
      private Type GetCollectionItemType(object list)
      {
         if (list == null)
            return null;

         var genericTypes = list.GetType().GetTypeInfo().GetGenericArguments();
         if (genericTypes?.Length > 0)
         {
            var listType = typeof(ICollection<>).MakeGenericType(genericTypes[0]);
            if (listType.GetTypeInfo().IsAssignableFrom(list.GetType()))
               return genericTypes[0];
         }
         return null;
      }

      /// <summary>
      /// Returns the property info of an object type given the property name.
      /// </summary>
      /// <param name="type">Object type.</param>
      /// <param name="propName">Property name.</param>
      /// <returns></returns>
      private PropertyInfo GetProperty(Type type, string propName)
      {
         return type.GetTypeInfo().GetProperty(propName);
      }

      /// <summary>
      /// Raises property changed event.
      /// </summary>
      /// <param name="propertyName">Property name.</param>
      private void RaisePropertyChanged(string propertyName)
      {
         if (_propChangedEvent == null)
         {
            Type viewType = _view.GetType();
            while (_propChangedEvent == null && viewType != null)
            {
               _propChangedEvent = (MulticastDelegate)viewType
                 .GetTypeInfo()
                 .GetField(nameof(INotifyPropertyChanged.PropertyChanged), BindingFlags.Instance | BindingFlags.NonPublic)?
                 .GetValue(_view);
               viewType = viewType.GetTypeInfo().BaseType;
            };
         }

         if (_propChangedEvent != null)
         {
            var eventArgs = new object[] { _view, new PropertyChangedEventArgs(propertyName) };
            foreach (var d in _propChangedEvent.GetInvocationList())
               d.DynamicInvoke(eventArgs);
         }
      }

      #endregion Private Methods
   }
}