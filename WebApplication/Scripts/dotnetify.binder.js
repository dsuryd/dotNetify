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
                        bind = koBinder.setAttrBindings.apply(this, [elem, bind]);
                        return bind;
                    },

                    // Attr bindings.
                    setAttrBindings: function (elem, bind) {
                        var vm = this;
                        var id = elem.id;

                        // Find other view model properties that starts with the id followed by the underscore.
                        // These properties will be bound using the attr binding notation.
                        var props = {};
                        for (prop in vm) {
                            if (prop != id && prop.startsWith(id + "_") && ko.observable(vm[prop]) && typeof vm[prop].$bound === "undefined") 
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
                    }
                }
            })();

            return {
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
            }
        })(iVM)
    }

    // Register the plugin to dotNetify.
    dotnetify.plugins["binder"] = dotnetify.binder;
}))