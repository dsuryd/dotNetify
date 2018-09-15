using System;
using System.Linq;
using System.Reactive.Linq;
using DotNetify;

namespace SPA
{
    public class LiveChartVM : BaseVM
    {
        public string[][] Waveform
        {
            get => Get<string[][]>();
            set => Set(value);
        }

        public int[] Bar
        {
            get => Get<int[]>();
            set => Set(value);
        }

        public double[] Pie
        {
            get => Get<double[]>();
            set => Set(value);
        }

        public LiveChartVM()
        {
            var timer = Observable.Interval(TimeSpan.FromSeconds(1));
            var random = new Random();

            Waveform = Enumerable.Range(1, 30).Select(x => new string[] { $"{x}", $"{Math.Sin(x / Math.PI)}" }).ToArray();
            Bar = Enumerable.Range(1, 8).Select(_ => random.Next(500, 1000)).ToArray();
            Pie = Enumerable.Range(1, 3).Select(_ => random.NextDouble()).ToArray();

            timer.Subscribe(x =>
            {
                x += 31;
                this.AddList(nameof(Waveform), new string[] { $"{x}", $"{Math.Sin(x / Math.PI)}" });

                Bar = Enumerable.Range(1, 12).Select(_ => random.Next(500, 1000)).ToArray();
                Pie = Enumerable.Range(1, 3).Select(_ => random.NextDouble()).ToArray();

                PushUpdates();
            });
        }
    }
}