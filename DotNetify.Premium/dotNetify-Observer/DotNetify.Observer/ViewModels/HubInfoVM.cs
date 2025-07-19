using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace DotNetify.Observer
{
   public class HubInfoVM : BaseVM
   {
      private readonly IConnectionTracker _connectionTracker;
      private List<IDisposable> _subs = new List<IDisposable>();

      public class HubInfoItem
      {
         public string Id { get; set; }
         public string Name { get; set; }
         public int? Clients { get; set; }
         public double? Cpu { get; set; }
         public double? Memory { get; set; }

         public HubInfoItem(string id, string name, int? clients, double? cpu, double? memory)
         {
            Id = id;
            Name = name;
            Clients = clients;
            Cpu = cpu;
            Memory = memory;
         }
      }

      [ItemKey(nameof(HubInfoItem.Id))]
      public IList<HubInfoItem> InfoItems { get; set; } = new List<HubInfoItem>();

      public HubInfoVM(IConnectionTracker connectionTracker)
      {
         _connectionTracker = connectionTracker;

         _subs.Add(_connectionTracker.Telemetry.Subscribe(data =>
         {
            OnUpdateHubInfo(data);
            PushUpdates();
         }));
      }

      public override void Dispose() => _subs.ForEach(x => x.Dispose());

      private void OnUpdateHubInfo(TelemetryData data)
      {
         var cpuInfo = Telemetry.MetricsInfo[MetricsType.total_cpu];
         var memInfo = Telemetry.MetricsInfo[MetricsType.total_mem];
         var clientsInfo = Telemetry.MetricsInfo[MetricsType.total_clients];

         double? cpu = data.Metrics.ContainsKey(cpuInfo.Id) && data.Metrics[cpuInfo.Id] != null ? (double?) double.Parse(data.Metrics[cpuInfo.Id].ToString()) : null;
         double? memory = data.Metrics.ContainsKey(memInfo.Id) && data.Metrics[memInfo.Id] != null ? (double?) double.Parse(data.Metrics[memInfo.Id].ToString()) : null;
         int? clients = data.Metrics.ContainsKey(clientsInfo.Id) && data.Metrics[clientsInfo.Id] != null ? (int?) int.Parse(data.Metrics[clientsInfo.Id].ToString()) : null;

         var item = new HubInfoItem(data.HubId, data.Name, clients, cpu, memory);

         if (!InfoItems.Any(x => x.Id == data.HubId))
         {
            InfoItems.Add(item);
            this.AddList(nameof(InfoItems), item);
         }
         else
            this.UpdateList(nameof(InfoItems), item);
      }
   }
}