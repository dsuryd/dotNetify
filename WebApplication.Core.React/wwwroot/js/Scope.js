// The <Scope> component uses React's 'context' to pass down the component hierarchy the name of the back-end view model
// of the parent component, so that when the child component connects to its back-end view model, the child view model
// instance is created within the scope of the parent view model.
// The <Scope> component also provides the 'connect' function for a component to connect to the back-end view model and
// injects properties and dispatch functions into the component.
"use strict";

var Scope = React.createClass({
   displayName: "Scope",

   propTypes: { vm: React.PropTypes.string },
   contextTypes: { scoped: React.PropTypes.func },
   childContextTypes: {
      scoped: React.PropTypes.func.isRequired,
      connect: React.PropTypes.func.isRequired
   },
   scoped: function scoped(vmId) {
      var scope = this.context.scoped ? this.context.scoped(this.props.vm) : this.props.vm;
      return scope ? scope + "." + vmId : vmId;
   },
   getChildContext: function getChildContext() {
      var _this = this;

      return {
         scoped: function scoped(vmId) {
            return _this.scoped(vmId);
         },
         connect: function connect(vmId, component, compGetState, compSetState, vmArg) {
            var getState = typeof compGetState === "function" ? compGetState : function () {
               return component.state;
            };
            var setState = typeof compSetState === "function" ? compSetState : function (state) {
               return component.setState(state);
            };
            if (typeof compGetState !== "function") vmArg = compGetState;

            component.vmId = _this.scoped(vmId);
            component.vm = dotnetify.react.connect(component.vmId, component, getState, setState, vmArg);
            component.dispatch = function (state) {
               return component.vm.$dispatch(state);
            };
            component.dispatchState = function (state) {
               setState(state);
               component.vm.$dispatch(state);
            };
            return window.vmStates ? window.vmStates[component.vmId] : null;
         }
      };
   },
   render: function render() {
      return this.props.children;
   }
});

