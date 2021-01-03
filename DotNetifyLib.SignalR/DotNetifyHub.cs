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
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetify.Util;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;

namespace DotNetify
{
   /// <summary>
   /// Interface for enabling compile-time check for public hub methods. Not meant to be implemented!
   /// </summary>
   public interface IDotNetifyHubMethod
   {
      void Request_VM();

      void Update_VM();

      void Dispose_VM();

      void Response_VM();

      void Invoke();
   }

   /// <summary>
   /// This class is a SignalR hub for communicating with browser clients.
   /// </summary>
   public class DotNetifyHub : Hub
   {
      private readonly IDotNetifyHubHandler _hubHandler;
      private readonly IHubPipeline _hubPipeline;

      /// <summary>
      /// Handles hub method invocation.
      /// </summary>
      protected internal IDotNetifyHubHandler HubHandler
      {
         get
         {
            _hubHandler.CallerContext = Context;
            return _hubHandler;
         }
      }

      /// <summary>
      /// Constructor for dependency injection.
      /// </summary>
      /// <param name="hubHandler">Handles hub method invocation.</param>
      /// <param name="hubPipeline">Middleware pipeline.</param>
      public DotNetifyHub(IDotNetifyHubHandler hubHandler, IHubPipeline hubPipeline)
      {
         _hubHandler = hubHandler;
         _hubPipeline = hubPipeline;
      }

      /// <summary>
      /// Handles when a client gets disconnected.
      /// </summary>
      public override async Task OnDisconnectedAsync(Exception exception)
      {
         await HubHandler.OnDisconnectedAsync(exception);
         await base.OnDisconnectedAsync(exception);
      }

      /// <summary>
      /// This method is called by browser clients to request view model data.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmArg">Optional argument that may contain view model's initialization argument and/or request headers.</param>
      [HubMethodName(nameof(IDotNetifyHubMethod.Request_VM))]
      public async Task RequestVMAsync(string vmId, object vmArg)
      {
         await HubHandler.RequestVMAsync(vmId, vmArg);
      }

      /// <summary>
      /// This method is called by browser clients to update a view model's value.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model update data, where key is the property path and value is the property's new value.</param>
      [HubMethodName(nameof(IDotNetifyHubMethod.Update_VM))]
      public async Task UpdateVMAsync(string vmId, Dictionary<string, object> vmData)
      {
         await HubHandler.UpdateVMAsync(vmId, vmData);
      }

      /// <summary>
      /// This method is called by browser clients to remove its view model as it's no longer used.
      /// </summary>
      /// <param name="vmId">Identifies the view model.  By convention, this should match a view model class name.</param>
      [HubMethodName(nameof(IDotNetifyHubMethod.Dispose_VM))]
      public async Task DisposeVMAsyc(string vmId)
      {
         await HubHandler.DisposeVMAsync(vmId);
      }

      /// <summary>
      /// This method is called by the hub server to forward messages to another server.
      /// </summary>
      /// <param name="methodName">Method name to invoke.</param>
      /// <param name="methodArgs">Method arguments.</param>
      /// <param name="metadata">Any metadata.</param>
      [HubMethodName(nameof(IDotNetifyHubMethod.Invoke))]
      public async Task InvokeAsync(string methodName, object[] methodArgs, IDictionary<string, object> metadata)
      {
         Context.Items.Clear();
         if (metadata != null)
         {
            foreach (var kvp in metadata)
               Context.Items[kvp.Key] = kvp.Value;
         }

         switch (methodName)
         {
            case nameof(IDotNetifyHubMethod.Request_VM):
               methodName = nameof(RequestVMAsync);
               break;

            case nameof(IDotNetifyHubMethod.Update_VM):
               methodName = nameof(UpdateVMAsync);
               break;

            case nameof(IDotNetifyHubMethod.Dispose_VM):
               methodName = nameof(DisposeVMAsyc);
               break;

            case nameof(IDotNetifyHubMethod.Response_VM):
               methodName = nameof(OnHubForwardResponseAsync);
               break;
         }

         var methodInfo = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
         if (methodInfo != null)
         {
            var methodParams = methodInfo.GetParameters();
            for (int i = 0; i < methodArgs.Length; i++)
            {
               if (methodArgs[i] is JsonElement || methodArgs[i] is JObject)
                  methodArgs[i] = methodArgs[i].ToString().ConvertFromString(methodParams[i].ParameterType);
            }

            await methodInfo.InvokeAsync(this, methodArgs);
         }
         else
         {
            // Unknown method.  Run it through the middleware pipeline in case one wants to take action on it.
            var hubContext = new DotNetifyHubContext(Context, methodName, null, methodArgs, null, Context.User);
            await _hubPipeline.RunMiddlewaresAsync(hubContext, ctx => Task.CompletedTask);
         }
      }

      /// <summary>
      /// Handles responses back to the hub forwarder by running it through the middleware pipeline.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model response data.</param>
      private async Task OnHubForwardResponseAsync(string vmId, string vmData)
      {
         var hubContext = new DotNetifyHubContext(Context, nameof(IDotNetifyHubMethod.Response_VM), vmId, vmData, null, Context.User);
         await _hubPipeline.RunMiddlewaresAsync(hubContext, ctx => Task.CompletedTask);
      }

      #region Obsolete Methods

      [Obsolete]
      [HubMethodName("Request_VM_Obsolete")]
      public void Request_VM(string vmId, object vmArg) => _ = RequestVMAsync(vmId, vmArg);

      [Obsolete]
      [HubMethodName("Update_VM_Obsolete")]
      public void Update_VM(string vmId, Dictionary<string, object> vmData) => _ = UpdateVMAsync(vmId, vmData);

      [Obsolete]
      [HubMethodName("Dispose_VM_Obsolete")]
      public void Dispose_VM(string vmId) => _ = DisposeVMAsyc(vmId);

      #endregion Obsolete Methods
   }
}