/* 
Copyright 2017 Dicky Suryadi

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
using Newtonsoft.Json.Linq;

namespace DotNetify
{
    public static class DotNetifyHubContextExtensions
    {
      public static T Headers<T>(this IDotNetifyHubContext context)
      {
         try
         {
            if (context.Data is JObject && (context.Data as JObject)[DotNetifyHub.JTOKEN_headers] != null)
               return (context.Data as JObject)[DotNetifyHub.JTOKEN_headers].ToObject<T>();
         }
         catch (Exception) { }
         return default(T);
      }
   }
}
