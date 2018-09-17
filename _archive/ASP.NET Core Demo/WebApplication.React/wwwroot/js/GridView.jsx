class GridView extends React.Component {

   constructor(props) {
      super(props);
      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("GridViewVM", this);

      // Functions to dispatch state to the back-end.
      this.dispatch = state => this.vm.$dispatch(state);
      this.dispatchState = state => {
         this.setState(state);
         this.vm.$dispatch(state);
      }

      // This component's JSX was loaded along with the VM's initial state for faster rendering.
      this.state = window.vmStates.GridViewVM || {};
      this.state["openWizard"] = false;
   }
   componentWillUnmount() {
      this.vm.$destroy();
   }
   render() {
      const handleFinish = value => {
         this.setState({ openWizard: false });
         this.dispatch({ Update: value });
      }

      const wizard = (isOpen) => {
         if (isOpen)
            return <EditWizard open={true}
               strings={this.state.LocalizedStrings}
               employeeDetails={this.state.Details}
               reportToSearchResult={this.state.ReportToSearchResult}
               reportToError={this.state.ReportToError}
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

                     <div className="col-md-12">
                        <AppBar style={{ marginBottom: "1em" }}
                           iconElementLeft={<SearchBox strings={this.state.LocalizedStrings} onChange={value => this.dispatch({ EmployeeSearch: value })} />}
                           iconElementRight={<LanguageToggle onToggle={code => this.dispatch({ CultureCode: code })} />} />
                     </div>
                  </div>
                  <div className="row">
                     <div className="col-md-8">
                        <EmployeeTable data={this.state.Employees}
                           strings={this.state.LocalizedStrings}
                           select={this.state.SelectedId}
                           onSelect={id => this.dispatchState({ SelectedId: id })} />
                        <Pagination style={{ marginTop: "1em", float: "right" }}
                           pages={this.state.Pagination}
                           select={this.state.SelectedPage}
                           onSelect={page => this.dispatchState({ SelectedPage: page })} />
                     </div>
                     <div className="col-md-4">
                        <EmployeeDetails data={this.state.Details}
                           strings={this.state.LocalizedStrings}
                           onEdit={() => this.setState({ openWizard: true })} />
                     </div>
                  </div>
                  {wizard(this.state.openWizard)}
               </div>
            </MuiThemeProvider>
         </div>
      );
   }
}

class SearchBox extends React.Component {
   constructor(props) {
      super(props);
      this.state = { searchText: "" }
   }
   render() {
      const handleChange = event => {
         this.setState({ searchText: event.target.value });
         this.props.onChange(event.target.value);
      }

      return (
         <div style={{ padding: "0 1em", borderRadius: "4px", backgroundColor: "#11cde5" }}>
            <IconSearch style={{ width: 20, height: 20 }} />
            <TextField id="SearchBox" hintText={this.props.strings.SearchLabel}
               value={this.state.searchText} onChange={handleChange} />
         </div>
      );
   }
}

class LanguageToggle extends React.Component {
   constructor(props) {
      super(props);
      this.state = {
         code: "en-US",
         language: "English"
      }
   }
   render() {
      const handleToggle = (event, checked) => {
         var code = !checked ? "en-US" : "fr-FR";
         this.setState({ code: code });
         this.setState({ language: !checked ? "English" : "Français" });
         this.props.onToggle(code);
      }

      return (
         <Toggle style={{ marginTop: "1em", width: "7em" }}
            trackSwitchedStyle={{ backgroundColor: "#e0e0e0" }}
            thumbSwitchedStyle={{ backgroundColor: "#11cde5" }}
            onToggle={handleToggle}
            label={this.state.language}
            labelStyle={{ color: "white", fontSize: "medium" }}
         />
      );
   }
}

class EmployeeTable extends React.Component {
   constructor(props) {
      super(props);
   }
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
                  <TableHeaderColumn>{this.props.strings.FirstName}</TableHeaderColumn>
                  <TableHeaderColumn>{this.props.strings.LastName}</TableHeaderColumn>
               </TableRow>
            </TableHeader>
            <TableBody displayRowCheckbox={false} showRowHover={true}>
               {employees}
            </TableBody>
         </Table>
      );
   }
}

class Pagination extends React.Component {
   constructor(props) {
      super(props);
   }
   render() {
      const pageButtons = this.props.pages.map(page =>
         <Paper key={page} style={{ display: "inline", padding: ".5em 0" }}>
            <FlatButton style={{ minWidth: "1em" }}
               label={page}
               disabled={this.props.select == page}
               onClick={() => this.props.onSelect(page)} />
         </Paper>
      );

      return (
         <div style={this.props.style}>{pageButtons}</div>
      );
   }
}

class EmployeeDetails extends React.Component {
   constructor(props) {
      super(props);
   }
   render() {
      const employee = this.props.data;
      const iconEdit = <IconEdit style={{ width: 20, height: 20 }} color="#8b8c8d" />
      const iconPhone = <IconPhone style={{ width: 24, height: 24 }} />

      const reportsTo = name => name != null ? this.props.strings.ReportTo + " " + name : "";

      const editButton = () => {
         if (this.props.data.Id > 0)
            return <FlatButton label={this.props.strings.EditLabel} icon={iconEdit} onClick={this.props.onEdit} />
      }

      return (
         <Card>
            <CardHeader title={employee.FullName} subtitle={reportsTo(employee.ReportToName)}
               style={{ borderBottom: "solid 1px #e6e6e6" }} subtitleColor="#00abc4" />
            <CardText>
               {iconPhone}<span style={{ verticalAlign: "super" }}>{employee.Phone}</span>
            </CardText>
            <CardActions>
               {editButton()}
            </CardActions>
         </Card>
      );
   }
}

class EditWizard extends React.Component {
   constructor(props) {
      super(props);
      this.state = {
         firstName: this.props.employeeDetails.FirstName,
         lastName: this.props.employeeDetails.LastName,
         reportToName: this.props.employeeDetails.ReportToName,
         reportTo: this.props.employeeDetails.ReportTo,
         step: 0,
         maxStep: 2,
         disableNext: false
      }
   }
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
         <FlatButton label={this.props.strings.Back} onClick={handleBack} disabled={this.state.step == 0} />,
         <FlatButton label={this.props.strings.Next} onClick={handleNext} disabled={this.state.step == this.state.maxStep || this.state.disableNext} />,
         <FlatButton label={this.props.strings.Finish} primary={true} onClick={handleFinish} disabled={this.state.step != this.state.maxStep} />,
         <FlatButton label={this.props.strings.Cancel} onClick={() => this.props.onCancel()} />
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
                     <TextField id="FirstName" floatingLabelText={this.props.strings.FirstName}
                        value={this.state.firstName}
                        onChange={event => this.setState({ firstName: event.target.value })} />
                     <TextField id="LastName" floatingLabelText={this.props.strings.LastName}
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
                     floatingLabelText={this.props.strings.ReportTo}
                     hintText={this.props.strings.ReportToHintText}
                     filter={AutoComplete.caseInsensitiveFilter}
                     searchText={initialText}
                     errorText={this.props.strings[this.props.reportToError]}
                     dataSource={reportToSearchResult}
                     onUpdateInput={handleUpdate} />
               );
            case 2:
               const paperStyle = { display: "inline", padding: ".5em 1em", backgroundColor: "#e6e6e6" }
               const reportToName = this.state.reportToName != null ? this.state.reportToName : this.props.strings.NoOne;
               return (
                  <div style={{ paddingTop: "2.2em" }}>
                     <Paper style={paperStyle}>{this.state.firstName} {this.state.lastName}</Paper>
                     <span style={{ margin: "0 1em" }}>{this.props.strings.ReportTo} </span>
                     <Paper style={paperStyle}>{reportToName}</Paper>
                  </div>
               );
         }
      }

      return (
         <Dialog open={this.props.open} actions={actions}>
            <Stepper activeStep={this.state.step}>
               <Step><StepLabel>{this.props.strings.Name}</StepLabel></Step>
               <Step><StepLabel>{this.props.strings.Manager}</StepLabel></Step>
               <Step><StepLabel>{this.props.strings.Confirm}</StepLabel></Step>
            </Stepper>
            <div style={{ height: "4em" }}>
               {content(this.state.step)}
            </div>
         </Dialog>
      );
   }
}
