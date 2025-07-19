using Newtonsoft.Json;

namespace DotNetify.LoadTester
{
   public delegate void ServerResponseDelegate(IClientVM vm, ServerResponse response);

   public class ServerResponse
   {
      private readonly object[] _response;

      public string VMId { get; }
      public object Data { get; }

      public ServerResponse(object[] response)
      {
         _response = response;
         if (_response.Length == 2)
         {
            VMId = _response[0]?.ToString();
            Data = _response[1] ?? string.Empty;
         }
      }

      public T As<T>() => JsonConvert.DeserializeObject<T>(Data.ToString());

      public override string ToString() => Data != null ? JsonConvert.SerializeObject(Data) : string.Empty;
   }
}