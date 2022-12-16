/*
Copyright 2019-2023 Dicky Suryadi

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
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DotNetify.WebApi
{
   public interface IWebApiResponseManager : IDotNetifyHubResponseManager
   { }

   public class WebApiResponseManager : IWebApiResponseManager
   {
      private readonly IHttpClientFactory _httpClientFactory;

      private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
      {
         PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
         Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
      };

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="httpClientFactory">Http client used for making callback to the websocket server that handles client connections.</param>
      public WebApiResponseManager(IHttpClientFactory httpClientFactory)
      {
         _httpClientFactory = httpClientFactory;
      }

      public Task AddToGroupAsync(string connectionId, string groupName)
      {
         return Task.CompletedTask;
      }

      public void CreateInstance(HubCallerContext context)
      {
      }

      public HubCallerContext GetCallerContext(string connectionId)
      {
         return null;
      }

      public Task RemoveFromGroupAsync(string connectionId, string groupName)
      {
         return Task.CompletedTask;
      }

      public void RemoveInstance(string connectionId)
      {
      }

      public Task SendAsync(string connectionId, string vmId, string vmData)
      {
         var response = new DotNetifyWebApi.IntegrationResponse { VMId = vmId, Data = vmData };

         var httpClient = _httpClientFactory.CreateClient(nameof(DotNetifyWebApi));

         if (httpClient != null)
            _ = httpClient.PostAsync($"{connectionId}", new StringContent(JsonSerializer.Serialize(response, _jsonSerializerOptions), Encoding.UTF8, "application/json"));
         else
            throw new Exception("Missing HttpClient. Include 'services.AddDotNetifyHttpClient()' in the startup.");

         return Task.CompletedTask;
      }

      public Task SendToGroupAsync(string groupName, string vmId, string vmData)
      {
         throw new NotImplementedException();
      }

      public Task SendToGroupExceptAsync(string groupName, IReadOnlyList<string> excludedIds, string vmId, string vmData)
      {
         throw new NotImplementedException();
      }

      public Task SendToManyAsync(IReadOnlyList<string> connectionIds, string vmId, string vmData)
      {
         throw new NotImplementedException();
      }

      public Task SendToUsersAsync(IReadOnlyList<string> userIds, string vmId, string vmData)
      {
         throw new NotImplementedException();
      }
   }
}