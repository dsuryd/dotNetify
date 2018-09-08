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
import _dotnetify from '../dotnetify-base';
import dotnetifyVM from '../core/dotnetify-vm';

if (typeof window == 'undefined') window = global;
let dotnetify = window.dotnetify || _dotnetify;

dotnetify.react = {
  version: '2.0.0',
  viewModels: {},
  plugins: {},
  core: dotnetify,

  // Internal variables.
  _responseSubs: null,
  _reconnectedSubs: null,
  _connectedSubs: null,
  _connectionFailedSubs: null,

  // Initializes connection to SignalR server hub.
  init: function() {
    const self = dotnetify.react;

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
  connect: function(iVMId, iReact, iOptions) {
    if (arguments.length < 2) throw new Error('[dotNetify] Missing arguments. Usage: connect(vmId, component) ');

    if (dotnetify.ssr && dotnetify.react.ssrConnect) {
      var vmArg = iOptions && iOptions['vmArg'];
      return dotnetify.react.ssrConnect(iVMId, iReact, vmArg);
    }

    var self = dotnetify.react;
    if (!self.viewModels.hasOwnProperty(iVMId)) {
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
      self.viewModels[iVMId] = new dotnetifyVM(iVMId, component, iOptions, dotnetify.react);
    }
    else
      console.error(
        `Component is attempting to connect to an already active '${iVMId}'. ` +
          ` If it's from a dismounted component, you must add vm.$destroy to componentWillUnmount().`
      );

    self.init();
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

export default dotnetify;
