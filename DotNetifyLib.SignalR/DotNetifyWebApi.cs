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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DotNetify.WebApi
{
   /// <summary>
   /// This class allows access to view models through a web API.
   /// </summary>
   [Route("api/dotnetify")]
   [ApiController]
   public class DotNetifyWebApi : ControllerBase
   {
      [HttpGet("{vmId}")]
      public async Task<string> Request_VM(string vmId, [FromQuery] string vmArg, [FromServices] IVMFactory vmFactory)
      {
         var taskCompletionSource = new TaskCompletionSource<string>();
         var vmController = new VMController((arg1, arg2, arg3) => taskCompletionSource.SetResult(arg3), vmFactory);

         vmController.OnRequestVM(Guid.NewGuid().ToString(), vmId, vmArg);
         vmController.Dispose();

         return await taskCompletionSource.Task;
      }

      [HttpPost("{vmId}")]
      public async Task<string> Update_VM(string vmId, [FromQuery] string vmArg, [FromBody] Dictionary<string, object> vmData, [FromServices] IVMFactory vmFactory)
      {
         var taskCompletionSource1 = new TaskCompletionSource<string>();
         var taskCompletionSource2 = new TaskCompletionSource<string>();
         var correlationId = Guid.NewGuid().ToString();

         var vmController = new VMController((arg1, arg2, arg3) =>
         {
            if (!taskCompletionSource1.TrySetResult(arg3))
               taskCompletionSource2.SetResult(arg3);
         }, vmFactory);

         vmController.OnRequestVM(correlationId, vmId, vmArg);
         await taskCompletionSource1.Task;

         vmController.OnUpdateVM(correlationId, vmId, vmData);
         vmController.Dispose();

         return await taskCompletionSource2.Task;
      }
   }
}