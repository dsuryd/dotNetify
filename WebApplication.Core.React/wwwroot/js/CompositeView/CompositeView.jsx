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
      const iconMovie = <IconMovie style={{ color: "white" }} />
      return (
         <Scope vm="AFITop100VM">
            <div>
               <div className="row">
                  <div className="col-md-12">
                     <AppBar title="AFI's 100 Greatest American Films of All Time"
                             iconElementLeft={iconMovie}
                             iconStyleLeft={{marginTop: "20px"}}
                             style={{marginBottom: "1em"}} />
                  </div>
               </div>
               <div className="row">
                  <div className="col-md-8">
                     <Scope vm="FilterableMovieTableVM">
                        <MovieTable />
                     </Scope>
                  </div>
                  <div className="col-md-4">
                     <MovieDetails />
                     <br />
                     <Scope vm="FilterableMovieTableVM">
                        <MovieFilter />
                     </Scope>
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
      const movie = this.state.Movie || {};
      const casts = movie.Cast ? movie.Cast.split(",") : [];

      return (
         <Card>
            <CardHeader title={movie.Movie} subtitle={movie.Year}
                        titleStyle={{color: "#ff4081", fontSize: "large"}}
                        style={{borderBottom: "solid 1px #e6e6e6"}} />
            <CardText>
               <p>
                  <b>Director</b><br />
                  {movie.Director}<br />
               </p>
                <p>
                  <b>Cast</b><br />
                   {casts.map((cast, idx) => <span key={idx }>{cast}<br /></span>)}
                </p>
            </CardText>
         </Card>
      );
   }
});

var MovieFilter = React.createClass({
   contextTypes: { connect: React.PropTypes.func },
   getInitialState() {
      return this.context.connect(this, "MovieFilterVM") || {
         filter: "Any",
         operation: "has",
         operations: ["has"],
         text: ""
      };
   },
   componentWillUnmount() {
      this.vm.$destroy();
   },
   render() {
      const movieProps = ["Any", "Rank", "Movie", "Year", "Cast", "Director"];

      const handleChangeFilter = (event, idx, value) => {
         this.setState({ filter: value });
         if (value == "Rank" || value == "Year")
            this.setState({ operations: ["equals", ">=", "<="], operation: "equals" });
         else
            this.setState({ operations: ["has"], operation: "has" });
      }
      return (
         <Card>
            <CardHeader title="Filters" style={{borderBottom: "solid 1px #e6e6e6"}} />
            <CardText>
               <div className="row">
                  <div className="col-md-4">
                     <SelectField fullWidth={true}
                                  value={this.state.filter}
                                  onChange={handleChangeFilter}>
                        {movieProps.map((prop, idx) => <MenuItem key={idx} value={prop} primaryText={prop } />)}
                     </SelectField>
                  </div>
                  <div className="col-md-4">
                     <SelectField fullWidth={true}
                                  value={this.state.operation}
                                  onChange={(event, index, value) => this.setState({ operation: value })}>
                        {this.state.operations.map((prop, idx) => <MenuItem key={idx} value={prop} primaryText={prop } />)}
                     </SelectField>
                  </div>
                   <div className="col-md-4">
                      <TextField id="TextField" fullWidth={true}
                                 value={this.state.text}
                                 onChange={event => this.setState({ text: event.target.value })} />
                  </div>
               </div>
            </CardText>
            <CardActions>
               <FlatButton label="Apply" icon={<IconFilter />} 
                           onClick={() => this.dispatch({Filter: {property: this.state.filter, operation: this.state.operation, text: this.state.text}})} />
            </CardActions>
         </Card>
      );
   }
});