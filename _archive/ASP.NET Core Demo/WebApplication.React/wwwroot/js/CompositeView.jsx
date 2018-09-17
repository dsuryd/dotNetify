const Scope = dotnetify.react.Scope;

class CompositeView extends React.Component {
   constructor(props) {
      super(props);
   }
   render() {
      return (
         <div className="container-fluid">
            <div className="header clearfix">
               <h3>Example: Composite View</h3>
            </div>
            <MuiThemeProvider>
               <Scope>
                  <AFITop100 />
               </Scope>
            </MuiThemeProvider>
         </div>
      );
   }
}

class AFITop100 extends React.Component {
   constructor(props, context) {
      super(props, context);
      this.state = this.context.connect("AFITop100VM", this) || {};
   }
   componentWillUnmount() {
      this.vm.$destroy();
   }
   render() {
      const iconMovie = <IconMovie style={{ color: "white" }} />
      return (
         <Scope vm="AFITop100VM">
            <div>
               <div className="row">
                  <div className="col-md-12">
                     <AppBar title="AFI's 100 Greatest American Films of All Time"
                        iconElementLeft={iconMovie}
                        iconStyleLeft={{ marginTop: "20px" }}
                        style={{ marginBottom: "1em" }} />
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
}

AFITop100.contextTypes = { connect: PropTypes.func };

class MovieTable extends React.Component {
   constructor(props, context) {
      super(props, context);
      this.state = this.context.connect("MovieTableVM", this) || {};
   }
   componentWillUnmount() {
      this.vm.$destroy();
   }
   render() {
      return (
         <PaginatedTable colWidths={["1em", "20%", "80px", "40%", "20%"]}
            headers={this.state.Headers}
            data={this.state.Data}
            itemKey={this.state.ItemKey}
            select={this.state.SelectedKey}
            pagination={this.state.Pagination}
            page={this.state.SelectedPage}
            onSelect={id => this.dispatchState({ SelectedKey: id })}
            onSelectPage={page => this.dispatchState({ SelectedPage: page })} />
      );
   }
}

MovieTable.contextTypes = { connect: PropTypes.func };

class MovieDetails extends React.Component {
   constructor(props, context) {
      super(props, context);
      this.state = this.context.connect("MovieDetailsVM", this) || { Movie: {} };
   }
   componentWillUnmount() {
      this.vm.$destroy();
   }
   render() {
      const movie = this.state.Movie || {};
      const casts = movie.Cast ? movie.Cast.split(",") : [];

      return (
         <Card>
            <CardHeader title={movie.Movie} subtitle={movie.Year}
               titleStyle={{ color: "#ff4081", fontSize: "large" }}
               style={{ borderBottom: "solid 1px #e6e6e6" }} />
            <CardText>
               <p>
                  <b>Director</b><br />
                  {movie.Director}<br />
               </p>
               <p>
                  <b>Cast</b><br />
                  {casts.map((cast, idx) => <span key={idx}>{cast}<br /></span>)}
               </p>
            </CardText>
         </Card>
      );
   }
}

MovieDetails.contextTypes = { connect: PropTypes.func };

class MovieFilter extends React.Component {
   constructor(props, context) {
      super(props, context);
      // Combine state from back-end with local state.
      // This can be more concise using Object.assign if not for IE 11 support.
      var state = this.context.connect("MovieFilterVM", this) || {};
      var localState = {
         filters: [],
         filterId: 0,
         filter: "Any",
         operation: "has",
         operations: ["has"],
         text: ""
      };
      for (var prop in localState)
         state[prop] = localState[prop];
      this.state = state;
   }
   componentWillUnmount() {
      this.vm.$destroy();
   }
   render() {
      const movieProps = ["Any", "Rank", "Movie", "Year", "Cast", "Director"];
      const iconApply = <IconFilter />

      const filterProps = movieProps.map((prop, idx) => <MenuItem key={idx} value={prop} primaryText={prop} />)
      const filterOperations = this.state.operations.map((prop, idx) => <MenuItem key={idx} value={prop} primaryText={prop} />)

      const updateFilterDropdown = value => {
         this.setState({ filter: value });
         if (value == "Rank" || value == "Year")
            this.setState({ filter: value, operations: ["equals", ">=", "<="], operation: "equals" });
         else
            this.setState({ filter: value, operations: ["has"], operation: "has" });
      }

      const handleApply = () => {
         let newId = this.state.filterId + 1;
         let filter = { id: newId, property: this.state.filter, operation: this.state.operation, text: this.state.text };
         this.setState({ filterId: newId, filters: [filter, ...this.state.filters], text: "" });
         this.dispatch({ Apply: filter });

         updateFilterDropdown("Any");  // Reset filter dropdown.
      }

      const handleDelete = id => {
         this.dispatch({ Delete: id });
         this.setState({ filters: this.state.filters.filter(filter => filter.id != id) });
      }

      const filters = this.state.filters.map(filter =>
         <Chip key={filter.id} style={{ margin: 4 }} onRequestDelete={() => handleDelete(filter.id)}>
            {filter.property} {filter.operation} {filter.text}
         </Chip>
      );

      return (
         <Card>
            <CardHeader title="Filters" style={{ borderBottom: "solid 1px #e6e6e6" }} />
            <CardText>
               <div className="row">
                  <div className="col-md-4">
                     <SelectField fullWidth={true}
                        value={this.state.filter}
                        onChange={(event, idx, value) => updateFilterDropdown(value)}>
                        {filterProps}
                     </SelectField>
                  </div>
                  <div className="col-md-4">
                     <SelectField fullWidth={true}
                        value={this.state.operation}
                        onChange={(event, index, value) => this.setState({ operation: value })}>
                        {filterOperations}
                     </SelectField>
                  </div>
                  <div className="col-md-4">
                     <TextField id="FilterText" fullWidth={true}
                        value={this.state.text}
                        onChange={event => this.setState({ text: event.target.value })} />
                  </div>
               </div>
               <div className="row">
                  <div className="col-md-12" style={{ display: 'flex', flexWrap: 'wrap' }}>
                     {filters}
                  </div>
               </div>
            </CardText>
            <CardActions>
               <FlatButton label="Apply" icon={iconApply}
                  onClick={handleApply}
                  disabled={this.state.text.length == 0} />
            </CardActions>
         </Card>
      );
   }
}

MovieFilter.contextTypes = { connect: PropTypes.func };