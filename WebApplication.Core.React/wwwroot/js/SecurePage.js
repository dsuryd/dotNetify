"use strict";

var SecurePage = React.createClass({
   displayName: "SecurePage",

   getInitialState: function getInitialState() {
      this.vm = dotnetify.react.connect("SecurePageVM", this);
      return { Title: "" };
   },
   componentWillUnmount: function componentWillUnmount() {
      this.vm.$destroy();
   },
   render: function render() {
      return React.createElement(
         MuiThemeProvider,
         null,
         React.createElement(
            "div",
            { className: "container-fluid" },
            React.createElement(
               "div",
               { className: "header clearfix" },
               React.createElement(
                  "h3",
                  null,
                  "Example: Secure Page *** UNDER CONSTRUCTION ***"
               )
            ),
            React.createElement(
               "div",
               null,
               this.state.Title
            )
         )
      );
   }
});

