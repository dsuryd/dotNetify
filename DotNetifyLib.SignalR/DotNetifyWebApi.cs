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
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DotNetify.WebApi
{
   /// <summary>
   /// This class allows access to view models through a web API.
   /// </summary>
   [Route("api/dotnetify/vm")]
   [ApiController]
   public class DotNetifyWebApi : ControllerBase
   {
      [HttpGet("{vmId}")]
      public async Task<string> Request_VM(
         string vmId,
         [FromQuery] string vmArg,
         [FromServices] IVMFactory vmFactory,
         [FromServices] IHubPipeline hubPipeline)
      {
         var taskCompletionSource = new TaskCompletionSource<string>();
         var vmController = new VMController((arg1, arg2, arg3) => taskCompletionSource.SetResult(arg3), vmFactory)
         {
            ResponseVMFilter = CreateRespondingVMFilter(hubPipeline, vmId, vmArg)
         };

         try
         {
            var hubContext = new DotNetifyHubContext(null, nameof(Request_VM), vmId, vmArg, null, HttpContext?.User);
            vmController.RequestVMFilter = CreateVMFilter(hubContext, hubPipeline);

            hubPipeline.RunMiddlewares(hubContext, ctx =>
            {
               vmController.OnRequestVM(Guid.NewGuid().ToString(), ctx.VMId, ctx.Data);
               vmController.Dispose();
               return Task.CompletedTask;
            });
         }
         catch (Exception ex)
         {
            var finalEx = hubPipeline.RunExceptionMiddleware(null, ex);
            if (finalEx is OperationCanceledException == false)
               taskCompletionSource.SetResult(DotNetifyHub.SerializeException(finalEx));
         }

         return await taskCompletionSource.Task;
      }

      [HttpPost("{vmId}")]
      public async Task<string> Update_VM(
         string vmId,
         [FromQuery] string vmArg,
         [FromBody] Dictionary<string, object> vmData,
         [FromServices] IVMFactory vmFactory,
         [FromServices] IHubPipeline hubPipeline)
      {
         var taskCompletionSource1 = new TaskCompletionSource<string>();
         var taskCompletionSource2 = new TaskCompletionSource<string>();
         var correlationId = Guid.NewGuid().ToString();

         var vmController = new VMController((arg1, arg2, arg3) =>
         {
            if (!taskCompletionSource1.TrySetResult(arg3))
               taskCompletionSource2.SetResult(arg3);
         }, vmFactory)
         {
            ResponseVMFilter = CreateRespondingVMFilter(hubPipeline, vmId, vmData)
         };

         try
         {
            var hubContext = new DotNetifyHubContext(null, nameof(Request_VM), vmId, vmArg, null, HttpContext?.User);
            vmController.RequestVMFilter = CreateVMFilter(hubContext, hubPipeline);

            hubPipeline.RunMiddlewares(hubContext, ctx =>
            {
               vmController.OnRequestVM(correlationId, ctx.VMId, ctx.Data);
               return Task.CompletedTask;
            });
         }
         catch (Exception ex)
         {
            var finalEx = hubPipeline.RunExceptionMiddleware(null, ex);
            if (finalEx is OperationCanceledException == false)
               taskCompletionSource1.SetResult(DotNetifyHub.SerializeException(finalEx));
         }

         await taskCompletionSource1.Task;

         try
         {
            var hubContext = new DotNetifyHubContext(null, nameof(Update_VM), vmId, vmData, null, HttpContext?.User);
            vmController.UpdateVMFilter = CreateVMFilter(hubContext, hubPipeline);

            hubPipeline.RunMiddlewares(hubContext, ctx =>
            {
               vmController.OnUpdateVM(correlationId, ctx.VMId, ctx.Data as Dictionary<string, object>);
               vmController.Dispose();
               return Task.CompletedTask;
            });
         }
         catch (Exception ex)
         {
            var finalEx = hubPipeline.RunExceptionMiddleware(null, ex);
            if (finalEx is OperationCanceledException == false)
               taskCompletionSource2.SetResult(DotNetifyHub.SerializeException(finalEx));
         }

         return await taskCompletionSource2.Task;
      }

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

      private VMController.FilterDelegate CreateRespondingVMFilter(IHubPipeline hubPipeline, string vmId, object vmData)
      {
         return (_vmId, vm, data, vmAction) =>
         {
            var hubContext = new DotNetifyHubContext(null, nameof(DotNetifyHub.Response_VM), vmId, vmData, null, HttpContext?.User);
            hubPipeline.RunMiddlewares(hubContext, ctx =>
            {
               CreateVMFilter(hubContext, hubPipeline)(_vmId, vm, data, vmAction);
               return Task.CompletedTask;
            });
         };
      }
   }
}