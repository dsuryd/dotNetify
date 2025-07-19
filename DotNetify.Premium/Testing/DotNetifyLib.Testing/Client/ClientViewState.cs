using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNetify.Testing
{
   /// <summary>
   /// Emulates client state.
   /// </summary>
   internal class ClientViewState
   {
      private readonly JObject _state;
      private Dictionary<string, string> _itemKeys = new Dictionary<string, string>();

      public ClientViewState(EmulationResponse initialResponse)
      {
         _state = initialResponse.As<JObject>();
      }

      /// <summary>
      /// Converts the state to a type.
      /// </summary>
      public T As<T>() => _state.ToObject<T>();

      /// <summary>
      /// Sets the view's properties given a dictionary of property names and values, and raises PropertyChanged events.
      /// </summary>
      /// <param name="states">Dictionary of property names and values.</param>
      public void Set(EmulationResponse response)
      {
         var newState = JObject.FromObject(Preprocess(response.As<Dictionary<string, object>>()));
         _state.Merge(newState, new JsonMergeSettings
         {
            MergeArrayHandling = MergeArrayHandling.Replace,
            MergeNullValueHandling = MergeNullValueHandling.Merge
         });
      }

      /// <summary>
      /// Returns the value of a property.
      /// </summary>
      /// <typeparam name="T">Property type.</typeparam>
      /// <param name="name">Property name.</param>
      /// <returns>Property value.</returns>
      private T Get<T>(string name) => _state[name].Value<T>();

      /// <summary>
      /// Returns whether the view has the specified property.
      /// </summary>
      /// <param name="name">Property name.</param>
      /// <returns>True if the view has the property.</returns>
      private bool HasProperty(string name) => _state.ContainsKey(name);

      /// <summary>
      /// Adds a new item to a list.
      /// </summary>
      /// <param name="listName">Property name of the list.</param>
      /// <param name="data">Item to add to the list.</param>
      private void AddList(string listName, JObject data)
      {
         string itemKeyName = $"{listName}_itemKey";
         string itemKey = _state.ContainsKey(itemKeyName) ? _state[itemKeyName].ToString() : null;

         var list = _state[listName].ToList();
         if (itemKey != null)
            list = list.Where(item => (string)item[itemKey] != (string)data[itemKey]).ToList();

         list.Add(data);
         _state[listName] = JArray.FromObject(list);
      }

      /// <summary>
      /// Adds a new array item to a list.
      /// </summary>
      /// <param name="listName">Property name of the list.</param>
      /// <param name="data">Item to add to the list.</param>
      private void AddList(string listName, JArray data)
      {
         var list = _state[listName].ToList();
         list.Add(data);
         _state[listName] = JArray.FromObject(list);
      }

      /// <summary>
      /// Updates an item on a list.
      /// </summary>
      /// <param name="listName">Property name of the list.</param>
      /// <param name="data">Item to update on the list.</param>
      private void UpdateList(string listName, JObject data)
      {
         string itemKeyName = $"{listName}_itemKey";
         string itemKey = _state.ContainsKey(itemKeyName) ? _state[itemKeyName].ToString() : null;

         var list = _state[listName].ToList();
         if (itemKey != null)
         {
            list = list.Where(item => (string)item[itemKey] != (string)data[itemKey]).ToList();
            list.Add(data);

            _state[listName] = JArray.FromObject(list);
         }
      }

      /// <summary>
      /// Removes an item from a list.
      /// </summary>
      /// <param name="listName">Property name of the list.</param>
      /// <param name="key">Identifies the item to remove.</param>
      private void RemoveList(string listName, object key)
      {
         string itemKeyName = $"{listName}_itemKey";
         string itemKey = _state.ContainsKey(itemKeyName) ? _state[itemKeyName].ToString() : null;

         var list = _state[listName].ToList();
         if (itemKey != null)
            list = list.Where(item => (string)item[itemKey] != key.ToString()).ToList();

         _state[listName] = JArray.FromObject(list);
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
      }

      /// <summary>
      /// Preprocess view model update from the server before we set the state.
      /// </summary>
      /// <param name="data">Dictionary of property names and values.</param>
      /// <returns>Preprocessed data.</returns>
      private Dictionary<string, object> Preprocess(Dictionary<string, object> data)
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
                  if (HasProperty(listName))
                  {
                     if (kvp.Value is JArray)
                        AddList(listName, kvp.Value as JArray);
                     else
                        AddList(listName, JObject.FromObject(kvp.Value));
                  }
                  else
                     throw new ApplicationException($"Unable to resolve `${propName}`");

                  data.Remove(propName);
                  continue;
               }

               // Look for property that end with '_update'. Interpret the value as a list item to be updated
               // to an existing list whose property name precedes that suffix.
               match = Regex.Match(propName, @"(.*)_update");
               if (match.Success && kvp.Value != null)
               {
                  var listName = match.Groups[1].Value;
                  if (HasProperty(listName))
                     UpdateList(listName, JObject.FromObject(kvp.Value));
                  else
                     throw new ApplicationException($"Unable to resolve `${propName}`");
                  data.Remove(propName);
                  continue;
               }

               // Look for property that end with '_remove'. Interpret the value as a list item key to remove
               // from an existing list whose property name precedes that suffix.
               match = Regex.Match(propName, @"(.*)_remove");
               if (match.Success && kvp.Value != null)
               {
                  var listName = match.Groups[1].Value;
                  if (HasProperty(listName))
                     RemoveList(listName, kvp.Value);
                  else
                     throw new ApplicationException($"Unable to resolve `${propName}`");
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
               Trace.TraceWarning($"{ex.Message} {ex.InnerException?.Message}");
               data.Remove(propName);
            }
         }
         return data;
      }
   }
}