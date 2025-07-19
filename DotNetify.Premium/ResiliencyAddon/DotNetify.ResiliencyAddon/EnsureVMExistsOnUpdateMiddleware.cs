/*
Copyright 2023 Dicky Suryadi

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
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetify.WebApi
{
   /// <summary>
   /// This middleware creates a view model if the update VM is received but no instance is found.
   /// The scenario where this would occur is when there's load-balancing.
   /// </summary>
   public class EnsureVMExistsOnUpdateMiddleware
   {
      private readonly RequestDelegate _next;

      public EnsureVMExistsOnUpdateMiddleware(RequestDelegate next)
      {
         _next = next;
      }

      public async Task InvokeAsync(HttpContext context, IServiceProvider services)
      {
         if (context.Request.Path.Value.EndsWith("/api/dotnetify/vm", StringComparison.OrdinalIgnoreCase))
         {
            context.Request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
            await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            context.Request.Body.Position = 0;  //rewinding the stream to 0

            var requestContent = Encoding.UTF8.GetString(buffer);
            try
            {
               var integrationRequest = JsonSerializer.Deserialize<DotNetifyWebApi.IntegrationRequest>(requestContent, new JsonSerializerOptions {  PropertyNameCaseInsensitive = true });
               if (integrationRequest.Payload.CallType == "update_vm")
               {
                  var vmControllerFactory = services.GetRequiredService<IWebApiVMControllerFactory>();
                  if (vmControllerFactory.GetInstance(integrationRequest.ConnectionId)?.HasVM(integrationRequest.Payload.VMId) == false)
                     await RestoreManager.RestoreVMAsync(services, integrationRequest.ConnectionId, integrationRequest.Payload.VMId, null);
               }
            }
            catch { }
         }

         await _next(context);
      }
   }
}