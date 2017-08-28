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
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
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
      private readonly IHubPipeline _hubPipeline;
      private DotNetifyHubContext _hubContext;
      private IPrincipal _principal;

      /// <summary>
      /// Identity principal of the hub connection.
      /// </summary>
      private IPrincipal Principal
      {
         get { return _principal ?? Context.User; }
         set { _principal = value; }
      }

      /// <summary>
      /// View model controller associated with the current connection.
      /// </summary>
      private VMController VMController
      {
         get
         {
            if (_principalAccessor is HubPrincipalAccessor)
               (_principalAccessor as HubPrincipalAccessor).Principal = Principal;

            var vmController = _vmControllerFactory.GetInstance(Context.ConnectionId);
            vmController.RequestVMFilter = RunRequestingVMFilters;
            vmController.UpdateVMFilter = RunUpdatingVMFilters;
            vmController.ResponseVMFilter = RunRespondingVMFilters;

            return vmController;
         }
      }

      /// <summary>
      /// Default constructor.
      /// </summary>
      public DotNetifyHub() : this(
         ActivatorUtilities.ServiceProvider.GetService<IVMControllerFactory>(),
         ActivatorUtilities.ServiceProvider.GetService<IPrincipalAccessor>(),
         ActivatorUtilities.ServiceProvider.GetService<IHubPipeline>()
         )
      {
      }

      /// <summary>
      /// Constructor for dependency injection.
      /// </summary>
      /// <param name="vmControllerFactory">Factory of view model controllers.</param>
      /// <param name="principalAccessor">Allow to pass the hub principal.</param>
      /// <param name="hubPipeline">Manages middlewares and view model filters.</param>
      public DotNetifyHub(IVMControllerFactory vmControllerFactory, IPrincipalAccessor principalAccessor, IHubPipeline hubPipeline)
      {
         _vmControllerFactory = vmControllerFactory;
         _vmControllerFactory.ResponseDelegate = Response_VM;
         _principalAccessor = principalAccessor;
         _hubPipeline = hubPipeline;
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

         // Allow middlewares to hook to the event.
         _hubPipeline.RunDisconnectionMiddlewares(Context);

         return base.OnDisconnected(stopCalled);
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
            _hubContext = new DotNetifyHubContext(Context, nameof(Request_VM), vmId, vmArg, null, Principal);
            _hubPipeline.RunMiddlewares(_hubContext, ctx =>
            {
               Principal = ctx.Principal;
               VMController.OnRequestVM(Context.ConnectionId, ctx.VMId, ctx.Data);
               return Task.FromResult(0);
            });
         }
         catch (Exception ex)
         {
            var finalEx = _hubPipeline.RunExceptionMiddleware(Context, ex);
            if (finalEx is OperationCanceledException == false)
               Response_VM(Context.ConnectionId, vmId, SerializeException(finalEx));
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
            _hubContext = new DotNetifyHubContext(Context, nameof(Request_VM), vmId, vmData, null, Principal);
            _hubPipeline.RunMiddlewares(_hubContext, ctx =>
            {
               Principal = ctx.Principal;
               VMController.OnUpdateVM(ctx.CallerContext.ConnectionId, ctx.VMId, ctx.Data as Dictionary<string, object>);
               return Task.FromResult(0);
            });
         }
         catch (Exception ex)
         {
            var finalEx = _hubPipeline.RunExceptionMiddleware(Context, ex);
            if (finalEx is OperationCanceledException == false)
               Response_VM(Context.ConnectionId, vmId, SerializeException(finalEx));
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
            _hubPipeline.RunExceptionMiddleware(Context, ex);
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
         if (_vmControllerFactory.GetInstance(connectionId) != null) // Touch the factory to push the timeout.
            Clients.Client(connectionId).Response_VM(vmId, vmData);
      }

      #endregion

      /// <summary>
      /// Runs the view model filter.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vm">View model instance.</param>
      /// <param name="data">View model data.</param>
      /// <param name="vmArg">Optional view model argument.</param>
      private void RunVMFilters(BaseVM vm, object data, Action<object> vmAction)
      {
         try
         {
            _hubContext.Data = data;
            _hubPipeline.RunVMFilters(_hubContext, vm, ctx =>
            {
               vmAction(ctx.HubContext.Data);
               return Task.FromResult(0);
            });
         }
         catch (TargetInvocationException ex)
         {
            throw ex.InnerException;
         }
      }

      /// <summary>
      /// Runs the filter before the view model is requested.
      /// </summary>
      private void RunRequestingVMFilters(string vmId, BaseVM vm, object vmArg, Action<object> vmAction) => RunVMFilters(vm, vmArg, vmAction);

      /// <summary>
      /// Runs the filter before the view model is updated.
      /// </summary>
      private void RunUpdatingVMFilters(string vmId, BaseVM vm, object vmData, Action<object> vmAction) => RunVMFilters(vm, vmData, vmAction);

      /// <summary>
      /// Runs the filter before the view model respond to something.
      /// </summary>
      private void RunRespondingVMFilters(string vmId, BaseVM vm, object vmData, Action<object> vmAction)
      {
         try
         {
            _hubContext = new DotNetifyHubContext(Context, nameof(Response_VM), vmId, vmData, null, Principal);
            _hubPipeline.RunMiddlewares(_hubContext, ctx =>
            {
               Principal = ctx.Principal;
               RunVMFilters(vm, vmData, vmAction);
               return Task.FromResult(0);
            });
         }
         catch (Exception ex)
         {
            var finalEx = _hubPipeline.RunExceptionMiddleware(Context, ex);
            if (finalEx is OperationCanceledException == false)
               Response_VM(Context.ConnectionId, vmId, SerializeException(finalEx));
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