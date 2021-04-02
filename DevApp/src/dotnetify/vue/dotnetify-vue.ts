/* 
Copyright 2018-2021 Dicky Suryadi

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
import { watch } from "vue";
import _dotnetify, { Dotnetify, IDotnetifyImpl } from "../core/dotnetify";
import dotnetifyVM from "../core/dotnetify-vm";
import DotnetifyVM from "../core/dotnetify-vm";
import { IDotnetifyVue, IConnectOptions, IDotnetifyHub, IDotnetifyVM } from "../_typings";

const _window = window || global || <any>{};
let dotnetify: Dotnetify = _window.dotnetify || _dotnetify;

export interface IVueConnectOptions extends IConnectOptions {
  watch: string[];
  useState: boolean;
}

export class DotnetifyVue implements IDotnetifyVue, IDotnetifyImpl {
  version = "3.0.0";
  viewModels: { [vmId: string]: DotnetifyVM } = {};
  plugins: { [pluginId: string]: any } = {};
  controller = dotnetify;

  // Internal variables.
  _hubs = [];

  // Initializes connection to SignalR server hub.
  init(iHub: IDotnetifyHub) {
    const hubInitialized = this._hubs.some(hub => hub === iHub);

    const start = () => {
      if (!iHub.isHubStarted)
        Object.keys(this.viewModels)
          .filter(vmId => this.viewModels[vmId].$hub === iHub)
          .forEach(vmId => (this.viewModels[vmId].$requested = false));

      dotnetify.startHub(iHub);
    };

    if (!hubInitialized) {
      iHub.responseEvent.subscribe((iVMId, iVMData) => this._responseVM(iVMId, iVMData));
      iHub.connectedEvent.subscribe(() =>
        Object.keys(this.viewModels)
          .filter(vmId => this.viewModels[vmId].$hub === iHub && !this.viewModels[vmId].$requested)
          .forEach(vmId => this.viewModels[vmId].$request())
      );
      iHub.reconnectedEvent.subscribe(start);
      this._hubs.push(iHub);
    }

    start();
  }

  // Connects to a server view model.
  connect(iVMId: string, iVue: any, iOptions: IVueConnectOptions): IDotnetifyVM {
    if (this.viewModels.hasOwnProperty(iVMId)) {
      console.error(
        `Component is attempting to connect to an already active '${iVMId}'. ` +
          ` If it's from a dismounted component, you must call vm.$destroy on unmount.`
      );
      this.viewModels[iVMId].$destroy();
    }

    const self = this;
    const component = {
      get props() {
        return iVue.$props;
      },
      get state() {
        const vm = self.viewModels[iVMId];
        return vm && vm["$useState"] ? { ...iVue.$data, ...iVue.state } : iVue.$data;
      },
      setState(state: any) {
        Object.keys(state).forEach(key => {
          const value = state[key];

          // If 'useState' option is enabled, store server state in the Vue instance's 'state' property.
          const vm = self.viewModels[iVMId];
          if (vm && vm["$useState"]) {
            if (iVue.state.hasOwnProperty(key)) iVue.state[key] = value;
            else if (value) self._vueSetState(iVue, key, value);
          } else {
            if (self._vueHasProperty(iVue, key)) iVue[key] = value;
            else if (value) console.error(`'${key}' is received, but the Vue instance doesn't declare the property.`);
          }
        });
      }
    };

    const connectInfo = dotnetify.selectHub({
      vmId: iVMId,
      options: iOptions,
      hub: null
    });
    this.viewModels[iVMId] = new dotnetifyVM(connectInfo.vmId, component, connectInfo.options, this, connectInfo.hub);
    if (connectInfo.hub) this.init(connectInfo.hub);

    if (iOptions) {
      const vm = this.viewModels[iVMId];

      // If 'useState' is true, server state will be placed in the Vue component's 'state' data property.
      // Otherwise, they will be placed in the root data property.
      if (iOptions.useState) {
        if (this._vueHasProperty(iVue, "state")) vm["$useState"] = true;
        else console.error(`Option 'useState' requires the 'state' data property on the Vue instance.`);
      }

      // 'watch' array specifies properties to dispatch to server when the values change.
      if (Array.isArray(iOptions.watch)) this._addWatchers(iOptions.watch, vm, iVue);
    }

    return this.viewModels[iVMId];
  }

  // Creates a Vue component with pre-configured connection to a server view model.
  component(iComponentOrName: any, iVMId: string, iOptions: IConnectOptions) {
    let obj = {
      vm: null,
      created() {
        this.vm = dotnetify.vue.connect(iVMId, this, {
          ...iOptions,
          useState: true
        });
      },
      unmounted() {
        this.vm.$destroy();
      },
      data() {
        return { state: {} };
      }
    };

    this._vueAddUnmounted(obj);

    if (typeof iComponentOrName == "string") return { name: iComponentOrName, ...obj };
    else return { ...obj, ...iComponentOrName };
  }

  // Gets all view models.
  getViewModels(): IDotnetifyVM[] {
    const self = dotnetify.vue;
    return Object.keys(self.viewModels).map(vmId => self.viewModels[vmId]);
  }

  _addWatchers(iWatchlist, iVM: DotnetifyVM, iVue: any) {
    const callback = (prop: string) =>
      function (newValue: any) {
        iVM.$serverUpdate !== false && iVM.$dispatch({ [prop]: newValue });
      }.bind(iVM);

    const useState = iVM["$useState"];
    if (watch) {
      // Vue 3.x
      iWatchlist.forEach((prop: string) => watch(() => (useState ? iVue.state[prop] : iVue[prop]), callback(prop)));
    } else {
      // Vue 2.x
      iWatchlist.forEach((prop: string) => iVue.$watch(useState ? `state.${prop}` : prop, callback(prop)));
    }
  }

  _responseVM(iVMId: string, iVMData: any) {
    const self = dotnetify.vue;

    if (self.viewModels.hasOwnProperty(iVMId)) {
      const vm = self.viewModels[iVMId];
      dotnetify.checkServerSideException(iVMId, iVMData, vm.$exceptionHandler);

      // Disable server update while updating Vue so the change event won't cause rebound.
      vm.$serverUpdate = false;
      vm.$update(iVMData);
      setTimeout(() => (vm.$serverUpdate = true));
      return true;
    }
    return false;
  }

  _vueHasProperty(iVue: any, iKey: string) {
    if (iVue) {
      // Vue 2.x
      if (typeof iVue.hasOwnProperty === "function") return iVue.hasOwnProperty(iKey);
      // Vue 3.x
      else return typeof iVue[iKey] !== "undefined";
    }
    return false;
  }

  _vueSetState(iVue: any, iKey: any, iValue: any) {
    if (iVue) {
      // Vue 2.x
      if (typeof iVue.$set === "function") iVue.$set(iVue.state, iKey, iValue);
      // Vue 3.x
      else iVue.state[iKey] = iValue;
    }
  }

  _vueAddUnmounted(iVue: any) {
    const vmDestroyFunc = function () {
      this.vm.$destroy();
    };

    if (Vue && (<any>Vue).directive)
      // Vue 2.x
      Object.assign(iVue, { destroyed: vmDestroyFunc });
    // Vue 3.x
    else Object.assign(iVue, { unmounted: vmDestroyFunc });
  }
}

dotnetify.vue = new DotnetifyVue();
dotnetify.addVMAccessor(dotnetify.vue.getViewModels);

export default dotnetify;
