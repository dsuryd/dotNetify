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
using System.Net.Http;

namespace DotNetify.WebApi
{
   public class DotNetifyWebApiConfiguration
   {
      /// <summary>
      /// Configure HTTP client used for sending responses to clients.
      /// </summary>
      public Action<HttpClient> ConfigureHttpClient { get; set; }

      /// <summary>
      /// Maximum number of parallel HTTP requests when broadcasting a response.
      /// </summary>
      public int MaxParallelHttpRequests { get; set; } = 1000;
   }
}