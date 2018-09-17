var LiveChartVM = (function () {
   return {
      updateChart: function (iItem, iElement) {
         var vm = this;

         if (vm._chart == null)
            vm._chart = this.createChart(iItem, iElement);
         else {
            var data = iItem.Data();
            if (data != null) {
               for (var i = 0; i < data.length; i++) {
                  vm._chart.addData([data[i][1]], data[i][0]);
                  // Remove the oldest data.
                  vm._chart.removeData();  
               }
            }
         }
         iItem.Data(null); // Reset the data.
      },

      createChart: function (iItem, iElement) {
         // Create the chart with ChartJS.
         var dataset = iItem.Data();
         var labels = [], data = [];
         for (var i = 0 ; i < dataset.length; i++) {
            labels.push(dataset[i][0]);
            data.push(dataset[i][1]);
         }
         return new Chart(iElement.getContext('2d'))
             .Line({
                labels: labels,
                datasets: [{ data: data, fillColor: "rgba(217,237,245,0.2)", strokeColor: "#9acfea", pointColor: "#9acfea", pointStrokeColor: "#fff" }]
             }, { responsive: true, animation: false });
      }
   }
})();