/* 
Copyright 2016 Dicky Suryadi

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

// Support using RequireJS that loads our app.js, or being placed in <script> tag.
(function (factory) {
   if (typeof define === "function" && define["amd"]) {
      define(['jquery', 'knockout', 'dotnetify'], factory);
   }
   else {
      factory(jQuery, ko, dotnetify);
   }
}
(function ($, ko, dotnetify) {

   // Add plugin functions.
   dotnetify.binder =
      {
         version: "1.0.5",

         // Placeholder for binder extensions.
         extensions: {},

         // Called by dotNetify before binding is applied.
         $init: function () {
            this.$binder.init();
         },

         // Called by dotNetify after binding is applied.
         $ready: function() {
            this.$binder.ready();
   }
      }

   // Inject a view model with functions.
   dotnetify.binder.$inject = function (iVM) {
      // Put functions inside $components namespace.
      iVM["$binder"] = (function (iScope) {

         // Knockout binder functions.
         var koBinder = (function () {
            return {
               setBindings: function (elem, bind) {
                  var vm = this;
                  var koNotations = ["enable", "visible", "css"];

                  bind = koBinder.setIdBinding.apply(vm, [elem, bind]);
                  for (i in koNotations)
                     bind = vm.$binder.bindToObservable(koNotations[i], elem, bind);

                  // If a 'select' element, set the options bindings.
                  if (elem.tagName.toLowerCase() == "select") {
                     bind = vm.$binder.bindToObservable("options", elem, bind);

                     var koOptionsNotations = ["optionsCaption", "optionsText", "optionsValue"];
                     for (i in koOptionsNotations)
                        bind = koBinder.bindToObservableValue.apply(vm, [koOptionsNotations[i], elem, bind]);
                  }
                  else {
                     // If not a 'select' element, mark all options properties as bound so that
                     // it won't be included in the attr binding.
                     var notations = ["options", "optionsCaption", "optionsText", "optionsValue"];
                     for (i in notations) {
                        var prop = elem.id + notations[i];
                        if (vm.hasOwnProperty(prop))
                           vm[prop].$bound = true;
                     }
                  }

                  bind = koBinder.setAttrBindings.apply(vm, [elem, bind]);
                  return bind;
               },

               // Id binding.
               setIdBinding: function (elem, bind) {
                  var vm = this;
                  var id = elem.id;
                  var tagName = elem.tagName.toLowerCase();

                  if (!vm.$binder.bindable(id))
                     return bind;

                  // If the id is period-delimited, use the last item as id.
                  var ids = id.split(".");
                  if (ids.length > 1)
                     id = ids[ids.length - 1];

                  bind += bind.length > 0 ? ", " : "";
                  if (tagName == "input") {
                     var type = elem.type.toLowerCase();
                     if (type == "search")
                        bind += "textInput: " + id;
                     else if (type == "checkbox" || type == "radio")
                        bind += "checked: " + id;
                     else if (type == "button")
                        bind += "vmCommand: " + id;
                     else
                        bind += "value: " + id;
                  }
                  else if (tagName == "button")
                     bind += "vmCommand: " + id;
                  else if ( tagName == "a" )
                     bind += "vmRoute: " + id;
                  else if (tagName == "select" || tagName == "textarea")
                     bind += "value: " + id;
                  else if (vm.hasOwnProperty(id) && 'push' in vm[id])    // If property is an observable array.
                     bind += "foreach: " + id;
                  else if (tagName.indexOf("-") < 0)   // If element isn't a web component.
                     bind += "html: " + id;

                  bind = bind.replace(/,\s*$/, "");
                  return bind;
               },

               // Attr bindings.
               setAttrBindings: function (elem, bind) {
                  var vm = this;
                  var id = elem.id;

                  var startsWith = function (iStr, iValue) { return iStr.toLowerCase().slice(0, iValue.length) == iValue.toLowerCase() }

                  // Find other view model properties that starts with the id followed by the underscore.
                  // These properties will be bound using the attr binding notation.
                  var props = {};
                  for (prop in vm) {
                     if (prop != id && startsWith(prop, id + "_") && vm.$binder.bindable(prop) && typeof vm[prop].$bound === "undefined")
                        props[prop] = vm[prop];
                  }

                  // If no property to bind, leave now.
                  if (Object.keys(props).length === 0)
                     return bind;

                  // Get existing attr binding.
                  var match = /\battr:(.*)}/.exec(bind);
                  var existingAttrBind = match != null ? match[1] : "";

                  // Build the new attr binding.
                  // Underscore in names is replaced with dash.
                  var attrBind = existingAttrBind;
                  for (prop in props) {
                     // Replace the underscore in attribute name with dash since property name 
                     // defined in the C# view model cannot have dashes.
                     var nth = 0;
                     var attr = prop.replace(/_/g, function (match, i) { return ++nth === 2 ? "-" : match; });

                     var match = /.*_(.*)/.exec(attr);
                     if (match != null) {
                        if (attr != prop)
                           match[1] = "'" + match[1] + "'";
                        attrBind += attrBind.length > 0 ? ", " : "{ ";
                        attrBind += match[1] + ": " + prop;
                        vm[prop].$bound = true;
                     }
                  }

                  // Add/replace the attr binding in the binding value.
                  attrBind = "attr: " + attrBind + "}";
                  if (existingAttrBind != "")
                     bind = bind.replace(/\battr:(.*)}/, attrBind);
                  else {
                     bind += bind.length > 0 ? ", " : "";
                     bind += attrBind;
                  }

                  return bind;
               },

               // Binding to the value of an observable view model property.
               bindToObservableValue: function (notation, elem, bind) {
                  var vm = this;
                  var id = elem.id;
                  var prop = id + "_" + notation;

                  if (vm.hasOwnProperty(prop) && vm.$binder.bindable(prop)) {
                     bind += bind.length > 0 ? ", " : "";
                     bind += notation + ": '" + vm[prop]() + "'";
                     vm[prop].$bound = true;
                  }
                  return bind;
               }
            }
         })();

         return {
            // Initialize bindings.
            init: function () {
               var vm = this;

               // Find all elements that have id attribute.
               $.each(vm.$element.find("[id]"), function (idx, elem) {
                  var id = elem.id;
                  var tagName = elem.tagName.toLowerCase();

                  // Get existing binding value.
                  var bind = $(elem).attr("data-bind");
                  if (typeof bind === "undefined")
                     bind = "";

                  // Let the extensions set their bindings.
                  for (ext in dotnetify.binder.extensions)
                     bind = dotnetify.binder.extensions[ext].setBindings.apply(vm, [elem, bind]);

                  // Set knockout bindings.
                  bind = koBinder.setBindings.apply(vm, [elem, bind]);

                  // Set the binding.
                  $(elem).attr("data-bind", bind);
               });

            }.bind(iScope),

            // Allow post-processing after bindings are applied.
            ready: function () {
               var vm = this;

               // Find all elements that have id attribute.
               $.each(vm.$element.find("[id]"), function (idx, elem) {
                  for (ext in dotnetify.binder.extensions)
                     dotnetify.binder.extensions[ext].postBindings.apply(vm, [elem]);
               });

            }.bind(iScope),

            // Whether a view model property is bindable.
            bindable: function (prop, parent) {
               var vm = this;
               if (typeof parent === "undefined")
                  parent = vm;

               // If it's a nested property, resolve it recursively.
               var props = prop.split(".");
               if (props.length > 1 && vm.hasOwnProperty(props[0]))
                  return vm.$binder.bindable(prop.substring(props[0].length + 1), vm[props[0]]);

               if (prop == "$data")
                  return true;
               else if (parent != vm && ko.observable(parent)) {
                  var parent = parent();
                  if (parent instanceof Array)
                     return parent.length > 0 ? parent[0].hasOwnProperty(prop) && ko.observable(prop) : true;
               }
               return parent.hasOwnProperty(prop) && ko.observable(prop);

            }.bind(iScope),

            // Binding to an observable view model property.
            bindToObservable: function (notation, elem, bind) {
               var vm = this;
               var id = elem.id;
               var prop = id + "_" + notation;

               if (vm.hasOwnProperty(prop) && vm.$binder.bindable(prop)) {
                  bind += bind.length > 0 ? ", " : "";
                  bind += notation + ": " + prop;
                  vm[prop].$bound = true;
               }
               return bind;
            }.bind(iScope)
         }
      })(iVM)
   }

   // Register the plugin to dotNetify.
   dotnetify.plugins["binder"] = dotnetify.binder;
}))