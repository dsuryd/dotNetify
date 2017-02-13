var SimpleList = React.createClass({

   getInitialState() {
      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("SimpleListVM", () => this.state, state => this.setState(state));
      this.dispatchState = this.vm.$dispatchState.bind(this.vm);

      // This is for dispatching to the back-end without updating the component state.
      this.dispatch = this.vm.$dispatch.bind(this.vm);

      // This is for dispatching a list item to the back-end and update the component state.
      // Use $setItemKey to register the property name of the item key.
      this.vm.$setItemKey({ "Employees": "Id" });
      this.dispatchListState = this.vm.$dispatchListState.bind(this.vm);

      // This component's JSX was loaded along with the VM's initial state for faster rendering.
      return window.vmStates.SimpleListVM;
   },
   componentWillUnmount() {
      this.vm.$destroy();
   },
   render() {
      const lastColWidth = { width: "10em" }

      const employees = this.state.Employees.map(employee =>
         <EmployeeTableRow key={employee.Id} data={employee} lastColWidth={lastColWidth}
                           onUpdate={value => this.dispatchListState({ "Employees": value })}
                           onRemove={id => this.dispatchState({ "Remove": id })} />
         );

      return (
         <div className="container-fluid">
            <div className="header clearfix">
               <h3>Example: Simple List</h3>
            </div>
            <MuiThemeProvider>
            <div>
               <AddNameBox onAdd={value => this.dispatch({ "Add": value })} />
               <Table>
                  <TableHeader displaySelectAll={false} adjustForCheckbox={false}>
                     <TableRow>
                        <TableHeaderColumn>First Name</TableHeaderColumn>
                        <TableHeaderColumn>Last Name</TableHeaderColumn>
                        <TableHeaderColumn style={lastColWidth}></TableHeaderColumn>
                     </TableRow>
                  </TableHeader>
                  <TableBody displayRowCheckbox={false} showRowHover={true}>
                     {employees}
                  </TableBody>
               </Table>
            </div>
            </MuiThemeProvider>
         </div>
      );
   }
});

var EmployeeTableRow = React.createClass({
   render() {
      const employee = this.props.data;
      const iconDelete = <IconDelete style={{ width: 20, height: 20 }} color='#8B8C8D' />

      return (
         <TableRow>
            <TableRowColumn><InlineEdit text={employee.FirstName} onChange={value => this.props.onUpdate({ "Id": employee.Id, "FirstName":value })} /></TableRowColumn>
            <TableRowColumn><InlineEdit text={employee.LastName} onChange={value => this.props.onUpdate({ "Id": employee.Id, "LastName": value })} /></TableRowColumn>
            <TableRowColumn style={this.props.lastColWidth}>
               <FlatButton label="Remove" labelStyle={{fontSize: "8pt"}}
                           onClick={() => this.props.onRemove(employee.Id)}
                           icon={iconDelete} />
            </TableRowColumn>
         </TableRow>
         );
   }
});

var AddNameBox = React.createClass({
   getInitialState() {
      return {
         FullName: ""
      }
   },
   render() {
      const handleAdd = () => {
         if (this.state.FullName) {
            this.props.onAdd(this.state.FullName);
            this.setState({ "FullName": "" });
         }
      };
      return (
         <div>
            <TextField id="FullName"
                       floatingLabelText="Full name"
                       value={this.state.FullName}
                       onChange={event => this.setState({ "FullName": event.target.value })} />
            <RaisedButton style={{ "marginLeft": "1em" }} label="Add" primary={true} onClick={handleAdd } />
         </div>
      );
   }
});

var InlineEdit = React.createClass({
   getInitialState() {
      return {
         Edit: false,
         Value: this.props.text
      }
   },
   render() {
      const handleClick = event => {
         event.stopPropagation();
         if (!this.state.Edit) {
            this.setState({ "Value": this.props.text });
            this.setState({ "Edit": true });
         }
      };

      const handleBlur = event => {
         this.setState({ "Edit": false });
         if (this.state.Value.length > 0 && this.state.Value != this.props.text)
            this.props.onChange(this.state.Value);
         else
            this.setState({ "Value": this.props.text });
      }

      const setFocus = input => { if (input) input.focus(); }

      var elem;
      if (!this.state.Edit)
         elem = <div style={{ "minHeight": "2em" }} onClick={handleClick }>{this.props.text}</div>
      else
         elem = <TextField id="EditField" ref={input => setFocus(input)}
                           value={this.state.Value}
                           onClick={handleClick}
                           onBlur={handleBlur}
                           onChange={event => this.setState({ "Value": event.target.value })} />
      return (
         <div>
            {elem}
         </div>
      );
   }
});