import React from 'react';
import dotnetify from 'dotnetify';
import styled from 'styled-components';
import { Line, Bar, Doughnut } from 'react-chartjs-2';
import RenderExample from '../../components/RenderExample';

const Container = styled.div`
  padding: 0 1rem;
  width: 100%;
  max-width: 1268px;
  > div:first-child {
    display: inline-block;
    width: 70%;
  }
  > div:last-child {
    display: inline-block;
    width: 30%;
    > *:last-child {
      margin-top: 2rem;
    }
  }
  @media (max-width: 1170px) {
    > div:first-child,
    > div:last-child {
      display: block;
      width: 100%;
      margin-bottom: 2rem;
    }
  }
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

export default _ => (
  <RenderExample vm="LiveChartExample">
    <LiveChart />
  </RenderExample>
);
