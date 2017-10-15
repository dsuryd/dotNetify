"use strict";

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _get = function get(_x, _x2, _x3) { var _again = true; _function: while (_again) { var object = _x, property = _x2, receiver = _x3; _again = false; if (object === null) object = Function.prototype; var desc = Object.getOwnPropertyDescriptor(object, property); if (desc === undefined) { var parent = Object.getPrototypeOf(object); if (parent === null) { return undefined; } else { _x = parent; _x2 = property; _x3 = receiver; _again = true; desc = parent = undefined; continue _function; } } else if ("value" in desc) { return desc.value; } else { var getter = desc.get; if (getter === undefined) { return undefined; } return getter.call(receiver); } } };

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var LiveChart = (function (_React$Component) {
   _inherits(LiveChart, _React$Component);

   function LiveChart(props) {
      _classCallCheck(this, LiveChart);

      _get(Object.getPrototypeOf(LiveChart.prototype), "constructor", this).call(this, props);

      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("LiveChartVM", this);

      // This component's JSX was loaded along with the VM's initial state for faster rendering.
      this.state = window.vmStates.LiveChartVM;
   }

   _createClass(LiveChart, [{
      key: "componentWillUnmount",
      value: function componentWillUnmount() {
         this.vm.$destroy();
      }
   }, {
      key: "render",
      value: function render() {
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
   }]);

   return LiveChart;
})(React.Component);

var LiveLineChart = (function (_React$Component2) {
   _inherits(LiveLineChart, _React$Component2);

   function LiveLineChart(props) {
      _classCallCheck(this, LiveLineChart);

      _get(Object.getPrototypeOf(LiveLineChart.prototype), "constructor", this).call(this, props);

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

      this.state = { chartData: initialData };
   }

   _createClass(LiveLineChart, [{
      key: "render",
      value: function render() {
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
   }]);

   return LiveLineChart;
})(React.Component);

var LiveBarChart = (function (_React$Component3) {
   _inherits(LiveBarChart, _React$Component3);

   function LiveBarChart(props) {
      _classCallCheck(this, LiveBarChart);

      _get(Object.getPrototypeOf(LiveBarChart.prototype), "constructor", this).call(this, props);

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
      this.state = { chartData: initialData };
   }

   _createClass(LiveBarChart, [{
      key: "render",
      value: function render() {
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
   }]);

   return LiveBarChart;
})(React.Component);

var LiveDoughnutChart = (function (_React$Component4) {
   _inherits(LiveDoughnutChart, _React$Component4);

   function LiveDoughnutChart(props) {
      _classCallCheck(this, LiveDoughnutChart);

      _get(Object.getPrototypeOf(LiveDoughnutChart.prototype), "constructor", this).call(this, props);

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
      this.state = { chartData: initialData };
   }

   _createClass(LiveDoughnutChart, [{
      key: "render",
      value: function render() {
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
   }]);

   return LiveDoughnutChart;
})(React.Component);

