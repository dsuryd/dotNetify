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
import { createEventEmitter } from '../libs/utils';

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
      connectionStateHandler: null, // (state, exception) => void

      // Use this to assign a hub to which a view model will connect.
      hubHandler: null, // (vmId) => dotnetifyHub

      // Connection events.
      responseEvent: createEventEmitter(),
      reconnectedEvent: createEventEmitter(),
      connectedEvent: createEventEmitter(),
      connectionFailedEvent: createEventEmitter(),

      // Whether connected to SignalR hub server.
      isConnected: function() {
        return dotnetify.hub.isConnected;
      },

      // Whether SignalR hub is started.
      isHubStarted: function() {
        return !!dotnetify.hub.startInfo;
      },

      // Generic connect function for non-React app.
      connect: function(iVMId, iOptions) {
        return dotnetify.react.connect(iVMId, null, iOptions);
      },

      // Creates a SignalR hub.
      createHub(hubServerUrl, hubPath, hubLib) {
        const hub = dotnetifyHubFactory.create();
        dotnetify._initHub(hub, hubPath, hubServerUrl, hubLib);
        return hub;
      },

      initHub(iVMId) {
        // Allow switching to another hub by providing hubHandler function that returns a signalR hub object.
        if (typeof dotnetify.hubHandler == 'function') {
          const hub = dotnetify.hubHandler(iVMId);
          dotnetify.hub = hub || dotnetify.hub;
        }
        dotnetify._initHub(dotnetify.hub);
      },

      _initHub(hub, hubPath, hubServerUrl, hubLib) {
        if (hub.startInfo !== null) return;

        hubPath = hubPath || dotnetify.hubPath;
        hubServerUrl = hubServerUrl || dotnetify.hubServerUrl;
        hubLib = hubLib || dotnetify.hubLib;

        hub.init(hubPath, hubServerUrl, hubLib);

        // Setup SignalR server method handler.
        hub.client.response_VM = function(iVMId, iVMData) {
          // SignalR .NET Core is sending an array of arguments.
          if (Array.isArray(iVMId)) {
            iVMData = iVMId[1];
            iVMId = iVMId[0];
          }

          let handled = dotnetify.responseEvent.emit(iVMId, iVMData);

          // If we get to this point, that means the server holds a view model instance
          // whose view no longer existed.  So, tell the server to dispose the view model.
          if (!handled) hub.server.dispose_VM(iVMId);
        };

        // On disconnected, keep attempting to start the connection.
        hub.disconnected(function() {
          hub.startInfo = null;
          hub.reconnect(function() {
            dotnetify.reconnectedEvent.emit();
          });
        });

        // Use SignalR event to raise the connection state event.
        hub.stateChanged(function(state) {
          dotnetify._triggerConnectionStateEvent(state);
        });
      },

      startHub: function() {
        const doneHandler = function() {
          dotnetify.connectedEvent.emit();
        };
        const failHandler = function(ex) {
          dotnetify.connectionFailedEvent.emit();
          dotnetify._triggerConnectionStateEvent('error', ex);
          throw ex;
        };

        if (dotnetify.hub.startInfo === null) {
          try {
            dotnetify.hub.startInfo = dotnetify.hub.start(dotnetify.hubOptions).done(doneHandler).fail(failHandler);
          } catch (err) {
            dotnetify.hub.startInfo = null;
          }
        }
        else {
          try {
            dotnetify.hub.startInfo.done(doneHandler);
          } catch (err) {
            dotnetify.hub.startInfo = null;
            return dotnetify.startHub();
          }
        }
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

      requestVM(iVMId, iOptions) {
        dotnetify.hub.server.request_VM(iVMId, iOptions);
      },

      updateVM(iVMId, iValue) {
        dotnetify.hub.server.update_VM(iVMId, iValue);
      },

      disposeVM(iVMId) {
        dotnetify.hub.server.dispose_VM(iVMId);
      },

      _triggerConnectionStateEvent: function(iState, iException) {
        if (dotnetify.debug) console.log('SignalR: ' + (iException ? iException.message : iState));

        if (typeof dotnetify.connectionStateHandler === 'function') dotnetify.connectionStateHandler(iState, iException);
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
