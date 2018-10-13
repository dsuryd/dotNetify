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

if (typeof window == 'undefined') window = global;
let dotnetify = window.dotnetify || _dotnetify;

dotnetify.vue = {
  version: '1.0.0',
  viewModels: {},
  plugins: {},
  controller: dotnetify,

  // Internal variables.
  _responseSubs: null,
  _reconnectedSubs: null,
  _connectedSubs: null,
  _connectionFailedSubs: null,

  // Initializes connection to SignalR server hub.
  init: function() {
    const self = dotnetify.vue;

    if (!self._responseSubs) {
      self._responseSubs = dotnetify.responseEvent.subscribe((iVMId, iVMData) => self._responseVM(iVMId, iVMData));
    }

    if (!self._connectedSubs) {
      self._connectedSubs = dotnetify.connectedEvent.subscribe(() =>
        Object.keys(self.viewModels).forEach(vmId => !self.viewModels[vmId].$requested && self.viewModels[vmId].$request())
      );
    }

    const start = function() {
      if (!dotnetify.isHubStarted) Object.keys(self.viewModels).forEach(vmId => (self.viewModels[vmId].$requested = false));
      dotnetify.startHub();
    };

    if (!self._reconnectedSubs) {
      self._reconnectedSubs = dotnetify.reconnectedEvent.subscribe(start);
    }

    dotnetify.initHub();
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
        return iVue.$data;
      },
      setState(state) {
        Object.keys(state).forEach(key => {
          const value = state[key];
          if (iVue.hasOwnProperty(key)) iVue[key] = value;
          else if (value) console.error(`'${key}' was received, but the property isn't defined in the Vue instance.`);
        });
      }
    };

    self.viewModels[iVMId] = new dotnetifyVM(iVMId, component, iOptions, self);
    if (iOptions && Array.isArray(iOptions.watch)) self._addWatchers(iOptions.watch, self.viewModels[iVMId], iVue);

    self.init();
    return self.viewModels[iVMId];
  },

  // Get all view models.
  getViewModels: function() {
    const self = dotnetify.vue;
    return Object.keys(self.viewModels).map(vmId => self.viewModels[vmId]);
  },

  _addWatchers(iWatchlist, iVM, iVue) {
    const callback = prop =>
      function(newValue) {
        iVM.$serverUpdate === true && iVM.$dispatch({ [prop]: newValue });
      }.bind(iVM);

    iWatchlist.forEach(prop => iVue.$watch(prop, callback(prop)));
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

export default dotnetify;
