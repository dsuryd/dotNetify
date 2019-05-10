using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DotNetify;
using DotNetify.Elements;
using DotNetify.Routing;
using DotNetify.Security;

namespace Blazor.Server
{
    [Authorize]
    public class Dashboard : BaseVM, IRoutable
    {
        private IDisposable _subscription;

        public RoutingState RoutingState { get; set; } = new RoutingState();

        public Dashboard(ILiveDataService liveDataService)
        {
            AddProperty<string>("Download")
               .WithAttribute(new { Label = "Download", Icon = "cloud_download" })
               .SubscribeTo(liveDataService.Download);

            AddProperty<string>("Upload")
               .WithAttribute(new { Label = "Upload", Icon = "cloud_upload" })
               .SubscribeTo(liveDataService.Upload);

            AddProperty<string>("Latency")
               .WithAttribute(new { Label = "Latency", Icon = "network_check" })
               .SubscribeTo(liveDataService.Latency);

            AddProperty<int>("Users")
               .WithAttribute(new { Label = "Users", Icon = "face" })
               .SubscribeTo(liveDataService.Users);

            AddProperty<int[]>("Traffic").SubscribeTo(liveDataService.Traffic);

            AddProperty<int[]>("Utilization")
               .WithAttribute(new ChartAttribute { Labels = new string[] { "Memory", "Disk", "Network" } })
               .SubscribeTo(liveDataService.Utilization);

            AddProperty<int[]>("ServerUsage").SubscribeTo(liveDataService.ServerUsage)
               .WithAttribute(new ChartAttribute { Labels = new string[] { "dns", "sql", "nethst", "w2k", "ubnt", "uat", "ftp", "smtp", "exch", "demo" } });

            AddProperty<Activity[]>("RecentActivities")
               .SubscribeTo(liveDataService.RecentActivity.Select(value =>
               {
                   var activities = new Queue<Activity>(Get<Activity[]>("RecentActivities")?.Reverse() ?? new Activity[] { });
                   activities.Enqueue(value);
                   if (activities.Count > 4)
                       activities.Dequeue();

                   return activities.Reverse().ToArray();
               }));

            // Regulate data update interval to no less than every 200 msecs.
            _subscription = Observable
               .Interval(TimeSpan.FromMilliseconds(200))
               .StartWith(0)
               .Subscribe(_ => PushUpdates());
        }

        public override void Dispose()
        {
            _subscription?.Dispose();
            base.Dispose();
        }
    }
}