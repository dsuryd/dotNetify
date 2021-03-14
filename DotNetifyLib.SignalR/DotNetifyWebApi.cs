/*
Copyright 2019-2020 Dicky Suryadi

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
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DotNetify.Security;
using DotNetify.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetify.WebApi
{
   /// <summary>
   /// Provides a separate view model controller factory for web API so that each request has its own factory to
   /// set its own response callback but still share in-memory cache for the view models.
   /// </summary>
   public class WebApiVMControllerFactory : VMControllerFactory
   {
      private readonly static IMemoryCache _cache = new MemoryCacheAdapter(new MemoryCache(new MemoryCacheOptions()));

      public WebApiVMControllerFactory(IVMFactory vmFactory, IVMServiceScopeFactory serviceScopeFactory) :
         base(_cache, vmFactory, serviceScopeFactory)
      {
         CacheExpiration = TimeSpan.FromMinutes(20);
      }
   }

   /// <summary>
   /// This class allows access to view models through a web API.
   /// </summary>
   [Route("api/dotnetify/vm")]
   [ApiController]
   public class DotNetifyWebApi : ControllerBase
   {
      private readonly TaskCompletionSource<string> _taskCompletionSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
      private readonly List<string> _responses = new List<string>();

      /// <summary>
      /// Adapter for the HTTP context.
      /// </summary>
      public class HttpCallerContext : HubCallerContext
      {
         private readonly HttpContext _httpContext;

         public override string ConnectionId => _httpContext.Connection.Id;
         public override string UserIdentifier => _httpContext.User?.Identity?.Name;
         public override ClaimsPrincipal User => _httpContext.User;
         public override IDictionary<object, object> Items => _httpContext.Items;
         public override IFeatureCollection Features => _httpContext.Features;
         public override CancellationToken ConnectionAborted => _httpContext.RequestAborted;

         public override void Abort() => _httpContext.Abort();

         public HttpCallerContext(HttpContext httpContext)
         {
            _httpContext = httpContext;
         }
      }

      /// <summary>
      /// This method is called by browser clients to request view model data.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmArg">Optional argument that may contain view model's initialization argument and/or request headers.</param>
      /// <param name="vmControllerFactory">Factory of view model controllers.</param>
      /// <param name="serviceProvider">Allows to provide scoped service provider for the view models.</param>
      /// <param name="principalAccessor">Allows to pass the hub principal.</param>
      /// <param name="hubPipeline">Manages middlewares and view model filters.</param>
      /// <param name="hubResponseFactory">Factory of objects to send responses back to hub clients.</param>
      /// <returns>View model state.</returns>
      [HttpGet("{vmId}")]
      public async Task<string> Request_VM(
         string vmId,
         [FromQuery] string vmArg,
         [FromServices] WebApiVMControllerFactory vmControllerFactory,
         [FromServices] IHubServiceProvider hubServiceProvider,
         [FromServices] IPrincipalAccessor principalAccessor,
         [FromServices] IHubPipeline hubPipeline,
         [FromServices] IDotNetifyHubResponseManager hubResponseManager

         )
      {
         var hub = CreateHubHandler(vmControllerFactory, hubServiceProvider, principalAccessor, hubPipeline, hubResponseManager, nameof(IDotNetifyHubMethod.Request_VM), vmId, vmArg);

         try
         {
            await hub.RequestVMAsync(vmId, vmArg);
            _taskCompletionSource.TrySetResult(_responses.LastOrDefault());
            _responses.Clear();
         }
         catch (Exception ex)
         {
            _taskCompletionSource.TrySetResult(ex.Serialize());
         }

         return await _taskCompletionSource.Task;
      }

      /// <summary>
      /// This method is called by browser clients to update view model's value.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model update data, where key is the property path and value is the property's new value.</param>
      /// <param name="vmControllerFactory">Factory of view model controllers.</param>
      /// <param name="serviceProvider">Allows to provide scoped service provider for the view models.</param>
      /// <param name="principalAccessor">Allows to pass the hub principal.</param>
      /// <param name="hubPipeline">Manages middlewares and view model filters.</param>
      /// <param name="hubResponseFactory">Factory of objects to send responses back to hub clients.</param>
      /// <returns>View model state.</returns>
      [HttpPost("{vmId}")]
      public async Task<string> Update_VM(
         string vmId,
         [FromBody] Dictionary<string, object> vmData,
         [FromServices] WebApiVMControllerFactory vmControllerFactory,
         [FromServices] IHubServiceProvider hubServiceProvider,
         [FromServices] IPrincipalAccessor principalAccessor,
         [FromServices] IHubPipeline hubPipeline,
         [FromServices] IDotNetifyHubResponseManager hubResponseManager
         )
      {
         var hub = CreateHubHandler(vmControllerFactory, hubServiceProvider, principalAccessor, hubPipeline, hubResponseManager, nameof(IDotNetifyHubMethod.Update_VM), vmId, vmData);

         try
         {
            await hub.UpdateVMAsync(vmId, vmData);
            _taskCompletionSource.TrySetResult(_responses.LastOrDefault());
            _responses.Clear();
         }
         catch (Exception ex)
         {
            _taskCompletionSource.TrySetResult(ex.Serialize());
         }

         return await _taskCompletionSource.Task;
      }

      /// <summary>
      /// This method is called by browser clients to dispose a view model.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model update data, where key is the property path and value is the property's new value.</param>
      /// <param name="vmControllerFactory">Factory of view model controllers.</param>
      /// <param name="serviceProvider">Allows to provide scoped service provider for the view models.</param>
      /// <param name="principalAccessor">Allows to pass the hub principal.</param>
      /// <param name="hubPipeline">Manages middlewares and view model filters.</param>
      /// <param name="hubResponseFactory">Factory of objects to send responses back to hub clients.</param>
      [HttpDelete("{vmId}")]
      public async Task Dispose_VM(
         string vmId,
         [FromServices] WebApiVMControllerFactory vmControllerFactory,
         [FromServices] IHubServiceProvider hubServiceProvider,
         [FromServices] IPrincipalAccessor principalAccessor,
         [FromServices] IHubPipeline hubPipeline,
         [FromServices] IDotNetifyHubResponseManager hubResponseManager
         )
      {
         var hub = CreateHubHandler(vmControllerFactory, hubServiceProvider, principalAccessor, hubPipeline, hubResponseManager, nameof(IDotNetifyHubMethod.Dispose_VM), vmId);
         await hub.DisposeVMAsync(vmId);
      }

      /// <summary>
      /// Creates a dotNetify hub that uses HTTP context for its hub context, and sets the response callback to a local function.
      /// </summary>
      /// <param name="vmControllerFactory">Factory of view model controllers.</param>
      /// <param name="serviceProvider">Allows to provide scoped service provider for the view models.</param>
      /// <param name="principalAccessor">Allows to pass the hub principal.</param>
      /// <param name="hubPipeline">Manages middlewares and view model filters.</param>
      /// <param name="hubResponseFactory">Factory of objects to send responses back to hub clients.</param>
      /// <param name="callType">Hub call type.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="data">View model data.</param>
      /// <returns>Hub instance.</returns>
      private IDotNetifyHubHandler CreateHubHandler(
         IVMControllerFactory vmControllerFactory,
         IHubServiceProvider hubServiceProvider,
         IPrincipalAccessor principalAccessor,
         IHubPipeline hubPipeline,
         IDotNetifyHubResponseManager hubResponseManager,
         string callType,
         string vmId,
         object data = null)
      {
         var httpCallerContext = new HttpCallerContext(HttpContext);

         if (principalAccessor is HubInfoAccessor)
         {
            var hubPrincipalAccessor = principalAccessor as HubInfoAccessor;
            hubPrincipalAccessor.Principal = HttpContext?.User;
            hubPrincipalAccessor.Context = new DotNetifyHubContext(httpCallerContext, callType, vmId, data, null, hubPrincipalAccessor.Principal);
         }

         return new DotNetifyHubHandler(vmControllerFactory, hubServiceProvider, principalAccessor, hubPipeline, hubResponseManager)
         {
            CallerContext = httpCallerContext,
            OnVMResponse = ResponseVMCallback
         };
      }

      /// <summary>
      /// Response delegate to pass to the VMControllerFactory instance.
      /// </summary>
      /// <param name="connectionId">Identifies the connection.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="data">Response data.</param>
      private Task ResponseVMCallback(string connectionId, string vmId, string data)
      {
         _responses.Add(data);
         return Task.CompletedTask;
      }
   }
}