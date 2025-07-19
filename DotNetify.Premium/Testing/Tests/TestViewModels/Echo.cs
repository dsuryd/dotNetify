using System;
using DotNetify;

namespace TestViewModels
{
   public class EchoVM : BaseVM
   {
      public class Receive
      {
         public uint Id { get; set; }
         public DateTimeOffset RequestTime { get; set; }
      }

      public class Response
      {
         public Receive Receive { get; set; } = new Receive();
         public DateTimeOffset ResponseTime { get; set; }
         public double Latency => (ResponseTime - Receive.RequestTime).TotalMilliseconds;
      }

      public Response Data { get; set; } = new Response();

      public void Ping(Receive receive)
      {
         Data.Receive = receive;
         Data.ResponseTime = DateTimeOffset.UtcNow;
         Changed(nameof(Data));
      }
   }
}