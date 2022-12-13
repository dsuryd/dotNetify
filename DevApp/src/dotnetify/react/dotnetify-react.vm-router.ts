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
import dotnetify from "./dotnetify-react";
import DotnetifyVMRouter from "../core/dotnetify-vm-router";
import DotnetifyRouter from "../core/dotnetify-router";
import DotnetifyVM from "../core/dotnetify-vm";
import * as $ from "../libs/jquery-shim";
import utils from "../libs/utils";
import { RoutingStateType, RoutingTemplateType } from "../_typings";

const _window = window || global || <any>{};

export default class DotnetifyReactVMRouter extends DotnetifyVMRouter {
  unmountTracker = [];

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
    let reactProps: any;

    // If the view model supports routing, add the root path to the view, to be used
    // to build the absolute route path, and view model argument if provided.
    if (this.hasRoutingState) {
      if (this.RoutingState === null) {
        console.error("router> the RoutingState prop of '" + vm.$vmId + "' was not initialized.");
        return;
      }

      let root = this.VMRoot;
      root = root != null ? "/" + utils.trim(this.RoutingState.Root) + "/" + utils.trim(root) : this.RoutingState.Root;
      reactProps = { vmRoot: root, vmArg: iVmArg };
    }

    // Provide the opportunity to override the URL.
    iViewUrl = this.router.overrideUrl(iViewUrl, iTargetSelector);
    iJsModuleUrl = this.router.overrideUrl(iJsModuleUrl, iTargetSelector);

    if (utils.endsWith(iViewUrl, "html")) this.loadHtmlView(iTargetSelector, iViewUrl, iJsModuleUrl, iCallbackFn);
    else {
      let component = iViewUrl;
      if (typeof iViewUrl === "string" && _window.hasOwnProperty(iViewUrl)) component = _window[iViewUrl];

      if (component instanceof HTMLElement) this.loadHtmlElementView(iTargetSelector, component, iJsModuleUrl, reactProps, iCallbackFn);
      else this.loadReactView(iTargetSelector, component, iJsModuleUrl, reactProps, iCallbackFn);
    }
  }

  // Loads a React view.
  loadReactView(iTargetSelector: string, iComponent: any, iJsModuleUrl?: string, iReactProps?: any, iCallbackFn?: Function) {
    return new Promise((resolve, reject) => {
      const vm = this.vm;
      const vmId = this.vm ? this.vm.$vmId : "";

      const mountViewFunc = () => {
        let reactElement = null;
        try {
          reactElement = dotnetify.react.router.createElement(iComponent, iReactProps);
        } catch (e) {
          console.error(`[${vmId}] failed to load view '${iComponent}' because it's not a valid React element.`);
          reject();
          return;
        }

        this.unmountView(iTargetSelector);

        try {
          const container = document.querySelector(iTargetSelector);
          if (vm.$dotnetify["ssrEnabled"] === true) {
            dotnetify.react.router.hydrate(reactElement, container);
          } else {
            const unmount = dotnetify.react.router.render(reactElement, container);
            if (typeof unmount === "function") this.unmountTracker.push({ selector: iTargetSelector, unmount });
          }
        } catch (e) {
          console.error(e);
        }
        if (typeof iCallbackFn === "function") iCallbackFn.call(vm, reactElement);
        resolve(reactElement);
      };

      if (iJsModuleUrl == null || this.vm.$dotnetify["ssr"] === true) mountViewFunc();
      else {
        // Load all javascripts first. Multiple files can be specified with comma delimiter.
        var getScripts = iJsModuleUrl.split(",").map(i => $.getScript(i));
        $.when.apply($, getScripts).done(mountViewFunc);
      }
    });
  }

  // Unmount a React view if there's one on the target selector.
  unmountView(iTargetSelector: string) {
    try {
      const unmount = this.unmountTracker.find(x => x.selector === iTargetSelector)?.unmount;
      if (typeof unmount === "function") {
        unmount();
        this.unmountTracker = this.unmountTracker.filter(x => x.selector !== iTargetSelector);
      }
    } catch (e) {
      console.warn(e);
    }
  }
}
