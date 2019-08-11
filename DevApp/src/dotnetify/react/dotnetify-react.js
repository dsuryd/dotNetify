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
import _dotnetify from '../core/dotnetify';
import dotnetifyVM from '../core/dotnetify-vm';

if (typeof window == 'undefined') window = global;
let dotnetify = window.dotnetify || _dotnetify;

dotnetify.react = {
  version: '3.0.0',
  viewModels: {},
  plugins: {},
  controller: dotnetify,

  // Internal variables.
  _responseSubs: null,
  _reconnectedSubs: null,
  _connectedSubs: null,
  _connectionFailedSubs: null,

  // Initializes connection to SignalR server hub.
  init: function(iVMId, iVMArg) {
    const self = dotnetify.react;
    const hub = dotnetify.selectHub(iVMId, iVMArg);

    if (!self._responseSubs) {
      self._responseSubs = hub.responseEvent.subscribe((iVMId, iVMData) => self._responseVM(iVMId, iVMData));
    }

    if (!self._connectedSubs) {
      self._connectedSubs = hub.connectedEvent.subscribe(() =>
        Object.keys(self.viewModels).forEach(vmId => !self.viewModels[vmId].$requested && self.viewModels[vmId].$request())
      );
    }

    const start = function() {
      if (!hub.isHubStarted) Object.keys(self.viewModels).forEach(vmId => (self.viewModels[vmId].$requested = false));
      dotnetify.startHub(hub);
      return hub;
    };

    if (!self._reconnectedSubs) {
      self._reconnectedSubs = hub.reconnectedEvent.subscribe(start);
    }

    return start();
  },

  // Connects to a server view model.
  connect: function(iVMId, iReact, iOptions) {
    if (arguments.length < 2) throw new Error('[dotNetify] Missing arguments. Usage: connect(vmId, component) ');

    const vmArg = iOptions && iOptions['vmArg'];

    if (dotnetify.ssr && dotnetify.react.ssrConnect) {
      return dotnetify.react.ssrConnect(iVMId, iReact, vmArg);
    }

    const self = dotnetify.react;
    if (self.viewModels.hasOwnProperty(iVMId)) {
      console.error(
        `Component is attempting to connect to an already active '${iVMId}'. ` +
          ` If it's from a dismounted component, you must add vm.$destroy to componentWillUnmount().`
      );
      self.viewModels[iVMId].$destroy();
      return setTimeout(() => self.connect(iVMId, iReact, iOptions));
    }

    const component = {
      get props() {
        return iReact.props;
      },
      get state() {
        return iReact.state;
      },
      setState(state) {
        iReact.setState(state);
      }
    };

    const hub = self.init(iVMId, vmArg);
    self.viewModels[iVMId] = new dotnetifyVM(iVMId, component, iOptions, self, hub);
    return self.viewModels[iVMId];
  },

  // Get all view models.
  getViewModels: function() {
    const self = dotnetify.react;
    return Object.keys(self.viewModels).map(vmId => self.viewModels[vmId]);
  },

  _responseVM: function(iVMId, iVMData) {
    const self = dotnetify.react;

    if (self.viewModels.hasOwnProperty(iVMId)) {
      const vm = self.viewModels[iVMId];
      dotnetify.checkServerSideException(iVMId, iVMData, vm.$exceptionHandler);
      vm.$update(iVMData);
      return true;
    }
    return false;
  }
};

dotnetify.addVMAccessor(dotnetify.react.getViewModels);

export default dotnetify;
