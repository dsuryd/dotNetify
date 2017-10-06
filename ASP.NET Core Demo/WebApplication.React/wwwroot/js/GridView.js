"use strict";

var GridView = React.createClass({
   displayName: "GridView",

   getInitialState: function getInitialState() {
      var _this = this;

      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("GridViewVM", this);

      // Functions to dispatch state to the back-end.
      this.dispatch = function (state) {
         return _this.vm.$dispatch(state);
      };
      this.dispatchState = function (state) {
         _this.setState(state);
         _this.vm.$dispatch(state);
      };

      // This component's JSX was loaded along with the VM's initial state for faster rendering.
      var state = window.vmStates.GridViewVM || {};
      state["openWizard"] = false;
      return state;
   },
   componentWillUnmount: function componentWillUnmount() {
      this.vm.$destroy();
   },
   render: function render() {
      var _this2 = this;

      var handleFinish = function handleFinish(value) {
         _this2.setState({ openWizard: false });
         _this2.dispatch({ Update: value });
      };

      var wizard = function wizard(isOpen) {
         if (isOpen) return React.createElement(EditWizard, { open: true,
            strings: _this2.state.LocalizedStrings,
            employeeDetails: _this2.state.Details,
            reportToSearchResult: _this2.state.ReportToSearchResult,
            reportToError: _this2.state.ReportToError,
            onReportToChange: function (value) {
               return _this2.dispatch({ ReportToSearch: value });
            },
            onFinish: handleFinish,
            onCancel: function () {
               return _this2.setState({ openWizard: false });
            } });
      };

      return React.createElement(
         "div",
         { className: "container-fluid" },
         React.createElement(
            "div",
            { className: "header clearfix" },
            React.createElement(
               "h3",
               null,
               "Example: Grid View"
            )
         ),
         React.createElement(
            MuiThemeProvider,
            null,
            React.createElement(
               "div",
               null,
               React.createElement(
                  "div",
                  { className: "row" },
                  React.createElement(
                     "div",
                     { className: "col-md-12" },
                     React.createElement(AppBar, { style: { marginBottom: "1em" },
                        iconElementLeft: React.createElement(SearchBox, { strings: this.state.LocalizedStrings, onChange: function (value) {
                              return _this2.dispatch({ EmployeeSearch: value });
                           } }),
                        iconElementRight: React.createElement(LanguageToggle, { onToggle: function (code) {
                              return _this2.dispatch({ CultureCode: code });
                           } }) })
                  )
               ),
               React.createElement(
                  "div",
                  { className: "row" },
                  React.createElement(
                     "div",
                     { className: "col-md-8" },
                     React.createElement(EmployeeTable, { data: this.state.Employees,
                        strings: this.state.LocalizedStrings,
                        select: this.state.SelectedId,
                        onSelect: function (id) {
                           return _this2.dispatchState({ SelectedId: id });
                        } }),
                     React.createElement(Pagination, { style: { marginTop: "1em", float: "right" },
                        pages: this.state.Pagination,
                        select: this.state.SelectedPage,
                        onSelect: function (page) {
                           return _this2.dispatchState({ SelectedPage: page });
                        } })
                  ),
                  React.createElement(
                     "div",
                     { className: "col-md-4" },
                     React.createElement(EmployeeDetails, { data: this.state.Details,
                        strings: this.state.LocalizedStrings,
                        onEdit: function () {
                           return _this2.setState({ openWizard: true });
                        } })
                  )
               ),
               wizard(this.state.openWizard)
            )
         )
      );
   }
});

var SearchBox = React.createClass({
   displayName: "SearchBox",

   getInitialState: function getInitialState() {
      return {
         searchText: ""
      };
   },
   render: function render() {
      var _this3 = this;

      var handleChange = function handleChange(event) {
         _this3.setState({ searchText: event.target.value });
         _this3.props.onChange(event.target.value);
      };

      return React.createElement(
         "div",
         { style: { padding: "0 1em", borderRadius: "4px", backgroundColor: "#11cde5" } },
         React.createElement(IconSearch, { style: { width: 20, height: 20 } }),
         React.createElement(TextField, { id: "SearchBox", hintText: this.props.strings.SearchLabel,
            value: this.state.searchText, onChange: handleChange })
      );
   }
});

var LanguageToggle = React.createClass({
   displayName: "LanguageToggle",

   getInitialState: function getInitialState() {
      return {
         code: "en-US",
         language: "English"
      };
   },
   render: function render() {
      var _this4 = this;

      var handleToggle = function handleToggle(event, checked) {
         var code = !checked ? "en-US" : "fr-FR";
         _this4.setState({ code: code });
         _this4.setState({ language: !checked ? "English" : "Français" });
         _this4.props.onToggle(code);
      };

      return React.createElement(Toggle, { style: { marginTop: "1em", width: "7em" },
         trackSwitchedStyle: { backgroundColor: "#e0e0e0" },
         thumbSwitchedStyle: { backgroundColor: "#11cde5" },
         onToggle: handleToggle,
         label: this.state.language,
         labelStyle: { color: "white", fontSize: "medium" }
      });
   }
});

var EmployeeTable = React.createClass({
   displayName: "EmployeeTable",

   render: function render() {
      var _this5 = this;

      var handleRowSelection = function handleRowSelection(rows) {
         if (rows.length > 0) handleSelect(_this5.props.data[rows[0]].Id);
      };

      var handleSelect = function handleSelect(id) {
         if (id != _this5.props.select) _this5.props.onSelect(id);
      };

      var employees = this.props.data.map(function (employee, index) {
         return React.createElement(
            TableRow,
            { key: employee.Id, selected: _this5.props.select == employee.Id },
            React.createElement(
               TableRowColumn,
               null,
               React.createElement(
                  "div",
                  null,
                  employee.FirstName
               )
            ),
            React.createElement(
               TableRowColumn,
               null,
               React.createElement(
                  "div",
                  null,
                  employee.LastName
               )
            )
         );
      });

      return React.createElement(
         Table,
         { selectable: true, onRowSelection: handleRowSelection },
         React.createElement(
            TableHeader,
            { displaySelectAll: false, adjustForCheckbox: false },
            React.createElement(
               TableRow,
               null,
               React.createElement(
                  TableHeaderColumn,
                  null,
                  this.props.strings.FirstName
               ),
               React.createElement(
                  TableHeaderColumn,
                  null,
                  this.props.strings.LastName
               )
            )
         ),
         React.createElement(
            TableBody,
            { displayRowCheckbox: false, showRowHover: true },
            employees
         )
      );
   }
});

var Pagination = React.createClass({
   displayName: "Pagination",

   render: function render() {
      var _this6 = this;

      var pageButtons = this.props.pages.map(function (page) {
         return React.createElement(
            Paper,
            { key: page, style: { display: "inline", padding: ".5em 0" } },
            React.createElement(FlatButton, { style: { minWidth: "1em" },
               label: page,
               disabled: _this6.props.select == page,
               onClick: function () {
                  return _this6.props.onSelect(page);
               } })
         );
      });

      return React.createElement(
         "div",
         { style: this.props.style },
         pageButtons
      );
   }
});

var EmployeeDetails = React.createClass({
   displayName: "EmployeeDetails",

   render: function render() {
      var _this7 = this;

      var employee = this.props.data;
      var iconEdit = React.createElement(IconEdit, { style: { width: 20, height: 20 }, color: "#8b8c8d" });
      var iconPhone = React.createElement(IconPhone, { style: { width: 24, height: 24 } });

      var reportsTo = function reportsTo(name) {
         return name != null ? _this7.props.strings.ReportTo + " " + name : "";
      };

      var editButton = function editButton() {
         if (_this7.props.data.Id > 0) return React.createElement(FlatButton, { label: _this7.props.strings.EditLabel, icon: iconEdit, onClick: _this7.props.onEdit });
      };

      return React.createElement(
         Card,
         null,
         React.createElement(CardHeader, { title: employee.FullName, subtitle: reportsTo(employee.ReportToName),
            style: { borderBottom: "solid 1px #e6e6e6" }, subtitleColor: "#00abc4" }),
         React.createElement(
            CardText,
            null,
            iconPhone,
            React.createElement(
               "span",
               { style: { verticalAlign: "super" } },
               employee.Phone
            )
         ),
         React.createElement(
            CardActions,
            null,
            editButton()
         )
      );
   }
});

var EditWizard = React.createClass({
   displayName: "EditWizard",

   getInitialState: function getInitialState() {
      return {
         firstName: this.props.employeeDetails.FirstName,
         lastName: this.props.employeeDetails.LastName,
         reportToName: this.props.employeeDetails.ReportToName,
         reportTo: this.props.employeeDetails.ReportTo,
         step: 0,
         maxStep: 2,
         disableNext: false
      };
   },
   render: function render() {
      var _this8 = this;

      var handleBack = function handleBack() {
         return _this8.setState({ step: _this8.state.step - 1 });
      };
      var handleNext = function handleNext() {
         return _this8.setState({ step: _this8.state.step + 1 });
      };
      var handleFinish = function handleFinish() {
         return _this8.props.onFinish({
            Id: _this8.props.employeeDetails.Id,
            FirstName: _this8.state.firstName,
            LastName: _this8.state.lastName,
            ReportTo: _this8.state.reportTo
         });
      };

      var actions = [React.createElement(FlatButton, { label: this.props.strings.Back, onClick: handleBack, disabled: this.state.step == 0 }), React.createElement(FlatButton, { label: this.props.strings.Next, onClick: handleNext, disabled: this.state.step == this.state.maxStep || this.state.disableNext }), React.createElement(FlatButton, { label: this.props.strings.Finish, primary: true, onClick: handleFinish, disabled: this.state.step != this.state.maxStep }), React.createElement(FlatButton, { label: this.props.strings.Cancel, onClick: function () {
            return _this8.props.onCancel();
         } })];

      var handleUpdateReportTo = function handleUpdateReportTo(value) {
         _this8.state.details.ReportToName = value;
         _this8.props.onReportToChange(value);
      };

      var content = function content(step) {
         switch (step) {
            case 0:
               return React.createElement(
                  "div",
                  null,
                  React.createElement(TextField, { id: "FirstName", floatingLabelText: _this8.props.strings.FirstName,
                     value: _this8.state.firstName,
                     onChange: function (event) {
                        return _this8.setState({ firstName: event.target.value });
                     } }),
                  React.createElement(TextField, { id: "LastName", floatingLabelText: _this8.props.strings.LastName,
                     value: _this8.state.lastName,
                     onChange: function (event) {
                        return _this8.setState({ lastName: event.target.value });
                     } })
               );
            case 1:
               var reportToSearchResult = _this8.props.reportToSearchResult.map(function (i) {
                  return i.Name;
               });
               var initialText = _this8.state.reportToName;

               var handleUpdate = function handleUpdate(value) {
                  var match = value.length > 0 ? _this8.props.reportToSearchResult.filter(function (i) {
                     return i.Name.toUpperCase() == value.toUpperCase();
                  }) : { Id: 0, Name: "" };
                  _this8.setState({ reportTo: match.length > 0 ? match[0].Id : -1 });
                  _this8.setState({ reportToName: match.length > 0 ? match[0].Name : value });
                  _this8.setState({ disableNext: match.length == 0 });

                  _this8.props.onReportToChange(value);
               };

               return React.createElement(AutoComplete, { id: "AutoComplete",
                  floatingLabelText: _this8.props.strings.ReportTo,
                  hintText: _this8.props.strings.ReportToHintText,
                  filter: AutoComplete.caseInsensitiveFilter,
                  searchText: initialText,
                  errorText: _this8.props.strings[_this8.props.reportToError],
                  dataSource: reportToSearchResult,
                  onUpdateInput: handleUpdate });
            case 2:
               var paperStyle = { display: "inline", padding: ".5em 1em", backgroundColor: "#e6e6e6" };
               var reportToName = _this8.state.reportToName != null ? _this8.state.reportToName : _this8.props.strings.NoOne;
               return React.createElement(
                  "div",
                  { style: { paddingTop: "2.2em" } },
                  React.createElement(
                     Paper,
                     { style: paperStyle },
                     _this8.state.firstName,
                     " ",
                     _this8.state.lastName
                  ),
                  React.createElement(
                     "span",
                     { style: { margin: "0 1em" } },
                     _this8.props.strings.ReportTo,
                     " "
                  ),
                  React.createElement(
                     Paper,
                     { style: paperStyle },
                     reportToName
                  )
               );
         }
      };

      return React.createElement(
         Dialog,
         { open: this.props.open, actions: actions },
         React.createElement(
            Stepper,
            { activeStep: this.state.step },
            React.createElement(
               Step,
               null,
               React.createElement(
                  StepLabel,
                  null,
                  this.props.strings.Name
               )
            ),
            React.createElement(
               Step,
               null,
               React.createElement(
                  StepLabel,
                  null,
                  this.props.strings.Manager
               )
            ),
            React.createElement(
               Step,
               null,
               React.createElement(
                  StepLabel,
                  null,
                  this.props.strings.Confirm
               )
            )
         ),
         React.createElement(
            "div",
            { style: { height: "4em" } },
            content(this.state.step)
         )
      );
   }
});

