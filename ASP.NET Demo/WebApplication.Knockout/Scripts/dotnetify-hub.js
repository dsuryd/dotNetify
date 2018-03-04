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
      var jquery = window ? window.jQuery || require('./jquery-shim') : require('./jquery-shim');
      var signalR = window ? window.signalR || require('./signalR-netcore') : require('./signalR-netcore');
      module.exports = factory(jquery, signalR, _window);
   }
   else if (typeof define === "function" && define["amd"]) {
      define(['jquery', 'signalR'], factory);
   }
   else {
      factory(jQuery, _window.signalR, _window);
   }
}
   (function ($, defaultSignalR, window) {

      dotnetifyHub = $.extend(dotnetifyHub, {
         version: "1.1.1-beta",
         type: null,
         _init: false,

      });

      dotnetifyHub.init = function (iHubPath, iServerUrl, signalR) {
         if (dotnetifyHub._init)
            return;

         dotnetifyHub._init = true;
         signalR = signalR || defaultSignalR;

         // SignalR .NET Core.
         if (signalR && signalR.HubConnection) {
            dotnetifyHub.type = "netcore";

            Object.defineProperty(dotnetifyHub, "isConnected", {
               get: function () { return dotnetifyHub._connection && dotnetifyHub._connection.connection.connectionState === 1 },
            });

            dotnetifyHub = $.extend(dotnetifyHub, {
               hubPath: iHubPath || "/dotnetify",
               url: iServerUrl,
               reconnectDelay: [2, 5, 10],
               reconnectRetry: null,

               // Internal variables. Do not modify!
               _connection: null,
               _reconnectCount: 0,
               _startDoneHandler: null,
               _startFailHandler: null,
               _disconnectedHandler: function () { },
               _stateChangedHandler: function (iNewState) { },

               _onDisconnected: function () {
                  dotnetifyHub._changeState(4);
                  dotnetifyHub._disconnectedHandler();
               },

               _changeState: function (iNewState) {
                  if (iNewState == 1)
                     dotnetifyHub._reconnectCount = 0;

                  var stateText = { 0: 'connecting', 1: 'connected', 2: 'reconnecting', 4: 'disconnected', 99: 'terminated' };
                  dotnetifyHub._stateChangedHandler(stateText[iNewState]);
               },

               _startConnection: function (iHubOptions, iTransportArray) {
                  var url = dotnetifyHub.url ? dotnetifyHub.url + dotnetifyHub.hubPath : dotnetifyHub.hubPath;
                  var hubOptions = {};
                  Object.keys(iHubOptions).forEach(function (key) { hubOptions[key] = iHubOptions[key] });
                  hubOptions.transport = iTransportArray.shift();

                  dotnetifyHub._connection = new signalR.HubConnection(url, hubOptions);
                  dotnetifyHub._connection.on("response_vm", dotnetifyHub.client.response_VM);
                  dotnetifyHub._connection.onclose(dotnetifyHub._onDisconnected);

                  var promise = dotnetifyHub._connection.start()
                     .then(function () { dotnetifyHub._changeState(1); })
                     .catch(function () {
                        // If failed to start, fallback to the next transport.
                        if (iTransportArray.length > 0)
                           dotnetifyHub._startConnection(iHubOptions, iTransportArray);
                        else
                           dotnetifyHub._onDisconnected();
                     });

                  if (typeof dotnetifyHub._startDoneHandler === "function")
                     promise.then(dotnetifyHub._startDoneHandler).catch(dotnetifyHub._startFailHandler || function () { });
                  return promise;
               },

               start: function (iHubOptions) {
                  dotnetifyHub._startDoneHandler = null;
                  dotnetifyHub._startFailHandler = null;

                  // Map the transport option.
                  var transport = [0];
                  var transportOptions = { 'webSockets': 0, 'serverSentEvents': 1, 'longPolling': 2 };
                  if (iHubOptions && Array.isArray(iHubOptions.transport))
                     transport = iHubOptions.transport.map(function (arg) { return transportOptions[arg] });

                  var promise = dotnetifyHub._startConnection(iHubOptions, transport);
                  return {
                     done: function (iHandler) {
                        dotnetifyHub._startDoneHandler = iHandler;
                        promise.then(iHandler).catch(function () { });
                        return this;
                     },
                     fail: function (iHandler) {
                        dotnetifyHub._startFailHandler = iHandler;
                        promise.catch(iHandler);
                        return this;
                     }
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
                  if (typeof iStartHubFunc === "function") {
                     // Only attempt reconnect if the specified retry hasn't been exceeded.
                     if (!dotnetifyHub.reconnectRetry || dotnetifyHub._reconnectCount < dotnetifyHub.reconnectRetry) {

                        // Determine reconnect delay from the specified configuration array.
                        var delay = dotnetifyHub._reconnectCount < dotnetifyHub.reconnectDelay.length ?
                           dotnetifyHub.reconnectDelay[dotnetifyHub._reconnectCount] :
                           dotnetifyHub.reconnectDelay[dotnetifyHub.reconnectDelay.length - 1];

                        dotnetifyHub._reconnectCount++;

                        setTimeout(function () {
                           dotnetifyHub._changeState(2);
                           iStartHubFunc();
                        }, delay * 1000);
                     }
                     else
                        dotnetifyHub._changeState(99);
                  }
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
            dotnetifyHub.type = "netfx";

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
               hubPath: iHubPath || "/signalr",
               url: iServerUrl,
               reconnectDelay: [2, 5, 10],
               reconnectRetry: null,

               _reconnectCount: 0,
               _stateChangedHandler: function (iNewState) { },

               start: function (iHubOptions) {

                  if (dotnetifyHub.url)
                     $.connection.hub.url = dotnetifyHub.url;

                  var deferred;
                  if (iHubOptions)
                     deferred = $.connection.hub.start(iHubOptions);
                  else
                     deferred = $.connection.hub.start();
                  deferred.fail(function (error) {
                     if (error.source && error.source.message === "Error parsing negotiate response.")
                        console.warn("This client may be attempting to connect to an incompatible SignalR .NET Core server.")
                  });
                  return deferred;
               },

               disconnected: function (iHandler) {
                  return $.connection.hub.disconnected(iHandler);
               },

               stateChanged: function (iHandler) {
                  dotnetifyHub._stateChangedHandler = iHandler;
                  return $.connection.hub.stateChanged(function (state) {
                     if (state == 1)
                        dotnetifyHub._reconnectCount = 0;

                     var stateText = { 0: 'connecting', 1: 'connected', 2: 'reconnecting', 4: 'disconnected' };
                     iHandler(stateText[state.newState]);
                  });
               },

               reconnect: function (iStartHubFunc) {
                  if (typeof iStartHubFunc === "function") {
                     // Only attempt reconnect if the specified retry hasn't been exceeded.
                     if (!dotnetifyHub.reconnectRetry || dotnetifyHub._reconnectCount < dotnetifyHub.reconnectRetry) {

                        // Determine reconnect delay from the specified configuration array.
                        var delay = dotnetifyHub._reconnectCount < dotnetifyHub.reconnectDelay.length ?
                           dotnetifyHub.reconnectDelay[dotnetifyHub._reconnectCount] :
                           dotnetifyHub.reconnectDelay[dotnetifyHub.reconnectDelay.length - 1];

                        dotnetifyHub._reconnectCount++;

                        setTimeout(function () {
                           dotnetifyHub._stateChangedHandler('reconnecting');
                           iStartHubFunc();
                        }, delay * 1000);
                     }
                     else
                        dotnetifyHub._stateChangedHandler('terminated');
                  }
               }
            });
         }
      };

      return dotnetifyHub;
   }))