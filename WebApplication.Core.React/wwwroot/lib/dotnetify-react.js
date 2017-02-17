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
      define(['jquery', 'signalr', 'signalr-hub'], factory);
   }
   else if (typeof exports === "object" && typeof module === "object") {
      module.exports = factory(require('jquery'), require('signalr'), require('signalr-hub'));
   }
   else {
      factory(jQuery);
   }
}
(function ($) {

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
      version: "0.1.0",
      viewModels: {},
      plugins: {},

      // Initializes connection to SignalR server hub.
      init: function () {
         var self = dotnetify.react;
         var getInitialStates = function () {
            for (var vmId in self.viewModels) {
               if (!self.viewModels[vmId].$loaded)
                  self.viewModels[vmId].$request();
            }
         };

         if (dotnetify.hub === null) {
            // Setup SignalR server method handler.
            var hub = $.connection.dotNetifyHub;
            hub.client.response_VM = function (iVMId, iVMData) {

               // Report unauthorized access.
               if (iVMData == "403") {
                  console.error("Unauthorized access to " + iVMId);
                  return;
               }

               if (self.viewModels.hasOwnProperty(iVMId))
                  self.viewModels[iVMId].$update(iVMData);
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
      connect: function (iVMId, iGetState, iSetState, iVMArg) {
         var self = dotnetify.react;
         if (!self.viewModels.hasOwnProperty(iVMId))
            self.viewModels[iVMId] = new dotnetifyVM(iVMId, iGetState, iSetState, iVMArg);

         dotnetify.react.init();
         return self.viewModels[iVMId];
      }
   });

   // Client-side view model that acts as a proxy of the server view model.
   // iVMId - identifies the view model.
   // iGetState - React state accessor.
   // iSetState - React state mutator.
   // iVMArg - optional view model arguments.
   function dotnetifyVM(iVMId, iGetState, iSetState, iVMArg) {

      this.$vmId = iVMId;
      this.$vmArg = iVMArg;
      this.$loaded = false;
      this.$itemKey = {};
      this.State = function (state) { return typeof state === "undefined" ? iGetState() : iSetState(state) };

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
            if (prop != key)
               this.$dispatch({ [listName + ".$" +item[key]+ "." +prop]: item[prop] });
         }

         this.$updateList(listName, item);
      }
   }

   // Loads a view.
   dotnetifyVM.prototype.$loadView = function (iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, callbackFn) {

      var getScripts = iJsModuleUrl.split(",").map(function (i) { return $.getScript(i); });
      getScripts.push($.Deferred(function (deferred) { $(deferred.resolve); }));

      $.when.apply($, getScripts).done(function () {
         ReactDOM.render(React.createElement(window[iViewUrl], null), document.querySelector(iTargetSelector));
         if (typeof callbackFn === "function")
            callbackFn.call(this);
      });
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
            vm.$setItemKey({ [listName]: iVMUpdate[prop] });
            delete iVMUpdate[prop];
            continue;
         }
      }
   },

   // Requests state from the server view model.
   dotnetifyVM.prototype.$request = function () {
      if (dotnetify.isConnected())
         dotnetify.hubServer.request_VM(this.$vmId, this.$vmArg);
   }

   // Updates state from the server view model to the view.
   // iVMData - Serialized state from the server.
   dotnetifyVM.prototype.$update = function (iVMData) {
      if (dotnetify.debug) {
         console.log("[" + this.$vmId + "] received> ");
         console.log(JSON.parse(iVMData));
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
      this.State({ [iListName]: items });
   }

   // Removes an item from a state array.
   dotnetifyVM.prototype.$removeList = function (iListName, iFilter) {
      var items = this.State()[iListName].filter(function (i) { return !iFilter(i) });
      this.State({ [iListName]: items });
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
         var items = this.State()[iListName].map(function (i) { return i[key] == iNewItem[key] ? $.extend(i, iNewItem) : i });
         this.State({ [iListName]: items });
      }
      else
         console.error("[" + this.$vmId + "] missing item key for '" + listName + "'; add " + listName + "_itemKey property to the view model.");
   }

   return dotnetify.react;
}))
