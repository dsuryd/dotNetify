/* 
Copyright 2017 Dicky Suryadi

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
using System.Reflection;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DotNetify
{
   /// <summary>
   /// Base class for all DotNetify view models.  
   /// </summary>
   public partial class BaseVM
   {
      /// <summary>
      /// Exception that gets thrown a JSON view model update from the client cannot be resolved.
      /// </summary>
      public class UnresolvedVMUpdateException : Exception { }

      /// <summary>
      /// Serializes the instance into JSON-formatted string.
      /// </summary>
      /// <returns>Serialized string.</returns>
      public string Serialize()
      {
         List<string> ignoredPropertyNames = IgnoredProperties;
         return Serialize(_vmInstance, new VMContractResolver(ignoredPropertyNames));
      }

      /// <summary>
      /// Serializes only changed properties into JSON-formatted string.
      /// </summary>
      /// <returns>Serialized string.</returns>
      public string SerializeChangedProperties()
      {
         var changedProperties = new Dictionary<string, object>(ChangedProperties);
         return changedProperties.Count > 0 ? Serialize(changedProperties) : string.Empty;
      }

      /// <summary>
      /// Deserializes a property value of the instance.
      /// </summary>
      /// <param name="vmPath">View model property path.</param>
      /// <param name="newValue">New value.</param>
      public void Deserialize(string vmPath, string newValue)
      {
         if (!Deserialize(_vmInstance, vmPath, newValue))
            // If we cannot resolve the property path, forward the info to the instance to give it a chance to resolve it.
            OnUnresolvedUpdate(vmPath, newValue);
      }

      /// <summary>
      /// Serializes a view model into JSON-formatted string.
      /// </summary>
      /// <param name="viewModel">View model to serialize.</param>
      /// <param name="contractResolver">Optional JSON contract resolver.</param>
      /// <returns>Serialized view model.</returns>
      protected virtual string Serialize(object viewModel, IContractResolver contractResolver = null)
      {
         try
         {
            return JsonConvert.SerializeObject(viewModel, new JsonSerializerSettings { ContractResolver = contractResolver ?? new VMContractResolver() });
         }
         catch (Exception ex)
         {
            Trace.Fail(ex.ToString());
            return string.Empty;
         }
      }

      /// <summary>
      /// Deserializes a property value of view model.
      /// </summary>
      /// <param name="viewModel">View model to deserialize to.</param>
      /// <param name="vmPath">View model property path.</param>
      /// <param name="newValue">New value.</param>
      /// <returns>True if the value was deserialized.</returns>
      protected virtual bool Deserialize(object viewModel, string vmPath, string newValue)
      {
         try
         {
            object vmObject = _vmInstance;
            var vmType = vmObject.GetType();
            var path = vmPath.Split('.');
            for (int i = 0; i < path.Length; i++)
            {
               var propName = path[i];
               var propInfo = vmType.GetTypeInfo().GetProperty(propName);
               if (propInfo == null)
                  return false;

               var propType = propInfo.PropertyType.GetTypeInfo();

               if (i < path.Length - 1)
               {
                  // Path that starts with $ sign means it is a key to an IEnumerable property.
                  // By convention we expect a method whose name is in this format:
                  // <IEnumerable property name>_get (for example: ListContent_get) 
                  // to get the object whose key matches the given value in the path.
                  if (path[i + 1].StartsWith("$"))
                  {
                     var key = path[i + 1].TrimStart('$');
                     var methodInfo = vmType.GetTypeInfo().GetMethod(propName + "_get");
                     if (methodInfo == null)
                        return false;

                     vmObject = methodInfo.Invoke(vmObject, new object[] { key });
                     if (vmObject == null)
                        return false;

                     vmType = vmObject.GetType();
                     i++;
                  }
                  else
                  {
                     vmObject = propInfo.GetValue(vmObject);
                     vmType = vmObject != null ? vmObject.GetType() : propInfo.PropertyType;
                  }
               }
               else if (typeof(ICommand).GetTypeInfo().IsAssignableFrom(propInfo.PropertyType) && vmObject != null)
               {
                  // If the property type is ICommand, execute the command.
                  (propInfo.GetValue(vmObject) as ICommand)?.Execute(newValue);
               }
               else if (propType.IsSubclassOf(typeof(MulticastDelegate)) && propType.GetMethod(nameof(Action.Invoke)).ReturnType == typeof(void))
               {
                  // If the property type is Action, wrap the action in a Command object and execute it.
                  var argTypes = propType.GetGenericArguments();
                  var cmdType = argTypes.Length > 0 ? typeof(Command<>).MakeGenericType(argTypes) : typeof(Command);
                  (Activator.CreateInstance(cmdType, new object[] { propInfo.GetValue(vmObject) }) as ICommand)?.Execute(newValue);
               }
               else if (propInfo.SetMethod != null && vmObject != null)
               {
                  // Update the new value to the property.
                  if (propType.IsClass && propInfo.PropertyType != typeof(string))
                     propInfo.SetValue(vmObject, JsonConvert.DeserializeObject(newValue, propInfo.PropertyType));
                  else
                  {
                     var typeConverter = TypeDescriptor.GetConverter(propInfo.PropertyType);
                     if (typeConverter != null)
                        propInfo.SetValue(vmObject, typeConverter.ConvertFromString(newValue));
                  }

                  // Don't include the property we just updated in the ChangedProperties of the view model
                  // unless the value is changed internally, so that we don't send the same value back to the client
                  // during PushUpdates call by this VMController.
                  var changedProperties = ChangedProperties;
                  if (changedProperties.ContainsKey(vmPath) && (changedProperties[vmPath] ?? string.Empty).ToString() == newValue)
                  {
                     object value;
                     changedProperties.TryRemove(vmPath, out value);
                  }
               }
            }
         }
         catch (Exception ex)
         {
            Trace.Fail(ex.ToString());
            return false;
         }

         return true;
      }
   }
}
