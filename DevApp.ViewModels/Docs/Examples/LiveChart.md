##### LiveChart.js

```jsx
import React from 'react';
import dotnetify from 'dotnetify';
import styled from 'styled-components';
import { Line, Bar, Doughnut } from 'react-chartjs-2'; 

const Container = styled.div`
  /* styles */
`;

class LiveChart extends React.Component {
  constructor(props) {
    super(props);
    this.state = { Waveform: [], Bar: [], Pie: [] };

    // Connect this component to the back-end view model.
    this.vm = dotnetify.react.connect('LiveChartVM', this);
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    return (
      <Container>
        <div>
          <LineChart data={this.state.Waveform} />
        </div>
        <div>
          <PieChart data={this.state.Pie} />
          <BarChart data={this.state.Bar} />
        </div>
      </Container>
    );
  }
}
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
          label: 'Waveform',
          data: [],
          backgroundColor: [ 'rgba(217,237,245,0.4)' ],
          borderColor: [ '#9acfea' ],
          borderWidth: 2
        }
      ]
    };
    this.chartOptions = { responsive: true };
  }
  render() {
    const data = this.props.data.slice(Math.max(this.props.data.length - 30, 0));
    this.chartData.labels = data.map(x => x[0]);
    this.chartData.datasets[0].data = data.map(x => +x[1]);

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
      labels: [ 'user', 'sys', 'eth0', 'lo', 'ker', 'sda1', 'sda2', 'sda3' ],
      datasets: [
        {
          label: '',
          data: [],
          backgroundColor: [
            'rgba(255, 99, 132, 0.8)',
            'rgba(54, 162, 235, 0.8)',
            'rgba(255, 206, 86, 0.8)',
            'rgba(75, 192, 192, 0.8)',
            'rgba(153, 102, 255, 0.8)',
            'rgba(255, 159, 64, 0.8)',
            'rgba(255, 99, 132, 0.8)',
            'rgba(54, 162, 235, 0.8)'
          ],
          borderColor: [ '#9acfea' ],
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
      labels: [ 'CPU', 'Memory', 'Disk' ],
      datasets: [
        {
          label: '',
          data: [],
          backgroundColor: [ 'rgba(255, 99, 132, 0.8)', 'rgba(54, 162, 235, 0.8)', 'rgba(255, 206, 86, 0.8)' ],
          borderColor: [ '#9acfea' ],
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