import React, { useEffect, useRef } from "react";
import { useConnect } from "dotnetify";
import { LiveChartCss } from "../components/css";
import { Line, Bar, Doughnut } from "react-chartjs-2";
import "chartjs-plugin-streaming";

export const LiveChart = () => {
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

export const LineChart = ({ data }) => {
  const chartData = useRef({
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
  });

  const chartOptions = useRef({
    responsive: true,
    scales: {
      xAxes: [{ type: "realtime", realtime: { delay: 2000 } }],
      yAxes: [{ ticks: { suggestedMin: -1, suggestedMax: 1 } }]
    }
  });

  if (data.length > 0) {
    if (chartData.current.datasets[0].data.length === 0) {
      const maxIdx = data.length - 1;
      chartData.current.datasets[0].data = data.map((item, idx) => ({
        x: Date.now() - (maxIdx - idx) * 1000,
        y: item[1]
      }));
    } else {
      const lastItem = data[data.length - 1];
      chartData.current.datasets[0].data.push({ x: Date.now(), y: lastItem[1] });
    }
  }

  return <Line data={chartData.current} options={chartOptions.current} />;
};

export const BarChart = ({ data }) => {
  const chartData = useRef({
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
  });
  const chartOptions = useRef({ responsive: true, legend: { display: false } });

  chartData.current.datasets[0].data = data;
  return <Bar data={chartData.current} options={chartOptions.current} />;
};

export const PieChart = ({ data }) => {
  const chartData = useRef({
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
  });
  const chartOptions = useRef({ responsive: true });

  chartData.current.datasets[0].data = [...data];
  return <Doughnut data={chartData.current} options={chartOptions.current} />;
};

export default LiveChart;
