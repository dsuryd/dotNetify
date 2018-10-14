/* 
Copyright 2018 Dicky Suryadi

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
import Vue from 'vue';
import dotnetifyVMRouter from '../core/dotnetify-vm-router';
import $ from '../libs/jquery-shim';
import utils from '../libs/utils';

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
    let componentProps;

    // If the view model supports routing, add the root path to the view, to be used
    // to build the absolute route path, and view model argument if provided.
    if (this.hasRoutingState) {
      if (this.RoutingState === null) {
        console.error("router> the RoutingState prop of '" + vm.$vmId + "' was not initialized.");
        return;
      }

      let root = this.VMRoot;
      root = root != null ? '/' + utils.trim(this.RoutingState.Root) + '/' + utils.trim(root) : this.RoutingState.Root;
      componentProps = { vmRoot: root, vmArg: iVmArg };
    }

    // Provide the opportunity to override the URL.
    iViewUrl = this.router.overrideUrl(iViewUrl, iTargetSelector);
    iJsModuleUrl = this.router.overrideUrl(iJsModuleUrl, iTargetSelector);

    if (utils.endsWith(iViewUrl, 'html')) this.loadHtmlView(iTargetSelector, iViewUrl, iJsModuleUrl, iCallbackFn);
    else this.loadVueView(iTargetSelector, iViewUrl, iJsModuleUrl, componentProps, iCallbackFn);
  }

  // Loads an HTML view.
  loadHtmlView(iTargetSelector, iViewUrl, iJsModuleUrl, callbackFn) {
    const vm = this.vm;

    // Load the HTML view.
    $(iTargetSelector).load(iViewUrl, null, function() {
      if (iJsModuleUrl != null) {
        $.getScript(iJsModuleUrl, function() {
          if (typeof callbackFn === 'function') callbackFn.call(vm);
        });
      }
      else if (typeof callbackFn === 'function') callbackFn.call(vm);
    });
  }

  // Loads a Vue view.
  loadVueView(iTargetSelector, iVueClassOrClassName, iJsModuleUrl, iProps, callbackFn) {
    return new Promise((resolve, reject) => {
      const vm = this.vm;
      const vmId = this.vm ? this.vm.$vmId : '';
      const createViewFunc = () => {
        // Resolve the vue class from the argument, which can be the object itself, or a global window variable name.
        let vueClass = null;
        if (typeof iVueClassOrClassName === 'string' && window.hasOwnProperty(iVueClassOrClassName))
          vueClass = Object.assign({}, window[iVueClassOrClassName]);
        else if (typeof iVueClassOrClassName === 'object' && iVueClassOrClassName.name) vueClass = iVueClassOrClassName;

        if (!vueClass) {
          console.error(`[${vmId}] failed to load view '${iVueClassOrClassName}' because it's not a Vue element.`);
          reject();
          return;
        }

        // Declare 'RoutingState' property in the component.
        let data = typeof vueClass.data == 'function' ? vueClass.data() : vueClass.data || {};
        if (!data.hasOwnProperty('RoutingState')) {
          data.RoutingState = {};
          vueClass.data = function() {
            return data;
          };
        }

        // Add any undeclared property to the vue class.
        if (iProps) {
          vueClass.props = vueClass.props || {};
          for (const prop in iProps) if (!vueClass.props.hasOwnProperty(prop)) vueClass.props[prop] = { type: null };
        }

        const vueComponentType = Vue.extend(vueClass);
        const vueComponent = new vueComponentType({ propsData: { ...iProps } });

        document.querySelector(iTargetSelector).innerHTML = '<div />';
        vueComponent.$mount(iTargetSelector + ' > div');

        if (typeof callbackFn === 'function') callbackFn.call(vm, vueComponent);
        resolve(vueComponent);
      };

      if (iJsModuleUrl == null) createViewFunc();
      else {
        // Load all javascripts first. Multiple files can be specified with comma delimiter.
        const getScripts = iJsModuleUrl.split(',').map(i => $.getScript(i));
        $.when.apply($, getScripts).done(createViewFunc);
      }
    });
  }
}
