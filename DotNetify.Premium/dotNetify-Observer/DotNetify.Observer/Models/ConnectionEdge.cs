using System;

namespace DotNetify.Observer
{
   public class ConnectionEdge
   {
      public class Message
      {
         public object Data { get; set; }
         public DateTimeOffset TimeStamp { get; set; }

         public Message(object data, DateTimeOffset timeStamp)
         {
            Data = data;
            TimeStamp = timeStamp;
         }
      }

      public string Id { get; set; }
      public string SourceId { get; set; }
      public string TargetId { get; set; }
      public string Label { get; set; }
      public ConnectionInfo Info { get; set; }
      public Message Inbound { get; set; }
      public Message Outbound { get; set; }
      public DateTime CreatedTime { get; set; }
   }
}