"use strict";

var LiveChart = React.createClass({
   displayName: "LiveChart",

   getInitialState: function getInitialState() {
      var _this = this;

      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("LiveChartVM", function () {
         return _this.state;
      }, function (state) {
         return _this.setState(state);
      });

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
         React.createElement(LiveLineChart, { data: this.state.InitialData, nextData: this.state.NextData })
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

