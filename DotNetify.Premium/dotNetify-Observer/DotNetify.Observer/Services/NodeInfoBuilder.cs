using System;
using System.Collections.Generic;
using System.Linq;
using UAParser;

namespace DotNetify.Observer
{
   public class NodeInfoItem
   {
      public string Label { get; set; }
      public string Value { get; set; }

      public NodeInfoItem(string label, string value)
      {
         Label = label;
         Value = value;
      }
   }

   public interface INodeInfoBuilder
   {
      IList<NodeInfoItem> GetClientInfo(ConnectionVertex vertex, ConnectionEdge edge);

      IList<NodeInfoItem> GetHubInfo(ConnectionVertex vertex);
   }

   public class NodeInfoBuilder : INodeInfoBuilder
   {
      private const string PARSED_USER_AGENT = "$parsedUserAgent";

      public IList<NodeInfoItem> GetClientInfo(ConnectionVertex vertex, ConnectionEdge edge)
      {
         var connectionInfo = edge.Info;
         var context = connectionInfo.OriginContext ?? connectionInfo.Context;
         var userAgent = context.HttpRequestHeaders.UserAgent;

         // Cache parsed user agent in the context because parsing is expensive.
         if (!context.Items.ContainsKey(PARSED_USER_AGENT))
            context.Items[PARSED_USER_AGENT] = !string.IsNullOrWhiteSpace(userAgent) ? Parser.GetDefault().ParseUserAgent(userAgent).ToString() : "n/a";

         var inbound = edge.Inbound?.Data?.SerializeToText();
         var outbound = edge.Outbound?.Data?.SerializeToText();
         var inboundTime = edge.Inbound?.TimeStamp.ToString("s");
         var outboundTime = edge.Outbound?.TimeStamp.ToString("s");

         var result = new List<NodeInfoItem>()
         {
            new NodeInfoItem("IP Address", context.HttpConnection.RemoteIpAddressString),
            new NodeInfoItem("Connection Id", context.ConnectionId),
            new NodeInfoItem("User Agent", context.Items[PARSED_USER_AGENT].ToString()),
            new NodeInfoItem("Outbound/sec", vertex.OutboundThroughput.ToString()),
            new NodeInfoItem("Inbound/sec", vertex.InboundThroughput.ToString()),
            new NodeInfoItem("Last Message", connectionInfo.TimeStamp.RelativeTo(DateTimeOffset.UtcNow)),
            new NodeInfoItem("Last Outbound", $"[{outboundTime}] {outbound}"),
            new NodeInfoItem("Last Inbound", $"[{inboundTime}] {inbound}"),
         };

         if (!string.IsNullOrEmpty(vertex.GroupName))
            result.Insert(2, new NodeInfoItem("Group Name", vertex.GroupName));

         return result;
      }

      public IList<NodeInfoItem> GetHubInfo(ConnectionVertex vertex)
      {
         var result = new List<NodeInfoItem>()
         {
            new NodeInfoItem("Host Name", vertex.Context?.GetHostName()),
         };

         if (vertex.Metrics != null)
         {
            foreach (var kvp in vertex.Metrics.Where(x => x.Value != null))
            {
               if (Enum.TryParse<MetricsType>(kvp.Key, out MetricsType key))
               {
                  var metricsInfo = Telemetry.MetricsInfo[key];
                  result.Add(new NodeInfoItem(metricsInfo.Name, $"{kvp.Value} {metricsInfo.Unit}"));
               }
            }
         }

         return result;
      }
   }
}