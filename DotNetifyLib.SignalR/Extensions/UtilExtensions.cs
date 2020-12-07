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
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetify.Util
{
   public static class UtilExtensions
   {
      /// <summary>
      /// Converts from string.
      /// </summary>
      public static object ConvertFromString(this string value, Type type)
      {
         if (type.GetTypeInfo().IsClass && type != typeof(string))
            return System.Text.Json.JsonSerializer.Deserialize(value, type);
         else
            return TypeDescriptor.GetConverter(type)?.ConvertFromString(value);
      }

      /// <summary>
      /// Invokes a reflection method asynchronously.
      /// </summary>
      public static async Task<object> InvokeAsync(this MethodInfo methodInfo, object obj, params object[] args)
      {
         var task = (Task) methodInfo.Invoke(obj, args);
         await task.ConfigureAwait(false);
         return task.GetType().GetProperty(nameof(Task<object>.Result));
      }

      /// <summary>
      /// Normalizes the type of the object argument to JObject when possible.
      /// </summary>
      /// <param name="data">Arbitrary object.</param>
      /// <returns>JObject if the object is convertible; otherwise unchanged.</returns>
      public static object NormalizeType(this object data)
      {
         if (data == null)
            return null;
         else if (data is JsonElement jElement)
         {
            // System.Text.Json protocol.
            var value = JToken.Parse(jElement.GetRawText());
            return value is JValue ? (value as JValue).Value : value;
         }
         else if (data is JObject)
            // Newtonsoft.Json protocol.
            return data as JObject;
         else if (!(data.GetType().IsPrimitive || data is string))
            // MessagePack protocol.
            return JObject.FromObject(data);

         return data;
      }

      /// <summary>
      /// Serializes an exception.
      /// </summary>
      /// <param name="ex">Exception to serialize.</param>
      /// <returns>Serialized exception.</returns>
      public static string Serialize(this Exception ex) => JsonConvert.SerializeObject(new { ExceptionType = ex.GetType().Name, ex.Message });
   }
}