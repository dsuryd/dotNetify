/* 
Copyright 2015 Dicky Suryadi

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
var dotnetify = {};
var dotNetify = {};

// Support using RequireJS that loads our app.js, or being placed in <script> tag.
(function (factory) {
   if (typeof define === "function" && define["amd"]) {
      define(['jquery', 'knockout', 'ko-mapping', 'jquery-ui', 'signalr-hub'], factory);
   }
   else {
      factory(jQuery, ko, ko.mapping);
   }
}
(function ($, ko, komapping) {
   ko.mapping = komapping;
   dotnetify =
   {
      version: "1.0.0-BETA",
      hub: null,
      debug: false,
      debugFn: null,
      init: function () {

         if (dotnetify.hub === null) {
            // Setup SignalR server method handler.
            var hub = $.connection.dotNetifyHub;
            hub.client.response_VM = function (iVMId, iVMData) {

               // Construct a selector from iVMId to find the associated widget.
               // First parse the instance Id out of the string, if present.
               var vmType = iVMId;
               var vmInstanceId = null;
               if (vmType.indexOf("$") >= 0) {
                  var path = iVMId.split("$");
                  vmType = path[0];
                  vmInstanceId = path[1];
               }

               var selector = "[data-vm='" + vmType + "']";

               // If present, add the master view models to the selector.
               var path = vmType.split(".");
               if (path.length > 1) {
                  selector = "";
                  for (i = 0; i < path.length - 1; i++)
                     selector += "[data-master-vm='" + path[i] + "'] ";
                  selector += "[data-vm='" + path[i] + "']";
               }

               // If present, add the instance Id to the selector.
               if (vmInstanceId != null)
                  selector += "[data-vm-id='" + vmInstanceId + "']";

               // Use the selector to locate the view model widget and pass the data.
               var element = $(selector);
               if (element.length > 0) {
                  element.data("ko-dotnetify").UpdateVM(iVMData);
               }
               else
                  // If we get to this point, that means the server holds a view model instance
                  // whose view no longer existed.  So, tell the server to dispose the view model.
                  hub.server.dispose_VM(iVMId);
            };

            // Start SignalR hub connection, and if successful, apply the widget to all scoped elements.
            var startHub = function () {
               var hub = $.connection.hub.start();
                hub.done(function () {
                   dotnetify._connectRetry = 0;
                   $.each($("[data-vm]"), function () { $(this).dotnetify() });
                })
                .fail(function (e) { console.log(e); });
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
         }
         else
            dotnetify.hub.done(function () { $.each($("[data-vm]"), function () { $(this).dotnetify() }) });
      },
      widget: function (iElement) {
         return $(iElement).data("ko-dotnetify");
      },
      plugins: {},
      _connectRetry: 0
   };

   dotNetify = dotnetify;

   $(function () {
      dotnetify.init();
   });

   $.widget("ko.dotnetify", {

      // Widget constructor.
      _create: function () {
         var self = this;

         self.VMType = self.element.attr("data-vm");
         self.VMId = self.VMType;
         self.Hub = $.connection.dotNetifyHub;

         // If an instance Id is specified, add it to VMId.
         var instanceId = self.element.attr("data-vm-id");
         if (instanceId != null)
            self.VMId += "$" + instanceId;

         // If inside master view scope, combine the names into VMId.
         $.each(self.element.parents("[data-master-vm]"), function () {
            self.VMId = $(this).attr("data-master-vm") + "." + self.VMId;
         });

         // Request the server VM. 
         if (self.VMId != null) {
            var vmArg = self.element.attr("data-vm-arg");
            vmArg = vmArg != null ? $.parseJSON(vmArg.replace(/'/g, "\"")) : null;
            self.Hub.server.request_VM(self.VMId, vmArg);
         }
         else
            console.log("ERROR: dotnetify - failed to find 'data-vm' attribute in the element where .dotnetify() was applied.")
      },

      // Widget destructor.
      _destroy: function () {
         try {
            var self = this;

            // Call any plugin's $destroy function if provided.
            $.each(dotnetify.plugins, function (pluginId, plugin) {
               if (typeof plugin["$destroy"] === "function")
                  plugin.$init.apply(self.VM);
            });

            // Call view model's $destroy function if provided.
            if (self.VM != null && self.VM.hasOwnProperty("$destroy"))
               self.VM.$destroy();
         }
         catch (e) {
            console.log(e.stack);
         }

         this.Hub.server.dispose_VM(self.VMId);
      },

      // Convert the server VM into knockout VM.
      UpdateVM: function (iVMData) {
         var self = this;
         try {
            // If no view model yet, create one from the server data.
            if (self.VM == null) {
               self.VM = ko.mapping.fromJS(JSON.parse(iVMData));

               // Set essential info to the view model.
               self.VM.$vmId = self.VMId;
               self.VM.$element = self.element;

               // Add built-in functions to the view model.
               this._AddBuiltInFunctions();

               // Call any plugin's $init function if provided to give a chance to do
               // things before initial binding is applied.
               $.each(dotnetify.plugins, function (pluginId, plugin) {
                  if (typeof plugin["$init"] === "function")
                     plugin.$init.apply(self.VM);
               });

               // Call view model's init function if provided.
               if (typeof self.VM["$init"] === "function")
                  self.VM.$init();

               // Apply knockout view model to the HTML element.
               ko.applyBindings(self.VM, self.element[0]);

               // Enable server update so that every changed value goes to server.
               self.VM.$serverUpdate = true;

               // Call any plugin's $ready function if provided to give a chance to do
               // things when the view model is ready.
               $.each(dotnetify.plugins, function (pluginId, plugin) {
                  if (typeof plugin["$ready"] === "function")
                     plugin.$ready.apply(self.VM);
               });

               // Call view model's $ready function if provided.
               if (typeof self.VM["$ready"] === "function")
                  self.VM.$ready();

               // Send 'ready' event after a new view model was received.
               this.element.trigger("ready", { VMId: self.VMId, VM: self.VM });
            }
            else {
               // Disable server update because we're going to update the value in the knockout VM
               // and that will trigger change event back to server if we don't stop it now.
               self.VM.$serverUpdate = false;

               var vmUpdate = JSON.parse(iVMData);
               self._PreProcess(vmUpdate);
               ko.mapping.fromJS(vmUpdate, self.VM);

               // Don't forget to re-enable sending changed values to server.
               self.VM.$serverUpdate = true;
            }

            // Subscribe to change events to allow sending updates back to server.
            self._SubscribeObservables(this.VM);
         }
         catch (e) {
            console.log(e.stack);
         }

         if (dotnetify.debug) {
            console.log("[" + self.VMId + "] received> ");
            console.log(JSON.parse(iVMData));

            if (dotnetify.debugFn != null)
               dotnetify.debugFn(self.VMId, "received", JSON.parse(iVMData));
         }
      },

      // Adds built-in functions to the view model.
      _AddBuiltInFunctions: function () {
         var self = this;

         // Executes the given function in a scope where server update is temporarily disabled.
         self.VM.$preventBinding = function (fn) {
            self.VM.$serverUpdate = false;
            fn.apply(self.VM);
            self.VM.$serverUpdate = true;
         }

         // Adds a new item to an observable array.
         self.VM.$addList = function (iList, iNewItem) {
            var newItem = ko.mapping.fromJS(iNewItem);

            // Check if the list already has an item with the same key. If so, replace it.
            var key = iList()['$vmKey'];
            if (key != null) {
               var match = ko.utils.arrayFirst(iList(), function (i) { return i[key]() == newItem[key]() });
               if (match != null) {
                  iList.replace(match, newItem);
                  return;
               }
            }
            iList.push(newItem);
         }

         // Updates existing item to an observable array.
         self.VM.$updateList = function (iList, iNewItem) {
            var newItem = ko.mapping.fromJS(iNewItem);

            // Check if the list already has an item with the same key. If so, update it.
            var key = iList()['$vmKey'];
            if (key != null) {
               var match = ko.utils.arrayFirst(iList(), function (i) { return i[key]() == newItem[key]() });
               if (match != null) {
                  for (prop in newItem)
                     match[prop](newItem[prop]());
                  return;
               }
            }
            iList.push(newItem);
         }

         // Removes an item from an observable array.
         // Unlike the push operation, the ko remove operation will cause the list to trigger 
         // change event, therefore disable server update while we do this.
         self.VM.$removeList = function (iList, iCriteria) {
            self.VM.$preventBinding(function () { iList.remove(iCriteria) });
         }

         // Listens to a view model property change event.
         self.VM.$on = function (iProperty, iCallback) {
            iProperty.subscribe(function (iNewValue) { iCallback(iNewValue); });
         }

         // Listens to a view model property change event once.
         self.VM.$once = function (iProperty, iCallback) {
            var subscription = iProperty.subscribe(function (iNewValue) {
               subscription.dispose();
               iCallback(iNewValue);
            });
         }

         // Loads a view into a target element.
         // Method parameters: TargetSelector, ViewUrl, [iJsModuleUrl], [iVmArg], iCallbackFn
         self.VM.$loadView = function (iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, iCallbackFn) {
            var vm = this;

            if (typeof iJsModuleUrl === "object") {
               iCallbackFn = iVMArg;
               iVMArg = iJsModuleUrl;
               iJsModuleUrl = null;
            }
            else if (typeof iJsModuleUrl === "function") {
               iCallbackFn = iJsModuleUrl;
               iJsModuleUrl = null;
            }
            else if (typeof iVmArg === "function") {
               iCallbackFn = iVmArg;
               iVMArg = null;
            }

            // If no view URL is given, empty the target DOM element.
            if (iViewUrl == null || iViewUrl == "") {
               $(iTargetSelector).empty();
               return;
            }

            // Loads the view template to the target DOM element.
            $(iTargetSelector).load(iViewUrl, null, function () {

               // Adds view model arguments when provided.
               if (iVmArg != null && !$.isEmptyObject(iVmArg))
                  $(this).attr("data-vm-arg", JSON.stringify(iVmArg));

               // Call the callback function.  
               if (typeof iCallbackFn === "function")
                  iCallbackFn.apply(this);

               // Load the Javascript module if specified.
               if (iJsModuleUrl != null) {
                  $.getScript(iJsModuleUrl, function () { dotnetify.init() });
               }
               else
                  dotnetify.init();
            });
         }

         // Injects a context with observables mapped from an object. Context can be an object or an observable array.
         self.VM.$inject = function (iContext, iObject) {
            if (ko.isObservable(iContext) && 'push' in iContext)
               $.each(iContext(), function (idx, item) { self._Inject(item, iObject) });
            else
               self._Inject(iContext, iObject);
         }

         // Map the module in global namespace whose name matches the view model type.
         self._Inject(self.VM, window[self.VMType]);

         // Add plugin functions.
         $.each(dotnetify.plugins, function (pluginId, plugin) {
            if (plugin.hasOwnProperty("$inject"))
               plugin.$inject(self.VM);
         });
      },

      // Inject the context with observables mapped from an object.
      // Functions that start with underscore are mapped to pure computed observables.
      _Inject: function (iContext, iObject) {
         for (prop in iObject) {
            // Skip if the context already has a property with the same name.
            if (iContext.hasOwnProperty(prop))
               continue;

            if (typeof iObject[prop] === "function") {
               if (prop.indexOf("_") == 0) {
                  iContext[prop] = ko.pureComputed(iObject[prop], iContext);
               }
               else
                  iContext[prop] = iObject[prop];
            }
            else {
               iContext[prop] = ko.observable(iObject[prop]);

               // Prevent it from being subscribed so it won't get sent to server. 
               iContext[prop].$subscribe = true;
            }
         }
      },

      // Preprocess view model update from the server before we map it to knockout view model.
      _PreProcess: function (iVMUpdate) {
         for (var prop in iVMUpdate) {
            // Look for property that end with '_add'. Interpret the value as a list item to be added 
            // to an existing list whose property name precedes that suffix.
            var match = /(.*)_add/.exec(prop);
            if (match != null) {
               var list = this.VM[match[1]];
               if (list != null)
                  this.VM.$addList(list, iVMUpdate[prop]);
               else
                  throw new Error("unable to resolve " + prop);
               delete iVMUpdate[prop];
               continue;
            }

            // Look for property that end with '_update'. Interpret the value as a list item to be updated 
            // to an existing list whose property name precedes that suffix.
            var match = /(.*)_update/.exec(prop);
            if (match != null) {
               var list = this.VM[match[1]];
               if (list != null)
                  this.VM.$updateList(list, iVMUpdate[prop]);
               else
                  throw new Error("unable to resolve " + prop);
               delete iVMUpdate[prop];
               continue;
            }

            // Look for property that end with '_remove'. Interpret the value as a list item key to remove
            // from an existing list whose property name precedes that suffix.
            var match = /(.*)_remove/.exec(prop);
            if (match != null) {
               var list = this.VM[match[1]];
               if (list != null) {
                  var key = list()['$vmKey'];
                  if (key != null)
                     this.VM.$removeList(this.VM[match[1]], function (i) { return i[key]() == iVMUpdate[prop] });
                  else
                     throw new Error("unable to resolve " + prop + " due to missing vmItemKey attribute");
               }
               else
                  throw new Error("unable to resolve " + prop);
               delete iVMUpdate[prop];
               continue;
            }
         }
      },

      // Subscribe to value change events raised by knockout VM's observables.
      _SubscribeObservables: function (iParam, iVMPath) {
         var self = this;

         if (iParam == null)
            return;
         else if (ko.isObservable(iParam)) {
            if ('$subscribe' in iParam == false) {
               iParam.subscribe(function (iNewValue) {
                  // Handle value change event from observables.
                  if (self.VM.$serverUpdate == true)
                     self._OnValueChanged(iVMPath, iNewValue);
               });
               iParam['$subscribe'] = true;
            }
            this._SubscribeObservables(iParam(), iVMPath);
         }
         else if (typeof iParam == 'object') {

            // The property with $vmKey means it's an enumerable and the $vmKey indicates the key to identify
            // the item in that enumerable.  When we send value update to the server, we'll use the property
            // path in this format: <enumerable property name>.$<key value>.<property name>.
            // For example: ListContent.$3.FirstName.
            var key = '$vmKey' in iParam ? iParam['$vmKey'] : null;

            for (property in iParam) {
               if (property.charAt(0) == '$' || property.charAt(0) == '_')
                  continue;
               path = key != null ? "$" + iParam[property][key]() : property;
               this._SubscribeObservables(iParam[property], iVMPath == null ? path : iVMPath + "." + path);
            }
         }
         else if (iParam instanceof Array) {
            for (index in iParam) {
               path = "$" + index;
               this._SubscribeObservables(iParam[index], iVMPath == null ? path : iVMPath + "." + path);
            }
         }
      },

      // On value changed from a knockout VM's observable, update the server VM.
      _OnValueChanged: function (iVMPath, iNewValue) {
         var update = {};
         update[iVMPath] = iNewValue instanceof Object ? $.extend({}, iNewValue) : iNewValue;
         this.Hub.server.update_VM(this.VMId, update);

         if (dotnetify.debug) {
            console.log("[" + this.VMId + "] sent> ");
            console.log(update);

            if (dotnetify.debugFn != null)
               dotnetify.debugFn(this.VMId, "sent", update);
         }
      }
   });

   // Custom knockout binding to indicate the item key of an items collection property.
   ko.bindingHandlers.vmItemKey = {
      preprocess: function (value) {
         // Make sure the item key is enclosed with quotes.
         return value.charAt(0) != "'" ? "'" + value + "'" : value;
      },
      update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {

         // Store the item key in a special property '$vmKey' in the element's view model.
         var value = valueAccessor();
         var items = allBindings.get("foreach");
         if (ko.isObservable(items) && items() != null && value != null)
            items()['$vmKey'] = value;
      }
   };

   // Custom knockout binding to bind the specified function the click event of the element.
   ko.bindingHandlers.vmCommand = {
      init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {

         var vm = bindingContext.$root;
         var fnName = null;
         var fnArg = null;

         // Parse the value. It supports either a function name or an object literal { funcName: argument }
         // where argument can be either data or obsevables.
         var matchFnNameArg = /return\s{(.*):(.*)}\s/.exec(valueAccessor.toString());
         if (matchFnNameArg != null) {
            fnName = matchFnNameArg[1].trim();
            fnArg = matchFnNameArg[2].trim();
         }
         else {
            var matchFnName = /return\s(.*)\s/.exec(valueAccessor.toString());
            if (matchFnName != null)
               fnName = matchFnName[1].trim();
         }

         if (fnName == null)
            throw new Error("invalid vmCommand value at " + element.outerHTML);

         // Support whether function is defined globally or inside a namespace that matches view model Id.
         var getFn = function () { return vm[fnName] != null ? vm[fnName] : valueAccessor() }

         // Trim the argument from enclosing quotes.  If it's an observable name, replace it with the value.
         if (fnArg != null) {
            if (fnArg.charAt(0) == "'")
               fnArg = fnArg.replace(/(^'|'$)/g, '');
            else if (ko.isObservable(viewModel[fnArg]))
               fnArg = ko.unwrap(viewModel[fnArg]);
            else if (fnArg == "$data")
               fnArg = viewModel;
         }
         else
            fnArg = true;

         var newValueAccessor = function () {
            return function () {
               var fn = getFn();

               // If function is an observable, which means it's a server view model property, then set its value to 
               // trigger the invocation of its setter property on the server side.  If it's not an observable, then it must
               // be a client-side function, in which case just invoke it and pass all possible objects it may need.
               if (ko.isObservable(fn)) {

                  // Reset the value locally first to ensure that setting the value will raise change events.
                  vm.$preventBinding(function () { fn(fnArg == true ? false : null) });

                  fn(fnArg);
               }
               else
                  fn.apply(vm, [viewModel, element, bindingContext.$parent]);
            }
         }
         ko.bindingHandlers.click.init(element, newValueAccessor, allBindings, viewModel, bindingContext);
      }
   };

   // Custom knockout binding to call a function on initial property change event.
   ko.bindingHandlers.vmOnce = {
      init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
         ko.bindingHandlers.vmOn.init(element, valueAccessor, allBindings, viewModel, bindingContext, true);
      }
   };

   // Custom knockout binding to call a function on a property change event.
   ko.bindingHandlers.vmOn = {
      init: function (element, valueAccessor, allBindings, viewModel, bindingContext, once) {

         var vm = bindingContext.$root;
         var property = null;
         var fnName = null;
         var value = valueAccessor.toString();

         // Parse the value, which should be in object literal { property: fnName }.
         var match = /return\s{(.*):(.*)}\s/.exec(value);
         if (match != null) {
            property = match[1].trim();
            fnName = match[2].trim();
         }

         if (fnName == null)
            throw new Error("invalid vmOn function at " + element.outerHTML);

         // Support whether function is defined globally or inside a namespace that matches view model Id.
         var getFn = function () { return vm[fnName] != null ? vm[fnName] : valueAccessor()[property] }

         // Make sure the property is an observable.
         if (property != null && !ko.isObservable(viewModel[property]))
            throw new Error("invalid vmOn data: " + valueAccessor());

         // Call the function with the initial data.
         getFn().apply(vm, [viewModel, element, bindingContext.$parent]);

         // Call the function on every data update.
         if (once == null)
            viewModel[property].subscribe(function (iNewValue) {
               getFn().apply(vm, [viewModel, element, bindingContext.$parent]);
            });
      }
   };

   return dotnetify;
}))
