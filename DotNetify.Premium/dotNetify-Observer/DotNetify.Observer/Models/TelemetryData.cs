using System.Collections.Generic;

namespace DotNetify.Observer
{
   public class TelemetryData
   {
      public string HubId { get; set; }
      public string Name { get; set; }
      public Dictionary<string, object> Metrics { get; set; }

      public TelemetryData(string hubId, string name, Dictionary<string, object> metrics)
      {
         HubId = hubId;
         Name = name;
         Metrics = metrics;
      }
   }

   public class Metrics
   {
      public string Id { get; set; }
      public string Name { get; set; }
      public string Unit { get; set; }

      public Metrics(string id, string name, string unit)
      {
         Id = id;
         Name = name;
         Unit = unit;
      }
   }

   public enum MetricsType
   {
      proc_cpu,
      proc_mem,
      total_cpu,
      total_mem,
      total_clients,
      in_throughput,
      out_throughput
   }

   public static class Telemetry
   {
      public static readonly Dictionary<MetricsType, Metrics> MetricsInfo = new Dictionary<MetricsType, Metrics>
      {
         { MetricsType.proc_cpu, new Metrics("_proc_cpu", "Process CPU", "%") },
         { MetricsType.proc_mem, new Metrics("proc_mem", "Process Memory", "MB") },
         { MetricsType.total_cpu, new Metrics("total_cpu", "Total CPU", "%") },
         { MetricsType.total_mem, new Metrics("total_mem","Total Memory", "%") },
         { MetricsType.total_clients, new Metrics("total_clients", "Total Clients", "") },
         { MetricsType.in_throughput, new Metrics("in_throughput", "Inbound/Sec", "") },
         { MetricsType.out_throughput, new Metrics("out_throughput", "Outbound/Sec", "") }
      };
   }
}