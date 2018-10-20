<template>
  <section>
    <div>
      <canvas v-vmOn="{Waveform: updateLineChart}"></canvas>
    </div>
    <div>
      <canvas v-vmOn="{Pie: updatePieChart}"></canvas>
      <canvas v-vmOn="{Bar: updateBarChart}"></canvas>
    </div>
  </section>
</template>

<script>
import dotnetify from 'dotnetify/vue';

export default {
  name: 'LiveChart',
  created() {
    this.vm = dotnetify.vue.connect("LiveChartVM", this);
  },
  destroyed() {
    this.vm.$destroy();
  },
  data() {
    return {
      Waveform: [],
      Bar: [],
      Pie: []
    }
  },
  methods: {
    createLineChart: function (elem) {
      const chartData = {
        type: 'line',
        data: {
          labels: [],
          datasets: [
            {
              label: 'Waveform',
              data: [],
              backgroundColor: ['rgba(217,237,245,0.4)'],
              borderColor: ['#9acfea'],
              borderWidth: 2
            }
          ]
        }
      };
      const chartOptions = { responsive: true };
      return new Chart(elem.getContext('2d'), chartData, chartOptions);
    },
    createBarChart(elem) {
      const chartData = {
        type: 'bar',
        data: {
          labels: ['user', 'sys', 'eth0', 'lo', 'ker', 'sda1', 'sda2', 'sda3'],
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
              borderColor: ['#9acfea'],
              borderWidth: 1
            }
          ]
        },
        options: {
          responsive: true,
          legend: { display: false }
        }
      };
      return new Chart(elem.getContext('2d'), chartData);
    },
    createPieChart(elem) {
      const chartData = {
        type: 'doughnut',
        data: {
          labels: ['CPU', 'Memory', 'Disk'],
          datasets: [
            {
              label: '',
              data: [],
              backgroundColor: ['rgba(255, 99, 132, 0.8)', 'rgba(54, 162, 235, 0.8)', 'rgba(255, 206, 86, 0.8)'],
              borderColor: ['#9acfea'],
              borderWidth: 1
            }
          ]
        },
        options: {
          responsive: true
        }
      };
      return new Chart(elem.getContext('2d'), chartData);
    },
    updateLineChart: function (element) {
      if (!this.lineChart) this.lineChart = this.createLineChart(element);

      let data = this.Waveform;
      data = this.Waveform.slice(Math.max(data.length - 30, 0));
      this.lineChart.data.labels = data.map(x => x[0]);
      this.lineChart.data.datasets[0].data = data.map(x => +x[1]);
      this.lineChart.update();
    },
    updateBarChart(element) {
      if (!this.barChart) this.barChart = this.createBarChart(element);
      this.barChart.data.datasets[0].data = this.Bar;
      this.barChart.update();
    },
    updatePieChart(element) {
      if (!this.pieChart) this.pieChart = this.createPieChart(element);
      this.pieChart.data.datasets[0].data = this.Pie;
      this.pieChart.update();
    }
  }
}
</script>

