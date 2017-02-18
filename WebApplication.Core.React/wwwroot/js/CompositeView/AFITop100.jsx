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
                        <MovieTable vm="AFITop100VM.MovieTableVM" />
                     </div>
                     <div className="col-md-4">
                        <MovieDetails vm="AFITop100VM.MovieDetailsVM" />
                     </div>
                  </div>
               </div>
            </MuiThemeProvider>
         </div>
      );
   }
});

var MovieTable = React.createClass({

   getInitialState() {
      var vmName = this.props.vm || "MovieTableVM";
      this.vm = dotnetify.react.connect(vmName, () => this.state, state => this.setState(state));
      this.dispatchState = state => {
         this.setState(state);
         this.vm.$dispatch(state);
      }
      return window.vmStates[vmName];
   },
   componentWillUnmount() {
      this.vm.$destroy();
   },
   render() {
      return (
         <PaginatedTable colWidths={["1em", "20%", "4em", "40%", "20%"]}
                         headers={this.state.Headers}
                         data={this.state.Data}
                         itemKey={this.state.ItemKey}
                         select={this.state.SelectedKey}
                         pagination={this.state.Pagination}
                         page={this.state.SelectedPage}
                         onSelect={id => this.dispatchState({ SelectedKey: id })}
                         onSelectPage={page => this.dispatchState({SelectedPage: page})} />
      );
   }
});

var MovieDetails = React.createClass({

   getInitialState() {
      var vmName = this.props.vm || "MovieDetailsVM";
      this.vm = dotnetify.react.connect(vmName, () => this.state, state => this.setState(state));
      return window.vmStates[vmName] || { Movie: {}};
   },
   componentWillUnmount() {
      this.vm.$destroy();
   },
   render() {
      return (
      <Card>
         <CardHeader title={this.state.Movie.Movie} subtitle={this.state.Movie.Year}
                     style={{borderBottom: "solid 1px #e6e6e6"}} />
         <CardText>
         </CardText>
      </Card>
            );
   }
});