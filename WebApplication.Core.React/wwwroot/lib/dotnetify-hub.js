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
      module.exports = factory(jquery, _window, require('./no-jquery.signalR'));
   }
   else if (typeof define === "function" && define["amd"]) {
      define(['./jquery-shim', './no-jquery.signalR'], factory);
   }
   else {
      factory(jQuery, _window);
   }
}
   (function ($, window) {

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
            return $.connection.hub.stateChanged(iHandler);
         }
      });

      return dotnetifyHub;
   }))