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
import _dotnetify from '../core/dotnetify';
import dotnetifyVM from '../core/dotnetify-vm';

const window = window || global || {};
let dotnetify = window.dotnetify || _dotnetify;

dotnetify.vue = {
  version: '2.0.0',
  viewModels: {},
  plugins: {},
  controller: dotnetify,

  // Internal variables.
  _hubs: [],

  // Initializes connection to SignalR server hub.
  init: function(iHub) {
    const self = dotnetify.vue;
    const hubInitialized = self._hubs.some(hub => hub === iHub);

    const start = function() {
      if (!iHub.isHubStarted)
        Object.keys(self.viewModels)
          .filter(vmId => self.viewModels[vmId].$hub === iHub)
          .forEach(vmId => (self.viewModels[vmId].$requested = false));

      dotnetify.startHub(iHub);
    };

    if (!hubInitialized) {
      iHub.responseEvent.subscribe((iVMId, iVMData) => self._responseVM(iVMId, iVMData));
      iHub.connectedEvent.subscribe(() =>
        Object.keys(self.viewModels)
          .filter(vmId => self.viewModels[vmId].$hub === iHub && !self.viewModels[vmId].$requested)
          .forEach(vmId => self.viewModels[vmId].$request())
      );
      iHub.reconnectedEvent.subscribe(start);
      self._hubs.push(iHub);
    }

    start();
  },

  // Connects to a server view model.
  connect: function(iVMId, iVue, iOptions) {
    if (arguments.length < 2) throw new Error('[dotNetify] Missing arguments. Usage: connect(vmId, component) ');

    const self = dotnetify.vue;
    if (self.viewModels.hasOwnProperty(iVMId)) {
      console.error(
        `Component is attempting to connect to an already active '${iVMId}'. ` +
          ` If it's from a dismounted component, you must call vm.$destroy in destroyed().`
      );
      self.viewModels[iVMId].$destroy();
      return setTimeout(() => self.connect(iVMId, iVue, iOptions));
    }

    const component = {
      get props() {
        return iVue.$props;
      },
      get state() {
        const vm = self.viewModels[iVMId];
        return vm && vm.$useState ? { ...iVue.$data, ...iVue.state } : iVue.$data;
      },
      setState(state) {
        Object.keys(state).forEach(key => {
          const value = state[key];

          // If 'useState' option is enabled, store server state in the Vue instance's 'state' property.
          const vm = self.viewModels[iVMId];
          if (vm && vm.$useState) {
            if (iVue.state.hasOwnProperty(key)) iVue.state[key] = value;
            else if (value) iVue.$set(iVue.state, key, value);
          }
          else {
            if (iVue.hasOwnProperty(key)) iVue[key] = value;
            else if (value) console.error(`'${key}' is received, but the Vue instance doesn't declare the property.`);
          }
        });
      }
    };

    const connectInfo = dotnetify.selectHub({ vmId: iVMId, options: iOptions, hub: null });
    self.viewModels[iVMId] = new dotnetifyVM(connectInfo.vmId, component, connectInfo.options, self, connectInfo.hub);
    if (connectInfo.hub) self.init(connectInfo.hub);

    if (iOptions) {
      const vm = self.viewModels[iVMId];

      // If 'useState' is true, server state will be placed in the Vue component's 'state' data property.
      // Otherwise, they will be placed in the root data property.
      if (iOptions.useState) {
        if (iVue.hasOwnProperty('state')) vm.$useState = true;
        else console.error(`Option 'useState' requires the 'state' data property on the Vue instance.`);
      }

      // 'watch' array specifies properties to dispatch to server when the values change.
      if (Array.isArray(iOptions.watch)) self._addWatchers(iOptions.watch, vm, iVue);
    }

    return self.viewModels[iVMId];
  },

  // Creates a Vue component with pre-configured connection to a server view model.
  component: function(iComponentOrName, iVMId, iOptions) {
    const obj = {
      created() {
        this.vm = dotnetify.vue.connect(iVMId, this, { ...iOptions, useState: true });
      },
      destroyed() {
        this.vm.$destroy();
      },
      data() {
        return { state: {} };
      }
    };

    if (typeof iComponentOrName == 'string') return { name: iComponentOrName, ...obj };
    else return { ...obj, ...iComponentOrName };
  },

  // Gets all view models.
  getViewModels: function() {
    const self = dotnetify.vue;
    return Object.keys(self.viewModels).map(vmId => self.viewModels[vmId]);
  },

  _addWatchers(iWatchlist, iVM, iVue) {
    const callback = prop =>
      function(newValue) {
        iVM.$serverUpdate !== false && iVM.$dispatch({ [prop]: newValue });
      }.bind(iVM);

    iWatchlist.forEach(prop => iVue.$watch(iVM.$useState ? `state.${prop}` : prop, callback(prop)));
  },

  _responseVM: function(iVMId, iVMData) {
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
};

dotnetify.addVMAccessor(dotnetify.vue.getViewModels);

export default dotnetify;
