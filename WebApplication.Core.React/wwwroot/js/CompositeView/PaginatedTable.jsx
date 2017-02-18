var PaginatedTable = React.createClass({

   getInitialState() {

      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect(this.props.vm, () => this.state, state => this.setState(state));

      // Functions to dispatch state to the back-end.
      this.dispatch = state => this.vm.$dispatch(state);
      this.dispatchState = state => {
         this.setState(state);
         this.vm.$dispatch(state);
      }

      // This component's JSX was loaded along with the VM's initial state for faster rendering.
      return window.vmStates[this.props.vm] || {
         Data: [],
         Headers: [],
         Pagination: []
      };
   },
   componentWillUnmount() {
      this.vm.$destroy();
   },

   render() {
      const handleRowSelection = rows => {
         if (rows.length > 0)
            handleSelect(this.props.data[rows[0]].Id);
      }

      const handleSelect = id => {
         if (id != this.props.select)
            this.props.onSelect(id);
      }

      const colWidth = i => { return { width: this.props.colWidths[i] }; }

      const columns = data => this.state.Headers.map((header, index) =>
         <TableRowColumn key={header} style={colWidth(index)}>{data[header]}</TableRowColumn>
      );

      const rows = this.state.Data.map((data, index) =>
         <TableRow key={data[this.state.ItemKey]} selected={this.state.SelectedKey == data[this.state.ItemKey]}>
            {columns(data)}
         </TableRow>
      );

      const headers = this.state.Headers.map((header, index) =>
         <TableHeaderColumn key={header} style={colWidth(index)}>{header}</TableHeaderColumn>
      );

      return (
         <div>
         <Table selectable={true} onRowSelection={handleRowSelection}>
            <TableHeader displaySelectAll={false} adjustForCheckbox={false}>
               <TableRow>
                  {headers}
               </TableRow>
            </TableHeader>
            <TableBody displayRowCheckbox={false} showRowHover={true}>
               {rows}
            </TableBody>
         </Table>
         <Pagination style={{marginTop: "1em", float: "right"}}
                     pages={this.state.Pagination}
                     select={this.state.SelectedPage}
                     onSelect={page => this.dispatchState({SelectedPage: page})} />
         </div>
      );
   }
});

var Pagination = React.createClass({
   render() {
      const pageButtons = this.props.pages.map(page =>
         <Paper key={page} style={{display: "inline", padding: ".5em 0"}}>
            <FlatButton style={{minWidth: "1em"}}
                        label={page}
                        disabled={this.props.select == page}
                        onClick={() => this.props.onSelect(page)} />
         </Paper>
      );

      return (
         <div style={this.props.style}>{pageButtons}</div>
      );
   }
});