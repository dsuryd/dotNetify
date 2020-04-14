/* 
Copyright 2017-2019 Dicky Suryadi

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
import ReactDOM from 'react-dom';
import dotnetifyVMRouter from '../core/dotnetify-vm-router';
import $ from '../libs/jquery-shim';
import utils from '../libs/utils';

const window = window || global || {};

export default class dotnetifyReactVMRouter extends dotnetifyVMRouter {
  get hasRoutingState() {
    const state = this.vm.State();
    return state && state.hasOwnProperty('RoutingState');
  }
  get RoutingState() {
    return this.vm.State().RoutingState;
  }
  get VMRoot() {
    return this.vm.Props('vmRoot');
  }
  get VMArg() {
    return this.vm.Props('vmArg');
  }

  constructor(iVM, iDotNetifyRouter) {
    super(iVM, iDotNetifyRouter);
  }

  onRouteEnter(iPath, iTemplate) {
    if (!iTemplate.ViewUrl) iTemplate.ViewUrl = iTemplate.Id;
    return true;
  }

  // Loads a view into a target element.
  loadView(iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, iCallbackFn) {
    const vm = this.vm;
    let reactProps;

    // If the view model supports routing, add the root path to the view, to be used
    // to build the absolute route path, and view model argument if provided.
    if (this.hasRoutingState) {
      if (this.RoutingState === null) {
        console.error("router> the RoutingState prop of '" + vm.$vmId + "' was not initialized.");
        return;
      }

      let root = this.VMRoot;
      root = root != null ? '/' + utils.trim(this.RoutingState.Root) + '/' + utils.trim(root) : this.RoutingState.Root;
      reactProps = { vmRoot: root, vmArg: iVmArg };
    }

    // Provide the opportunity to override the URL.
    iViewUrl = this.router.overrideUrl(iViewUrl, iTargetSelector);
    iJsModuleUrl = this.router.overrideUrl(iJsModuleUrl, iTargetSelector);

    if (utils.endsWith(iViewUrl, 'html')) this.loadHtmlView(iTargetSelector, iViewUrl, iJsModuleUrl, iCallbackFn);
    else {
      let component = iViewUrl;
      if (typeof iViewUrl === 'string' && window.hasOwnProperty(iViewUrl)) component = window[iViewUrl];

      if (component instanceof HTMLElement) this.loadHtmlElementView(iTargetSelector, component, iJsModuleUrl, reactProps, iCallbackFn);
      else this.loadReactView(iTargetSelector, component, iJsModuleUrl, reactProps, iCallbackFn);
    }
  }

  // Loads a React view.
  loadReactView(iTargetSelector, iComponent, iJsModuleUrl, iReactProps, iCallbackFn) {
    return new Promise((resolve, reject) => {
      const vm = this.vm;
      const vmId = this.vm ? this.vm.$vmId : '';

      const mountViewFunc = () => {
        if (typeof iComponent !== 'function' && (typeof iComponent !== 'object' || iComponent.name == null)) {
          console.error(`[${vmId}] failed to load view '${iComponent}' because it's not a valid React element.`);
          reject();
          return;
        }

        this.unmountView(iTargetSelector);

        try {
          var reactElement = React.createElement(iComponent, iReactProps);
          ReactDOM.hydrate(reactElement, document.querySelector(iTargetSelector));
        } catch (e) {
          console.error(e);
        }
        if (typeof iCallbackFn === 'function') iCallbackFn.call(vm, reactElement);
        resolve(reactElement);
      };

      if (iJsModuleUrl == null || this.vm.$dotnetify.ssr === true) mountViewFunc();
      else {
        // Load all javascripts first. Multiple files can be specified with comma delimiter.
        var getScripts = iJsModuleUrl.split(',').map(i => $.getScript(i));
        $.when.apply($, getScripts).done(mountViewFunc);
      }
    });
  }

  // Unmount a React view if there's one on the target selector.
  unmountView(iTargetSelector) {
    try {
      ReactDOM.unmountComponentAtNode(document.querySelector(iTargetSelector));
    } catch (e) {
      console.warn(e);
    }
  }
}
