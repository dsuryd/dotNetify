"use strict";

var HelloWorld = React.createClass({
   displayName: "HelloWorld",

   getInitialState: function getInitialState() {
      var _this = this;

      // Connect this component to the back-end view model.
      this.vm = dotnetify.react.connect("HelloWorldVM", this);

      // Set up function to dispatch state to the back-end.
      this.dispatchState = function (state) {
         _this.setState(state);
         _this.vm.$dispatch(state);
      };

      // This component's JSX was loaded along with the VM's initial state for faster rendering.
      return window.vmStates.HelloWorldVM;
   },
   componentWillUnmount: function componentWillUnmount() {
      this.vm.$destroy();
   },
   render: function render() {
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
});

var TextBox = React.createClass({
   displayName: "TextBox",

   getInitialState: function getInitialState() {
      return {
         changed: false
      };
   },
   handleChange: function handleChange(event) {
      this.setState({ changed: true });
      this.props.onChange(event.target.value);
   },
   handleBlur: function handleBlur() {
      if (this.state.changed) this.props.onUpdate(this.props.value);
      this.setState({ changed: false });
   },
   render: function render() {
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
});

