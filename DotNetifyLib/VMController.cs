/* 
Copyright 2015 Dicky Suryadi

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
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using DotNetify.Routing;

namespace DotNetify
{
   /// <summary>
   /// This class manages instantiations and updates of view models as requested by browser clients.
   /// </summary>
   public class VMController : IDisposable
   {
      #region Fields

      /// <summary>
      /// This class encapsulates a view model information.
      /// </summary>
      protected class VMInfo
      {
         /// <summary>
         /// Instance of a view model.
         /// </summary>
         public BaseVM Instance { get; set; }

         /// <summary>
         /// Identifies the SignalR connection to the browser client associated with the view model.
         /// </summary>
         public string ConnectionId { get; set; }
      }

      /// <summary>
      /// Custom resolver for JSON serializer.
      /// </summary>
      protected class CustomResolver : DefaultContractResolver
      {
         private List<string> _IgnoredPropertyNames;

         /// <summary>
         /// Constructor that accepts list of property names to exclude from serialization.
         /// </summary>
         /// <param name="iIgnoredPropertyNames">Property names to exclude from serialization.</param>
         public CustomResolver(List<string> iIgnoredPropertyNames = null) : base()
         {
            _IgnoredPropertyNames = iIgnoredPropertyNames;
         }

         /// <summary>
         /// Overrides this method to exclude properties with [Ignore] attribute or those that are in the given list.
         /// </summary>
         protected override JsonProperty CreateProperty(MemberInfo iMember, MemberSerialization iMemberSerialization)
         {
            var property = base.CreateProperty(iMember, iMemberSerialization);

            // Don't serialize properties that are decorated with [Ignore] or whose name are in the given list.
            if (iMember.GetCustomAttribute(typeof(BaseVM.IgnoreAttribute)) != null)
               property.Ignored = true;
            else if (_IgnoredPropertyNames != null && _IgnoredPropertyNames.Contains(property.PropertyName))
               property.Ignored = true;

            return property;
         }
      }

      /// <summary>
      /// List of known view model classes.
      /// </summary>
      protected static List<Type> _VMTypes = new List<Type>();

      /// <summary>
      /// Active instances of view models.
      /// </summary>
      protected ConcurrentDictionary<string, VMInfo> _ActiveVMs = new ConcurrentDictionary<string, VMInfo>();

      /// <summary>
      /// Delegate used for creating view model instances. 
      /// </summary>
      protected static Func<Type, object[], object> _createInstanceFunc = (type, args) => Activator.CreateInstance(type, args);

      #endregion

      /// <summary>
      /// Exception that gets thrown when the VMController cannot resolve a JSON view model update from the client.
      /// </summary>
      public class UnresolvedVMUpdateException : Exception { }

      /// <summary>
      /// Disposes active view models.
      /// </summary>
      public virtual void Dispose()
      {
         foreach (var kvp in _ActiveVMs)
         {
            kvp.Value.Instance.RequestPushUpdates -= VmInstance_RequestPushUpdates;
            kvp.Value.Instance.Dispose();
         }
      }

      /// <summary>
      /// Delegate to override default mechanism used for creating view model instances.
      /// </summary>
      public static Func<Type, object[], object> CreateInstance
      {
         get { return _createInstanceFunc; }
         set { _createInstanceFunc = value; }
      }

      /// <summary>
      /// Registers all view model types in an assembly.
      /// </summary>
      /// <param name="iVMAssembly">Assembly.</param>
      public static void RegisterAssembly(Assembly iVMAssembly)
      {
         if (iVMAssembly == null)
            return;

         bool hasVMTypes = false;
         foreach (Type vmType in iVMAssembly.GetExportedTypes().Where(i => typeof(BaseVM).IsAssignableFrom(i)))
         {
            hasVMTypes = true;
            if (_VMTypes.Find(i => i == vmType) == null)
               _VMTypes.Add(vmType);
            else
               throw new Exception(String.Format("ERROR: View model '{0}' was already registered by another assembly!", vmType.Name));
         }

         if (!hasVMTypes)
            throw new Exception(String.Format("ERROR: Assembly '{0}' does not define any view model!", iVMAssembly.GetName().Name));
      }

      /// <summary>
      /// Handles a request for a view model from a browser client.
      /// </summary>
      /// <param name="iConnectionId">Identifies the client connection.</param>
      /// <param name="iVMId">Identifies the view model.</param>
      /// <param name="iVMArg">Optional view model's initialization argument.</param>
      public virtual void OnRequestVM(string iConnectionId, string iVMId, object iVMArg = null)
      {
         // Create a new view model instance whose class name is matching the given VMId.
         BaseVM vmInstance = !_ActiveVMs.ContainsKey(iVMId) ? CreateVM(iVMId, iVMArg) : _ActiveVMs[iVMId].Instance;

         var vmData = Serialize(vmInstance);

         // Send the view model data back to the browser client.
         DotNetifyHub.Response_VM(iConnectionId, iVMId, vmData);

         // Reset the changed property states.
         vmInstance.AcceptChangedProperties();

         // Add the view model instance to the controller.
         if (!_ActiveVMs.ContainsKey(iVMId))
         {
            _ActiveVMs.TryAdd(iVMId, new VMInfo { Instance = vmInstance, ConnectionId = iConnectionId });
            vmInstance.RequestPushUpdates += VmInstance_RequestPushUpdates;
         }
         else
            _ActiveVMs[iVMId].ConnectionId = iConnectionId;

         // If this request causes other view models to change, push those new values back to the client.
         PushUpdates();
      }

      /// <summary>
      /// Handles view model update from a browser client.
      /// </summary>
      /// <param name="iConnectionId">Identifies the client connection.</param>
      /// <param name="iVMId">Identifies the view model.</param>
      /// <param name="iData">View model update.</param>
      public virtual void OnUpdateVM(string iConnectionId, string iVMId, Dictionary<string, object> iData)
      {
         bool isRecreated = false;
         if (!_ActiveVMs.ContainsKey(iVMId))
         {
            // No view model found; it must have expired and needs to be recreated.
            isRecreated = true;
            OnRequestVM(iConnectionId, iVMId);
            if (!_ActiveVMs.ContainsKey(iVMId))
               return;
         }

         // Update the new values from the client to the server view model.
         var vmInstance = _ActiveVMs[iVMId].Instance;
         lock (vmInstance)
         {
            foreach (var kvp in iData)
            {
               UpdateVM(vmInstance, kvp.Key, kvp.Value != null ? kvp.Value.ToString() : "");

               // If the view model was recreated, include the changes that trigger this update to overwrite their initial values.
               if (isRecreated && !vmInstance.ChangedProperties.ContainsKey(kvp.Key))
                  vmInstance.ChangedProperties.TryAdd(kvp.Key, kvp.Value);
            }
         }

         // If the updates cause some properties of this and other view models to change, push those new values back to the client.
         PushUpdates();
      }

      /// <summary>
      /// Handles request from a browser client to remove a view model.
      /// </summary>
      /// <param name="iConnectionId">Identifies the client connection.</param>
      /// <param name="iVMId">Identifies the view model.</param>
      public virtual void OnDisposeVM(string iConnectionId, string iVMId)
      {
         if (_ActiveVMs.ContainsKey(iVMId))
         {
            VMInfo vmInfo;
            _ActiveVMs.TryRemove(iVMId, out vmInfo);
            vmInfo.Instance.RequestPushUpdates -= VmInstance_RequestPushUpdates;
            vmInfo.Instance.Dispose();
         }
      }

      /// <summary>
      /// Creates a view model.
      /// </summary>
      /// <param name="iVMId">Identifies the view model.</param>
      /// <param name="iVMArg">Optional view model's initialization argument.</param>
      /// <returns>View model instance.</returns>
      protected virtual BaseVM CreateVM(string iVMId, object iVMArg = null)
      {
         BaseVM vmInstance = null;

         // If the view model Id is in the form of a delimited path, it has a master view model.
         BaseVM masterVM = null;
         var path = iVMId.Split('.');
         if (path.Length > 1)
         {
            // Get the master view model; create the instance if it doesn't exist.
            var masterVMId = iVMId.Remove(iVMId.LastIndexOf('.'));
            lock (_ActiveVMs)
            {
               if (!_ActiveVMs.ContainsKey(masterVMId))
               {
                  masterVM = CreateVM(masterVMId);
                  _ActiveVMs.TryAdd(masterVMId, new VMInfo { Instance = masterVM });
               }
               else
                  masterVM = _ActiveVMs[masterVMId].Instance;
            }
            iVMId = iVMId.Remove(0, iVMId.LastIndexOf('.') + 1);
         }

         // If the view model Id contains instance Id, parse it out.
         string vmTypeName = iVMId;
         string vmInstanceId = null;
         if (vmTypeName.Contains('$'))
         {
            path = vmTypeName.Split('$');
            vmTypeName = path[0];
            vmInstanceId = path[1];
         }

         // Get the view model instance from the master view model.
         if (masterVM != null)
            vmInstance = masterVM.GetSubVM(vmTypeName, vmInstanceId);

         // If still no view model instance, create it ourselves here.
         if (vmInstance == null)
         {
            var vmType = _VMTypes.FirstOrDefault(i => i.Name == vmTypeName);
            if (vmType == null)
               throw new Exception(String.Format("ERROR: '{0}' is not a known view model! Its assembly must be registered through VMController.RegisterAssembly.", iVMId));

            try
            {
               if (vmInstanceId != null)
                  vmInstance = CreateInstance(vmType, new object[] { vmInstanceId }) as BaseVM;
            }
            catch (MissingMethodException)
            {
               Debug.Fail(String.Format("ERROR: '{0}' has no constructor accepting instance ID.", vmTypeName));
            }

            try
            {
               if (vmInstance == null)
                  vmInstance = CreateInstance(vmType, null) as BaseVM;
            }
            catch (MissingMethodException)
            {
               Debug.Fail(String.Format("ERROR: '{0}' has no parameterless constructor.", vmTypeName));
            }
         }

         // If there are view model arguments, set them into the instance.
         if (iVMArg != null)
         {
            var vmArg = (JObject)iVMArg;
            foreach (var prop in vmArg.Properties())
               UpdateVM(vmInstance, prop.Name, prop.Value.ToString());
         }

         return vmInstance;
      }

      /// <summary>
      /// Updates a value of a view model.
      /// </summary>
      /// <param name="iVMInstance">View model instance.</param>
      /// <param name="iVMPath">View model property path.</param>
      /// <param name="iNewValue">New value.</param>
      protected virtual void UpdateVM(BaseVM iVMInstance, string iVMPath, string iNewValue)
      {
         try
         {
            object vmObject = iVMInstance;
            var vmType = vmObject.GetType();
            var path = iVMPath.Split('.');
            for (int i = 0; i < path.Length; i++)
            {
               var propName = path[i];
               var propInfo = vmType.GetProperty(propName);
               if (propInfo == null)
                  throw new UnresolvedVMUpdateException();

               if (i < path.Length - 1)
               {
                  // Path that starts with $ sign means it is a key to an IEnumerable property.
                  // By convention we expect a method whose name is in this format:
                  // <IEnumerable property name>_get (for example: ListContent_get) 
                  // to get the object whose key matches the given value in the path.
                  if (path[i + 1].StartsWith("$"))
                  {
                     var key = path[i + 1].TrimStart('$');
                     var methodInfo = vmType.GetMethod(propName + "_get");
                     if (methodInfo == null)
                        throw new UnresolvedVMUpdateException();

                     vmObject = methodInfo.Invoke(vmObject, new object[] { key });
                     if (vmObject == null)
                        throw new UnresolvedVMUpdateException();

                     vmType = vmObject.GetType();
                     i++;
                  }
                  else
                  {
                     vmObject = propInfo.GetValue(vmObject);
                     vmType = vmObject != null ? vmObject.GetType() : propInfo.PropertyType;
                  }
               }
               else if (propInfo.SetMethod != null && vmObject != null)
               {
                  // Update the new value to the property.
                  if (propInfo.PropertyType.IsClass && propInfo.PropertyType != typeof(string))
                     propInfo.SetValue(vmObject, JsonConvert.DeserializeObject(iNewValue, propInfo.PropertyType));
                  else
                  {
                     var typeConverter = TypeDescriptor.GetConverter(propInfo.PropertyType);
                     if (typeConverter != null)
                        propInfo.SetValue(vmObject, typeConverter.ConvertFromString(iNewValue));
                  }

                  // Don't include the property we just updated in the ChangedProperties of the view model
                  // unless the value is changed internally, so that we don't send the same value back to the client
                  // during PushUpdates call by this VMController.
                  var changedProperties = iVMInstance.ChangedProperties;
                  if (changedProperties.ContainsKey(iVMPath) && (changedProperties[iVMPath] ?? String.Empty).ToString() == iNewValue)
                  {
                     object value;
                     changedProperties.TryRemove(iVMPath, out value);
                  }
               }
            }
         }
         catch (UnresolvedVMUpdateException)
         {
            // If we cannot resolve the property path, forward the info to the instance
            // to give it a chance to resolve it.
            iVMInstance.OnUnresolvedUpdate(iVMPath, iNewValue);
         }
      }

      /// <summary>
      /// Push property changed updates on all view models back to the client.
      /// </summary>
      protected virtual void PushUpdates()
      {
         foreach (var kvp in _ActiveVMs)
         {
            var vmInstance = kvp.Value.Instance;
            lock (vmInstance)
            {
               var changedProperties = new Dictionary<string, object>(vmInstance.ChangedProperties);
               if (changedProperties.Count > 0)
               {
                  var vmData = Serialize(changedProperties);
                  DotNetifyHub.Response_VM(kvp.Value.ConnectionId, kvp.Key, vmData);

                  // After the changes are forwarded, accept the changes so they won't be marked as changed anymore.
                  vmInstance.AcceptChangedProperties();
               }
            }
         }
      }

      /// <summary>
      /// Serializes an object.
      /// </summary>
      /// <param name="iData">Data to serialize.</param>
      /// <returns>Serialized string.</returns>
      protected virtual string Serialize(object iData)
      {
         List<string> ignoredPropertyNames = iData is BaseVM ? (iData as BaseVM).IgnoredProperties : null;
         return JsonConvert.SerializeObject(iData, new JsonSerializerSettings { ContractResolver = new CustomResolver(ignoredPropertyNames) });
      }

      /// <summary>
      /// Handles push updates request from a view model.
      /// </summary>
      private void VmInstance_RequestPushUpdates(object sender, EventArgs e)
      {
         PushUpdates();
      }
   }
}
