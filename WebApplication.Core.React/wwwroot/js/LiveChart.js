"use strict";

var LiveChart = React.createClass({
   displayName: "LiveChart",

   getInitialState: function getInitialState() {
      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("LiveChartVM", this);

      // This component's JSX was loaded along with the VM's initial state for faster rendering.
      return window.vmStates.LiveChartVM;
   },
   componentWillUnmount: function componentWillUnmount() {
      this.vm.$destroy();
   },
   render: function render() {
      return React.createElement(
         "div",
         { className: "container-fluid" },
         React.createElement(
            "div",
            { className: "header clearfix" },
            React.createElement(
               "h3",
               null,
               "Example: Live Chart"
            )
         ),
         React.createElement(
            MuiThemeProvider,
            null,
            React.createElement(
               "div",
               { className: "row" },
               React.createElement(
                  "div",
                  { className: "col-md-8" },
                  React.createElement(
                     Paper,
                     { style: { padding: "2em" } },
                     React.createElement(LiveLineChart, { data: this.state.InitialLineData, nextData: this.state.NextLineData })
                  )
               ),
               React.createElement(
                  "div",
                  { className: "col-md-4" },
                  React.createElement(
                     Paper,
                     { style: { padding: "2em" } },
                     React.createElement(LiveDoughnutChart, { data: this.state.InitialDoughnutData, nextData: this.state.NextDoughnutData })
                  ),
                  React.createElement(
                     Paper,
                     { style: { padding: "2em", marginTop: "1em" } },
                     React.createElement(LiveBarChart, { data: this.state.InitialBarData, nextData: this.state.NextBarData })
                  )
               )
            )
         )
      );
   }
});

var LiveLineChart = React.createClass({
   displayName: "LiveLineChart",

   getInitialState: function getInitialState() {
      // Build the ChartJS data parameter with initial data.
      var initialData = {
         labels: [],
         datasets: [{
            label: "",
            data: [],
            fillColor: "rgba(217,237,245,0.2)",
            strokeColor: "#9acfea",
            pointColor: "#9acfea",
            pointStrokeColor: "#fff"
         }]
      };
      this.props.data.map(function (data) {
         initialData.labels.push(data[0]);
         initialData.datasets[0].data.push(data[1]);
      });

      return { chartData: initialData };
   },

   render: function render() {
      var chartData = this.state.chartData;
      var chartOptions = { responsive: true, animation: false };

      var updateData = function updateData(data) {
         if (data) {
            chartData.labels.shift();
            chartData.labels.push(data[0]);

            chartData.datasets[0].data.shift();
            chartData.datasets[0].data.push(data[1]);
         }
      };

      return React.createElement(
         LineChart,
         { data: chartData, options: chartOptions },
         updateData(this.props.nextData)
      );
   }
});

var LiveBarChart = React.createClass({
   displayName: "LiveBarChart",

   getInitialState: function getInitialState() {
      // Build the ChartJS data parameter with initial data.
      var initialData = {
         labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun"],
         datasets: [{
            label: "",
            data: [],
            fillColor: "#4caf50",
            strokeColor: "#4caf50"
         }]
      };
      initialData.datasets[0].data = this.props.data;
      return { chartData: initialData };
   },

   render: function render() {
      var chartData = this.state.chartData;
      var chartOptions = { responsive: true, animation: true };

      var updateData = function updateData(data) {
         if (data) chartData.datasets[0].data = data;
      };

      return React.createElement(
         BarChart,
         { data: chartData, options: chartOptions },
         updateData(this.props.nextData)
      );
   }
});

var LiveDoughnutChart = React.createClass({
   displayName: "LiveDoughnutChart",

   getInitialState: function getInitialState() {
      // Build the ChartJS data parameter with initial data.
      var initialData = [{
         color: "#FF6384",
         highlight: "#FF6384",
         label: "Red"
      }, {
         color: "#36A2EB",
         highlight: "#36A2EB",
         label: "Blue"
      }, {
         color: "#FFCE56",
         highlight: "#FFCE56",
         label: "Yellow"
      }];

      this.props.data.map(function (val, idx) {
         return initialData[idx].value = val;
      });
      return { chartData: initialData };
   },

   render: function render() {
      var chartData = this.state.chartData;
      var chartOptions = { responsive: true, animation: true };

      var updateData = function updateData(data) {
         if (data) data.map(function (val, idx) {
            return chartData[idx].value = val;
         });
      };

      return React.createElement(
         DoughnutChart,
         { data: chartData, options: chartOptions },
         updateData(this.props.nextData)
      );
   }
});

