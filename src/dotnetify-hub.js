/* 
Copyright 2017 Dicky Suryadi

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

// Namespace.
var dotnetifyHub = typeof dotnetifyHub === "undefined" ? {} : dotnetifyHub;

(function (factory) {
   var _window = window || global;

   if (typeof exports === "object" && typeof module === "object") {
      var jquery = typeof window !== "undefined" ? window.jQuery || require('./jquery-shim') : require('./jquery-shim');
      module.exports = factory(jquery, require('signalR'), _window);
   }
   else if (typeof define === "function" && define["amd"]) {
      define(['jquery', 'signalr'], factory);
   }
   else {
      factory(jQuery, signalR, _window);
   }
}
   (function ($, signalR, window) {

      // SignalR .NET Core.
      if (signalR && signalR.HubConnection) {

         Object.defineProperty(dotnetifyHub, "isConnected", {
            get: function () { return dotnetifyHub._connection && dotnetifyHub._connection.connection.connectionState === 2 },
         });

         dotnetifyHub = $.extend(dotnetifyHub, {
            hubPath: "/dotnetify",
            url: null,
            reconnectDelay: 5000,

            // Internal variables. Do not modify!
            _connection: null,
            _disconnectedHandler: function () { },
            _stateChangedHandler: function (iNewState) { },

            _onDisconnected: function () {
               dotnetifyHub._changeState(4);
               dotnetifyHub._disconnectedHandler();
            },
            _changeState: function (iNewState) {
               var stateText = { 0: 'connecting', 1: 'connected', 2: 'reconnecting', 4: 'disconnected' };
               dotnetifyHub._stateChangedHandler(stateText[iNewState]);
            },

            start: function (iHubOptions) {
               // Map the transport option.
               var transportOptions = { 'webSockets': 0, 'serverSentEvents': 1, 'longPolling': 2 };
               if (iHubOptions && Array.isArray(iHubOptions.transport))
                  iHubOptions.transport = transportOptions[iHubOptions.transport[0]];

               var url = dotnetifyHub.url ? dotnetifyHub.url + dotnetifyHub.hubPath : dotnetifyHub.hubPath;
               dotnetifyHub._connection = new signalR.HubConnection(url, iHubOptions);
               dotnetifyHub._connection.on("response_vm", dotnetifyHub.client.response_VM);
               dotnetifyHub._connection.onclose(dotnetifyHub._onDisconnected);

               var promise = dotnetifyHub._connection.start();
               promise
                  .then(function () { dotnetifyHub._changeState(1); })
                  .catch(dotnetifyHub._onDisconnected);

               return {
                  done: function (iHandler) { promise.then(iHandler).catch(function () { }); return this; },
                  fail: function (iHandler) { promise.catch(iHandler); return this; }
               };
            },

            disconnected: function (iHandler) {
               if (typeof iHandler === "function")
                  dotnetifyHub._disconnectedHandler = iHandler;
            },

            stateChanged: function (iHandler) {
               if (typeof iHandler === "function")
                  dotnetifyHub._stateChangedHandler = iHandler;
            },

            reconnect: function (iStartHubFunc) {
               if (typeof iStartHubFunc === "function")
                  setTimeout(function () {
                     dotnetifyHub._changeState(2);
                     iStartHubFunc();
                  }, dotnetifyHub.reconnectDelay);
            },

            client: {},

            server: {
               dispose_VM: function (iVMId) { dotnetifyHub._connection.invoke("Dispose_VM", iVMId); },
               update_VM: function (iVMId, iValue) { dotnetifyHub._connection.invoke("Update_VM", iVMId, iValue); },
               request_VM: function (iVMId, iArgs) { dotnetifyHub._connection.invoke("Request_VM", iVMId, iArgs); }
            }
         });
      }
      // SignalR .NET FX.
      else {
         // SignalR hub auto-generated from /signalr/hubs.
         /// <reference path="..\..\SignalR.Client.JS\Scripts\jquery-1.6.4.js" />
         /// <reference path="jquery.signalR.js" />
         (function ($, window, undefined) {
            /// <param name="$" type="jQuery" />
            "use strict";

            if (typeof ($.signalR) !== "function") {
               throw new Error("SignalR: SignalR is not loaded. Please ensure jquery.signalR-x.js is referenced before ~/signalr/js.");
            }

            var signalR = $.signalR;

            function makeProxyCallback(hub, callback) {
               return function () {
                  // Call the client hub method
                  callback.apply(hub, $.makeArray(arguments));
               };
            }

            function registerHubProxies(instance, shouldSubscribe) {
               var key, hub, memberKey, memberValue, subscriptionMethod;

               for (key in instance) {
                  if (instance.hasOwnProperty(key)) {
                     hub = instance[key];

                     if (!(hub.hubName)) {
                        // Not a client hub
                        continue;
                     }

                     if (shouldSubscribe) {
                        // We want to subscribe to the hub events
                        subscriptionMethod = hub.on;
                     } else {
                        // We want to unsubscribe from the hub events
                        subscriptionMethod = hub.off;
                     }

                     // Loop through all members on the hub and find client hub functions to subscribe/unsubscribe
                     for (memberKey in hub.client) {
                        if (hub.client.hasOwnProperty(memberKey)) {
                           memberValue = hub.client[memberKey];

                           if (!$.isFunction(memberValue)) {
                              // Not a client hub function
                              continue;
                           }

                           subscriptionMethod.call(hub, memberKey, makeProxyCallback(hub, memberValue));
                        }
                     }
                  }
               }
            }

            $.hubConnection.prototype.createHubProxies = function () {
               var proxies = {};
               this.starting(function () {
                  // Register the hub proxies as subscribed
                  // (instance, shouldSubscribe)
                  registerHubProxies(proxies, true);

                  this._registerSubscribedHubs();
               }).disconnected(function () {
                  // Unsubscribe all hub proxies when we "disconnect".  This is to ensure that we do not re-add functional call backs.
                  // (instance, shouldSubscribe)
                  registerHubProxies(proxies, false);
               });

               proxies['dotNetifyHub'] = this.createHubProxy('dotNetifyHub');
               proxies['dotNetifyHub'].client = {};
               proxies['dotNetifyHub'].server = {
                  dispose_VM: function (vmId) {
                     return proxies['dotNetifyHub'].invoke.apply(proxies['dotNetifyHub'], $.merge(["Dispose_VM"], $.makeArray(arguments)));
                  },

                  request_VM: function (vmId, vmArg) {
                     return proxies['dotNetifyHub'].invoke.apply(proxies['dotNetifyHub'], $.merge(["Request_VM"], $.makeArray(arguments)));
                  },

                  update_VM: function (vmId, vmData) {
                     return proxies['dotNetifyHub'].invoke.apply(proxies['dotNetifyHub'], $.merge(["Update_VM"], $.makeArray(arguments)));
                  }
               };

               return proxies;
            };

            signalR.hub = $.hubConnection(dotnetifyHub.hubPath, { useDefaultPath: false });
            $.extend(signalR, signalR.hub.createHubProxies());

         }($, window));

         Object.defineProperty(dotnetifyHub, "state", {
            get: function () { return $.connection.hub.state; },
            set: function (val) { $.connection.hub.state = val; }
         });

         Object.defineProperty(dotnetifyHub, "url", {
            get: function () { return $.connection.hub.url; },
            set: function (val) { $.connection.hub.url = val; }
         });

         Object.defineProperty(dotnetifyHub, "client", {
            get: function () { return $.connection.dotNetifyHub.client; },
         });

         Object.defineProperty(dotnetifyHub, "server", {
            get: function () { return $.connection.dotNetifyHub.server; },
         });

         Object.defineProperty(dotnetifyHub, "isConnected", {
            get: function () { return $.connection.hub.state == $.signalR.connectionState.connected; },
         });

         dotnetifyHub = $.extend(dotnetifyHub, {
            hubPath: "/signalr",

            start: function (iHubOptions) {
               if (typeof iHubOptions === "undefined")
                  return $.connection.hub.start();
               return $.connection.hub.start(iHubOptions);
            },

            disconnected: function (iHandler) {
               return $.connection.hub.disconnected(iHandler);
            },

            stateChanged: function (iHandler) {
               return $.connection.hub.stateChanged(function (state) {
                  var stateText = { 0: 'connecting', 1: 'connected', 2: 'reconnecting', 4: 'disconnected' };
                  iHandler(stateText[state.newState]);
               });
            }
         });
      }

      dotnetifyHub = $.extend(dotnetifyHub, {
         version: "1.1.0-beta"
      });

      return dotnetifyHub;
   }))