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
   var _window = window || global;

   if (typeof exports === "object" && typeof module === "object") {
      var jquery = typeof window !== "undefined" ? window.jQuery || require('./jquery-shim') : require('./jquery-shim');
      module.exports = factory(jquery, _window, require('./dotnetify-hub'));
   }
   else if (typeof define === "function" && define["amd"]) {
      define(['./jquery-shim', './dotnetify-hub'], factory);
   }
   else {
      factory(jQuery, _window, dotnetifyHub);
   }
}
   (function ($, window, dotnetifyHub) {

      dotnetify = $.extend(dotnetify, {
         // SignalR hub options.
         hub: dotnetifyHub,
         hubOptions: { transport: ["webSockets", "longPolling"] },
         hubPath: null,
         hubServerUrl: null,

         // Debug mode.
         debug: false,
         debugFn: null,

         // Handler for connection state changed events.
         connectionStateHandler: null,

         // Whether connected to SignalR hub server.
         isConnected: function () { return dotnetifyHub.isConnected; },

         // Generic connect function for non-React app.
         connect: function (iVMId, iOptions) { return dotnetify.react.connect(iVMId, null, iOptions); },

         _triggerConnectionStateEvent: function (iState, iException) {
            if (dotnetify.debug)
               console.log("SignalR: " + (iException ? iException.message : iState));

            if (typeof dotnetify.connectionStateHandler === "function")
               dotnetify.connectionStateHandler(iState, iException);
            else if (iException)
               console.error(iException);
         },

         // Internal variables. Do not modify!
         _hub: null
      });

      dotnetify.react = $.extend(dotnetify.hasOwnProperty("react") ? dotnetify.react : {}, {
         version: "1.1.0",
         viewModels: {},
         plugins: {},

         // Initializes connection to SignalR server hub.
         init: function () {
            var self = dotnetify.react;

            if (dotnetify._hub === null) {
               dotnetifyHub.init(dotnetify.hubPath, dotnetify.hubServerUrl, dotnetify.hubLib);

               // Setup SignalR server method handler.
               dotnetifyHub.client.response_VM = function (iVMId, iVMData) {

                  // SignalR .NET Core is sending an array of arguments.
                  if (Array.isArray(iVMId)) {
                     iVMData = iVMId[1];
                     iVMId = iVMId[0];
                  }

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
                     dotnetifyHub.server.dispose_VM(iVMId);
               };

               // On disconnected, keep attempting to start the connection.
               dotnetifyHub.disconnected(function () {
                  dotnetify._hub = null;
                  dotnetifyHub.reconnect(self.startHub);
               });

               // Use SignalR event to raise the connection state event.
               dotnetifyHub.stateChanged(function (state) {
                  dotnetify._triggerConnectionStateEvent(state);
               });
            }

            self.startHub();
         },

         // Starts the connection to the SignalR server hub.
         startHub: function () {
            var self = dotnetify.react;
            var getInitialStates = function () {
               for (var vmId in self.viewModels) {
                  if (!self.viewModels[vmId].$requested)
                     self.viewModels[vmId].$request();
               }
            };

            if (dotnetify._hub === null) {
               for (var vmId in self.viewModels)
                  self.viewModels[vmId].$requested = false;

               dotnetify._hub = dotnetifyHub.start(dotnetify.hubOptions)
                  .done(function () { getInitialStates(); })
                  .fail(function (ex) { dotnetify._triggerConnectionStateEvent("error", ex); });
            }
            else if (dotnetify.isConnected())
               dotnetify._hub.done(getInitialStates);

            return dotnetify._hub;
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
            var options = {
               getState: getState,
               setState: setState,
               vmArg: iVMArg
            };
            var vm = self.viewModels[iVMId] = new dotnetifyVM(iVMId, iReact, options);

            // Need to be asynch to allow initial state to be processed.
            setTimeout(function () { vm.$update(JSON.stringify(window.ssr[iVMId])); }, 100);
            return vm;
         },

         // Get all view models.
         getViewModels: function () {
            var self = dotnetify.react;
            var vmArray = [];
            for (var vmId in self.viewModels)
               vmArray.push(self.viewModels[vmId]);
            return vmArray;
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

         if (iReact && iReact.props && iReact.props.hasOwnProperty("vmArg"))
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

         if (dotnetify.isConnected()) {
            try {
               dotnetifyHub.server.dispose_VM(this.$vmId);
            }
            catch (ex) {
               dotnetify._triggerConnectionStateEvent("error", ex);
            }
         }

         delete dotnetify.react.viewModels[this.$vmId];
      }

      // Dispatches a value to the server view model.
      // iValue - Vvalue to dispatch.
      dotnetifyVM.prototype.$dispatch = function (iValue) {

         if (dotnetify.isConnected()) {
            try {
               dotnetifyHub.server.update_VM(this.$vmId, iValue);

               if (dotnetify.debug) {
                  console.log("[" + this.$vmId + "] sent> ");
                  console.log(iValue);

                  if (dotnetify.debugFn != null)
                     dotnetify.debugFn(this.$vmId, "sent", iValue);
               }
            }
            catch (ex) {
               dotnetify._triggerConnectionStateEvent("error", ex);
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
      }

      // Requests state from the server view model.
      dotnetifyVM.prototype.$request = function () {
         if (dotnetify.isConnected()) {
            dotnetifyHub.server.request_VM(this.$vmId, { $vmArg: this.$vmArg, $headers: this.$headers });
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
         state = $.extend({}, state, vmData);
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
               console.error("[" + this.$vmId + "] couldn't add item to '" + iListName + "' because the key already exists");
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

      return dotnetify;
   }))