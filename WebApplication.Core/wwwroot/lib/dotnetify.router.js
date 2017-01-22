/* 
Copyright 2015-2017 Dicky Suryadi

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

// Support using AMD or CommonJS that loads our app.js, or being placed in <script> tag.
(function (factory) {
   if (typeof define === "function" && define["amd"]) {
      define(['jquery', 'knockout', 'dotnetify', 'path'], factory);
   }
   else if (typeof exports === "object" && typeof module === "object") {
      window.Path = require("path");
      module.exports = factory(require('jquery'), require('knockout'), require('dotnetify'));
   }
   else {
      factory(jQuery, ko, dotnetify);
   }
}
(function ($, ko, dotnetify) {

   // Add plugin functions.
   dotnetify.router =
      {
         version: "1.1.0",

         // URL path that will be parsed when performing routing.
         urlPath: document.location.pathname,

         // Initialize routing using PathJS.
         init: function () {
            if (typeof Path !== "undefined") {
               Path.history.listen(true);
               Path.routes.rescue = function () {
                  window.location.replace(document.location.pathname);
               };
            }
            else
               throw new Error("Pathjs library is required for routing.");
         },

         // Map a route to an action.
         mapTo: function (iPath, iFn) {
            if (typeof Path !== "undefined")
               Path.map(iPath).to(function () { iFn(this.params) });
         },

         // Match a URL path to a route and run the action.
         match: function (iUrlPath) {
            if (typeof Path !== "undefined") {
               var matched = Path.match(iUrlPath, true);
               if (matched != null) {
                  matched.run();
                  return true;
               }
            }
            return false;
         },

         // Optional callback to override a URL before performing routing.
         overrideUrl: function (iUrl) { return iUrl },

         // Push state to HTML history.
         pushState: function (iState, iTitle, iPath) {
            dotnetify.router.urlPath = "";
            if (typeof Path !== "undefined")
               Path.history.pushState(iState, iTitle, iPath);
         },

         // Redirect to the a URL.
         redirect: function (iUrl) {
            // Check first whether existing views can handle routing this URL.
            // Otherwise, do a hard browser redirect.
            dotnetify.router.urlPath = iUrl;
            var vmElements = $("[data-vm]");
            for (j = 0; j < vmElements.length; j++) {
               var widget = dotnetify.widget(vmElements[j]);
               if (widget != null && widget.VM.$router.routeUrl())
                  return;
            }
            window.location.replace(iUrl);
         },

         // Called by dotNetify when a view model is ready.
         $ready: function () {
            this.$router.initRouting();
         }
      }

   // Inject a view model with functions.
   dotnetify.router.$inject = function (iVM) {

      // Put functions inside $router namespace.
      iVM["$router"] = (function (iScope) {

         var utils = (function () {
            return {
               // Trim slashes from start and end of string.
               trim: function (iStr) {
                  if (typeof iStr !== "string")
                     return "";

                  while (iStr.indexOf("/", iStr.length - 1) >= 0)
                     iStr = iStr.substr(0, iStr.length - 1);
                  while (iStr.indexOf("/") == 0)
                     iStr = iStr.substr(1, iStr.length - 1);
                  return iStr;
               },
               // Match two strings case-insensitive.
               equal: function (iStr1, iStr2) { return iStr1 != null && iStr2 != null && iStr1.toLowerCase() == iStr2.toLowerCase() },
               // Whether the string starts or ends with a value.
               startsWith: function (iStr, iValue) { return iStr.toLowerCase().slice(0, iValue.length) == iValue.toLowerCase() },
               endsWith: function (iStr, iValue) { return iValue == '' || iStr.toLowerCase().slice(-iValue.length) == iValue.toLowerCase(); }
            }
         })();

         return {
            // Build the absolute root path from the "data-vm-root" attribute.
            initRoot: function () {
               var vm = this;
               if (!vm.hasOwnProperty("RoutingState") || typeof vm.RoutingState.Root !== "function")
                  return;

               if (vm.$router._absRoot != vm.RoutingState.Root()) {
                  var absRoot = utils.trim(vm.$element.attr("data-vm-root"));
                  if (absRoot != "")
                     absRoot = "/" + absRoot;
                  var root = utils.trim(vm.RoutingState.Root());
                  vm.$router._absRoot = root != "" ? absRoot + "/" + root : absRoot;
                  vm.RoutingState.Root(vm.$router._absRoot);
               }
            }.bind(iScope),

            // Initialize routing templates if the view model implements IRoutable.
            initRouting: function () {
               var vm = this;
               if (!vm.hasOwnProperty("RoutingState") || typeof vm.RoutingState.Templates !== "function")
                  return;

               var templates = vm.RoutingState.Templates();
               if (templates == null)
                  return;

               // Initialize the router.
               if (!dotnetify.router.$init) {
                  dotnetify.router.init();
                  dotnetify.router.$init = true;
               }

               // Build the absolute root path.
               vm.$router.initRoot();

               $.each(templates, function (idx, template) {
                  var mapUrl = vm.$router.toUrl(template.UrlPattern());

                  if (dotnetify.debug)
                     console.log("router> map " + mapUrl + " to template id=" + template.Id());

                  dotnetify.router.mapTo(mapUrl, function (iParams) {

                     dotnetify.router.urlPath = "";

                     // Construct the path from the template pattern and the params passed by PathJS.
                     var path = template.UrlPattern();
                     for (param in iParams)
                        path = path.replace(":" + param, iParams[param]);
                     path = path.replace(/\(\/:([^)]+)\)/g, "").replace(/\(|\)/g, "");

                     vm.$router.routeTo(path, template);
                  });
               });

               // Route initial URL.
               var activeUrl = vm.$router.toUrl(vm.RoutingState.Active());
               if (dotnetify.router.urlPath == "")
                  dotnetify.router.urlPath = activeUrl;
               vm.$router.routeUrl();

            }.bind(iScope),

            // Whether a route is active.
            isActive: function (iRoute) {
               if (iRoute != null && iRoute.hasOwnProperty("Path") && typeof iRoute.Path === "function")
                  return utils.equal(iRoute.Path(), this.RoutingState.Active());
               return false;
            }.bind(iScope),

            // Loads a view into a target element.
            // Method parameters: TargetSelector, ViewUrl, iJsModuleUrl, iVmArg, iCallbackFn
            loadView: function (iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, iCallbackFn) {
               var vm = this;

               // If no view URL is given, empty the target DOM element.
               if (iViewUrl == null || iViewUrl == "") {
                  $(iTargetSelector).empty();
                  return;
               }

               var callbackFn = function () {

                  // If the view model supports routing, add the root path to the view, to be used
                  // to build the absolute route path, and view model argument if provided.
                  if (vm.hasOwnProperty("RoutingState") && typeof vm.RoutingState.Root === "function")
                     $.each($(this).find("[data-vm]"), function (idx, element) {
                        var root = $(element).attr("data-vm-root");
                        root = root != null ? "/" + utils.trim(vm.RoutingState.Root()) + "/" + utils.trim(root) : vm.RoutingState.Root();
                        $(element).attr("data-vm-root", root);

                        if (iVmArg != null && !$.isEmptyObject(iVmArg)) {
                           // If there's already a data-vm-arg, combine the values. 
                           // Take care not to override server-side routing arguments.
                           var vmArg = $(element).attr("data-vm-arg");
                           vmArg = vmArg != null ? $.extend(iVmArg, $.parseJSON(vmArg.replace(/'/g, "\""))) : iVmArg;

                           $(element).attr("data-vm-arg", JSON.stringify(vmArg));
                        }
                     });

                  // Call the callback function.  
                  if (typeof iCallbackFn === "function")
                     iCallbackFn.apply(this);
               };

               // Provide the opportunity to override the URL.
               iViewUrl = dotnetify.router.overrideUrl(iViewUrl);

               vm.$loadView(iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, callbackFn);

            }.bind(iScope),

            // Routes to a path.
            manualRouteTo: function (iPath, iTarget, iViewUrl, iJSModuleUrl) {
               var template = {};
               template.Target = function () { return iTarget }
               template.ViewUrl = function () { return iViewUrl }
               template.JSModuleUrl = function () { return iJSModuleUrl }
               this.$router.routeTo(iPath, template, true);
            }.bind(iScope),

            // Routes to a path.
            routeTo: function (iPath, iTemplate, iDisableEvent) {
               var vm = this;

               if (dotnetify.debug)
                  console.log("router> route '" + iPath + "' to template id=" + iTemplate.Id());

               // We can determine whether the view has already been loaded by matching the 'RoutingState.Origin' argument
               // on the existing view model inside that target selector with the path.
               var vmArg = $("#" + iTemplate.Target() + " [data-vm-arg]").attr("data-vm-arg");
               if (vmArg != null) {
                  vmArg = $.parseJSON(vmArg.replace(/'/g, "\""));
                  if (typeof vmArg["RoutingState.Origin"] === "string" && utils.equal(vmArg["RoutingState.Origin"], iPath))
                     return;
               }

               // Support enter interception.
               if (iDisableEvent != true && vm.hasOwnProperty("onRouteEnter"))
                  if (vm.onRouteEnter(iPath, iTemplate) == false)
                     return;

               // If target DOM element isn't found, redirect URL to the path.
               if ($("#" + iTemplate.Target()).length == 0)
                  return dotnetify.router.redirect(vm.$router.toUrl(iPath));

               // Load the view associated with the route asynchronously.
               vm.$router.loadView("#" + iTemplate.Target(), iTemplate.ViewUrl(), iTemplate.JSModuleUrl(), { "RoutingState.Origin": iPath }, function () {
                  // If load is successful, update the active route.
                  vm.RoutingState.Active(iPath);

                  // Support exit interception.
                  if (iDisableEvent != true && vm.hasOwnProperty("onRouteExit"))
                     vm.onRouteExit(iPath, iTemplate);
               }.bind(vm));
            }.bind(iScope),

            // Routes the URL if the view model implements IRoutable.
            // Returns true if the view model handles the routing.
            routeUrl: function () {
               var vm = this;
               if (!vm.hasOwnProperty("RoutingState") || typeof vm.RoutingState.Templates !== "function")
                  return false;

               var root = vm.RoutingState.Root();
               if (root == null)
                  return false;

               // Get the URL path to route.
               var urlPath = dotnetify.router.urlPath;

               if (dotnetify.debug)
                  console.log("router> routing " + urlPath);

               // If the URL path matches the root path of this view, use the template with a blank URL pattern if provided.
               if (utils.equal(urlPath, root)) {
                  var match = $.grep(vm.RoutingState.Templates(), function (iTemplate) { return iTemplate.UrlPattern() === "" });
                  if (match.length > 0)
                     vm.$router.routeTo("", match[0]);
                  return;
               }

               // If the URL path starts with the root path of this view, look at the next path and try to match it with the
               // anchor tags in this view that are bound with the vmRoute binding type.  If there is match, route to that path.
               root = root + "/";
               if (utils.startsWith(urlPath, root)) {

                  var routeElem = null;
                  var match = $.grep(vm.$element.find("[data-vm-route]"), function (elem) {
                     return utils.startsWith(urlPath, $(elem).attr("href"))
                  });
                  if (match.length > 0) {
                     // If more than one match, find the best match.
                     for (i = 0; i < match.length; i++)
                        if (routeElem == null || $(routeElem).attr("href").length < $(match[i]).attr("href").length)
                           routeElem = match[i];
                  }

                  if (routeElem != null) {
                     var path = $(routeElem).attr("data-vm-route");
                     var template = vm.$router.hasOwnProperty("pathToRoute") && vm.$router.pathToRoute.hasOwnProperty(path) ? vm.$router.pathToRoute[path].$template : null;
                     if (template != null) {
                        // If the URL path is completely routed, clear it.
                        if (utils.equal(dotnetify.router.urlPath, vm.$router.toUrl(path)))
                           dotnetify.router.urlPath = "";

                        // If route's not already active, route to it.
                        if (!utils.equal(vm.RoutingState.Active(), path))
                           vm.$router.routeTo(path, template);

                        return true;
                     }
                  }
                  else if (dotnetify.router.match(urlPath)) {
                     // If no vmRoute binding matches, try to match with any template's URL pattern.
                     dotnetify.router.urlPath = "";
                     return true;
                  }
               }
               return false;
            }.bind(iScope),

            // Builds an absolute URL from a path.
            toUrl: function (iPath) {
               var path = utils.trim(iPath);
               if (path.charAt(0) != '(' && path != "")
                  path = "/" + path;
               return this.hasOwnProperty("RoutingState") && typeof (this.RoutingState.Root === "function") ? this.RoutingState.Root() + path : iPath;
            }.bind(iScope)
         }
      })(iVM)
   }

   // Register the plugin to dotNetify.
   dotnetify.plugins["router"] = dotnetify.router;

   // Custom knockout binding to do routing.
   ko.bindingHandlers.vmRoute = {
      update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
         var vm = bindingContext.$root;
         var route = ko.unwrap(valueAccessor());

         if (!route.hasOwnProperty("Path") || !route.hasOwnProperty("TemplateId"))
            throw new Error("invalid vmRoute data at " + element.outerHTML);

         // Build the absolute root path.
         vm.$router.initRoot();

         // If the route path is not defined, use the URL pattern from the associated template.
         // This is so that we don't send the same data twice if both are equal.
         path = route.Path();
         var template = null;
         if (vm.hasOwnProperty("RoutingState") && typeof vm.RoutingState.Templates === "function" && vm.RoutingState.Templates() != null) {
            var match = $.grep(vm.RoutingState.Templates(), function (iTemplate) { return iTemplate.Id() == route.TemplateId() });
            if (match.length > 0) {
               template = match[0];
               if (path == null) {
                  path = template.UrlPattern();
                  route.Path(path);
               }
            }
            else if (route.RedirectRoot() == null)
               throw new Error("vmRoute cannot find route template '" + route.TemplateId() + "' at " + element.outerHTML);
         }

         // If the path has a redirect root, the path doesn't belong to the current root and needs to be
         // redirected to a different one.  Set the absolute path to the HREF attribute, and prevent the
         // default behavior of the anchor click event and instead do push to HTML5 history state, which 
         // would attempt to resolve the path first before resorting to hard browser redirect.
         if (route.RedirectRoot() != null) {

            // Combine the redirect root with the view model's root.
            var redirectRoot = route.RedirectRoot();
            if (redirectRoot.charAt(0) == '/')
               redirectRoot = redirectRoot.substr(0, redirectRoot.length - 1);
            var redirectRootPath = route.RedirectRoot().split("/");

            var url = "";
            var absRoot = vm.$element.attr("data-vm-root");
            if (absRoot != null) {
               var absRootPath = absRoot.split("/");
               for (i = 0; i < absRootPath.length; i++) {
                  if (absRootPath[i] != "" && absRootPath[i] == redirectRootPath[0])
                     break;
                  url += absRootPath[i] + "/";
               }
            }
            url += redirectRoot + "/" + path;

            $(element).attr("href", url)
               .attr("data-vm-route", path)
               .click(function (iEvent) {
                  iEvent.preventDefault();
                  dotnetify.router.pushState({}, "", $(this).attr("href"));
               })
            return;
         }

         // For quick lookup, save the mapping between the path to the route inside the view model.
         if (template == null)
            throw new Error("vmRoute cannot find any route template at " + element.outerHTML);

         route.$template = template;
         vm.$router.pathToRoute = vm.$router.pathToRoute || {};
         vm.$router.pathToRoute[path] = route;

         // Set the absolute path to the HREF attribute, and prevent the default behavior of 
         // the anchor click event and instead do push to HTML5 history state.
         $(element).attr("href", vm.$router.toUrl(path))
            .attr("data-vm-route", path)
            .click(function (iEvent) {
               iEvent.preventDefault();
               dotnetify.router.pushState({}, "", $(this).attr("href"));
            });
      }
   };
}))