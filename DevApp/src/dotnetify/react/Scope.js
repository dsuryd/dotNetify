/* 
Copyright 2017-2018 Dicky Suryadi

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

import React from 'react';
import PropTypes from 'prop-types';
import dotnetify from './dotnetify-react';

const window = window || global || {};

// The <Scope> component uses React's 'context' to pass down the component hierarchy the name of the back-end view model
// of the parent component, so that when the child component connects to its back-end view model, the child view model
// instance is created within the scope of the parent view model.
//
// The <Scope> component also provides the 'connect' function for a component to connect to the back-end view model and
// injects properties and dispatch functions into the component.
export default class Scope extends React.Component {
  static displayName = 'Scope';
  static version = '1.3.0';

  static propTypes = {
    vm: PropTypes.string,
    options: PropTypes.object
  };
  static contextTypes = {
    scoped: PropTypes.func,
    scopedOptions: PropTypes.func
  };
  static childContextTypes = {
    scoped: PropTypes.func.isRequired,
    scopedOptions: PropTypes.func.isRequired,
    connect: PropTypes.func.isRequired
  };

  scoped(vmId) {
    let scope = this.context.scoped ? this.context.scoped(this.props.vm) : this.props.vm;
    return scope ? scope + '.' + vmId : vmId;
  }

  scopedOptions(options) {
    let scopedOptions = this.context.scoped ? this.context.scopedOptions(this.props.options) : this.props.options;
    return scopedOptions ? { ...scopedOptions, ...options } : options;
  }

  getChildContext() {
    const _this = this;

    return {
      scoped: vmId => _this.scoped(vmId),
      scopedOptions: options => _this.scopedOptions(options),
      connect: (vmId, component, options) => {
        component.vmId = _this.scoped(vmId);
        component.options = _this.scopedOptions(options);
        component.vm = dotnetify.react.connect(component.vmId, component, component.options);
        component.dispatch = state => component.vm.$dispatch(state);

        component.dispatchState = state => {
          component.vm.State(state);
          component.vm.$dispatch(state);
        };
        return window.vmStates ? window.vmStates[component.vmId] : null;
      }
    };
  }
  render() {
    return this.props.children;
  }
}

dotnetify.react.Scope = Scope;
