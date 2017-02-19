var CompositeView = React.createClass({
   render() {
      return (
         <div className="container-fluid">
            <div className="header clearfix">
               <h3>Example: Composite View *** UNDER CONSTRUCTION ***</h3>
            </div>
            <MuiThemeProvider>
               <Scope>
                  <AFITop100 />
               </Scope>
            </MuiThemeProvider>

         </div>
     );
   }
});

var AFITop100 = React.createClass({
   contextTypes: { connect: React.PropTypes.func },

   getInitialState() {
      return this.context.connect(this, "AFITop100VM") || {};
   },
   componentWillUnmount() {
      this.vm.$destroy();
   },
   render() {
      return (
         <Scope vm="AFITop100VM">
            <div>
               <div className="row">
                  <div className="col-md-12">
                     <AppBar title="AFI's 100 Greatest American Films of All Time" style={{marginBottom: "1em"}} />
                  </div>
               </div>
               <div className="row">
                  <div className="col-md-8">
                     <MovieTable />
                  </div>
                  <div className="col-md-4">
                     <MovieDetails />
                  </div>
               </div>
            </div>
         </Scope>
      );
   }
});

var MovieTable = React.createClass({
   contextTypes: { connect: React.PropTypes.func },

   getInitialState() {
      return this.context.connect(this, "MovieTableVM") || {};
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
   contextTypes: { connect: React.PropTypes.func },

   getInitialState() {
      return this.context.connect(this, "MovieDetailsVM") || { Movie: {} };
   },
   componentWillUnmount() {
      this.vm.$destroy();
   },
   render() {
      const movie = this.state.Movie;
      return (
         <Card>
            <CardHeader title={movie.Movie} subtitle={movie.Year}
                        style={{borderBottom: "solid 1px #e6e6e6"}} />
            <CardText>
            </CardText>
         </Card>
      );
   }
});