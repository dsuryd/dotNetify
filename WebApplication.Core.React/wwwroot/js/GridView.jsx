var GridView = React.createClass({

   getInitialState() {
      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("GridViewVM", () => this.state, state => this.setState(state));
      this.dispatchState = this.vm.$dispatchState.bind(this.vm);

      // This is for dispatching to the back-end without updating the component state.
      this.dispatch = this.vm.$dispatch.bind(this.vm);

      // This component's JSX was loaded along with the VM's initial state for faster rendering.
      return Object.assign({ openWizard: false }, window.vmStates.GridViewVM);
   },
   componentWillUnmount() {
      this.vm.$destroy();
   },
   render() {
      const handleFinish = value => {
         this.setState({ openWizard: false });
         this.dispatch({ Update: value });
      }

      const wizard = (isOpen) => {
         if (isOpen)
            return <EditWizard open={true}
                               employeeDetails={this.state.Details}
                               reportToSearchResult={this.state.ReportToSearchResult}
                               reportToErrorText={this.state.ReportToErrorText}
                               onReportToChange={value => this.dispatch({ ReportToSearch: value })}
                               onFinish={handleFinish}
                               onCancel={() => this.setState({ openWizard: false })} />
      }

      return (
         <div className="container-fluid">
            <div className="header clearfix">
               <h3>Example: Grid View</h3>
            </div>
            <MuiThemeProvider>
               <div>
                  <div className="row">
                     <div className="col-md-8">
                        <SearchBox label="Type a name" onChange={value => this.dispatch({ EmployeeSearch: value })} />
                     </div>
                  </div>
                  <div className="row">
                     <div className="col-md-8">
                        <EmployeeTable data={this.state.Employees} select={this.state.SelectedId}
                                       onSelect={id => this.dispatchState({ SelectedId: id })} />
                     </div>
                     <div className="col-md-4">
                        <EmployeeDetails data={this.state.Details}
                                         onEdit={() => this.setState({ openWizard: true })} />
                     </div>
                  </div>
                  {wizard(this.state.openWizard)}
               </div>
            </MuiThemeProvider>
         </div>
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
         this.props.onChange(event.target.value);
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

var EmployeeTable = React.createClass({
   render() {
      const handleRowSelection = rows => {
         if (rows.length > 0)
            handleSelect(this.props.data[rows[0]].Id);
      }

      const handleSelect = id => {
         if (id != this.props.select)
            this.props.onSelect(id);
      }

      const employees = this.props.data.map((employee, index) =>
         <TableRow key={employee.Id} selected={this.props.select == employee.Id}>
            <TableRowColumn><div>{employee.FirstName}</div></TableRowColumn>
            <TableRowColumn><div>{employee.LastName}</div></TableRowColumn>
         </TableRow>
      );

      return (
         <Table selectable={true} onRowSelection={handleRowSelection}>
            <TableHeader displaySelectAll={false} adjustForCheckbox={false}>
               <TableRow>
                  <TableHeaderColumn>First Name</TableHeaderColumn>
                  <TableHeaderColumn>Last Name</TableHeaderColumn>
               </TableRow>
            </TableHeader>
            <TableBody displayRowCheckbox={false} showRowHover={true}>
               {employees}
            </TableBody>
         </Table>
      );
   }
});

var EmployeeDetails = React.createClass({
   render() {
      const employee = this.props.data;
      const iconEdit = <IconEdit style={{ width: 20, height: 20 }} color="#8b8c8d" />
      const iconPhone = <IconPhone style={{ width: 24, height: 24 }} />

      const reportsTo = name => name != null ? "reports to: " + name : "";

      const editButton = () => {
         if (this.props.data.Id > 0)
            return <FlatButton label="Edit" icon={iconEdit} onClick={this.props.onEdit } />
      }

      return (
         <Card>
            <CardHeader title={employee.FullName} subtitle={reportsTo(employee.ReportToName)}
                        style={{borderBottom: "solid 1px #e6e6e6"}} subtitleColor="#00abc4" />
            <CardText>
               {iconPhone}<span style={{ verticalAlign: "super" } }>{employee.Phone}</span>
            </CardText>
            <CardActions>
               {editButton()}
            </CardActions>
         </Card>
         );
   }
});

var EditWizard = React.createClass({
   getInitialState() {
      return {
         firstName: this.props.employeeDetails.FirstName,
         lastName: this.props.employeeDetails.LastName,
         reportToName: this.props.employeeDetails.ReportToName,
         reportTo: this.props.employeeDetails.ReportTo,
         step: 0,
         maxStep: 2,
         disableNext: false
      }
   },
   render() {
      const handleBack = () => this.setState({ step: this.state.step - 1 });
      const handleNext = () => this.setState({ step: this.state.step + 1 });
      const handleFinish = () => this.props.onFinish({
         Id: this.props.employeeDetails.Id,
         FirstName: this.state.firstName,
         LastName: this.state.lastName,
         ReportTo: this.state.reportTo
      });

      const actions = [
         <FlatButton label="Back" onClick={handleBack} disabled={this.state.step == 0} />,
         <FlatButton label="Next" onClick={handleNext} disabled={this.state.step == this.state.maxStep || this.state.disableNext} />,
         <FlatButton label="Finish" primary={true} onClick={handleFinish} disabled={this.state.step != this.state.maxStep} />,
         <FlatButton label="Cancel" onClick={() => this.props.onCancel()} />
      ];

      const handleUpdateReportTo = value => {
         this.state.details.ReportToName = value;
         this.props.onReportToChange(value);
      }

      const content = (step) => {
         switch (step) {
            case 0:
               return (
                  <div>
                     <TextField id="FirstName" floatingLabelText="First Name"
                                value={this.state.firstName}
                                onChange={event => this.setState({ firstName: event.target.value })} />
                     <TextField id="LastName" floatingLabelText="Last Name"
                                value={this.state.lastName}
                                onChange={event => this.setState({ lastName: event.target.value })} />
                  </div>
               );
            case 1:
               const reportToSearchResult = this.props.reportToSearchResult.map(i => i.Name);
               const initialText = this.state.reportToName;

               const handleUpdate = value => {
                  var match = value.length > 0 ?
                     this.props.reportToSearchResult.filter(i => i.Name.toUpperCase() == value.toUpperCase())
                     : { Id: 0, Name: "" };
                  this.setState({ reportTo: match.length > 0 ? match[0].Id : -1 });
                  this.setState({ reportToName: match.length > 0 ? match[0].Name : value });
                  this.setState({ disableNext: match.length == 0 });

                  this.props.onReportToChange(value);
               }

               return (
                  <AutoComplete id="AutoComplete"
                                floatingLabelText="Report To"
                                hintText="Type the manager name here"
                                filter={AutoComplete.caseInsensitiveFilter}
                                searchText={initialText}
                                errorText={this.props.reportToErrorText}
                                dataSource={reportToSearchResult}
                                onUpdateInput={handleUpdate} />
               );
            case 2:
               const paperStyle = { display: "inline", padding: ".5em 1em", backgroundColor: "#e6e6e6" }
               const reportToName = this.state.reportToName.length > 0 ? this.state.reportToName : "no one";
               return (
                  <div style={{paddingTop: "2.2em"}}>
                     <Paper style={paperStyle}>{this.state.firstName} {this.state.lastName}</Paper>
                     <span style={{margin: "0 1em"}}>reports to </span>
                     <Paper style={paperStyle}>{reportToName}</Paper>
                  </div>
               );
         }
      }

      return (
         <Dialog open={this.props.open} actions={actions }>
            <Stepper activeStep={this.state.step}>
               <Step><StepLabel>Name</StepLabel></Step>
               <Step><StepLabel>Manager</StepLabel></Step>
               <Step><StepLabel>Confirm</StepLabel></Step>
            </Stepper>
            <div style={{height: "4em"}}>
               {content(this.state.step)}
            </div>
         </Dialog>
      );
   }
});
