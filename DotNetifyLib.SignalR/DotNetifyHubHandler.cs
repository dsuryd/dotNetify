/*
Copyright 2016-2020 Dicky Suryadi

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
using System.Security.Principal;
using System.Threading.Tasks;
using DotNetify.Forwarding;
using DotNetify.Security;
using DotNetify.Util;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using static DotNetify.VMController;

namespace DotNetify
{
   /// <summary>
   /// Handles hub method calls.
   /// </summary>
   public interface IDotNetifyHubHandler
   {
      HubCallerContext CallerContext { set; }

      Task RequestVMAsync(string vmId, object vmArg);

      Task UpdateVMAsync(string vmId, Dictionary<string, object> vmData);

      Task DisposeVMAsync(string vmId);

      Task OnDisconnectedAsync(Exception exception);
   }

   /// <summary>
   /// This class handles hub method calls for requesting, updating and disposing view models.
   /// </summary>
   public class DotNetifyHubHandler : IDotNetifyHubHandler
   {
      private static readonly ConcurrentDictionary<string, IDictionary<object, object>> _callerContextItems = new ConcurrentDictionary<string, IDictionary<object, object>>();

      private readonly IVMControllerFactory _vmControllerFactory;
      private readonly IHubServiceProvider _serviceProvider;
      private readonly IPrincipalAccessor _principalAccessor;
      private readonly IDotNetifyHubResponse _hubResponse;
      private readonly IHubPipeline _hubPipeline;
      private readonly IDotNetifyHubForwardResponseFactory _hubForwardResponseFactory;
      private readonly IHubContext<DotNetifyHub> _globalHubContext;

      private DotNetifyHubContext _hubContext;
      private HubCallerContext _callerContext;
      private IPrincipal _principal;

      /// <summary>
      /// Hub caller context.
      /// </summary>
      public HubCallerContext CallerContext
      {
         get => _callerContext;
         set
         {
            _callerContext = value;

            // Cache caller context items so we can restore the one associated with the origin connection on sending response.
            var originContext = value.GetOriginConnectionContext();
            if (originContext != null && !_callerContextItems.ContainsKey(originContext.ConnectionId))
               _callerContextItems.TryAdd(originContext.ConnectionId, new Dictionary<object, object>(value.Items));
         }
      }

      /// <summary>
      /// Returns object to send responses to the client.
      /// </summary>
      private IDotNetifyHubResponse HubResponse
      {
         get => CallerContext.GetOriginConnectionContext() != null ? _hubForwardResponseFactory.GetInstance(CallerContext.ConnectionId) : _hubResponse;
      }

      internal VMResponseDelegate ResponseVM { get; set; }

      /// <summary>
      /// Identity principal of the hub connection.
      /// </summary>
      private IPrincipal Principal
      {
         get => _principal ?? CallerContext?.User;
         set => _principal = value;
      }

      /// <summary>
      /// Identifies the hub connection.
      /// </summary>
      private string ConnectionId => CallerContext.GetOriginConnectionContext()?.ConnectionId ?? CallerContext.ConnectionId;

      /// <summary>
      /// View model controller associated with the current connection.
      /// </summary>
      private VMController VMController
      {
         get
         {
            SetHubPrincipalAccessor();

            var vmController = _vmControllerFactory.GetInstance(ConnectionId);
            vmController.RequestVMFilter = RunRequestingVMFilters;
            vmController.UpdateVMFilter = RunUpdatingVMFilters;
            vmController.ResponseVMFilter = RunRespondingVMFilters;
            vmController.VMResponse = ResponseVM ?? ResponseVMAsync;

            if (_serviceProvider is HubServiceProvider)
               (_serviceProvider as HubServiceProvider).ServiceProvider = vmController.ServiceProvider;

            return vmController;
         }
      }

      /// <summary>
      /// Constructor for dependency injection.
      /// </summary>
      /// <param name="vmControllerFactory">Factory of view model controllers.</param>
      /// <param name="serviceProvider">Allows providing scoped service provider for the view models.</param>
      /// <param name="principalAccessor">Allows passing the hub principal.</param>
      /// <param name="hubPipeline">Manages middlewares and view model filters.</param>
      /// <param name="hubResponse">Sends responses back to hub clients.</param>
      /// <param name="hubForwardResponseFactory">Send responses back to hub forwarder.</param>
      public DotNetifyHubHandler(
            IVMControllerFactory vmControllerFactory,
            IHubServiceProvider serviceProvider,
            IPrincipalAccessor principalAccessor,
            IHubPipeline hubPipeline,
            IDotNetifyHubResponse hubResponse,
            IDotNetifyHubForwardResponseFactory hubForwardResponseFactory)
      {
         _vmControllerFactory = vmControllerFactory;
         _serviceProvider = serviceProvider;
         _principalAccessor = principalAccessor;
         _hubPipeline = hubPipeline;
         _hubForwardResponseFactory = hubForwardResponseFactory;
         _hubResponse = hubResponse;

         _vmControllerFactory.ResponseDelegate = ResponseVMAsync;
      }

      /// <summary>
      /// Handles when a client gets disconnected.
      /// </summary>
      public async Task OnDisconnectedAsync(Exception exception)
      {
         // Access VMController to set the ambient context.
         VMController _ = VMController;

         // Remove the controller on disconnection.
         _vmControllerFactory.Remove(ConnectionId);

         // Clean up origin context items.
         _callerContextItems.TryRemove(ConnectionId, out IDictionary<object, object> _);

         // Allow middlewares to hook to the event.
         await _hubPipeline.RunDisconnectionMiddlewaresAsync(CallerContext);
      }

      #region Client Requests

      /// <summary>
      /// This method is called by browser clients to request view model data.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmArg">Optional argument that may contain view model's initialization argument and/or request headers.</param>
      public async Task RequestVMAsync(string vmId, object vmArg)
      {
         object data = vmArg.NormalizeType();

         try
         {
            _hubContext = new DotNetifyHubContext(CallerContext, nameof(IDotNetifyHubMethod.Request_VM), vmId, data, null, Principal);

            await _hubPipeline.RunMiddlewaresAsync(_hubContext, async ctx =>
            {
               Principal = ctx.Principal;
               string groupName = await VMController.OnRequestVMAsync(ConnectionId, ctx.VMId, ctx.Data);

               // A multicast view model may be assigned to a SignalR group. If so, add the connection to the group.
               if (!string.IsNullOrEmpty(groupName))
                  await HubResponse.AddToGroupAsync(ConnectionId, groupName);
            });
         }
         catch (Exception ex)
         {
            var finalEx = await _hubPipeline.RunExceptionMiddlewareAsync(CallerContext, ex);
            if (finalEx is OperationCanceledException == false)
               await ResponseVMAsync(ConnectionId, vmId, finalEx.Serialize());
         }
      }

      /// <summary>
      /// This method is called by browser clients to update a view model's value.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model update data, where key is the property path and value is the property's new value.</param>
      public async Task UpdateVMAsync(string vmId, Dictionary<string, object> vmData)
      {
         var data = vmData?.ToDictionary(x => x.Key, x => x.Value.NormalizeType());

         try
         {
            _hubContext = new DotNetifyHubContext(CallerContext, nameof(IDotNetifyHubMethod.Update_VM), vmId, data, null, Principal);
            await _hubPipeline.RunMiddlewaresAsync(_hubContext, async ctx =>
            {
               Principal = ctx.Principal;
               await VMController.OnUpdateVMAsync(ctx.ConnectionId, ctx.VMId, ctx.Data as Dictionary<string, object>);
            });
         }
         catch (Exception ex)
         {
            var finalEx = await _hubPipeline.RunExceptionMiddlewareAsync(CallerContext, ex);
            if (finalEx is OperationCanceledException == false)
               await ResponseVMAsync(ConnectionId, vmId, finalEx.Serialize());
         }
      }

      /// <summary>
      /// This method is called by browser clients to remove its view model as it's no longer used.
      /// </summary>
      /// <param name="vmId">Identifies the view model.  By convention, this should match a view model class name.</param>
      public async Task DisposeVMAsync(string vmId)
      {
         try
         {
            _hubContext = new DotNetifyHubContext(CallerContext, nameof(IDotNetifyHubMethod.Dispose_VM), vmId, null, null, Principal);
            await _hubPipeline.RunMiddlewaresAsync(_hubContext, ctx =>
            {
               Principal = ctx.Principal;
               VMController.OnDisposeVM(ConnectionId, ctx.VMId);
               return Task.CompletedTask;
            });
         }
         catch (Exception ex)
         {
            await _hubPipeline.RunExceptionMiddlewareAsync(CallerContext, ex);
         }
      }

      #endregion Client Requests

      #region Server Responses

      /// <summary>
      /// This method is called internally to send response back to browser clients.
      /// This is also overloaded to handle SignalR groups for multicast view models.
      /// </summary>
      /// <param name="connectionId">Identifies the browser client making prior request.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data in serialized JSON.</param>
      internal Task ResponseVMAsync(string connectionId, string vmId, string vmData)
      {
         if (connectionId.StartsWith(VMController.MULTICAST))
         {
            var connectionIds = HandleMulticastMessage(HubResponse, connectionId, vmId, vmData);

            // Touch the factory to push the timeout.
            connectionIds?.ForEach(id => _vmControllerFactory.GetInstance(id));
         }
         else
         {
            if (_vmControllerFactory.GetInstance(connectionId) != null) // Touch the factory to push the timeout.
               HubResponse.SendAsync(connectionId, vmId, vmData);
         }
         return Task.CompletedTask;
      }

      /// <summary>
      /// Handles messages dealing with group multicasting.
      /// </summary>
      /// <param name="messageType">Message type.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="serializedMessage">Serialized message.</param>
      static internal List<string> HandleMulticastMessage(IDotNetifyHubResponse hubResponse, string messageType, string vmId, string serializedMessage)
      {
         if (messageType.EndsWith(nameof(VMController.GroupSend)))
         {
            var message = JsonConvert.DeserializeObject<VMController.GroupSend>(serializedMessage);

            if (!string.IsNullOrEmpty(message.GroupName))
            {
               if (message.ExcludedConnectionIds?.Count == 0)
                  hubResponse.SendToGroupAsync(message.GroupName, vmId, message.Data);
               else
               {
                  var excludedIds = new List<string>(message.ExcludedConnectionIds);
                  hubResponse.SendToGroupExceptAsync(message.GroupName, excludedIds, vmId, message.Data);
               }
            }
            else if (message.UserIds?.Count > 0)
            {
               var userIds = new List<string>(message.UserIds);
               hubResponse.SendToUsersAsync(userIds, vmId, message.Data);
            }
            else if (message.ConnectionIds?.Count > 0)
            {
               foreach (var connectionId in message.ConnectionIds)
                  hubResponse.SendAsync(connectionId, vmId, message.Data);
            }

            return message.ConnectionIds.ToList();
         }
         else if (messageType.EndsWith(nameof(VMController.GroupRemove)))
         {
            var message = JsonConvert.DeserializeObject<VMController.GroupRemove>(serializedMessage);
            hubResponse.RemoveFromGroupAsync(message.ConnectionId, message.GroupName);
         }

         return null;
      }

      #endregion Server Responses

      /// <summary>
      /// Runs the view model filter.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vm">View model instance.</param>
      /// <param name="data">View model data.</param>
      /// <param name="vmAction">Filter action.</param>
      private async Task RunVMFilters(BaseVM vm, object data, VMController.VMActionDelegate vmAction)
      {
         try
         {
            _hubContext.Data = data;
            await _hubPipeline.RunVMFiltersAsync(_hubContext, vm, async ctx =>
            {
               await vmAction(ctx.HubContext.Data);
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
      private Task RunRequestingVMFilters(string vmId, BaseVM vm, object vmArg, VMController.VMActionDelegate vmAction) => RunVMFilters(vm, vmArg, vmAction);

      /// <summary>
      /// Runs the filter before the view model is updated.
      /// </summary>
      private Task RunUpdatingVMFilters(string vmId, BaseVM vm, object vmData, VMController.VMActionDelegate vmAction) => RunVMFilters(vm, vmData, vmAction);

      /// <summary>
      /// Runs the filter before the view model respond to something.
      /// </summary>
      private async Task RunRespondingVMFilters(VMInfo vm, object vmData, VMController.VMActionDelegate vmAction)
      {
         try
         {
            // Restore the caller context items that are associated with the origin connection.
            if (_callerContextItems.TryGetValue(vm.ConnectionId, out IDictionary<object, object> items))
            {
               CallerContext.Items.Clear();
               foreach (var kvp in items)
                  CallerContext.Items[kvp.Key] = kvp.Value;
            }

            _hubContext = new DotNetifyHubContext(CallerContext, nameof(IDotNetifyHubMethod.Response_VM), vm.Id, vmData, null, Principal);
            await _hubPipeline.RunMiddlewaresAsync(_hubContext, async ctx =>
            {
               Principal = ctx.Principal;
               await RunVMFilters(vm.Instance, ctx.Data, vmAction);
            });
         }
         catch (Exception ex)
         {
            var finalEx = await _hubPipeline.RunExceptionMiddlewareAsync(CallerContext, ex);
            if (finalEx is OperationCanceledException == false && CallerContext != null)
               await ResponseVMAsync(ConnectionId, vm.Id, finalEx.Serialize());
         }
      }

      /// <summary>
      /// Sets the hub principal and connection context to the ambient accessor object.
      /// </summary>
      private void SetHubPrincipalAccessor()
      {
         if (_principalAccessor is HubInfoAccessor)
         {
            var hubPrincipalAccessor = _principalAccessor as HubInfoAccessor;
            hubPrincipalAccessor.Principal = Principal;
            hubPrincipalAccessor.Context = _hubContext;
         }
      }
   }
}