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
import dotnetifyHub from './dotnetify-hub';
import { createEventEmitter } from '../libs/utils';

const dotnetify = {
  // SignalR hub options.
  hub: dotnetifyHub,
  hubOptions: { transport: [ 'webSockets', 'longPolling' ] },
  hubPath: null,
  hubServerUrl: null,

  // Debug mode.
  debug: false,
  debugFn: null,

  // Offline mode. (WIP)
  offline: false,
  isOffline: true,
  offlineTimeout: 5000,
  offlineCacheFn: null,

  // Handler for connection state changed events.
  connectionStateHandler: null,

  // Connection events.
  responseEvent: createEventEmitter(),
  reconnectedEvent: createEventEmitter(),
  connectedEvent: createEventEmitter(),
  connectionFailedEvent: createEventEmitter(),

  // Whether connected to SignalR hub server.
  isConnected: function() {
    return dotnetifyHub.isConnected;
  },

  // Whether SignalR hub is started.
  isHubStarted: function() {
    return !!dotnetify._hub;
  },

  // Generic connect function for non-React app.
  connect: function(iVMId, iOptions) {
    return dotnetify.react.connect(iVMId, null, iOptions);
  },

  initHub() {
    if (dotnetify._hub !== null) return;

    dotnetifyHub.init(dotnetify.hubPath, dotnetify.hubServerUrl, dotnetify.hubLib);

    // Setup SignalR server method handler.
    dotnetifyHub.client.response_VM = function(iVMId, iVMData) {
      // SignalR .NET Core is sending an array of arguments.
      if (Array.isArray(iVMId)) {
        iVMData = iVMId[1];
        iVMId = iVMId[0];
      }

      let handled = dotnetify.responseEvent.emit(iVMId, iVMData);

      // If we get to this point, that means the server holds a view model instance
      // whose view no longer existed.  So, tell the server to dispose the view model.
      if (!handled) dotnetifyHub.server.dispose_VM(iVMId);
    };

    // On disconnected, keep attempting to start the connection.
    dotnetifyHub.disconnected(function() {
      dotnetify._hub = null;
      dotnetifyHub.reconnect(function() {
        dotnetify.reconnectedEvent.emit();
      });
    });

    // Use SignalR event to raise the connection state event.
    dotnetifyHub.stateChanged(function(state) {
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

    if (dotnetify._hub === null) {
      try {
        dotnetify._hub = dotnetifyHub.start(dotnetify.hubOptions).done(doneHandler).fail(failHandler);
      } catch (err) {
        dotnetify._hub = null;
      }
    }
    else {
      try {
        dotnetify._hub.done(doneHandler);
      } catch (err) {
        dotnetify._hub = null;
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
    dotnetifyHub.server.request_VM(iVMId, iOptions);
  },

  updateVM(iVMId, iValue) {
    dotnetifyHub.server.update_VM(iVMId, iValue);
  },

  disposeVM(iVMId) {
    dotnetifyHub.server.dispose_VM(iVMId);
  },

  _triggerConnectionStateEvent: function(iState, iException) {
    if (dotnetify.debug) console.log('SignalR: ' + (iException ? iException.message : iState));

    if (typeof dotnetify.connectionStateHandler === 'function') dotnetify.connectionStateHandler(iState, iException);
    else if (iException) console.error(iException);
  },

  // Internal variables. Do not modify!
  _hub: null
};

// Support changing hub server URL after first init.
Object.defineProperty(dotnetify, 'hubServerUrl', {
  get: () => dotnetify.hub.url,
  set: url => {
    dotnetify.hub.url = url;
    if (dotnetify.debug) console.log('SignalR: connecting to ' + dotnetify.hubServerUrl);
    if (dotnetify._hub) {
      dotnetify._hub = null;
      dotnetify.startHub();
    }
  }
});

export default dotnetify;
