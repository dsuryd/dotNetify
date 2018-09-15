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
import dotnetifyVMRouter from '../core/dotnetify-vm-router';
import $ from 'jquery';
import utils from '../libs/utils';

class templateWrapper {
  constructor(template) {
    this.template = template;
  }
  get Id() {
    return this.template.Id();
  }
  get Root() {
    return this.template.Root();
  }
  get UrlPattern() {
    return this.template.UrlPattern();
  }
  get ViewUrl() {
    return this.template.ViewUrl();
  }
  set ViewUrl(value) {
    this.template.ViewUrl(value);
  }
  get JSModuleUrl() {
    return this.template.JSModuleUrl();
  }
  set JSModuleUrl(value) {
    this.template.JSModuleUrl(value);
  }
  get Target() {
    return this.template.Target();
  }
  set Target(value) {
    this.template.Target(value);
  }
}

class routingStateWrapper {
  constructor(routingState) {
    this.routingState = routingState;
  }
  get Root() {
    return this.routingState.Root();
  }
  set Root(value) {
    this.routingState.Root(value);
  }
  get Templates() {
    const templates = typeof this.routingState.Templates == 'function' ? this.routingState.Templates() : null;
    return templates ? templates.map(template => new templateWrapper(template)) : null;
  }
  get Active() {
    return this.routingState.Active();
  }
  set Active(value) {
    this.routingState.Active(value);
  }
  get Origin() {
    return this.routingState.Origin();
  }
  set Origin(value) {
    this.routingState.Origin(value);
  }
}

export default class dotnetifyKoVMRouter extends dotnetifyVMRouter {
  get hasRoutingState() {
    return this.vm.hasOwnProperty('RoutingState');
  }
  get RoutingState() {
    return new routingStateWrapper(this.vm.RoutingState);
  }
  get VMRoot() {
    return this.vm.$element.attr('data-vm-root');
  }
  get VMArg() {
    return this.vm.$element.attr('data-vm-arg');
  }

  constructor(iVM, iDotNetifyRouter) {
    super(iVM, iDotNetifyRouter);
  }

  // Dispatch the active routing state to the server.
  dispatchActiveRoutingState(iPath) {
    this.RoutingState.Active = iPath;
  }
  onRouteEnter(iPath, iTemplate) {
    if (!iTemplate.ViewUrl) iTemplate.ViewUrl = iTemplate.Id + '.html';
    return true;
  }

  // Loads a view into a target element.
  // Method parameters: TargetSelector, ViewUrl, iJsModuleUrl, iVmArg, iCallbackFn
  loadView(iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, iCallbackFn) {
    const vm = this.vm;

    // If no view URL is given, empty the target DOM element.
    if (iViewUrl == null || iViewUrl == '') {
      $(iTargetSelector).empty();
      return;
    }

    const callbackFn = () => {
      // If the view model supports routing, add the root path to the view, to be used
      // to build the absolute route path, and view model argument if provided.
      if (this.hasRoutingState && this.RoutingState.Root) {
        const vmElems = $(iTargetSelector).find('[data-vm]').toArray();
        vmElems.forEach(element => {
          var root = $(element).attr('data-vm-root');
          root = root != null ? '/' + utils.trim(vm.RoutingState.Root()) + '/' + utils.trim(root) : vm.RoutingState.Root();
          $(element).attr('data-vm-root', root);

          if (iVmArg != null && !$.isEmptyObject(iVmArg)) {
            // If there's already a data-vm-arg, combine the values.
            // Take care not to override server-side routing arguments.
            var vmArg = $(element).attr('data-vm-arg');
            vmArg = vmArg != null ? $.extend(iVmArg, $.parseJSON(vmArg.replace(/'/g, '"'))) : iVmArg;

            $(element).attr('data-vm-arg', JSON.stringify(vmArg));
          }
        });
      }

      // Call the callback function.
      if (typeof iCallbackFn === 'function') iCallbackFn.apply(this);
    };

    // Provide the opportunity to override the URL.
    iViewUrl = this.router.overrideUrl(iViewUrl);

    vm.$loadView(iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, callbackFn);
  }

  // Routes to a path.
  manualRouteTo(iPath, iTarget, iViewUrl, iJSModuleUrl) {
    const template = {
      Target: () => iTarget,
      ViewUrl: () => iViewUrl,
      JSModuleUrl: () => iJSModuleUrl
    };
    this.$router.routeTo(iPath, template, true);
  }
}
