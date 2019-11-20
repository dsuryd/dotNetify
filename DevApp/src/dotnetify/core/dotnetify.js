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
import dotnetifyHub, { dotnetifyHubFactory } from './dotnetify-hub';
import localHub, { hasLocalVM } from './dotnetify-hub-local';
import webApiHub, { createWebApiHub } from './dotnetify-hub-webapi';

export class dotnetifyFactory {
  static create() {
    const dotnetify = {
      // SignalR hub options.
      hub: dotnetifyHub,
      hubOptions: {
        transport: [ 'webSockets', 'longPolling' ],

        // Use this to add customize HubConnectionBuilder.
        connectionBuilder: builder => builder
      },
      hubPath: null,

      // Debug mode.
      debug: false,
      debugFn: null,

      // Offline mode. (WIP)
      offline: false,
      isOffline: true,
      offlineTimeout: 5000,
      offlineCacheFn: null,

      // Internal variables.
      _vmAccessors: [],

      // Use this to get notified of connection state changed events.
      // (state, exception, hub) => void
      connectionStateHandler: null,

      // Use this intercept a view model prior to establishing connection,
      // with the option to provide any connect parameters.
      // ({vmId, options}) => {vmId, options, hub}
      connectHandler: null,

      // Support changing hub server URL after first init.
      get hubServerUrl() {
        return this.hub.url;
      },

      set hubServerUrl(url) {
        this.hub.url = url;
        if (this.debug) console.log('SignalR: connecting to ' + this.hubServerUrl);
        if (this.hub.isHubStarted) this.startHub(this.hub, true);
      },

      // Generic connect function for non-React app.
      connect(iVMId, iOptions) {
        return dotnetify.react.connect(iVMId, {}, iOptions);
      },

      // Creates a SignalR hub client.
      createHub(hubServerUrl, hubPath, hubLib) {
        return this.initHub(dotnetifyHubFactory.create(), hubPath, hubServerUrl, hubLib);
      },

      // Creates a Web API hub client.
      createWebApiHub(baseUrl, onRequestHandler) {
        return createWebApiHub(baseUrl, onRequestHandler);
      },

      // Configures hub connection to SignalR hub server.
      initHub(hub, hubPath, hubServerUrl, hubLib) {
        hub = hub || this.hub;
        hubPath = hubPath || this.hubPath;
        hubServerUrl = hubServerUrl || this.hubServerUrl;
        hubLib = hubLib || this.hubLib;

        if (!hub.isHubStarted) {
          hub.init(hubPath, hubServerUrl, hubLib);

          // Use SignalR event to raise the connection state event.
          hub.stateChanged(state => this.handleConnectionStateChanged(state, null, hub));
        }
        return hub;
      },

      // Used by a view to select a hub, and provides the opportunity to override any connect info.
      selectHub(vmConnectArgs) {
        vmConnectArgs = vmConnectArgs || {};
        vmConnectArgs.options = vmConnectArgs.options || {};
        let override = (typeof this.connectHandler == 'function' && this.connectHandler(vmConnectArgs)) || {};
        if (!override.hub) {
          override.hub = hasLocalVM(vmConnectArgs.vmId) ? localHub : vmConnectArgs.options.webApi ? webApiHub : this.initHub();
          override.hub.debug = this.debug;
        }
        return Object.assign(vmConnectArgs, override);
      },

      // Starts hub connection to SignalR hub server.
      startHub(hub, forceRestart) {
        hub = hub || this.hub;

        const doneHandler = () => {};
        const failHandler = ex => this.handleConnectionStateChanged('error', ex, hub);
        hub.startHub(this.hubOptions, doneHandler, failHandler, forceRestart);
      },

      // Used by dotnetify-react and -vue to expose their view model accessors.
      addVMAccessor(vmAccessor) {
        !this._vmAccessors.includes(vmAccessor) && this._vmAccessors.push(vmAccessor);
      },

      checkServerSideException(iVMId, iVMData, iExceptionHandler) {
        const vmData = JSON.parse(iVMData);
        if (vmData && vmData.hasOwnProperty('ExceptionType') && vmData.hasOwnProperty('Message')) {
          const exception = { name: vmData.ExceptionType, message: vmData.Message };

          if (typeof iExceptionHandler === 'function') {
            return iExceptionHandler(exception);
          }
          else {
            console.error('[' + iVMId + '] ' + exception.name + ': ' + exception.message);
            throw exception;
          }
        }
      },

      // Get all view models.
      getViewModels() {
        return this._vmAccessors
          .reduce((prev, current) => [ ...prev, ...current() ], [])
          .filter((val, idx, self) => self.indexOf(val) === idx); // returns distinct items.
      },

      handleConnectionStateChanged(iState, iException, iHub) {
        if (this.debug) console.log('SignalR: ' + (iException ? iException.message : iState));
        if (typeof this.connectionStateHandler === 'function') this.connectionStateHandler(iState, iException, iHub);
        else if (iException) console.error(iException);
      }
    };

    return dotnetify;
  }
}

export default dotnetifyFactory.create();
