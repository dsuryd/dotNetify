"use strict";

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _get = function get(_x, _x2, _x3) { var _again = true; _function: while (_again) { var object = _x, property = _x2, receiver = _x3; _again = false; if (object === null) object = Function.prototype; var desc = Object.getOwnPropertyDescriptor(object, property); if (desc === undefined) { var parent = Object.getPrototypeOf(object); if (parent === null) { return undefined; } else { _x = parent; _x2 = property; _x3 = receiver; _again = true; desc = parent = undefined; continue _function; } } else if ("value" in desc) { return desc.value; } else { var getter = desc.get; if (getter === undefined) { return undefined; } return getter.call(receiver); } } };

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var HelloWorld = (function (_React$Component) {
   _inherits(HelloWorld, _React$Component);

   function HelloWorld(props) {
      var _this = this;

      _classCallCheck(this, HelloWorld);

      _get(Object.getPrototypeOf(HelloWorld.prototype), "constructor", this).call(this, props);

      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("HelloWorldVM", this);

      // Set up function to dispatch state to the back-end.
      this.dispatchState = function (state) {
         _this.setState(state);
         _this.vm.$dispatch(state);
      };

      // This component's JSX was loaded along with the VM's initial state for faster rendering.
      this.state = window.vmStates.HelloWorldVM;
   }

   _createClass(HelloWorld, [{
      key: "componentWillUnmount",
      value: function componentWillUnmount() {
         this.vm.$destroy();
      }
   }, {
      key: "render",
      value: function render() {
         var _this2 = this;

         return React.createElement(
            "div",
            { className: "container-fluid" },
            React.createElement(
               "div",
               { className: "header clearfix" },
               React.createElement(
                  "h3",
                  null,
                  "Example: Hello World"
               )
            ),
            React.createElement(
               "div",
               { className: "jumbotron" },
               React.createElement(
                  "div",
                  { className: "row" },
                  React.createElement(
                     "div",
                     { className: "col-md-6" },
                     React.createElement(TextBox, { data: { Label: "First name:", Placeholder: "Type first name here" },
                        value: this.state.FirstName,
                        onChange: function (value) {
                           return _this2.setState({ FirstName: value });
                        },
                        onUpdate: function (value) {
                           return _this2.dispatchState({ FirstName: value });
                        } })
                  ),
                  React.createElement(
                     "div",
                     { className: "col-md-6" },
                     React.createElement(TextBox, { data: { Label: "Last name:", Placeholder: "Type last name here" },
                        value: this.state.LastName,
                        onChange: function (value) {
                           return _this2.setState({ LastName: value });
                        },
                        onUpdate: function (value) {
                           return _this2.dispatchState({ LastName: value });
                        } })
                  )
               ),
               React.createElement("hr", null),
               React.createElement(
                  "div",
                  null,
                  "Full name is ",
                  React.createElement(
                     "b",
                     null,
                     React.createElement(
                        "span",
                        null,
                        this.state.FullName
                     )
                  )
               )
            )
         );
      }
   }]);

   return HelloWorld;
})(React.Component);

var TextBox = (function (_React$Component2) {
   _inherits(TextBox, _React$Component2);

   function TextBox(props) {
      _classCallCheck(this, TextBox);

      _get(Object.getPrototypeOf(TextBox.prototype), "constructor", this).call(this, props);
      this.state = { changed: false };
      this.handleChange = this.handleChange.bind(this);
      this.handleBlur = this.handleBlur.bind(this);
   }

   _createClass(TextBox, [{
      key: "handleChange",
      value: function handleChange(event) {
         this.setState({ changed: true });
         this.props.onChange(event.target.value);
      }
   }, {
      key: "handleBlur",
      value: function handleBlur() {
         if (this.state.changed) this.props.onUpdate(this.props.value);
         this.setState({ changed: false });
      }
   }, {
      key: "render",
      value: function render() {
         return React.createElement(
            "div",
            null,
            React.createElement(
               "label",
               null,
               this.props.data.Label
            ),
            React.createElement("input", { className: "form-control", type: "text",
               value: this.props.value,
               placeholder: this.props.data.Placeholder,
               onChange: this.handleChange,
               onBlur: this.handleBlur })
         );
      }
   }]);

   return TextBox;
})(React.Component);

