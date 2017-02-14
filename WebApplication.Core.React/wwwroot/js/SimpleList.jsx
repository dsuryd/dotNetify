var SimpleList = React.createClass({

   getInitialState() {
      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("SimpleListVM", () => this.state, state => this.setState(state));
      this.dispatchState = this.vm.$dispatchState.bind(this.vm);

      // This is for dispatching to the back-end without updating the component state.
      this.dispatch = this.vm.$dispatch.bind(this.vm);

      // This is for dispatching a list item to the back-end and update the component state.
      // Use $setItemKey to register the property name of the item key.
      this.vm.$setItemKey({ Employees: "Id" });
      this.dispatchListState = this.vm.$dispatchListState.bind(this.vm);

      // This component's JSX was loaded along with the VM's initial state for faster rendering.
      return window.vmStates.SimpleListVM;
   },
   componentWillUnmount() {
      this.vm.$destroy();
   },
   render() {
      return (
         <div className="container-fluid">
            <div className="header clearfix">
               <h3>Example: Simple List</h3>
            </div>
            <MuiThemeProvider>
               <div>
                  <AddNameBox onAdd={value => this.dispatch({ Add: value })} />
                  <EmployeeTable data={this.state.Employees}
                                 onUpdate={value => this.dispatchListState({ Employees: value })}
                                 onRemove={id => this.dispatch({ Remove: id })} />
               </div>
            </MuiThemeProvider>
         </div>
      );
   }
});

var EmployeeTable = React.createClass({
   render() {
      const lastColWidth = { width: "10em" }
      const iconDelete = <IconDelete style={{ width: 20, height: 20 }} color='#8B8C8D' />

      const employees = this.props.data.map(employee =>
         <TableRow key={employee.Id}>
            <TableRowColumn><InlineEdit text={employee.FirstName} onChange={value => this.props.onUpdate({ Id: employee.Id, FirstName:value })} /></TableRowColumn>
            <TableRowColumn><InlineEdit text={employee.LastName} onChange={value => this.props.onUpdate({ Id: employee.Id, LastName: value })} /></TableRowColumn>
            <TableRowColumn style={lastColWidth}>
               <FlatButton label="Remove" labelStyle={{fontSize: "8pt"}} icon={iconDelete} onClick={() => this.props.onRemove(employee.Id)}  />
            </TableRowColumn>
         </TableRow>
      );

      return (
      <Table selectable={false}>
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
      );
   }
});

var AddNameBox = React.createClass({
   getInitialState() {
      return {
         fullName: ""
      }
   },
   render() {
      const handleAdd = () => {
         if (this.state.fullName) {
            this.props.onAdd(this.state.fullName);
            this.setState({ fullName: "" });
         }
      };
      return (
         <div>
            <TextField id="FullName" floatingLabelText="Full name"
                       value={this.state.fullName}
                       onChange={event => this.setState({ fullName: event.target.value })} />
            <RaisedButton style={{ marginLeft: "1em" }} label="Add" primary={true} onClick={handleAdd } />
         </div>
      );
   }
});

var InlineEdit = React.createClass({
   getInitialState() {
      return {
         edit: false,
         value: this.props.text
      }
   },
   render() {
      const handleClick = event => {
         event.stopPropagation();
         if (!this.state.edit) {
            this.setState({ value: this.props.text });
            this.setState({ edit: true });
         }
      };

      const handleBlur = event => {
         this.setState({ edit: false });
         if (this.state.value.length > 0 && this.state.value != this.props.text)
            this.props.onChange(this.state.value);
         else
            this.setState({ value: this.props.text });
      }

      const setFocus = input => { if (input) input.focus(); }

      var elem;
      if (!this.state.edit)
         elem = <div style={{ minHeight: "2em" }} onClick={handleClick }>{this.props.text}</div>
      else
         elem = <TextField id="EditField" ref={input => setFocus(input)}
                           value={this.state.value}
                           onClick={handleClick}
                           onBlur={handleBlur}
                           onChange={event => this.setState({ value: event.target.value })} />
      return (
         <div>{elem}</div>
      );
   }
});