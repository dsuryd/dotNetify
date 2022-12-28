﻿/*
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
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace DotNetify
{
   /// <summary>
   /// This class manages instantiations and updates of view models as requested by browser clients.
   /// </summary>
   public partial class VMController : IDisposable
   {
      public static readonly string MULTICAST = "multicast$";

      /// <summary>
      /// Specifies parameters for sending data to a group of connections.
      /// </summary>
      public class GroupSend
      {
         public string GroupName { get; set; }
         public IList<string> UserIds { get; set; }
         public IList<string> ConnectionIds { get; set; }
         public IList<string> ExcludedConnectionIds { get; set; }
         public string Data { get; set; }

         public bool IsEmpty() => string.IsNullOrWhiteSpace(GroupName) && (UserIds == null || UserIds.Count == 0) && (ConnectionIds == null || ConnectionIds.Count == 0);
      }

      /// <summary>
      /// Specifies paramaters for removing a connection from a group.
      /// </summary>
      public class GroupRemove
      {
         public string GroupName { get; set; }
         public string ConnectionId { get; set; }
      }

      #region Delegates

      /// <summary>
      /// Delegate used by the view model to provide response back to the client.
      /// </summary>
      /// <param name="connectionId">Identifies the connection.</param>
      /// <param name="vmId">Identifies the view model providing the response.</param>
      /// <param name="vmData">Response data.</param>
      public delegate Task VMResponseDelegate(string connectionId, string vmId, string vmData);

      /// <summary>
      /// Delegate to create view models.
      /// </summary>
      /// <param name="type">Class type.</param>
      /// <param name="args">Optional constructor arguments.</param>
      /// <returns>Object of type T.</returns>
      public delegate object CreateInstanceDelegate(Type type, object[] args);

      /// <summary>
      /// Delegate for a view model action.
      /// </summary>
      /// <param name="data">Context data.</param>
      /// <returns></returns>
      public delegate Task VMActionDelegate(object data);

      /// <summary>
      /// Delegate to provide interception hook for a view model action.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vm">View model instance.</param>
      /// <param name="data">Data being passed to the view model.</param>
      /// <param name="vmAction">View model action.</param>
      public delegate Task FilterDelegate(string vmId, BaseVM vm, object data, VMActionDelegate vmAction);

      /// <summary>
      /// Delegate to provide interception hook for a view model response action.
      /// </summary>
      /// <param name="vm">View model info.</param>
      /// <param name="data">Data being passed to the view model.</param>
      /// <param name="vmAction">View model action.</param>
      public delegate Task ResponseFilterDelegate(VMInfo vm, object data, VMActionDelegate vmAction);

      #endregion Delegates

      #region Fields

      /// <summary>
      /// View model factory.
      /// </summary>
      private readonly IVMFactory _vmFactory;

      /// <summary>
      /// Dependency injection service scope.
      /// </summary>
      private IVMServiceScope _serviceScope;

      private readonly object _serviceScopeLock = new object();

      /// <summary>
      /// This class encapsulates a view model information.
      /// </summary>
      public class VMInfo
      {
         /// <summary>
         /// Identifies the view model.
         /// </summary>
         public string Id { get; }

         /// <summary>
         /// Instance of the view model.
         /// </summary>
         public BaseVM Instance { get; }

         /// <summary>
         /// Identifies the SignalR connection to the browser client associated with the view model.
         /// </summary>
         public string ConnectionId { get; set; }

         /// <summary>
         /// Identifies the group the view model belongs to.
         /// </summary>
         public string GroupName { get; set; }

         public VMInfo(string id, BaseVM instance, string connectionId, string groupName = null)
         {
            Id = id;
            Instance = instance;
            ConnectionId = connectionId;
            GroupName = groupName;
         }
      }

      /// <summary>
      /// Active instances of view models.
      /// </summary>
      protected internal ConcurrentDictionary<string, VMInfo> _activeVMs = new ConcurrentDictionary<string, VMInfo>();

      /// <summary>
      /// Function invoked by the view model to provide response back to the client.
      /// </summary>
      public VMResponseDelegate VMResponse { get; set; }

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
      public ResponseFilterDelegate ResponseVMFilter { get; set; }

      #endregion Filters

      /// <summary>
      /// Provides a factory method using the service provider scoped to this instance.
      /// </summary>
      public Func<Type, object[], object> FactoryMethod => (Type type, object[] args) =>
      {
         lock (_serviceScopeLock)
         {
            var provider = _serviceScope?.ServiceProvider;
            return provider != null ? ActivatorUtilities.CreateInstance(provider, type, args ?? new object[] { }) : null;
         }
      };

      // Default constructor.
      internal VMController()
      {
         RequestVMFilter = (string vmId, BaseVM vm, object vmArg, VMActionDelegate vmAction) => vmAction(vmArg);
         UpdateVMFilter = (string vmId, BaseVM vm, object vmData, VMActionDelegate vmAction) => vmAction(vmData);
         ResponseVMFilter = (VMInfo vmInfo, object vmData, VMActionDelegate vmAction) => vmAction(vmData);
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="vmResponse">Function invoked by the view model to provide response back to the client.</param>
      /// <param name="serviceScope">Dependency injection service scope.</param>
      public VMController(VMResponseDelegate vmResponse, IVMFactory vmFactory, IVMServiceScope serviceScope = null) : this()
      {
         VMResponse = vmResponse ?? throw new ArgumentNullException(nameof(vmResponse));
         _vmFactory = vmFactory;
         _serviceScope = serviceScope;
      }

      /// <summary>
      /// Disposes active view models.
      /// </summary>
      public virtual void Dispose()
      {
         foreach (var kvp in _activeVMs)
            DisposeViewModel(kvp.Value);

         lock (_serviceScopeLock)
         {
            _serviceScope?.Dispose();
            _serviceScope = null;
         }
      }

      /// <summary>
      /// Returns initial serialized state of a view model.
      /// </summary>
      /// <param name="iVMTypeName">View model type name.</param>
      /// <returns>Serialized view model state.</returns>
      public string GetInitialState(string iVMTypeName, object iArgs = null)
      {
         var vm = CreateVM(iVMTypeName, iArgs);
         return vm?.Serialize();
      }

      /// <summary>
      /// Checks whether this controller has a view model instance.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <returns>True if the controller has the view model.</returns>
      public bool HasVM(string vmId) => _activeVMs.ContainsKey(vmId);

      /// <summary>
      /// Handles a request for a view model from a browser client.
      /// </summary>
      /// <param name="connectionId">Identifies the client connection.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmArg">Optional view model's initialization argument.</param>
      /// <returns>Group name, if the request is for a multicast view model associated with one.</returns>
      [Obsolete]
      public virtual string OnRequestVM(string connectionId, string vmId, object vmArg = null)
      {
         return OnRequestVMAsync(connectionId, vmId, vmArg).ConfigureAwait(false).GetAwaiter().GetResult();
      }

      /// <summary>
      /// Handles a request for a view model from a browser client.
      /// </summary>
      /// <param name="connectionId">Identifies the client connection.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmArg">Optional view model's initialization argument.</param>
      /// <returns>Group name, if the request is for a multicast view model associated with one.</returns>
      public virtual async Task<string> OnRequestVMAsync(string connectionId, string vmId, object vmArg = null)
      {
         BaseVM vmInstance = null;
         if (HasVM(vmId))
            vmInstance = _activeVMs[vmId].Instance;
         else
         {
            // Create a new view model instance whose class name is matching the given VMId.
            vmInstance = CreateVM(vmId, vmArg);

            // Let the instance complete its initialization. If multicast, make sure it's only called once.
            if ((vmInstance as MulticastVM)?.RaiseCreatedEvent != false)
               await vmInstance.OnCreatedAsync();
         }

         await RequestVMFilter.Invoke(vmId, vmInstance, vmArg, async data =>
         {
            string vmData = vmInstance.Serialize();
            string groupName = vmInstance is MulticastVM ? (vmInstance as MulticastVM).GroupName : null;

            // Send the view model data back to the browser client.
            await ResponseVMFilter.Invoke(new VMInfo(vmId, vmInstance, connectionId, groupName), vmData, filteredData => VMResponse(connectionId, vmId, (string) filteredData));

            // Reset the changed property states unless it's a multicast.
            if (vmInstance is MulticastVM == false)
               vmInstance.AcceptChangedProperties();

            // Add the view model instance to the controller.
            if (!HasVM(vmId))
            {
               var vmInfo = new VMInfo(id: vmId, instance: vmInstance, connectionId: connectionId, groupName: groupName);
               vmInstance.RequestPushUpdates += VmInstance_RequestPushUpdates;
               if (vmInstance is MulticastVM)
               {
                  var multicastVM = vmInstance as MulticastVM;
                  multicastVM.RequestMulticastPushUpdates += VMInstance_RequestMulticastPushUpdates;
                  multicastVM.RequestSend += VMInstance_RequestSend;
               }
               _activeVMs.TryAdd(vmId, vmInfo);
            }
            else
               _activeVMs[vmId].ConnectionId = connectionId;

            // If this request causes other view models to change, push those new values back to the client.
            foreach (var vmInfo in _activeVMs.Values)
               PushUpdates(vmInfo);
         });

         return _activeVMs[vmId].GroupName;
      }

      /// <summary>
      /// Handles view model update from a browser client.
      /// </summary>
      /// <param name="connectionId">Identifies the client connection.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="iData">View model update.</param>
      [Obsolete]
      public void OnUpdateVM(string connectionId, string vmId, Dictionary<string, object> data)
      {
         OnUpdateVMAsync(connectionId, vmId, data).ConfigureAwait(false).GetAwaiter().GetResult();
      }

      /// <summary>
      /// Handles view model update from a browser client.
      /// </summary>
      /// <param name="connectionId">Identifies the client connection.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="iData">View model update.</param>
      public async Task OnUpdateVMAsync(string connectionId, string vmId, Dictionary<string, object> data)
      {
         if (!HasVM(vmId))
         {
            Logger.LogError($"Update to '{vmId}' received before connect request");
            return;
         }

         // Update the new values from the client to the server view model.
         var vmInstance = _activeVMs[vmId].Instance;

         // Invoke the interception delegate.
         await UpdateVMFilter.Invoke(vmId, vmInstance, data, async filteredData =>
         {
            List<Task> asyncCommands = null;

            lock (vmInstance)
            {
               foreach (var kvp in filteredData as Dictionary<string, object>)
                  UpdateVM(vmInstance, kvp.Key, kvp.Value != null ? kvp.Value.ToString() : "");

               if (vmInstance.AsyncCommands.Count > 0)
               {
                  asyncCommands = new List<Task>(vmInstance.AsyncCommands);
                  vmInstance.AsyncCommands.Clear();
               }
            }

            /// Await for any asynchronous command executed during deserialization.
            if (asyncCommands != null)
               await Task.WhenAll(asyncCommands);

            // If the updates cause some properties of this and other view models to change, push those new values back to the client.
            lock (vmInstance)
            {
               var vmInfo = _activeVMs.Values.FirstOrDefault(vm => vm.Instance == vmInstance);
               if (vmInfo != null)
               {
                  var changedProperties = new Dictionary<string, object>(vmInstance.ChangedProperties);

                  // Exclude the changes that trigger this update if their values don't change.
                  foreach (var kvp in data)
                     if (vmInstance.IsEqualToChangedPropertyValue(kvp.Key, kvp.Value))
                        changedProperties.Remove(kvp.Key);

                  if (changedProperties.Count > 0)
                  {
                     var vmData = vmInstance.Serialize(changedProperties);
                     PushUpdates(vmInfo, vmData);
                  }

                  if (vmInstance is MulticastVM)
                     (vmInstance as MulticastVM).PushUpdatesExcept(vmInfo.ConnectionId);
                  else
                     vmInstance.AcceptChangedProperties();
               }
            }

            // Push updates on other view model instances in case they too change.
            foreach (var vmInfo in _activeVMs.Values.Where(vm => vm.Instance != vmInstance))
               PushUpdates(vmInfo);
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
               if (_activeVMs.TryRemove(id, out VMInfo vmInfo))
               {
                  // If the view model is within a master view model's scope, notify the
                  // master view model that the view model is being disposed.
                  if (id.Contains('.'))
                  {
                     var masterVMId = id.Remove(id.LastIndexOf('.'));
                     if (HasVM(masterVMId))
                        _activeVMs[masterVMId].Instance.OnSubVMDisposing(vmInfo.Instance);
                  }

                  DisposeViewModel(vmInfo);
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
         string masterVMId = null;
         var path = vmId.Split('.');
         if (path.Length > 1)
         {
            // Get the master view model; create the instance if it doesn't exist.
            masterVMId = vmId.Remove(vmId.LastIndexOf('.'));
            lock (_activeVMs)
            {
               if (!HasVM(masterVMId))
               {
                  masterVM = CreateVM(masterVMId, null, vmNamespace);
                  _activeVMs.TryAdd(masterVMId, new VMInfo(id: masterVMId, instance: masterVM, connectionId: null));
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

         BaseVM vmInstance = null;
         Task createVMAction(object data)
         {
            // Get the view model instance from the master view model, and if not, create it ourselves here.
            vmInstance = masterVM?.GetSubVM(vmTypeName, vmInstanceId) ?? _vmFactory.GetInstance(vmTypeName, vmInstanceId, vmNamespace);
            if (vmInstance != null)
            {
               // If there are view model arguments, set them into the instance.
               if (data is JObject)
                  foreach (var prop in (data as JObject).Properties())
                     UpdateVM(vmInstance, prop.Name, prop.Value.ToString());

               // Pass the view model instance to the master view model.
               masterVM?.OnSubVMCreated(vmInstance);
            }
            return Task.CompletedTask;
         }

         // If there's a master view model, run it through the view model filter.
         if (masterVM != null)
            RequestVMFilter(masterVMId, masterVM, vmArg, createVMAction);
         else
            createVMAction(vmArg);

         if (vmInstance == null)
            throw new Exception($"[dotNetify] ERROR: '{vmId}' is not a known view model! Its assembly must be registered through VMController.RegisterAssembly.");
         return vmInstance;
      }

      /// <summary>
      /// Disposes a view model.
      /// </summary>
      /// <param name="vmInfo">View model info.</param>
      protected void DisposeViewModel(VMInfo vmInfo)
      {
         vmInfo.Instance.RequestPushUpdates -= VmInstance_RequestPushUpdates;

         if (typeof(MulticastVM).GetTypeInfo().IsAssignableFrom(vmInfo.Instance.GetType()))
         {
            var multicastVM = vmInfo.Instance as MulticastVM;
            multicastVM.RequestMulticastPushUpdates -= VMInstance_RequestMulticastPushUpdates;
            multicastVM.RequestSend -= VMInstance_RequestSend;

            // If the multicast view model has a group, call the hub to disassociate the connection Id with that group.
            if (!string.IsNullOrEmpty(vmInfo.GroupName))
               RemoveConnectionFromGroup(vmInfo.ConnectionId, vmInfo.Id, vmInfo.GroupName);
         }
         vmInfo.Instance.Dispose();
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
      /// Push property changed updates on a view model back to the client.
      /// </summary>
      /// <param name="vmInfo">View model.</param>
      protected virtual void PushUpdates(VMInfo vmInfo)
      {
         var vmInstance = vmInfo.Instance;
         if (vmInstance is MulticastVM)
            vmInstance.PushUpdates();
         else
         {
            lock (vmInstance)
            {
               var changedProperties = vmInstance.AcceptChangedProperties();
               if (changedProperties != null && changedProperties.Count > 0)
                  PushUpdates(vmInfo, vmInstance.Serialize(changedProperties));
            }
         }
      }

      /// <summary>
      /// Push property changed updates on a view model back to the client.
      /// </summary>
      /// <param name="vmInfo">View model.</param>
      /// <param name="vmData">Serialized data to be pushed. If null, it's coming from the view model.</param>
      protected virtual void PushUpdates(VMInfo vmInfo, string vmData)
      {
         if (string.IsNullOrEmpty(vmData))
            return;

         ResponseVMFilter.Invoke(vmInfo, vmData, filteredData =>
         {
            var vmDataToSend = filteredData is GroupSend ? (filteredData as GroupSend).Data : (string) filteredData;
            VMResponse(vmInfo.ConnectionId, vmInfo.Id, vmDataToSend);
            return Task.CompletedTask;
         });
      }

      /// <summary>
      /// Disassociates a connection from a group.
      /// </summary>
      /// <param name="connectionId">Identifies the connection.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="groupName">Group name.</param>
      protected void RemoveConnectionFromGroup(string connectionId, string vmId, string groupName)
      {
         var message = new GroupRemove
         {
            GroupName = groupName,
            ConnectionId = connectionId
         };
         VMResponse(MULTICAST + nameof(GroupRemove), vmId, JsonSerializer.Serialize(message));
      }

      /// <summary>
      /// Sends a view model data to one or more clients.
      /// </summary>
      /// <param name="vmInfo">View model info.</param>
      /// <param name="args">Arguments containing information on clients to send to.</param>
      private void Send(VMInfo vmInfo, GroupSend args)
      {
         ResponseVMFilter.Invoke(vmInfo, args, filteredData =>
         {
            VMResponse(MULTICAST + nameof(GroupSend), vmInfo.Id, JsonSerializer.Serialize(args));
            return Task.CompletedTask;
         });
      }

      /// <summary>
      /// Handles send request from a multicast view model.
      /// </summary>
      private void VMInstance_RequestSend(object sender, SendEventArgs e)
      {
         var vmInfo = _activeVMs.FirstOrDefault(kvp => kvp.Value.Instance == sender).Value;
         if (vmInfo == null)
            return;

         var vmInstance = vmInfo.Instance;
         Send(vmInfo, new GroupSend
         {
            GroupName = e.GroupName,
            ConnectionIds = e.ConnectionIds,
            ExcludedConnectionIds = e.ExcludedConnectionIds,
            UserIds = e.UserIds,
            Data = vmInstance.Serialize(e.Properties)
         });
         e.SendData = false;
      }

      /// <summary>
      /// Handles multicast push updates request from a multicast view model.
      /// </summary>
      private void VMInstance_RequestMulticastPushUpdates(object sender, MulticastPushUpdatesEventArgs e)
      {
         var vmInfo = _activeVMs.FirstOrDefault(kvp => kvp.Value.Instance == sender).Value;
         if (vmInfo == null)
            return;

         if (e.PushData)
         {
            var vmInstance = vmInfo.Instance;
            lock (vmInstance)
            {
               var changedProperties = vmInstance.AcceptChangedProperties();
               if (changedProperties != null && changedProperties.Count > 0)
               {
                  Send(vmInfo, new GroupSend
                  {
                     GroupName = vmInfo.GroupName,
                     ConnectionIds = e.ConnectionIds,
                     ExcludedConnectionIds = new List<string> { e.ExcludedConnectionId },
                     Data = vmInstance.Serialize(changedProperties)
                  });
               }
               e.PushData = false;
            }
         }
         else if (e.ExcludedConnectionId != vmInfo.ConnectionId)
            e.ConnectionIds.Add(vmInfo.ConnectionId);
      }

      /// <summary>
      /// Handles push updates request from a view model.
      /// </summary>
      private void VmInstance_RequestPushUpdates(object sender, EventArgs e)
      {
         foreach (var vmInfo in _activeVMs.Values)
            PushUpdates(vmInfo);
      }
   }
}