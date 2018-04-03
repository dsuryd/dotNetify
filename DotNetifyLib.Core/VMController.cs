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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DotNetify
{
   /// <summary>
   /// This class manages instantiations and updates of view models as requested by browser clients.
   /// </summary>
   public partial class VMController : IDisposable
   {
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

      #endregion Delegates

      #region Fields

      /// <summary>
      /// Dependency injection service scope.
      /// </summary>
      private readonly IVMServiceScope _serviceScope;

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
      /// Active instances of view models.
      /// </summary>
      protected internal ConcurrentDictionary<string, VMInfo> _activeVMs = new ConcurrentDictionary<string, VMInfo>();

      /// <summary>
      /// Function invoked by the view model to provide response back to the client.
      /// </summary>
      protected internal readonly VMResponseDelegate _vmResponse;

      #endregion Fields

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

      #endregion Filters

      /// <summary>
      /// Provides scoped dependency injection service provider.
      /// </summary>
      public IServiceProvider ServiceProvider => _serviceScope?.ServiceProvider;

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
      /// <param name="serviceScope">Dependency injection service scope.</param>
      public VMController(VMResponseDelegate vmResponse, IVMServiceScope serviceScope = null) : this()
      {
         _vmResponse = vmResponse ?? throw new ArgumentNullException(nameof(vmResponse));
         _serviceScope = serviceScope;
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

         _serviceScope?.Dispose();
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
         return vm?.Serialize();
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
            var vmData = vmInstance.Serialize();

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
      /// <param name="vmNamespace">Optional view model type's namespace.</param>
      /// <returns>View model instance.</returns>
      protected virtual BaseVM CreateVM(string vmId, object vmArg = null, string vmNamespace = null)
      {
         // If the namespace argument is given, try to resolve the view model type to that namespace.
         vmNamespace = vmNamespace ?? ExtractNamespace(ref vmArg);

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
                  masterVM = CreateVM(masterVMId, null, vmNamespace);
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

         // Get the view model instance from the master view model, and if not, create it ourselves here.
         var vmInstance = masterVM?.GetSubVM(vmTypeName, vmInstanceId)
            ?? BaseVM.Create(VMTypes, vmTypeName, vmInstanceId, vmNamespace)
            ?? throw new Exception($"[dotNetify] ERROR: '{vmId}' is not a known view model! Its assembly must be registered through VMController.RegisterAssembly.");

         // If there are view model arguments, set them into the instance.
         if (vmArg is JObject)
            foreach (var prop in (vmArg as JObject).Properties())
               UpdateVM(vmInstance, prop.Name, prop.Value.ToString());

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
         vmInstance.DeserializeProperty(vmPath, newValue);
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
               var vmData = vmInstance.SerializeChangedProperties();
               if (!string.IsNullOrEmpty(vmData))
               {
                  ResponseVMFilter.Invoke(kvp.Key, vmInstance, vmData, filteredData =>
                  {
                     _vmResponse(kvp.Value.ConnectionId, kvp.Key, (string)filteredData);
                  });
               }
            }
         }
      }

      /// <summary>
      /// Extracts the namespace string from a view model initialization argument.
      /// </summary>
      /// <param name="vmArg">View model's initialization argument.</param>
      /// <returns>Namespace string or null.</returns>
      private string ExtractNamespace(ref object vmArg)
      {
         const string NAMESPACE = "namespace";
         string vmNamespace = null;
         JToken namespaceToken;
         if (vmArg is JObject && (vmArg as JObject).TryGetValue(NAMESPACE, StringComparison.OrdinalIgnoreCase, out namespaceToken))
         {
            vmNamespace = namespaceToken.ToString();
            (vmArg as JObject).Remove(NAMESPACE);
         }
         return vmNamespace;
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