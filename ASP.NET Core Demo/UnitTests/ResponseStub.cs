using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnitTests
{
   public class ResponseStub
   {
      private string _connectionId;
      private string _vmId;
      private string _vmData;

      public string VMId => _vmId;
      public JObject VMData => (JObject)JsonConvert.DeserializeObject(_vmData);

      public void Handler(string connectionId, string vmId, string vmData)
      {
         _connectionId = connectionId;
         _vmId = vmId;
         _vmData = vmData;
      }

      public T GetVM<T>() where T : class => JsonConvert.DeserializeObject<T>(_vmData);
      public T GetVMProperty<T>(string propName) where T : class => VMData[propName].ToObject(typeof(T)) as T; 

      public Dictionary<string, object> MockAction(string actionName, string actionValue) => new Dictionary<string, object>() { { actionName, actionValue } };
   }
}
