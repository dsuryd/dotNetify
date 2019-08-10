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
import dotnetifyHub, { dotnetifyHubFactory } from './dotnetify-hub';

export class dotnetifyFactory {
  static create() {
    const dotnetify = {
      // SignalR hub options.
      hub: dotnetifyHub,
      hubOptions: { transport: [ 'webSockets', 'longPolling' ] },
      hubPath: null,

      // Debug mode.
      debug: false,
      debugFn: null,

      // Offline mode. (WIP)
      offline: false,
      isOffline: true,
      offlineTimeout: 5000,
      offlineCacheFn: null,

      // Use this to get notified of connection state changed events.
      connectionStateHandler: null, // (state, exception, hub) => void

      // Use this to assign a hub to which a view model will connect.
      hubHandler: null, // (vmId, vmArg) => hub

      // Generic connect function for non-React app.
      connect: function(iVMId, iOptions) {
        return dotnetify.react.connect(iVMId, null, iOptions);
      },

      // Creates a SignalR hub client.
      createHub(hubServerUrl, hubPath, hubLib) {
        return dotnetify.initHub(dotnetifyHubFactory.create(), hubPath, hubServerUrl, hubLib);
      },

      getHub(iVMId, iVMArg) {
        // Allow switching to another hub by providing hubHandler function that returns a signalR hub object.
        const hub = typeof dotnetify.hubHandler == 'function' && dotnetify.hubHandler(iVMId, iVMArg);
        return hub || dotnetify.initHub();
      },

      initHub(hub, hubPath, hubServerUrl, hubLib) {
        hub = hub || dotnetify.hub;
        hubPath = hubPath || dotnetify.hubPath;
        hubServerUrl = hubServerUrl || dotnetify.hubServerUrl;
        hubLib = hubLib || dotnetify.hubLib;

        if (!hub.isHubStarted()) {
          hub.init(hubPath, hubServerUrl, hubLib);

          // Use SignalR event to raise the connection state event.
          hub.stateChanged(function(state) {
            dotnetify._triggerConnectionStateEvent(state, null, hub);
          });
        }
        return hub;
      },

      startHub(hub) {
        hub = hub || dotnetify.hub;

        const doneHandler = () => {};
        const failHandler = ex => dotnetify._triggerConnectionStateEvent('error', ex, hub);
        hub.startHub(dotnetify.hubOptions, doneHandler, failHandler);
      },

      checkServerSideException: function(iVMId, iVMData, iExceptionHandler) {
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

      _triggerConnectionStateEvent: function(iState, iException, iHub) {
        if (dotnetify.debug) console.log('SignalR: ' + (iException ? iException.message : iState));

        if (typeof dotnetify.connectionStateHandler === 'function') dotnetify.connectionStateHandler(iState, iException, iHub);
        else if (iException) console.error(iException);
      }
    };

    // Support changing hub server URL after first init.
    Object.defineProperty(dotnetify, 'hubServerUrl', {
      get: () => dotnetify.hub.url,
      set: url => {
        dotnetify.hub.url = url;
        if (dotnetify.debug) console.log('SignalR: connecting to ' + dotnetify.hubServerUrl);
        if (dotnetify.hub.startInfo) {
          dotnetify.hub.startInfo = null;
          dotnetify.startHub();
        }
      }
    });

    return dotnetify;
  }
}

export default dotnetifyFactory.create();
