using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using StringDictionary = System.Collections.Generic.Dictionary<string, string>;

namespace DotNetify.Testing
{
   public class VMConnectOptions
   {
      public StringDictionary VMArg { get; } = new StringDictionary();
      public StringDictionary Headers { get; } = new StringDictionary();

      public static explicit operator JObject(VMConnectOptions vmConnectOptions)
      {
         if (vmConnectOptions == null)
            return null;

         var options = new JObject();
         if (vmConnectOptions.VMArg != null)
            options["$vmArg"] = JObject.FromObject(vmConnectOptions.VMArg);
         if (vmConnectOptions.Headers != null)
            options["$headers"] = JObject.FromObject(vmConnectOptions.Headers);
         return options;
      }

      public VMConnectOptions()
      {
      }

      public VMConnectOptions(dynamic vmArg)
      {
         Dictionary<string, object> vmArgDict = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(vmArg));
         VMArg = vmArgDict.ToDictionary(x => x.Key, x => $"{x.Value}");
      }

      public VMConnectOptions(dynamic vmArg, dynamic headers)
      {
         Dictionary<string, object> vmArgDict = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(vmArg));
         VMArg = vmArgDict.ToDictionary(x => x.Key, x => $"{x.Value}");

         Dictionary<string, object> headersDict = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(headers));
         Headers = headersDict.ToDictionary(x => x.Key, x => $"{x.Value}");
      }
   }

   public static class StringDictionaryExtensions
   {
      public static StringDictionary Set(this StringDictionary dictionary, string key, string value)
      {
         dictionary.Add(key, value);
         return dictionary;
      }
   }
}