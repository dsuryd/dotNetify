##### LiveChart.js

```jsx
import React from "react";
import { useConnect } from "dotnetify";
import { LiveChartCss } from "./components/css";
import { Line, Bar, Doughnut } from "react-chartjs-2";
import "chartjs-plugin-streaming";

const LiveChart = () => {
  const { state } = useConnect("LiveChartVM", { Waveform: [], Bar: [], Pie: [] });

  return (
    <LiveChartCss>
      <div>
        <LineChart data={state.Waveform} />
      </div>
      <div>
        <PieChart data={state.Pie} />
        <BarChart data={state.Bar} />
      </div>
    </LiveChartCss>
  );
};
```

##### LineChart.js

```jsx
class LineChart extends React.Component {
  constructor(props) {
    super(props);
    this.chartData = {
      labels: [],
      datasets: [
        {
          label: "Waveform",
          data: [],
          backgroundColor: "rgba(217,237,245,0.4)",
          borderColor: "#9acfea",
          borderWidth: 2
        }
      ]
    };
    this.chartOptions = {
      responsive: true,
      scales: {
        xAxes: [
          {
            type: "realtime",
            realtime: { delay: 2000 }
          }
        ],
        yAxes: [
          {
            ticks: {
              suggestedMin: -1,
              suggestedMax: 1
            }
          }
        ]
      }
    };
  }
  shouldComponentUpdate() {
    if (this.props.data.length > 0) {
      const data = this.props.data[this.props.data.length - 1];
      this.chartData.datasets[0].data.push({ x: Date.now(), y: data[1] });
      return false;
    }
    return true;
  }
  render() {
    const maxIdx = this.props.data.length - 1;
    this.chartData.datasets[0].data = this.props.data.map((data, idx) => ({ x: Date.now() - (maxIdx - idx) * 1000, y: data[1] }));
    return <Line data={this.chartData} options={this.chartOptions} />;
  }
}
```

##### BarChart.js

```jsx
class BarChart extends React.Component {
  constructor(props) {
    super(props);
    this.chartData = {
      labels: ["user", "sys", "eth0", "lo", "ker", "sda1", "sda2", "sda3"],
      datasets: [
        {
          label: "",
          data: [],
          backgroundColor: [
            "rgba(255, 99, 132, 0.8)",
            "rgba(54, 162, 235, 0.8)",
            "rgba(255, 206, 86, 0.8)",
            "rgba(75, 192, 192, 0.8)",
            "rgba(153, 102, 255, 0.8)",
            "rgba(255, 159, 64, 0.8)",
            "rgba(255, 99, 132, 0.8)",
            "rgba(54, 162, 235, 0.8)"
          ],
          borderColor: ["#9acfea"],
          borderWidth: 1
        }
      ]
    };
    this.chartOptions = { responsive: true, legend: { display: false } };
  }
  render() {
    this.chartData.datasets[0].data = this.props.data;
    return <Bar data={this.chartData} options={this.chartOptions} />;
  }
}
```

##### PieChart.js

```jsx
class PieChart extends React.Component {
  constructor(props) {
    super(props);
    this.chartData = {
      labels: ["CPU", "Memory", "Disk"],
      datasets: [
        {
          label: "",
          data: [],
          backgroundColor: ["rgba(255, 99, 132, 0.8)", "rgba(54, 162, 235, 0.8)", "rgba(255, 206, 86, 0.8)"],
          borderColor: ["#9acfea"],
          borderWidth: 1
        }
      ]
    };
    this.chartOptions = { responsive: true };
  }
  render() {
    this.chartData.datasets[0].data = this.props.data;
    return <Doughnut data={this.chartData} options={this.chartOptions} />;
  }
}
```

##### LiveChartVM.cs

```csharp
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
```
