/* 
Copyright 2016 Dicky Suryadi

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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetify
{
   /// <summary>
   /// This class manages instantiations and updates of view models as requested by browser clients.
   /// </summary>
   public class VMController : IDisposable
   {
      /// <summary>
      /// Exception that gets thrown when the VMController cannot resolve a JSON view model update from the client.
      /// </summary>
      public class UnresolvedVMUpdateException : Exception { }

      #region Delegates

      /// <summary>
      /// Delegate used by the view model to provide response back to the client.
      /// </summary>
      /// <param name="connectionId">Identifies the connection.</param>
      /// <param name="vmId">Identifies the view model providing the response.</param>
      /// <param name="vmData">Response data.</param>
      public delegate void VMResponseDelegate(string connectionId, string vmId, string vmData);

      /// <summary>
      /// Delegate to create view models.
      /// </summary>
      /// <param name="type">Class type.</param>
      /// <param name="args">Optional constructor arguments.</param>
      /// <returns>Object of type T.</returns>
      public delegate object CreateInstanceDelegate(Type type, object[] args);

      /// <summary>
      /// Delegate to provide interception hook for a view model action.
      /// </summary>
      /// <param name="vm">View model.</param>
      /// <param name="data">Data being passed to the view model.</param>
      /// <param name="vmAction">View model action.</param>
      public delegate void FilterDelegate(string vmId, BaseVM vm, object data, Action<object> vmAction);

      #endregion

      #region Fields

      /// <summary>
      /// This class encapsulates a view model information.
      /// </summary>
      protected internal class VMInfo
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
      /// List of known view model classes.
      /// </summary>
      protected internal static List<Type> _vmTypes = new List<Type>();

      /// <summary>
      /// List of registered assemblies.
      /// </summary>
      protected internal static List<string> _registeredAssemblies = new List<string>();

      /// <summary>
      /// Active instances of view models.
      /// </summary>
      protected internal ConcurrentDictionary<string, VMInfo> _activeVMs = new ConcurrentDictionary<string, VMInfo>();

      /// <summary>
      /// Function invoked by the view model to provide response back to the client.
      /// </summary>
      protected internal readonly VMResponseDelegate _vmResponse;

      #endregion

      #region Filters

      /// <summary>
      /// Interception hook for incoming actions to request view models.
      /// </summary>
      public FilterDelegate RequestVMFilter { get; set; }

      /// <summary>
      /// Interception hook for incoming actions to update view models.
      /// </summary>
      public FilterDelegate UpdateVMFilter { get; set; }

      /// <summary>
      /// Interception hook for outgoing response from view models.
      /// </summary>
      public FilterDelegate ResponseVMFilter { get; set; }

      #endregion

      #region Factory Method

      /// <summary>
      /// Delegate to override default mechanism used for creating view model instances.
      /// </summary>
      public static CreateInstanceDelegate CreateInstance { get; set; } = (type, args) => Activator.CreateInstance(type, args);

      /// <summary>
      /// Creates a view model instance of type T.
      /// </summary>
      public static T Create<T>(object[] args = null) where T : class => CreateInstance(typeof(T), args) as T;

      #endregion

      // Default constructor.
      public VMController()
      {
         RequestVMFilter = (string vmId, BaseVM vm, object vmArg, Action<object> vmAction) => vmAction(vmArg);
         UpdateVMFilter = (string vmId, BaseVM vm, object vmData, Action<object> vmAction) => vmAction(vmData);
         ResponseVMFilter = (string vmId, BaseVM vm, object vmData, Action<object> vmAction) => vmAction(vmData);
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="vmResponse">Function invoked by the view model to provide response back to the client.</param>
      public VMController(VMResponseDelegate vmResponse) : this()
      {
         _vmResponse = vmResponse;
         if (_vmResponse == null)
            throw new ArgumentNullException();
      }

      /// <summary>
      /// Disposes active view models.
      /// </summary>
      public virtual void Dispose()
      {
         foreach (var kvp in _activeVMs)
         {
            kvp.Value.Instance.RequestPushUpdates -= VmInstance_RequestPushUpdates;
            kvp.Value.Instance.Dispose();
         }
      }

      /// <summary>
      /// Returns initial serialized state of a view model.
      /// </summary>
      /// <param name="iVMTypeName">View model type name.</param>
      /// <returns>Serialized view model state.</returns>
      public static string GetInitialState(string iVMTypeName, object iArgs = null)
      {
         var vmController = new VMController();
         var vm = vmController.CreateVM(iVMTypeName, iArgs);
         return vm != null ? vmController.Serialize(vm) : null;
      }

      /// <summary>
      /// Registers all view model types in an assembly.
      /// </summary>
      /// <param name="vmAssembly">Assembly.</param>
      public static void RegisterAssembly(Assembly vmAssembly)
      {
         if (vmAssembly == null)
            throw new ArgumentNullException();

         if (_registeredAssemblies.Exists(i => i == vmAssembly.FullName))
            return;

         _registeredAssemblies.Add(vmAssembly.FullName);

         bool hasVMTypes = false;
         foreach (Type vmType in vmAssembly.GetExportedTypes().Where(i => typeof(BaseVM).GetTypeInfo().IsAssignableFrom(i)))
         {
            hasVMTypes = true;
            if (_vmTypes.Find(i => i == vmType) == null)
               _vmTypes.Add(vmType);
            else
               throw new Exception($"ERROR: View model '{vmType.Name}' was already registered by another assembly!");
         }

         if (!hasVMTypes)
            throw new Exception($"ERROR: Assembly '{vmAssembly.GetName().Name}' does not define any view model!");
      }

      /// <summary>
      /// Handles a request for a view model from a browser client.
      /// </summary>
      /// <param name="connectionId">Identifies the client connection.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmArg">Optional view model's initialization argument.</param>
      public virtual void OnRequestVM(string connectionId, string vmId, object vmArg = null)
      {
         // Create a new view model instance whose class name is matching the given VMId.
         BaseVM vmInstance = !_activeVMs.ContainsKey(vmId) ? CreateVM(vmId, vmArg) : _activeVMs[vmId].Instance;

         RequestVMFilter.Invoke(vmId, vmInstance, vmArg, data =>
         {
            var vmData = Serialize(vmInstance);

            // Send the view model data back to the browser client.
            _vmResponse?.Invoke(connectionId, vmId, vmData);

            // Reset the changed property states.
            vmInstance.AcceptChangedProperties();

            // Add the view model instance to the controller.
            if (!_activeVMs.ContainsKey(vmId))
            {
               _activeVMs.TryAdd(vmId, new VMInfo { Instance = vmInstance, ConnectionId = connectionId });
               vmInstance.RequestPushUpdates += VmInstance_RequestPushUpdates;
            }
            else
               _activeVMs[vmId].ConnectionId = connectionId;

            // If this request causes other view models to change, push those new values back to the client.
            PushUpdates();
         });
      }

      /// <summary>
      /// Handles view model update from a browser client.
      /// </summary>
      /// <param name="connectionId">Identifies the client connection.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="iData">View model update.</param>
      public virtual void OnUpdateVM(string connectionId, string vmId, Dictionary<string, object> data)
      {
         bool isRecreated = false;
         if (!_activeVMs.ContainsKey(vmId))
         {
            // No view model found; it must have expired and needs to be recreated.
            isRecreated = true;
            OnRequestVM(connectionId, vmId);
            if (!_activeVMs.ContainsKey(vmId))
               return;
         }

         // Update the new values from the client to the server view model.
         var vmInstance = _activeVMs[vmId].Instance;

         // Invoke the interception delegate.
         UpdateVMFilter.Invoke(vmId, vmInstance, data, filteredData =>
         {
            lock (vmInstance)
            {
               foreach (var kvp in filteredData as Dictionary<string, object>)
               {
                  UpdateVM(vmInstance, kvp.Key, kvp.Value != null ? kvp.Value.ToString() : "");

                  // If the view model was recreated, include the changes that trigger this update to overwrite their initial values.
                  if (isRecreated && !vmInstance.ChangedProperties.ContainsKey(kvp.Key))
                     vmInstance.ChangedProperties.TryAdd(kvp.Key, kvp.Value);
               }
            }

            // If the updates cause some properties of this and other view models to change, push those new values back to the client.
            PushUpdates();
         });
      }

      /// <summary>
      /// Handles request from a browser client to remove a view model.
      /// </summary>
      /// <param name="connectionId">Identifies the client connection.</param>
      /// <param name="vmId">Identifies the view model.</param>
      public virtual void OnDisposeVM(string connectionId, string vmId)
      {
         lock (_activeVMs)
         {
            // Dispose not only the view model, but all view models within its scope.
            var vmIds = _activeVMs.Keys.Where(i => i == vmId || i.StartsWith(vmId + "."));
            foreach (var id in vmIds.OrderByDescending(i => i.Length))
            {
               VMInfo vmInfo;
               if (_activeVMs.TryRemove(id, out vmInfo))
               {
                  // If the view model is within a master view model's scope, notify the 
                  // master view model that the view model is being disposed.
                  if (id.Contains('.'))
                  {
                     var masterVMId = id.Remove(id.LastIndexOf('.'));
                     if (_activeVMs.ContainsKey(masterVMId))
                        _activeVMs[masterVMId].Instance.OnSubVMDisposing(vmInfo.Instance);
                  }

                  vmInfo.Instance.RequestPushUpdates -= VmInstance_RequestPushUpdates;
                  vmInfo.Instance.Dispose();
               }
            }
         }
      }

      /// <summary>
      /// Creates a view model.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmArg">Optional view model's initialization argument.</param>
      /// <returns>View model instance.</returns>
      protected virtual BaseVM CreateVM(string vmId, object vmArg = null)
      {
         // If the namespace argument is given, try to resolve the view model type to that namespace.
         const string NAMESPACE = "namespace";
         string vmNamespace = null;
         JToken namespaceToken;
         if (vmArg is JObject && (vmArg as JObject).TryGetValue(NAMESPACE, StringComparison.OrdinalIgnoreCase, out namespaceToken))
         {
            vmNamespace = namespaceToken.ToString();
            (vmArg as JObject).Remove(NAMESPACE);
         }

         // If the view model Id is in the form of a delimited path, it has a master view model.
         BaseVM masterVM = null;
         var path = vmId.Split('.');
         if (path.Length > 1)
         {
            // Get the master view model; create the instance if it doesn't exist.
            var masterVMId = vmId.Remove(vmId.LastIndexOf('.'));
            lock (_activeVMs)
            {
               if (!_activeVMs.ContainsKey(masterVMId))
               {
                  var arg = !string.IsNullOrEmpty(vmNamespace) ? JObject.Parse($"{{{NAMESPACE}: '{vmNamespace}'}}") : null;
                  masterVM = CreateVM(masterVMId, arg);
                  _activeVMs.TryAdd(masterVMId, new VMInfo { Instance = masterVM });
               }
               else
                  masterVM = _activeVMs[masterVMId].Instance;
            }
            vmId = vmId.Remove(0, vmId.LastIndexOf('.') + 1);
         }

         // If the view model Id contains instance Id, parse it out.
         string vmTypeName = vmId;
         string vmInstanceId = null;
         if (vmTypeName.Contains('$'))
         {
            path = vmTypeName.Split('$');
            vmTypeName = path[0];
            vmInstanceId = path[1];
         }

         // Get the view model instance from the master view model.
         var vmInstance = masterVM?.GetSubVM(vmTypeName, vmInstanceId);

         // If still no view model instance, create it ourselves here.
         if (vmInstance == null)
         {
            Type vmType = null;
            if (vmNamespace != null)
               vmType = _vmTypes.FirstOrDefault(i => i.FullName == $"{vmNamespace}.{vmTypeName}");

            vmType = vmType ?? _vmTypes.FirstOrDefault(i => i.Name == vmTypeName);
            if (vmType == null)
               throw new Exception($"[dotNetify] ERROR: '{vmId}' is not a known view model! Its assembly must be registered through VMController.RegisterAssembly.");

            try
            {
               if (vmInstanceId != null)
                  vmInstance = CreateInstance(vmType, new object[] { vmInstanceId }) as BaseVM;
            }
            catch (MissingMethodException)
            {
               Trace.Fail($"[dotNetify] ERROR: '{vmTypeName}' has no constructor accepting instance ID.");
            }

            try
            {
               if (vmInstance == null)
                  vmInstance = CreateInstance(vmType, null) as BaseVM;
            }
            catch (MissingMethodException)
            {
               Trace.Fail($"[dotNetify] ERROR: '{vmTypeName}' has no parameterless constructor.");
            }
         }

         // If there are view model arguments, set them into the instance.
         if (vmArg is JObject)
         {
            var vmJsonArg = (JObject)vmArg;
            foreach (var prop in vmJsonArg.Properties())
               UpdateVM(vmInstance, prop.Name, prop.Value.ToString());
         }

         // Pass the view model instance to the master view model.
         masterVM?.OnSubVMCreated(vmInstance);

         return vmInstance;
      }

      /// <summary>
      /// Updates a value of a view model.
      /// </summary>
      /// <param name="vmInstance">View model instance.</param>
      /// <param name="vmPath">View model property path.</param>
      /// <param name="newValue">New value.</param>
      protected virtual void UpdateVM(BaseVM vmInstance, string vmPath, string newValue)
      {
         try
         {
            object vmObject = vmInstance;
            var vmType = vmObject.GetType();
            var path = vmPath.Split('.');
            for (int i = 0; i < path.Length; i++)
            {
               var propName = path[i];
               var propInfo = vmType.GetTypeInfo().GetProperty(propName);
               if (propInfo == null)
                  throw new UnresolvedVMUpdateException();

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
                  var changedProperties = vmInstance.ChangedProperties;
                  if (changedProperties.ContainsKey(vmPath) && (changedProperties[vmPath] ?? string.Empty).ToString() == newValue)
                  {
                     object value;
                     changedProperties.TryRemove(vmPath, out value);
                  }
               }
            }
         }
         catch (UnresolvedVMUpdateException)
         {
            // If we cannot resolve the property path, forward the info to the instance
            // to give it a chance to resolve it.
            vmInstance.OnUnresolvedUpdate(vmPath, newValue);
         }
      }

      /// <summary>
      /// Push property changed updates on all view models back to the client.
      /// </summary>
      protected virtual void PushUpdates()
      {
         foreach (var kvp in _activeVMs)
         {
            var vmInstance = kvp.Value.Instance;
            lock (vmInstance)
            {
               var changedProperties = new Dictionary<string, object>(vmInstance.ChangedProperties);
               if (changedProperties.Count > 0)
               {
                  var vmData = Serialize(changedProperties);

                  ResponseVMFilter.Invoke(kvp.Key, vmInstance, vmData, filteredData =>
                  {
                     _vmResponse(kvp.Value.ConnectionId, kvp.Key, (string)filteredData);

                     // After the changes are forwarded, accept the changes so they won't be marked as changed anymore.
                     vmInstance.AcceptChangedProperties();
                  });
               }
            }
         }
      }

      /// <summary>
      /// Serializes an object.
      /// </summary>
      /// <param name="data">Data to serialize.</param>
      /// <returns>Serialized string.</returns>
      protected virtual string Serialize(object data)
      {
         try
         {
            List<string> ignoredPropertyNames = data is BaseVM ? (data as BaseVM).IgnoredProperties : null;
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings { ContractResolver = new VMContractResolver(ignoredPropertyNames) });
         }
         catch (Exception ex)
         {
            Trace.Fail(ex.ToString());
            return string.Empty;
         }
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
