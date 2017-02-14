var GridView = React.createClass({

   getInitialState() {
      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("GridViewVM", () => this.state, state => this.setState(state));
      this.dispatchState = this.vm.$dispatchState.bind(this.vm);

      // This is for dispatching to the back-end without updating the component state.
      this.dispatch = this.vm.$dispatch.bind(this.vm);

      // This is for dispatching a list item to the back-end and update the component state.
      // Use $setItemKey to register the property name of the item key.
      this.vm.$setItemKey({ Employees: "Id" });
      this.dispatchListState = this.vm.$dispatchListState.bind(this.vm);

      // This component's JSX was loaded along with the VM's initial state for faster rendering.
      return Object.assign({ openWizard: false }, window.vmStates.GridViewVM);
   },
   componentWillUnmount() {
      this.vm.$destroy();
   },
   render() {
      const handleFinish = () => {
         this.setState({ openWizard: false });
         this.dispatch({ Update: value });
      }
      const wizard = (isOpen) => {
         if (isOpen)
            return <EditWizard open={true} data={this.state.EmployeeDetails}
                               onFinish={handleFinish}
                               onCancel={() => this.setState({ openWizard: false })} />
      }

      return (
         <div className="container-fluid">
            <div className="header clearfix">
               <h3>Example: Grid View *** STILL UNDER CONSTRUCTION ***</h3>
            </div>
            <MuiThemeProvider>
               <div>
                  <SearchBox label="Type a name" />
                  <EmployeeTable data={this.state.Employees} defaultSelection={this.state.SelectedId}
                                 onSelect={id => this.dispatchState({ SelectedId: id })}
                                 onEdit={() => this.setState({ openWizard: true })}
                                 onRemove={id => this.dispatch({ Remove: id })} />
                  {wizard(this.state.openWizard)}

               </div>
            </MuiThemeProvider>
         </div>
      );
   }
});

var EditWizard = React.createClass({
   getInitialState() {
      return {
         employee: this.props.data,
         step: 0,
         maxStep: 2,
      }
   },
   render() {
      const handleBack = () => this.setState({ step: this.state.step - 1 });
      const handleNext = () => this.setState({ step: this.state.step + 1 });
      const handleFinish = () => this.props.onFinish(this.state.employee);

      const actions = [
         <FlatButton label="Back" onClick={handleBack} disabled={this.state.step == 0} />,
         <FlatButton label="Next" onClick={handleNext} disabled={this.state.step == this.state.maxStep } />,
         <FlatButton label="Finish" primary={true} onClick={handleFinish} disabled={this.state.step != this.state.maxStep} />,
         <FlatButton label="Cancel" onClick={() => this.props.onCancel()} />
      ];

      const content = (step) => {
         switch (step) {
            case 0:
               return <TextField id="FirstName" floatingLabelText="First Name" value={this.state.employee.FirstName}></TextField>

            case 1:
               return <TextField id="LastName" floatingLabelText="Last Name" value={this.state.employee.LastName}></TextField>

            case 2:
               return <div>{this.state.employee.FirstName} {this.state.employee.LastName}</div>
         }
      }

      return (
         <Dialog open={this.props.open} actions={actions}>
            <Stepper activeStep={this.state.step}>
               <Step><StepLabel>First Name</StepLabel></Step>
               <Step><StepLabel>Last Name</StepLabel></Step>
               <Step><StepLabel>Confirm</StepLabel></Step>
            </Stepper>
            {content(this.state.step)}
         </Dialog>
      );
   }
});

var EmployeeTable = React.createClass({
   getInitialState() {
      return {
         selectedId: this.props.defaultSelection
      }
   },
   render() {
      const lastColWidth = { width: "16.5em" }
      const fontStyle = { fontSize: "8pt" };
      const iconEdit = <IconEdit style={{ width: 20, height: 20 }} color='#8B8C8D' />
      const iconDelete = <IconDelete style={{ width: 20, height: 20 }} color='#8B8C8D' />

      const handleRowSelection = rows => {
         if (rows.length > 0)
            handleSelect(this.props.data[rows[0]].Id);
      }

      const handleSelect = id => {
         if (id != this.state.selectedId) {
            this.setState({ selectedId: id });
            this.props.onSelect(id);
         }
      }

      const employees = this.props.data.map((employee, index) =>
         <TableRow key={employee.Id} selected={this.state.selectedId === employee.Id}>
            <TableRowColumn><div>{employee.FirstName}</div></TableRowColumn>
            <TableRowColumn><div>{employee.LastName}</div></TableRowColumn>
            <TableRowColumn style={lastColWidth}>
               <FlatButton label="Edit" labelStyle={fontStyle} icon={iconEdit} onClick={() => this.props.onEdit(employee.Id)} />
               <FlatButton label="Remove" labelStyle={fontStyle} icon={iconDelete} onClick={() => this.props.onRemove(employee.Id)} />
            </TableRowColumn>
         </TableRow>
      );

      return (
         <Table selectable={true} onRowSelection={handleRowSelection}>
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

var SearchBox = React.createClass({
   getInitialState() {
      return {
         searchText: ""
      }
   },
   render() {
      const handleChange = event => {
         this.setState({ searchText: event.target.value });
         this.props.onChange(this.state.searchText);
      }

      return (
         <div>
            <IconSearch style={{ width: 20, height: 20 }} color='#8B8C8D' />
            <TextField id="SearchBox" floatingLabelText={this.props.label}
                       value={this.state.searchText} onChange={handleChange} />
         </div>
      );
   }
});
