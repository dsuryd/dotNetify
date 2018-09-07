declare var Chart: any;

export default class LiveChartVM {
  chart: any;
  chartData: any;
  chartOptions: any;

  constructor() {
    this.chartData = {
      type: "line",
      data: {
        labels: [],
        datasets: [
          {
            label: "Waveform",
            data: [],
            backgroundColor: "rgba(153,255,51,0.4)"
          }
        ]
      }
    };

    this.chartOptions = { responsive: true };
  }

  updateChart(iItem, iElement) {
    var vm: any = this;

    if (!this.chart) {
      this.chart = this.createChart(iItem, iElement);
    }

    const dataset = iItem.Waveform();
    //const data = dataset.slice(Math.max(dataset.length - 30, 0));
    this.chart.data.labels = dataset.map(x => x[0]);
    this.chart.data.datasets[0].data = dataset.map(x => +x[1]);
    console.warn(dataset);
    this.chart.update();
  }

  createChart(iItem, iElement) {
    return new Chart(iElement.getContext("2d"), this.chartData, this.chartOptions);
  }
}
