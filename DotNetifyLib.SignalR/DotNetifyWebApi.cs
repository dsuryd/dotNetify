/*
Copyright 2019 Dicky Suryadi

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
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DotNetify.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;

namespace DotNetify.WebApi
{
   /// <summary>
   /// This class allows access to view models through a web API.
   /// </summary>
   [Route("api/dotnetify/vm")]
   [ApiController]
   public class DotNetifyWebApi : ControllerBase
   {
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
      /// <param name="vmFactory">Factory object to create the view model instance.</param>
      /// <param name="hubPipeline">Middleware/VM filter pipeline.</param>
      /// <returns>View model state.</returns>
      [HttpGet("{vmId}")]
      public async Task<string> Request_VM(
         string vmId,
         [FromQuery] string vmArg,
         [FromServices] IVMFactory vmFactory,
         [FromServices] IHubServiceProvider hubServiceProvider,
         [FromServices] IVMServiceScopeFactory serviceScopeFactory,
         [FromServices] IHubPipeline hubPipeline,
         [FromServices] IPrincipalAccessor principalAccessor
         )
      {
         var taskCompletionSource = new TaskCompletionSource<string>();
         var vmController = new VMController((arg1, arg2, arg3) => taskCompletionSource.TrySetResult(arg3), vmFactory, serviceScopeFactory.CreateScope())
         {
            ResponseVMFilter = CreateRespondingVMFilter(hubPipeline, vmId, vmArg)
         };

         var httpCallerContext = InitializeContext(vmController, hubServiceProvider, principalAccessor);
         var connectionId = httpCallerContext.ConnectionId;

         try
         {
            var hubContext = new DotNetifyHubContext(httpCallerContext, nameof(Request_VM), vmId, vmArg, BuildHeaders(), httpCallerContext.User);
            vmController.RequestVMFilter = CreateVMFilter(hubContext, hubPipeline);

            hubPipeline.RunMiddlewares(hubContext, ctx =>
            {
               vmController.OnRequestVM(connectionId, ctx.VMId, ctx.Data);
               vmController.Dispose();
               return Task.CompletedTask;
            });
         }
         catch (Exception ex)
         {
            var finalEx = hubPipeline.RunExceptionMiddleware(httpCallerContext, ex);
            if (finalEx is OperationCanceledException == false)
               taskCompletionSource.TrySetResult(DotNetifyHub.SerializeException(finalEx));
         }

         return await taskCompletionSource.Task;
      }

      /// <summary>
      /// This method is called by browser clients to update view model's value.
      /// </summary>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model update data, where key is the property path and value is the property's new value.</param>
      /// <param name="vmFactory">Factory object to create the view model instance.</param>
      /// <param name="hubPipeline">Middleware/VM filter pipeline.</param>
      /// <returns>View model state.</returns>
      [HttpPost("{vmId}")]
      public async Task<string> Update_VM(
         string vmId,
         [FromQuery] string vmArg,
         [FromBody] Dictionary<string, object> vmData,
         [FromServices] IVMFactory vmFactory,
         [FromServices] IHubServiceProvider hubServiceProvider,
         [FromServices] IVMServiceScopeFactory serviceScopeFactory,
         [FromServices] IHubPipeline hubPipeline,
         [FromServices] IPrincipalAccessor principalAccessor)
      {
         var taskCompletionSource1 = new TaskCompletionSource<string>();
         var taskCompletionSource2 = new TaskCompletionSource<string>();

         var vmController = new VMController((arg1, arg2, arg3) =>
         {
            if (!taskCompletionSource1.TrySetResult(arg3))
               taskCompletionSource2.TrySetResult(arg3);
         }, vmFactory, serviceScopeFactory.CreateScope())
         {
            ResponseVMFilter = CreateRespondingVMFilter(hubPipeline, vmId, vmData)
         };

         var httpCallerContext = InitializeContext(vmController, hubServiceProvider, principalAccessor);
         var connectionId = httpCallerContext.ConnectionId;

         try
         {
            var hubContext = new DotNetifyHubContext(httpCallerContext, nameof(Request_VM), vmId, vmArg, BuildHeaders(), httpCallerContext.User);
            vmController.RequestVMFilter = CreateVMFilter(hubContext, hubPipeline);

            hubPipeline.RunMiddlewares(hubContext, ctx =>
            {
               vmController.OnRequestVM(connectionId, ctx.VMId, ctx.Data);
               return Task.CompletedTask;
            });
         }
         catch (Exception ex)
         {
            var finalEx = hubPipeline.RunExceptionMiddleware(httpCallerContext, ex);
            if (finalEx is OperationCanceledException == false)
               taskCompletionSource1.TrySetResult(DotNetifyHub.SerializeException(finalEx));
         }

         await taskCompletionSource1.Task;

         try
         {
            var hubContext = new DotNetifyHubContext(httpCallerContext, nameof(Update_VM), vmId, vmData, BuildHeaders(), httpCallerContext.User);
            vmController.UpdateVMFilter = CreateVMFilter(hubContext, hubPipeline);

            hubPipeline.RunMiddlewares(hubContext, ctx =>
            {
               vmController.OnUpdateVM(connectionId, ctx.VMId, ctx.Data as Dictionary<string, object>);
               vmController.Dispose();
               return Task.CompletedTask;
            });
         }
         catch (Exception ex)
         {
            var finalEx = hubPipeline.RunExceptionMiddleware(httpCallerContext, ex);
            if (finalEx is OperationCanceledException == false)
               taskCompletionSource2.TrySetResult(DotNetifyHub.SerializeException(finalEx));
         }

         return await taskCompletionSource2.Task;
      }

      /// <summary>
      /// Adapt HTTP request headers into the headers that dotNetify can consume.
      /// </summary>
      /// <returns>Headers object.</returns>
      private JObject BuildHeaders()
      {
         var headers = new JObject();
         foreach (var kvp in HttpContext.Request.Headers.Where(x => !string.IsNullOrEmpty(x.Value)))
            headers.Add(kvp.Key, kvp.Value.ToString());
         return headers;
      }

      /// <summary>
      /// Creates view model filter delegate that runs before it is requested/updated.
      /// </summary>
      /// <param name="hubContext">DotNetify hub context.</param>
      /// <param name="hubPipeline">Middleware/VM filter pipeline.</param>
      /// <returns>View model filter delegate.</returns>
      private VMController.FilterDelegate CreateVMFilter(DotNetifyHubContext hubContext, IHubPipeline hubPipeline)
      {
         return (vmId, vm, data, vmAction) =>
         {
            try
            {
               hubContext.Data = data;
               hubPipeline.RunVMFilters(hubContext, vm, ctx =>
               {
                  vmAction(ctx.HubContext.Data);
                  return Task.CompletedTask;
               });
            }
            catch (TargetInvocationException ex)
            {
               throw ex.InnerException;
            }
         };
      }

      /// <summary>
      /// Creates view model filter delegate that runs before it responds to something.
      /// </summary>
      /// <param name="hubContext">DotNetify hub context.</param>
      /// <param name="hubPipeline">Middleware/VM filter pipeline.</param>
      /// <param name="vmData">View model data from the request.</param>
      /// <returns>View model filter delegate.</returns>
      private VMController.FilterDelegate CreateRespondingVMFilter(IHubPipeline hubPipeline, string vmId, object vmData)
      {
         return (_vmId, vm, data, vmAction) =>
         {
            var hubContext = new DotNetifyHubContext(new HttpCallerContext(HttpContext), nameof(DotNetifyHub.Response_VM), vmId, vmData, BuildHeaders(), HttpContext?.User);
            hubPipeline.RunMiddlewares(hubContext, ctx =>
            {
               CreateVMFilter(hubContext, hubPipeline)(_vmId, vm, data, vmAction);
               return Task.CompletedTask;
            });
         };
      }

      /// <summary>
      /// Initializes the scoped context of the request.
      /// </summary>
      /// <param name="vmController">View model controller.</param>
      /// <param name="hubServiceProvider">Service provideer context.</param>
      /// <param name="principalAccessor">Principal user context.</param>
      /// <returns>HTTP caller context.</returns>
      private HttpCallerContext InitializeContext(VMController vmController, IHubServiceProvider hubServiceProvider, IPrincipalAccessor principalAccessor)
      {
         if (hubServiceProvider is HubServiceProvider)
            (hubServiceProvider as HubServiceProvider).ServiceProvider = vmController.ServiceProvider;

         var httpCallerContext = new HttpCallerContext(HttpContext);
         if (principalAccessor is HubPrincipalAccessor)
         {
            var hubPrincipalAccessor = principalAccessor as HubPrincipalAccessor;
            hubPrincipalAccessor.Principal = HttpContext?.User;
            hubPrincipalAccessor.CallerContext = httpCallerContext;
         }
         return httpCallerContext;
      }
   }
}