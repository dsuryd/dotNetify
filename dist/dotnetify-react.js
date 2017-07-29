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

// Our namespace.
var dotnetify = typeof dotnetify === "undefined" ? {} : dotnetify;

// Support using either AMD or CommonJS that loads our app.js, or being placed in <script> tag.
(function (factory) {
   if (typeof define === "function" && define["amd"]) {
      define(['react', 'react-dom', 'jquery', 'signalr'], factory);
   }
   else if (typeof exports === "object" && typeof module === "object") {
      if (typeof window.jQuery === "undefined")
         window.jQuery = require('jquery');
      module.exports = factory(require('react'), require('react-dom'), window.jQuery, require('signalr'));
   }
   else {
      factory(React, ReactDOM, jQuery);
   }
}
   (function (_React, _ReactDOM, $) {

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

         signalR.hub = $.hubConnection("/signalr", { useDefaultPath: false });
         $.extend(signalR, signalR.hub.createHubProxies());

      }($, window));

      dotnetify = $.extend(dotnetify, {
         // SignalR hub.
         hub: null,
         hubServer: $.connection.dotNetifyHub.server,
         hubOptions: { "transport": ["webSockets", "longPolling"] },

         // Debug mode.
         debug: true,
         debugFn: null,

         // Offline mode.
         offline: false,
         isOffline: true,
         offlineTimeout: 5000,
         offlineCacheFn: null,

         _connectRetry: 0,
         isConnected: function () {
            return $.connection.hub.state == $.signalR.connectionState.connected
         },
      });

      dotnetify.react = $.extend(dotnetify.hasOwnProperty("react") ? dotnetify.react : {}, {
         version: "1.0.3-beta",
         viewModels: {},
         plugins: {},

         // Initializes connection to SignalR server hub.
         init: function () {
            var self = dotnetify.react;
            var getInitialStates = function () {
               for (var vmId in self.viewModels) {
                  if (!self.viewModels[vmId].$requested)
                     self.viewModels[vmId].$request();
               }
            };

            if (dotnetify.hub === null) {
               // Setup SignalR server method handler.
               var hub = $.connection.dotNetifyHub;
               hub.client.response_VM = function (iVMId, iVMData) {

                  var vm = self.viewModels.hasOwnProperty(iVMId) ? self.viewModels[iVMId] : null;

                  // Handle server-side exception.
                  var vmData = JSON.parse(iVMData);
                  if (vmData && vmData.hasOwnProperty("ExceptionType") && vmData.hasOwnProperty("Message")) {
                     var exception = { name: vmData.ExceptionType, message: vmData.Message };

                     if (vm && typeof vm.$exceptionHandler === "function")
                        return vm.$exceptionHandler(exception);

                     console.error("[" + iVMId + "] " + exception.name + ": " + exception.message);
                     throw exception;
                  }

                  if (vm)
                     vm.$update(iVMData);
                  else
                     // If we get to this point, that means the server holds a view model instance
                     // whose view no longer existed.  So, tell the server to dispose the view model.
                     hub.server.dispose_VM(iVMId);
               };

               // Start SignalR hub connection, and if successful, apply the widget to all scoped elements.
               var startHub = function () {
                  var hub = typeof dotnetify.hubOptions === "undefined" ? $.connection.hub.start() : $.connection.hub.start(dotnetify.hubOptions);
                  hub.done(function () {
                     dotnetify._connectRetry = 0;
                     getInitialStates();
                  })
                     .fail(function (e) {
                        console.error(e);
                     });

                  // If offline mode is enabled, apply the widget anyway when there's no connection.
                  setTimeout(function () {
                     if (dotnetify.offline && !dotnetify.isConnected()) {
                        getInitialStates();
                        dotnetify.isOffline = true;
                        $(document).trigger("offline", dotnetify.isOffline);
                     }
                  }, dotnetify.offlineTimeout);

                  return hub;
               }
               dotnetify.hub = startHub();

               // On disconnected, keep attempting to start the connection in increasing interval.
               $.connection.hub.disconnected(function () {
                  setTimeout(function () {
                     dotnetify.hub = startHub();
                  }, dotnetify._connectRetry * 5000 + 500);

                  if (dotnetify._connectRetry < 3)
                     dotnetify._connectRetry++;
               });

               // Use SignalR event to raise the offline event with true/false argument.
               $.connection.hub.stateChanged(function (state) {
                  var stateText = { 0: 'connecting', 1: 'connected', 2: 'reconnecting', 4: 'disconnected' };
                  console.log("SignalR: " + stateText[state.newState]);

                  var isOffline = state.newState != 1;
                  if (dotnetify.isOffline != isOffline) {
                     dotnetify.isOffline = isOffline;
                     $(document).trigger("offline", dotnetify.isOffline);
                  }
               });
            }
            else if (dotnetify.isConnected())
               dotnetify.hub.done(getInitialStates);
            else if (dotnetify.offline)
               getInitialStates();
         },

         // Connects to a server view model.
         connect: function (iVMId, iReact, iOptions) {
            if (arguments.length < 2)
               throw new Error("[dotNetify] Missing arguments. Usage: connect(vmId, component) ");
            else if (arguments.length > 3)
               throw new Error("[dotNetify] Deprecated parameters. New usage: connect(vmId, component [,{getState, setState, vmArg, headers, exceptionHandler}]) ");

            if (dotnetify.ssr) {
               var vmArg = iOptions && iOptions["vmArg"];
               return dotnetify.react.ssrConnect(iVMId, iReact, vmArg);
            }

            var self = dotnetify.react;
            if (!self.viewModels.hasOwnProperty(iVMId))
               self.viewModels[iVMId] = new dotnetifyVM(iVMId, iReact, iOptions);
            else
               console.error("Component is attempting to connect to an already active '" + iVMId + "'.  If it's from a dismounted component, you must add vm.$destroy to componentWillUnmount().");

            self.init();
            return self.viewModels[iVMId];
         },

         // Used by server-side rendering in lieu of connect method.
         ssrConnect: function (iVMId, iReact, iVMArg) {
            if (window.ssr == null || !window.ssr.hasOwnProperty(iVMId))
               console.error("Server-side rendering requires initial state in 'window.ssr." + iVMId + "'.");

            var self = dotnetify.react;
            var vmState = window.ssr[iVMId];
            var getState = function () { return vmState; };
            var setState = function (state) { vmState = $.extend(vmState, state); };
            var vm = self.viewModels[iVMId] = new dotnetifyVM(iVMId, iReact, getState, setState, iVMArg);

            // Need to be asynch to allow initial state to be processed.
            setTimeout(function () { vm.$update(JSON.stringify(window.ssr[iVMId])); }, 100);
            return vm;
         }
      });

      // Client-side view model that acts as a proxy of the server view model.
      // iVMId - identifies the view model.
      // iReact - React component.
      // iOptions - Optional configuration options:
      //    getState: state accessor.
      //    setState: state mutator. 
      //    vmArg: view model arguments.
      //    headers: request headers, for things like authentication token.
      function dotnetifyVM(iVMId, iReact, iOptions) {

         this.$vmId = iVMId;
         this.$component = iReact;
         this.$vmArg = iOptions && iOptions["vmArg"];
         this.$headers = iOptions && iOptions["headers"];
         this.$exceptionHandler = iOptions && iOptions["exceptionHandler"];
         this.$requested = false;
         this.$loaded = false;
         this.$itemKey = {};

         var getState = iOptions && iOptions["getState"];
         var setState = iOptions && iOptions["setState"];
         getState = typeof getState === "function" ? getState : function () { return iReact.state; };
         setState = typeof setState === "function" ? setState : function (state) { iReact.setState(state); };

         if (iReact && iReact.props.hasOwnProperty("vmArg"))
            this.$vmArg = $.extend(this.$vmArg, iReact.props.vmArg);

         this.State = function (state) { return typeof state === "undefined" ? getState() : setState(state) };

         // Inject plugin functions into this view model.
         for (var pluginId in dotnetify.react.plugins) {
            var plugin = dotnetify.react.plugins[pluginId];
            if (plugin.hasOwnProperty("$inject"))
               plugin.$inject(this);
         }
      }

      // Disposes the view model, both here and on the server.
      dotnetifyVM.prototype.$destroy = function () {

         // Call any plugin's $destroy function if provided.
         for (var pluginId in dotnetify.react.plugins) {
            var plugin = dotnetify.react.plugins[pluginId];
            if (typeof plugin["$destroy"] === "function")
               plugin.$destroy.apply(this);
         }
         dotnetify.hubServer.dispose_VM(this.$vmId);
         delete dotnetify.react.viewModels[this.$vmId];
      }

      // Dispatches a value to the server view model.
      // iValue - Vvalue to dispatch.
      dotnetifyVM.prototype.$dispatch = function (iValue) {

         if (dotnetify.isConnected()) {
            try {
               dotnetify.hubServer.update_VM(this.$vmId, iValue);

               if (dotnetify.debug) {
                  console.log("[" + this.$vmId + "] sent> ");
                  console.log(iValue);

                  if (dotnetify.debugFn != null)
                     dotnetify.debugFn(this.$vmId, "sent", iValue);
               }
            }
            catch (e) {
               console.error(e);
            }
         }
      }

      // Dispatches a state value to the server view model.
      // iValue - State value to dispatch.
      dotnetifyVM.prototype.$dispatchListState = function (iValue) {
         for (var listName in iValue) {
            var key = this.$itemKey[listName];
            if (!key) {
               console.error("[" + this.$vmId + "] missing item key for '" + listName + "'; add " + listName + "_itemKey property to the view model.");
               return;
            }
            var item = iValue[listName];
            if (!item[key]) {
               console.error("[" + this.$vmId + "] couldn't dispatch data from '" + listName + "' due to missing property '" + key + "'");
               console.error(item);
               return;
            }
            for (var prop in item) {
               if (prop != key) {
                  var state = {};
                  state[listName + ".$" + item[key] + "." + prop] = item[prop];
                  this.$dispatch(state);
               }
            }

            this.$updateList(listName, item);
         }
      }

      // Loads an HTML view.
      dotnetifyVM.prototype.$loadHtmlView = function (iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, callbackFn) {
         var vm = this;

         try {
            // Unmount any React component before replacing with a new DOM. 
            _ReactDOM.unmountComponentAtNode(document.querySelector(iTargetSelector));
         }
         catch (e) {
            console.error(e);
         }

         // Load the HTML view.
         $(iTargetSelector).load(iViewUrl, null, function () {
            if (iJsModuleUrl != null) {
               $.getScript(iJsModuleUrl, function () {
                  if (typeof callbackFn === "function")
                     callbackFn.call(vm);
               });
            }
            else if (typeof callbackFn === "function")
               callbackFn.call(vm);
         });
      }

      // Loads a React view.
      dotnetifyVM.prototype.$loadReactView = function (iTargetSelector, iReactClassName, iJsModuleUrl, iVmArg, iReactProps, callbackFn) {
         var vm = this;
         var createViewFunc = function () {
            if (!window.hasOwnProperty(iReactClassName)) {
               console.error("[" + vm.$vmId + "] failed to load view '" + iReactClassName + "' because it's not a React element.");
               return;
            }

            try {
               _ReactDOM.unmountComponentAtNode(document.querySelector(iTargetSelector));
            }
            catch (e) {
               console.error(e);
            }

            try {
               var reactElement = _React.createElement(window[iReactClassName], iReactProps);
               _ReactDOM.render(reactElement, document.querySelector(iTargetSelector));
            }
            catch (e) {
               console.error(e);
            }
            if (typeof callbackFn === "function")
               callbackFn.call(vm, reactElement);
         }

         if (iJsModuleUrl == null)
            createViewFunc();
         else {
            // Load all javascripts first. Multiple files can be specified with comma delimiter.
            var getScripts = iJsModuleUrl.split(",").map(function (i) { return $.getScript(i); });
            getScripts.push($.Deferred(function (deferred) { $(deferred.resolve); }));

            $.when.apply($, getScripts).done(createViewFunc);
         }
      }

      // Preprocess view model update from the server before we set the state.
      dotnetifyVM.prototype.$preProcess = function (iVMUpdate) {
         var vm = this;

         for (var prop in iVMUpdate) {
            // Look for property that end with '_add'. Interpret the value as a list item to be added 
            // to an existing list whose property name precedes that suffix.
            var match = /(.*)_add/.exec(prop);
            if (match != null) {
               var listName = match[1];
               if (Array.isArray(this.State()[listName]))
                  vm.$addList(listName, iVMUpdate[prop]);
               else
                  console.error("unable to resolve " + prop);
               delete iVMUpdate[prop];
               continue;
            }

            // Look for property that end with '_update'. Interpret the value as a list item to be updated 
            // to an existing list whose property name precedes that suffix.
            var match = /(.*)_update/.exec(prop);
            if (match != null) {
               var listName = match[1];
               if (Array.isArray(this.State()[listName]))
                  vm.$updateList(listName, iVMUpdate[prop]);
               else
                  console.error("[" + this.$vmId + "] '" + listName + "' is not found or not an array.");
               delete iVMUpdate[prop];
               continue;
            }

            // Look for property that end with '_remove'. Interpret the value as a list item key to remove
            // from an existing list whose property name precedes that suffix.
            var match = /(.*)_remove/.exec(prop);
            if (match != null) {
               var listName = match[1];
               if (Array.isArray(this.State()[listName])) {
                  var key = this.$itemKey[listName];
                  if (key != null)
                     vm.$removeList(listName, function (i) { return i[key] == iVMUpdate[prop] });
                  else
                     console.error("[" + this.$vmId + "] missing item key for '" + listName + "'; add " + listName + "_itemKey property to the view model.");
               }
               else
                  console.error("[" + this.$vmId + "] '" + listName + "' is not found or not an array.");
               delete iVMUpdate[prop];
               continue;
            }

            // Look for property that end with '_itemKey'. Interpret the value as the property name that will
            // uniquely identify items in the list.
            var match = /(.*)_itemKey/.exec(prop);
            if (match != null) {
               var listName = match[1];
               var itemKey = {};
               itemKey[listName] = iVMUpdate[prop];
               vm.$setItemKey(itemKey);
               delete iVMUpdate[prop];
               continue;
            }
         }
      },

         // Requests state from the server view model.
         dotnetifyVM.prototype.$request = function () {
            if (dotnetify.isConnected()) {
               dotnetify.hubServer.request_VM(this.$vmId, { $vmArg: this.$vmArg, $headers: this.$headers });
               this.$requested = true;
            }
         }

      // Updates state from the server view model to the view.
      // iVMData - Serialized state from the server.
      dotnetifyVM.prototype.$update = function (iVMData) {
         if (dotnetify.debug) {
            console.log("[" + this.$vmId + "] received> ");
            console.log(JSON.parse(iVMData));

            if (dotnetify.debugFn != null)
               dotnetify.debugFn(this.$vmId, "received", JSON.parse(iVMData));
         }

         var vmData = JSON.parse(iVMData);
         this.$preProcess(vmData);

         var state = this.State();
         state = $.extend(state, vmData);
         this.State(state);

         if (!this.$loaded)
            this.$onLoad();
      }

      // Handles initial view model load event.
      dotnetifyVM.prototype.$onLoad = function () {

         // Call any plugin's $ready function if provided to give a chance to do
         // things when the view model is ready.
         for (var pluginId in dotnetify.react.plugins) {
            var plugin = dotnetify.react.plugins[pluginId];
            if (typeof plugin["$ready"] === "function")
               plugin.$ready.apply(this);
         }
         this.$loaded = true;
      }

      // *** CRUD Functions ***

      // Sets items key to identify individual items in a list.
      // Accepts object literal: { "<list name>": "<key prop name>", ... }
      dotnetifyVM.prototype.$setItemKey = function (iItemKey) {
         this.$itemKey = iItemKey;
      }

      //// Adds a new item to a state array.
      dotnetifyVM.prototype.$addList = function (iListName, iNewItem) {
         // Check if the list already has an item with the same key. If so, replace it.
         var key = this.$itemKey[iListName];
         if (key != null) {
            if (!iNewItem.hasOwnProperty(key)) {
               console.error("[" + this.$vmId + "] couldn't add item to '" + iListName + "' due to missing property '" + key + "'");
               return;
            }
            var match = this.State()[iListName].filter(function (i) { return i[key] == iNewItem[key] });
            if (match.length > 0) {
               console.error("[" + this.$vmId + "] couldn't add item to '" + listName + "' because the key already exists");
               return;
            }
         }

         var items = this.State()[iListName];
         items.push(iNewItem);

         var state = {};
         state[iListName] = items;
         this.State(state);
      }

      // Removes an item from a state array.
      dotnetifyVM.prototype.$removeList = function (iListName, iFilter) {
         var state = {};
         state[iListName] = this.State()[iListName].filter(function (i) { return !iFilter(i) });
         this.State(state);
      }

      //// Updates existing item to an observable array.
      dotnetifyVM.prototype.$updateList = function (iListName, iNewItem) {

         // Check if the list already has an item with the same key. If so, update it.
         var key = this.$itemKey[iListName];
         if (key != null) {
            if (!iNewItem.hasOwnProperty(key)) {
               console.error("[" + this.$vmId + "] couldn't update item to '" + iListName + "' due to missing property '" + key + "'");
               return;
            }
            var state = {};
            state[iListName] = this.State()[iListName].map(function (i) { return i[key] == iNewItem[key] ? $.extend(i, iNewItem) : i });
            this.State(state);
         }
         else
            console.error("[" + this.$vmId + "] missing item key for '" + listName + "'; add " + listName + "_itemKey property to the view model.");
      }

      // The <Scope> component uses React's 'context' to pass down the component hierarchy the name of the back-end view model
      // of the parent component, so that when the child component connects to its back-end view model, the child view model
      // instance is created within the scope of the parent view model.
      // The <Scope> component also provides the 'connect' function for a component to connect to the back-end view model and
      // injects properties and dispatch functions into the component.
      dotnetify.react.Scope = _React.createClass({
         displayName: "Scope",

         propTypes: { vm: _React.PropTypes.string },
         contextTypes: { scoped: _React.PropTypes.func },
         childContextTypes: {
            scoped: _React.PropTypes.func.isRequired,
            connect: _React.PropTypes.func.isRequired
         },
         scoped: function scoped(vmId) {
            var scope = this.context.scoped ? this.context.scoped(this.props.vm) : this.props.vm;
            return scope ? scope + "." + vmId : vmId;
         },
         getChildContext: function getChildContext() {
            var _this = this;

            return {
               scoped: function scoped(vmId) {
                  return _this.scoped(vmId);
               },
               connect: function connect(vmId, component, options) {
                  component.vmId = _this.scoped(vmId);
                  component.vm = dotnetify.react.connect(component.vmId, component, options);
                  component.dispatch = function (state) { return component.vm.$dispatch(state); };
                  component.dispatchState = function (state) {
                     component.vm.State(state);
                     component.vm.$dispatch(state);
                  };
                  return window.vmStates ? window.vmStates[component.vmId] : null;
               }
            };
         },
         render: function render() {
            return this.props.children;
         }
      });

      return dotnetify;
   }))