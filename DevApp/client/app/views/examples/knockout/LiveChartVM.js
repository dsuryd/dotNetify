"use strict";
exports.__esModule = true;
var LiveChartVM = (function () {
    function LiveChartVM() {
    }
    LiveChartVM.prototype.createLineChart = function (iElement) {
        var chartData = {
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
        var chartOptions = { responsive: true };
        return new Chart(iElement.getContext('2d'), chartData, chartOptions);
    };
    LiveChartVM.prototype.createBarChart = function (iElement) {
        var chartData = {
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
            options: { responsive: true, legend: { display: false } }
        };
        return new Chart(iElement.getContext('2d'), chartData);
    };
    LiveChartVM.prototype.createPieChart = function (iElement) {
        var chartData = {
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
            options: { responsive: true }
        };
        return new Chart(iElement.getContext('2d'), chartData);
    };
    LiveChartVM.prototype.updateLineChart = function (vm, element) {
        if (!this.lineChart)
            this.lineChart = this.createLineChart(element);
        var data = vm.Waveform();
        data = vm.Waveform().slice(Math.max(data.length - 30, 0));
        this.lineChart.data.labels = data.map(function (x) { return x[0]; });
        this.lineChart.data.datasets[0].data = data.map(function (x) { return +x[1]; });
        this.lineChart.update();
    };
    LiveChartVM.prototype.updateBarChart = function (vm, element) {
        if (!this.barChart)
            this.barChart = this.createBarChart(element);
        this.barChart.data.datasets[0].data = vm.Bar();
        this.barChart.update();
    };
    LiveChartVM.prototype.updatePieChart = function (vm, element) {
        if (!this.pieChart)
            this.pieChart = this.createPieChart(element);
        this.pieChart.data.datasets[0].data = vm.Pie();
        this.pieChart.update();
    };
    return LiveChartVM;
}());
exports["default"] = LiveChartVM;
