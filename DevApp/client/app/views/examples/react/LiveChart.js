import React, { useEffect, useLayoutEffect, useRef } from "react";
import { useConnect } from "dotnetify";
import { LiveChartCss } from "../components/css";

export const LiveChart = () => {
  const { state } = useConnect("LiveChartVM", { Waveform: [], Bar: [], Pie: [] });

  return (
    <LiveChartCss>
      <div>
        <LineChart data={[...state.Waveform]} />
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
      x: { type: "realtime", realtime: { delay: 2000 } },
      y: { ticks: { suggestedMin: -1, suggestedMax: 1 } }
    }
  });

  useEffect(() => {
    if (data.length > 0) {
      if (chartData.current.datasets[0].data.length === 0) {
        const maxIdx = data.length - 1;
        chartData.current.datasets[0].data = data.map((item, idx) => ({
          x: Date.now() - (maxIdx - idx) * 1000,
          y: item[1]
        }));
      } else {
        const lastItem = data[data.length - 1];
        chartData.current.datasets[0].data = [...chartData.current.datasets[0].data, { x: Date.now(), y: lastItem[1] }];
      }
      chartRef.current.update();
    }
  }, [data]);

  const elemRef = useRef();
  const chartRef = useRef();

  useLayoutEffect(() => {
    chartRef.current = new Chart(elemRef.current.getContext("2d"), {
      type: "line",
      data: chartData.current,
      options: chartOptions.current
    });
  }, []);

  return <canvas ref={elemRef} />;
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
  const chartOptions = useRef({ responsive: true, plugins: { legend: { display: false } } });
  const elemRef = useRef();
  const chartRef = useRef();

  useLayoutEffect(() => {
    chartRef.current = new Chart(elemRef.current.getContext("2d"), { type: "bar", data: chartData.current, options: chartOptions.current });
  }, []);

  useEffect(() => {
    chartData.current.datasets[0].data = [...data];
    chartRef.current.update();
  }, [data]);

  return <canvas ref={elemRef} />;
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
  const chartOptions = useRef({ responsive: true, plugins: { legend: { position: "left" } } });
  const elemRef = useRef();
  const chartRef = useRef();

  useLayoutEffect(() => {
    chartRef.current = new Chart(elemRef.current.getContext("2d"), {
      type: "doughnut",
      data: chartData.current,
      options: chartOptions.current
    });
  }, []);

  useEffect(() => {
    chartData.current.datasets[0].data = [...data];
    chartRef.current.update();
  }, [data]);

  return (
    <div style={{ width: "70%" }}>
      <canvas ref={elemRef} />
    </div>
  );
};

export default LiveChart;
