/* 
Copyright 2018-2019 Dicky Suryadi

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
import Vue from "vue";
import DotnetifyVM from "../core/dotnetify-vm";
import DotnetifyRouter from "../core/dotnetify-router";
import DotnetifyVMRouter from "../core/dotnetify-vm-router";
import utils from "../libs/utils";
import * as $ from "../libs/jquery-shim";
import { RoutingStateType, RoutingTemplateType } from "../_typings";

const _window = window || global || {};

export default class DotnetifyVueVMRouter extends DotnetifyVMRouter {
  private mountedComponents: any;

  get hasRoutingState(): boolean {
    const state = this.vm.State();
    return state && state.hasOwnProperty("RoutingState");
  }
  get RoutingState(): RoutingStateType {
    return this.vm.State().RoutingState;
  }
  get VMRoot(): string {
    return this.vm.Props("vmRoot");
  }
  get VMArg(): any {
    return this.vm.Props("vmArg");
  }

  constructor(iVM: DotnetifyVM, iDotNetifyRouter: DotnetifyRouter) {
    super(iVM, iDotNetifyRouter);
  }

  onRouteEnter(iPath: string, iTemplate: RoutingTemplateType) {
    if (!iTemplate.ViewUrl) iTemplate.ViewUrl = iTemplate.Id;
    return true;
  }

  // Loads a view into a target element.
  loadView(iTargetSelector: string, iViewUrl: any, iJsModuleUrl?: string, iVmArg?: any, iCallbackFn?: Function) {
    const vm = this.vm;
    let componentProps: any;

    // If the view model supports routing, add the root path to the view, to be used
    // to build the absolute route path, and view model argument if provided.
    if (this.hasRoutingState) {
      if (this.RoutingState === null) {
        console.error("router> the RoutingState prop of '" + vm.$vmId + "' was not initialized.");
        return;
      }

      let root = this.VMRoot;
      root = root != null ? "/" + utils.trim(this.RoutingState.Root) + "/" + utils.trim(root) : this.RoutingState.Root;
      componentProps = { vmRoot: root, vmArg: iVmArg };
    }

    // Provide the opportunity to override the URL.
    iViewUrl = this.router.overrideUrl(iViewUrl, iTargetSelector);
    iJsModuleUrl = this.router.overrideUrl(iJsModuleUrl, iTargetSelector);

    if (utils.endsWith(iViewUrl, "html")) this.loadHtmlView(iTargetSelector, iViewUrl, iJsModuleUrl, iCallbackFn);
    else {
      let component = iViewUrl;
      if (typeof iViewUrl === "string" && _window.hasOwnProperty(iViewUrl)) component = Object.assign({}, _window[iViewUrl]);

      if (component instanceof HTMLElement) this.loadHtmlElementView(iTargetSelector, component, iJsModuleUrl, componentProps, iCallbackFn);
      else this.loadVueView(iTargetSelector, component, iJsModuleUrl, componentProps, iCallbackFn);
    }
  }

  // Loads a Vue view.
  loadVueView(iTargetSelector: string, iComponent: any, iJsModuleUrl?: string, iProps?: any, iCallbackFn?: Function) {
    return new Promise((resolve, reject) => {
      const vm = this.vm;
      const vmId = this.vm ? this.vm.$vmId : "";
      const createViewFunc = () => {
        // Resolve the vue class from the argument, which can be the object itself, or a global _window variable name.
        let vueClass = iComponent;
        if (typeof iComponent !== "object" || (typeof iComponent.render !== "function" && !iComponent.template)) {
          console.error(`[${vmId}] failed to load view '${iComponent}' because it's not a Vue element.`);
          reject();
          return;
        }

        // Unmount any existing Vue component on the target selector.
        this.unmountView(iTargetSelector);

        // Declare 'RoutingState' property in the component.
        let data = typeof vueClass.data == "function" ? vueClass.data() : vueClass.data || {};
        if (!data.hasOwnProperty("RoutingState")) {
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

        document.querySelector(iTargetSelector).innerHTML = "<div />";
        vueComponent.$mount(iTargetSelector + " > div");
        this.mountedComponents[iTargetSelector] = () => vueComponent.$destroy();

        if (typeof iCallbackFn === "function") iCallbackFn.call(vm, vueComponent);
        resolve(vueComponent);
      };

      if (iJsModuleUrl == null) createViewFunc();
      else {
        // Load all javascripts first. Multiple files can be specified with comma delimiter.
        const getScripts = iJsModuleUrl.split(",").map(i => $.getScript(i));
        $.when.apply($, getScripts).done(createViewFunc);
      }
    });
  }

  // Unmount a Vue view if there's one on the target selector.
  unmountView(iTargetSelector: string) {
    if (!this.mountedComponents) this.mountedComponents = {};

    const unmount = this.mountedComponents[iTargetSelector];
    if (typeof unmount == "function") {
      unmount();
      delete this.mountedComponents[iTargetSelector];
    }
  }
}
