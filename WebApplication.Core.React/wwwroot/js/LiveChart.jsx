var LiveChart = React.createClass({
   getInitialState() {
      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("LiveChartVM", () => this.state, state => this.setState(state));

      // This component's JSX was loaded along with the VM's initial state for faster rendering.
      return window.vmStates.LiveChartVM;
   },
   componentWillUnmount() {
      this.vm.$destroy();
   },
   render() {
      return (
         <div className="container-fluid">
            <div className="header clearfix">
               <h3>Example: Live Chart</h3>
            </div>
            <LiveLineChart data={this.state.InitialData} nextData={this.state.NextData} />
         </div>
     );
   }
});

const LiveLineChart = React.createClass({
   getInitialState() {
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
      this.props.data.map(data => {
         initialData.labels.push(data[0]);
         initialData.datasets[0].data.push(data[1]);
      });

      return { chartData: initialData };
   },

   render() {
      var chartData = this.state.chartData;
      const chartOptions = { responsive: true, animation: false };

      const updateData = data => {
         if (data) {
            chartData.labels.shift();
            chartData.labels.push(data[0]);

            chartData.datasets[0].data.shift();
            chartData.datasets[0].data.push(data[1]);
         }
      }

      return (
         <LineChart data={chartData} options={chartOptions}>{updateData(this.props.nextData)}</LineChart>
     );
   }
});