var AFITop100 = React.createClass({

   getInitialState() {

      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("AFITop100VM", () => this.state, state => this.setState(state));

      // Set up function to dispatch state to the back-end.
      this.dispatchState = state => {
         this.setState(state);
         this.vm.$dispatch(state);
      }

      // This component's JSX was loaded along with the VM's initial state for faster rendering.
      return window.vmStates["AFITop100VM"];
   },
   componentWillUnmount() {
      this.vm.$destroy();
   },
   render() {
      return (
         <div className="container-fluid">
            <div className="header clearfix">
               <h3>Example: Composite View *** UNDER CONSTRUCTION ***</h3>
            </div>
            <MuiThemeProvider>
               <div>
                  <div className="row">
                     <div className="col-md-8">
                        <PaginatedTable vm="AFITop100VM.MovieTableVM" colWidths={["1em", "20%", "4em", "40%", "20%"]} />
                     </div>
                     <div className="col-md-4">
                     </div>
                  </div>
               </div>
            </MuiThemeProvider>
         </div>
      );
   }
});