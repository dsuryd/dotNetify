/* 
Copyright 2017 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

// Support using AMD or CommonJS that loads our app.js, or being placed in <script> tag.
(function (factory) {
   if (typeof exports === "object" && typeof module === "object") {
      module.exports = factory(require('react'), require('dotnetify'));
   }
   else if (typeof define === "function" && define["amd"]) {
      define(['react', 'dotnetify'], factory);
   }
   else {
      factory(React, dotnetify);
   }
}
   (function (_React, dotnetify) {

      // The <Scope> component uses React's 'context' to pass down the component hierarchy the name of the back-end view model
      // of the parent component, so that when the child component connects to its back-end view model, the child view model
      // instance is created within the scope of the parent view model.
      // The <Scope> component also provides the 'connect' function for a component to connect to the back-end view model and
      // injects properties and dispatch functions into the component.
      dotnetify.react.Scope = _React.createClass({
         displayName: "Scope",
         version: "1.0.1",

         propTypes: { vm: _React.PropTypes.string },
         contextTypes: { scoped: _React.PropTypes.func },
         childContextTypes: {
            scoped: _React.PropTypes.func.isRequired,
            connect: _React.PropTypes.func.isRequired
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
               connect: function connect(vmId, component, options) {
                  component.vmId = _this.scoped(vmId);
                  component.vm = dotnetify.react.connect(component.vmId, component, options);
                  component.dispatch = function (state) { return component.vm.$dispatch(state); };
                  component.dispatchState = function (state) {
                     component.vm.State(state);
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

      return dotnetify.react.Scope;
   }))