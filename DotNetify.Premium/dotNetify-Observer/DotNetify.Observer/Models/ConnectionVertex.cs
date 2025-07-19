using System;
using System.Collections.Generic;

namespace DotNetify.Observer
{
   public enum ConnectionVertexType
   {
      Client,
      Hub
   }

   public class ConnectionVertex
   {
      private readonly Throughput _inboundThroughput = new Throughput();
      private readonly Throughput _outboundThroughput = new Throughput();

      public string Id { get; set; }
      public string Name { get; set; }
      public ConnectionVertexType Type { get; set; }
      public ConnectionContext Context { get; set; }
      public Dictionary<string, object> Metrics { get; set; }
      public DateTime CreatedTime { get; set; }
      public string GroupName { get; set; }

      public double InboundThroughput => _inboundThroughput.GetThroughput();

      public double OutboundThroughput => _outboundThroughput.GetThroughput();

      public void IncrementInbound(int value = 1) => _inboundThroughput.Increment(value, DateTime.UtcNow);

      public void IncrementOutbound(int value = 1) => _outboundThroughput.Increment(value, DateTime.UtcNow);
   }
}