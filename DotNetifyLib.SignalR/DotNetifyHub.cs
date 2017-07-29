/* 
Copyright 2016-2017 Dicky Suryadi

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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DotNetify.Security;

namespace DotNetify
{
   /// <summary>
   /// This class is a SignalR hub for communicating with browser clients.
   /// </summary>
   public class DotNetifyHub : Hub
   {
      private readonly IVMControllerFactory _vmControllerFactory;
      private readonly IPrincipalAccessor _principalAccessor;
      private readonly IList<Func<IMiddleware>> _middlewareFactories;
      private readonly IDictionary<Type, Func<IVMFilter>> _vmFilterFactories;
      private readonly IMemoryCache _headersCache;
      private readonly Func<string, string> _headersKey = (string connectionId) => JTOKEN_HEADERS + connectionId;

      private const string JTOKEN_VMARG = "$vmArg";
      private const string JTOKEN_HEADERS = "$headers";

      private IPrincipal _principal;

      /// <summary>
      /// View model controller associated with the current connection.
      /// </summary>
      private VMController VMController
      {
         get
         {
            if (_principalAccessor is HubPrincipalAccessor)
               (_principalAccessor as HubPrincipalAccessor).Principal = _principal ?? Context.User;

            var vmController = _vmControllerFactory.GetInstance(Context.ConnectionId);
            vmController.RequestingVM = RunRequestingVMFilters;
            vmController.UpdatingVM = RunUpdatingVMFilters;

            return vmController;
         }
      }

      /// <summary>
      /// Request headers of the current connection.
      /// </summary>
      private object Headers
      {
         get { return _headersCache.Get(_headersKey(Context.ConnectionId)); }
         set { _headersCache.Set(_headersKey(Context.ConnectionId), value); }
      }

      /// <summary>
      /// Constructor for dependency injection.
      /// </summary>
      /// <param name="vmControllerFactory">Factory of view model controllers.</param>
      /// <param name="principalAccessor">Allow to pass the hub principal.</param>
      /// <param name="middlewareFactories">Middlewares to intercept incoming view model requests and updates.</param>
      public DotNetifyHub(IVMControllerFactory vmControllerFactory, IPrincipalAccessor principalAccessor, IMemoryCache headersCache,
            IList<Func<IMiddleware>> middlewareFactories, IDictionary<Type, Func<IVMFilter>> vmFilterFactories)
      {
         _vmControllerFactory = vmControllerFactory;
         _vmControllerFactory.ResponseDelegate = SendResponse;

         _principalAccessor = principalAccessor;
         _middlewareFactories = middlewareFactories;
         _vmFilterFactories = vmFilterFactories;
         _headersCache = headersCache;
      }

      /// <summary>
      /// Handles when a client gets disconnected.
      /// </summary>
      /// <param name="stopCalled">True, if stop was called on the client closing the connection gracefully;
      /// false, if the connection has been lost for longer than the timeout.</param>
      /// <returns></returns>
      public override Task OnDisconnected(bool stopCalled)
      {
         // Remove the controller on disconnection.
         _vmControllerFactory.Remove(Context.ConnectionId);
         _headersCache.Remove(_headersKey(Context.ConnectionId));
         return base.OnDisconnected(stopCalled);
      }

      /// <summary>
      /// This method is called by the VMManager to send response back to browser clients.
      /// </summary>
      /// <param name="connectionId">Identifies the browser client making prior request.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data in serialized JSON.</param>
      public void SendResponse(string connectionId, string vmId, string vmData)
      {
         try
         {
            vmData = RunMiddlewares(nameof(Response_VM), vmId, vmData, false);
            Response_VM(connectionId, vmId, vmData);
         }
         catch (OperationCanceledException ex)
         {
            Trace.WriteLine(ex.Message);
         }
         catch (Exception ex)
         {
            Trace.Fail(ex.ToString());
         }
      }

      #region Client Requests

      /// <summary>
      /// This method is called by browser clients to request view model data.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmArg">Optional argument that may contain view model's initialization argument and/or request headers.</param>
      public void Request_VM(string vmId, object vmArg)
      {
         try
         {
            vmArg = ExtractHeaders(vmArg);
            vmArg = RunMiddlewares(nameof(Request_VM), vmId, vmArg);

            Trace.WriteLine($"[dotNetify] Request_VM: {vmId} {Context.ConnectionId}");
            VMController.OnRequestVM(Context.ConnectionId, vmId, vmArg);
         }
         catch (OperationCanceledException ex)
         {
            Trace.WriteLine(ex.Message);
         }
         catch (UnauthorizedAccessException ex)
         {
            Response_VM(Context.ConnectionId, vmId, SerializeException(ex));
         }
         catch (Exception ex)
         {
            Trace.Fail(ex.ToString());
         }
      }

      /// <summary>
      /// This method is called by browser clients to update a view model's value.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model update data, where key is the property path and value is the property's new value.</param>
      public void Update_VM(string vmId, Dictionary<string, object> vmData)
      {
         try
         {
            // If this method is called only to refresh the connection request headers, leave as soon as the headers were extracted.
            if ((vmData = ExtractHeaders(vmData)).Count == 0)
               return;

            vmData = RunMiddlewares(nameof(Update_VM), vmId, vmData) as Dictionary<string, object>;

            Trace.WriteLine($"[dotNetify] Update_VM: {vmId} {Context.ConnectionId} {JsonConvert.SerializeObject(vmData)}");
            VMController.OnUpdateVM(Context.ConnectionId, vmId, vmData);
         }
         catch (OperationCanceledException ex)
         {
            Trace.WriteLine(ex.Message);
         }
         catch (UnauthorizedAccessException ex)
         {
            Response_VM(Context.ConnectionId, vmId, SerializeException(ex));
         }
         catch (Exception ex)
         {
            Trace.Fail(ex.ToString());
         }
      }

      /// <summary>
      /// This method is called by browser clients to remove its view model as it's no longer used.
      /// </summary>
      /// <param name="vmId">Identifies the view model.  By convention, this should match a view model class name.</param>
      public void Dispose_VM(string vmId)
      {
         try
         {
            VMController.OnDisposeVM(Context.ConnectionId, vmId);
         }
         catch (Exception ex)
         {
            Trace.Fail(ex.ToString());
         }
      }

      #endregion

      #region Server Responses

      /// <summary>
      /// This method is called internally to send response back to browser clients.
      /// </summary>
      /// <param name="connectionId">Identifies the browser client making prior request.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data in serialized JSON.</param>
      internal void Response_VM(string connectionId, string vmId, string vmData)
      {
         Trace.WriteLine($"[dotNetify] Response_VM: {vmId} {connectionId} {vmData}");
         if (_vmControllerFactory.GetInstance(connectionId) != null) // Touch the factory to push the timeout.
            Clients.Client(connectionId).Response_VM(vmId, vmData);
      }

      #endregion

      /// <summary>
      /// Extract headers from the given argument.
      /// </summary>
      /// <param name="data">Data that comes from Request_VM or Update_VM.</param>
      /// <returns>The input argument sans headers.</returns>
      private T ExtractHeaders<T>(T data) where T : class
      {
         if (typeof(T) == typeof(Dictionary<string, object>))
         {
            var vmData = data as Dictionary<string, object>;
            if (vmData.ContainsKey(JTOKEN_HEADERS))
            {
               Headers = vmData[JTOKEN_HEADERS];
               vmData.Remove(JTOKEN_HEADERS);
            }
            return vmData as T;
         }
         else
         {
            JObject arg = data as JObject;
            if (arg.Property(JTOKEN_HEADERS) != null)
               Headers = arg[JTOKEN_HEADERS];
            if (arg.Property(JTOKEN_VMARG) != null)
               data = arg[JTOKEN_VMARG] as T;
            return data;
         }
      }

      /// <summary>
      /// Run the middlewares on the data.
      /// </summary>
      /// <param name="callType">Call type: Request_VM, Update_VM or Response_VM.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="data">Call data.</param>
      /// <param name="exceptionSent">Whether the exception should be sent to the client.</param>
      /// <returns>Hub context data.</returns>
      private T RunMiddlewares<T>(string callType, string vmId, T data, bool exceptionSent = true) where T : class
      {
         try
         {
            var hubContext = new DotNetifyHubContext(Context, callType, vmId, data, Headers, _principal);
            _middlewareFactories?.ToList().ForEach(factory => factory().Invoke(hubContext));
            _principal = hubContext.Principal;
            return hubContext.Data as T;
         }
         catch (Exception ex)
         {
            if (exceptionSent)
               Response_VM(Context.ConnectionId, vmId, SerializeException(ex));
            throw new OperationCanceledException($"Middleware threw {ex.GetType().Name}: {ex.Message}", ex);
         }
      }

      /// <summary>
      /// Runs the view model filter.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vm">View model instance.</param>
      /// <param name="vmArg">Optional view model argument.</param>
      private void RunRequestingVMFilters(string vmId, BaseVM vm, ref object vmArg) => RunVMFilters(nameof(Request_VM), vmId, vm, ref vmArg);

      /// <summary>
      /// Runs the view model filter.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vm">View model instance.</param>
      /// <param name="data">Update data for the view model.</param>
      private void RunUpdatingVMFilters(string vmId, BaseVM vm, ref Dictionary<string, object> data) => RunVMFilters(nameof(Update_VM), vmId, vm, ref data);

      /// <summary>
      /// Runs the view model filter.
      /// </summary>
      /// <param name="callType">Call type: Request_VM, Update_VM or Response_VM.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vm">View model instance.</param>
      /// <param name="data">Call data.</param>
      private void RunVMFilters<T>(string callType, string vmId, BaseVM vm, ref T data) where T : class
      {
         try
         {
            var vmContext = new VMContext(new DotNetifyHubContext(Context, callType, vmId, data, Headers, _principal), vm);
            foreach (var attr in vm.GetType().GetTypeInfo().GetCustomAttributes())
            {
               var vmFilterType = typeof(IVMFilter<>).MakeGenericType(attr.GetType());
               if (_vmFilterFactories.Keys.Any(t => vmFilterType.IsAssignableFrom(t)))
               {
                  var vmFilter = _vmFilterFactories.FirstOrDefault(kvp => vmFilterType.IsAssignableFrom(kvp.Key)).Value();
                  vmFilterType.GetMethod("Invoke")?.Invoke(vmFilter, new object[] { attr, vmContext });
                  data = vmContext.HubContext.Data as T;
               }
            }
         }
         catch (Exception ex)
         {
            Response_VM(Context.ConnectionId, vmId, SerializeException(ex.InnerException));
            throw new OperationCanceledException($"Filter threw {ex.GetType().Name}: {ex.Message}", ex);
         }
      }

      /// <summary>
      /// Serializes an exception.
      /// </summary>
      /// <param name="ex">Exception to serialize.</param>
      /// <returns>Serialized exception.</returns>
      private string SerializeException(Exception ex) => JsonConvert.SerializeObject(new { ExceptionType = ex.GetType().Name, Message = ex.Message });
   }
}