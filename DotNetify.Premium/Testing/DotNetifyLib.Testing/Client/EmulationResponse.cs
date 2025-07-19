using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetify.Testing
{
   /// <summary>
   /// Emulation response.
   /// </summary>
   public class EmulationResponse
   {
      public string VMId { get; }
      public object Data { get; }
      public object[] Payload { get; }

      public EmulationResponse(object[] args)
      {
         Payload = args[0] as object[];
         if (Payload.Length == 2)
         {
            VMId = Payload[0]?.ToString();
            Data = Payload[1] ?? string.Empty;
         }
      }

      /// <summary>
      /// Converts the first response to a type.
      /// </summary>
      public T As<T>() => JsonConvert.DeserializeObject<T>(Data.ToString());

      /// <summary>
      /// Converts the response to string.
      /// </summary>
      public override string ToString()
      {
         if (Data == null)
            return null;

         var data = JsonConvert.DeserializeObject<dynamic>(Data.ToString());
         return JsonConvert.SerializeObject(data, Formatting.None);
      }
   }

   public class EmulationResponses : List<EmulationResponse>
   {
      public Exception Exception { get; set; }

      public T As<T>() => this.FirstOrDefault() != null ? this.First().As<T>() : default;
   }
}