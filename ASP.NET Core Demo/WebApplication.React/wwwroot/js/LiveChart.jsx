var LiveChart = React.createClass({
   getInitialState() {
      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("LiveChartVM", this);

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
            <MuiThemeProvider>
            <div className="row">
               <div className="col-md-8">
                  <Paper style={{padding: "2em"}}>
                     <LiveLineChart data={this.state.InitialLineData} nextData={this.state.NextLineData} />
                  </Paper>
               </div>
               <div className="col-md-4">
                  <Paper style={{padding: "2em"}}>
                     <LiveDoughnutChart data={this.state.InitialDoughnutData} nextData={this.state.NextDoughnutData} />
                  </Paper>
                  <Paper style={{padding: "2em", marginTop: "1em"}}>
                     <LiveBarChart data={this.state.InitialBarData} nextData={this.state.NextBarData} />
                  </Paper>
               </div>
            </div>
            </MuiThemeProvider>
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

const LiveBarChart = React.createClass({
   getInitialState() {
      // Build the ChartJS data parameter with initial data.
      var initialData = {
         labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun"],
         datasets: [{
            label: "",
            data: [],
            fillColor: "#4caf50",
            strokeColor: "#4caf50",
         }]
      };
      initialData.datasets[0].data = this.props.data;
      return { chartData: initialData };
   },

   render() {
      var chartData = this.state.chartData;
      const chartOptions = { responsive: true, animation: true };

      const updateData = data => {
         if (data)
            chartData.datasets[0].data = data;
      }

      return (
         <BarChart data={chartData} options={chartOptions}>{updateData(this.props.nextData)}</BarChart>
     );
}
});

const LiveDoughnutChart = React.createClass({
   getInitialState() {
      // Build the ChartJS data parameter with initial data.
      var initialData = [
         {
            color: "#FF6384",
            highlight: "#FF6384",
            label: "Red"
         },
         {
            color: "#36A2EB",
            highlight: "#36A2EB",
            label: "Blue"
         },
         {
            color: "#FFCE56",
            highlight: "#FFCE56",
            label: "Yellow"
         }];

      this.props.data.map((val, idx) => initialData[idx].value = val);
      return { chartData: initialData };
   },

   render() {
      var chartData = this.state.chartData;
      const chartOptions = { responsive: true, animation: true };

      const updateData = data => {
         if (data)
            data.map((val, idx) => chartData[idx].value = val);
      }

      return (
         <DoughnutChart data={chartData} options={chartOptions}>{updateData(this.props.nextData)}</DoughnutChart>
     );
   }
});