'use strict';

var _extends = Object.assign || function (target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i]; for (var key in source) { if (Object.prototype.hasOwnProperty.call(source, key)) { target[key] = source[key]; } } } return target; };

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ('value' in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _get = function get(_x, _x2, _x3) { var _again = true; _function: while (_again) { var object = _x, property = _x2, receiver = _x3; _again = false; if (object === null) object = Function.prototype; var desc = Object.getOwnPropertyDescriptor(object, property); if (desc === undefined) { var parent = Object.getPrototypeOf(object); if (parent === null) { return undefined; } else { _x = parent; _x2 = property; _x3 = receiver; _again = true; desc = parent = undefined; continue _function; } } else if ('value' in desc) { return desc.value; } else { var getter = desc.get; if (getter === undefined) { return undefined; } return getter.call(receiver); } } };

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError('Cannot call a class as a function'); } }

function _inherits(subClass, superClass) { if (typeof superClass !== 'function' && superClass !== null) { throw new TypeError('Super expression must either be null or a function, not ' + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var ControlTypes = (function (_React$Component) {
   _inherits(ControlTypes, _React$Component);

   function ControlTypes(props) {
      var _this = this;

      _classCallCheck(this, ControlTypes);

      _get(Object.getPrototypeOf(ControlTypes.prototype), 'constructor', this).call(this, props);

      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("ControlTypesVM", this);

      // Set up function to dispatch state to the back-end.
      this.dispatchState = function (state) {
         _this.setState(state);
         _this.vm.$dispatch(state);
      };

      // This component's JSX was loaded along with the VM's initial state for faster rendering.
      this.state = window.vmStates.ControlTypesVM;
   }

   _createClass(ControlTypes, [{
      key: 'componentWillUnmount',
      value: function componentWillUnmount() {
         this.vm.$destroy();
      }
   }, {
      key: 'render',
      value: function render() {
         var _this2 = this;

         var radioButtons = this.state.RadioButtons.map(function (radio) {
            return React.createElement(RadioButton, _extends({ key: radio.value }, radio));
         });

         var selectFieldMenu = this.state.SelectFieldMenu.map(function (menu) {
            return React.createElement(MenuItem, _extends({ key: menu.value }, menu));
         });

         var chipStyles = {
            chip: { margin: 4 },
            wrapper: { display: 'flex', flexWrap: 'wrap' }
         };

         var chips = this.state.Chips.map(function (chip) {
            return React.createElement(
               Chip,
               { key: chip.key, style: chipStyles.chip, onRequestDelete: function () {
                     return _this2.dispatchState({ DeleteChip: chip.key });
                  } },
               chip.label
            );
         });

         return React.createElement(
            'div',
            { className: 'container-fluid' },
            React.createElement(
               'div',
               { className: 'header clearfix' },
               React.createElement(
                  'h3',
                  null,
                  'Example: Control Types'
               )
            ),
            React.createElement(
               MuiThemeProvider,
               null,
               React.createElement(
                  'div',
                  { className: 'jumbotron' },
                  React.createElement(
                     'div',
                     { className: 'row' },
                     React.createElement(
                        'div',
                        { className: 'col-md-6' },
                        React.createElement(TextField, _extends({ id: 'TextField', fullWidth: true
                        }, this.state.TextFieldProps, {
                           value: this.state.TextFieldValue,
                           errorText: this.state.TextFieldErrorText,
                           onChange: function (event) {
                              return _this2.setState({ TextFieldValue: event.target.value });
                           },
                           onBlur: function (event) {
                              return _this2.dispatchState({ TextFieldValue: _this2.state.TextFieldValue });
                           } }))
                     ),
                     React.createElement(
                        'div',
                        { className: 'col-md-6' },
                        React.createElement(
                           'div',
                           { style: { "marginTop": "3em" } },
                           this.state.TextFieldResult
                        )
                     )
                  ),
                  React.createElement(
                     'div',
                     { className: 'row' },
                     React.createElement(
                        'div',
                        { className: 'col-md-6' },
                        React.createElement(AutoComplete, _extends({ id: 'AutoComplete', fullWidth: true
                        }, this.state.AutoCompleteProps, {
                           filter: AutoComplete.caseInsensitiveFilter,
                           dataSource: this.state.AutoCompleteResults,
                           onUpdateInput: function (value) {
                              return _this2.dispatchState({ AutoCompleteValue: value });
                           } }))
                     )
                  ),
                  React.createElement('br', null),
                  React.createElement(
                     'div',
                     { className: 'row' },
                     React.createElement(
                        'div',
                        { className: 'col-md-6' },
                        React.createElement(Checkbox, { label: this.state.CheckboxLabel,
                           checked: this.state.Checked,
                           onCheck: function (event, value) {
                              return _this2.dispatchState({ Checked: value });
                           } })
                     ),
                     React.createElement(
                        'div',
                        { className: 'col-md-6' },
                        React.createElement(RaisedButton, { label: this.state.CheckboxResult, disabled: !this.state.Checked })
                     )
                  ),
                  React.createElement('br', null),
                  React.createElement(
                     'div',
                     { className: 'row' },
                     React.createElement(
                        'div',
                        { className: 'col-md-6' },
                        React.createElement(
                           RadioButtonGroup,
                           { name: 'Radio',
                              valueSelected: this.state.RadioValue,
                              onChange: function (event, value) {
                                 return _this2.dispatchState({ RadioValue: value });
                              } },
                           radioButtons
                        )
                     ),
                     React.createElement(
                        'div',
                        { className: 'col-md-6' },
                        React.createElement(RaisedButton, { label: this.state.RadioResult,
                           primary: this.state.RadioValue == "primary",
                           secondary: this.state.RadioValue == "secondary" })
                     )
                  ),
                  React.createElement('br', null),
                  React.createElement(
                     'div',
                     { className: 'row' },
                     React.createElement(
                        'div',
                        { className: 'col-md-6' },
                        React.createElement(Toggle, { labelPosition: 'right',
                           label: this.state.ToggleLabel,
                           toggled: this.state.Toggled,
                           onToggle: function (event, value) {
                              return _this2.dispatchState({ Toggled: value });
                           } })
                     )
                  ),
                  React.createElement(
                     'div',
                     { className: 'row' },
                     React.createElement(
                        'div',
                        { className: 'col-md-6' },
                        React.createElement(
                           SelectField,
                           { id: 'SelectField',
                              floatingLabelText: this.state.SelectFieldLabel,
                              value: this.state.SelectFieldValue,
                              onChange: function (event, idx, value) {
                                 return _this2.dispatchState({ SelectFieldValue: value });
                              } },
                           selectFieldMenu
                        )
                     ),
                     React.createElement(
                        'div',
                        { className: 'col-md-6' },
                        React.createElement(
                           'div',
                           { style: { marginTop: "3em" } },
                           this.state.SelectFieldResult
                        )
                     )
                  ),
                  React.createElement('br', null),
                  React.createElement(
                     'div',
                     { className: 'row' },
                     React.createElement(
                        'div',
                        { className: 'col-md-6' },
                        React.createElement(
                           'div',
                           { style: chipStyles.wrapper },
                           chips,
                           React.createElement(FlatButton, { label: 'Reset', onClick: function () {
                                 return _this2.dispatchState({ ResetChips: true });
                              } })
                        )
                     )
                  )
               )
            )
         );
      }
   }]);

   return ControlTypes;
})(React.Component);

/* Text Field */ /* Auto Complete */ /* Checkbox */ /* Radio button */ /* Toggle button */ /* Select Field */ /* Chip */

