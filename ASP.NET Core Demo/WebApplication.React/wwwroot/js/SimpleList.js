"use strict";

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _get = function get(_x, _x2, _x3) { var _again = true; _function: while (_again) { var object = _x, property = _x2, receiver = _x3; _again = false; if (object === null) object = Function.prototype; var desc = Object.getOwnPropertyDescriptor(object, property); if (desc === undefined) { var parent = Object.getPrototypeOf(object); if (parent === null) { return undefined; } else { _x = parent; _x2 = property; _x3 = receiver; _again = true; desc = parent = undefined; continue _function; } } else if ("value" in desc) { return desc.value; } else { var getter = desc.get; if (getter === undefined) { return undefined; } return getter.call(receiver); } } };

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var SimpleList = (function (_React$Component) {
   _inherits(SimpleList, _React$Component);

   function SimpleList(props) {
      var _this = this;

      _classCallCheck(this, SimpleList);

      _get(Object.getPrototypeOf(SimpleList.prototype), "constructor", this).call(this, props);
      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("SimpleListVM", this);

      // Set up function to dispatch state to the back-end.
      this.dispatch = function (state) {
         return _this.vm.$dispatch(state);
      };
      this.dispatchState = function (state) {
         _this.setState(state);
         _this.vm.$dispatch(state);
      };

      // This component's JSX was loaded along with the VM's initial state for faster rendering.
      this.state = window.vmStates.SimpleListVM;
   }

   _createClass(SimpleList, [{
      key: "componentWillUnmount",
      value: function componentWillUnmount() {
         this.vm.$destroy();
      }
   }, {
      key: "render",
      value: function render() {
         var _this2 = this;

         var handleUpdate = function handleUpdate(value) {
            var update = _this2.state.Employees.map(function (employee) {
               return employee.Id == value.Id ? Object.keys(value).forEach(function (key) {
                  employee[key] = value[key];
               }) : employee;
            });
            _this2.setState({ Employee: update });
            _this2.dispatch({ Update: value });
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
                  "Example: Simple List"
               )
            ),
            React.createElement(
               MuiThemeProvider,
               null,
               React.createElement(
                  "div",
                  null,
                  React.createElement(AddNameBox, { onAdd: function (value) {
                        return _this2.dispatch({ Add: value });
                     } }),
                  React.createElement(EmployeeTable, { data: this.state.Employees,
                     onUpdate: handleUpdate,
                     onRemove: function (id) {
                        return _this2.dispatch({ Remove: id });
                     } }),
                  React.createElement(Snackbar, { open: this.state.ShowNotification, message: "Changes saved", autoHideDuration: 1000,
                     onRequestClose: function () {
                        return _this2.setState({ ShowNotification: false });
                     } })
               )
            )
         );
      }
   }]);

   return SimpleList;
})(React.Component);

var EmployeeTable = (function (_React$Component2) {
   _inherits(EmployeeTable, _React$Component2);

   function EmployeeTable(props) {
      _classCallCheck(this, EmployeeTable);

      _get(Object.getPrototypeOf(EmployeeTable.prototype), "constructor", this).call(this, props);
   }

   _createClass(EmployeeTable, [{
      key: "render",
      value: function render() {
         var _this3 = this;

         var lastColWidth = { width: "10em" };
         var iconDelete = React.createElement(IconDelete, { style: { width: 20, height: 20 }, color: "#8B8C8D" });

         var employees = this.props.data.map(function (employee) {
            return React.createElement(
               TableRow,
               { key: employee.Id },
               React.createElement(
                  TableRowColumn,
                  null,
                  React.createElement(InlineEdit, { text: employee.FirstName, onChange: function (value) {
                        return _this3.props.onUpdate({ Id: employee.Id, FirstName: value });
                     } })
               ),
               React.createElement(
                  TableRowColumn,
                  null,
                  React.createElement(InlineEdit, { text: employee.LastName, onChange: function (value) {
                        return _this3.props.onUpdate({ Id: employee.Id, LastName: value });
                     } })
               ),
               React.createElement(
                  TableRowColumn,
                  { style: lastColWidth },
                  React.createElement(FlatButton, { label: "Remove", labelStyle: { fontSize: "8pt" }, icon: iconDelete, onClick: function () {
                        return _this3.props.onRemove(employee.Id);
                     } })
               )
            );
         });

         return React.createElement(
            "div",
            null,
            React.createElement(
               Table,
               { selectable: false },
               React.createElement(
                  TableHeader,
                  { displaySelectAll: false, adjustForCheckbox: false },
                  React.createElement(
                     TableRow,
                     null,
                     React.createElement(
                        TableHeaderColumn,
                        null,
                        "First Name"
                     ),
                     React.createElement(
                        TableHeaderColumn,
                        null,
                        "Last Name"
                     ),
                     React.createElement(TableHeaderColumn, { style: lastColWidth })
                  )
               ),
               React.createElement(
                  TableBody,
                  { displayRowCheckbox: false, showRowHover: true },
                  employees
               )
            ),
            React.createElement(
               Paper,
               { style: { width: "11em", marginTop: "1em", backgroundColor: "#e0e0e0" } },
               React.createElement(
                  "i",
                  null,
                  "* Click a name to edit"
               )
            )
         );
      }
   }]);

   return EmployeeTable;
})(React.Component);

var AddNameBox = (function (_React$Component3) {
   _inherits(AddNameBox, _React$Component3);

   function AddNameBox(props) {
      _classCallCheck(this, AddNameBox);

      _get(Object.getPrototypeOf(AddNameBox.prototype), "constructor", this).call(this, props);
      this.state = { fullName: "" };
   }

   _createClass(AddNameBox, [{
      key: "render",
      value: function render() {
         var _this4 = this;

         var handleAdd = function handleAdd() {
            if (_this4.state.fullName) {
               _this4.props.onAdd(_this4.state.fullName);
               _this4.setState({ fullName: "" });
            }
         };
         return React.createElement(
            "div",
            null,
            React.createElement(TextField, { id: "FullName", floatingLabelText: "Full name",
               value: this.state.fullName,
               onChange: function (event) {
                  return _this4.setState({ fullName: event.target.value });
               } }),
            React.createElement(RaisedButton, { style: { marginLeft: "1em" }, label: "Add", primary: true, onClick: handleAdd })
         );
      }
   }]);

   return AddNameBox;
})(React.Component);

var InlineEdit = (function (_React$Component4) {
   _inherits(InlineEdit, _React$Component4);

   function InlineEdit(props) {
      _classCallCheck(this, InlineEdit);

      _get(Object.getPrototypeOf(InlineEdit.prototype), "constructor", this).call(this, props);
      this.state = {
         edit: false,
         value: this.props.text
      };
   }

   _createClass(InlineEdit, [{
      key: "render",
      value: function render() {
         var _this5 = this;

         var handleClick = function handleClick(event) {
            event.stopPropagation();
            if (!_this5.state.edit) {
               _this5.setState({ value: _this5.props.text });
               _this5.setState({ edit: true });
            }
         };

         var handleBlur = function handleBlur(event) {
            _this5.setState({ edit: false });
            if (_this5.state.value.length > 0 && _this5.state.value != _this5.props.text) _this5.props.onChange(_this5.state.value);else _this5.setState({ value: _this5.props.text });
         };

         var setFocus = function setFocus(input) {
            if (input) input.focus();
         };

         var elem;
         if (!this.state.edit) elem = React.createElement(
            "div",
            { style: { minHeight: "2em" }, onClick: handleClick },
            this.props.text
         );else elem = React.createElement(TextField, { id: "EditField", ref: function (input) {
               return setFocus(input);
            },
            value: this.state.value,
            onClick: handleClick,
            onBlur: handleBlur,
            onChange: function (event) {
               return _this5.setState({ value: event.target.value });
            } });
         return React.createElement(
            "div",
            null,
            elem
         );
      }
   }]);

   return InlineEdit;
})(React.Component);

