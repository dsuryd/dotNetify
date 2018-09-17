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

// Support using AMD or CommonJS that loads our app.js, or being placed in <script> tag.
(function (factory) {
   if (typeof exports === "object" && typeof module === "object") {
      var jquery = typeof jQuery !== "undefined" ? jQuery : require('./jquery-shim');
      module.exports = factory(require('react'), require('react-dom'), require('create-react-class'), jquery, require('dotnetify'));
   }
   else if (typeof define === "function" && define["amd"]) {
      define(['react', 'react-dom', 'create-react-class', './jquery-shim', 'dotnetify'], factory);
   }
   else {
      factory(React, ReactDOM, createReactClass, jQuery, dotnetify);
   }
}
   (function (_React, _ReactDOM, createReactClass, $, dotnetify) {

      // PathJS. Need this specific version, because latest version is causing issue.
      var Path = {
         'version': "0.8.5",
         'map': function (path) {
            if (Path.routes.defined.hasOwnProperty(path)) {
               return Path.routes.defined[path];
            } else {
               return new Path.core.route(path);
            }
         },
         'root': function (path) {
            Path.routes.root = path;
         },
         'rescue': function (fn) {
            Path.routes.rescue = fn;
         },
         'history': {
            'initial': {}, // Empty container for "Initial Popstate" checking variables.
            'pushState': function (state, title, path) {
               if (Path.history.supported) {
                  if (Path.dispatch(path)) {
                     history.pushState(state, title, path);
                  }
               } else {
                  if (Path.history.fallback) {
                     window.location.hash = "#" + path;
                  }
               }
            },
            'popState': function (event) {
               var initialPop = !Path.history.initial.popped && location.href == Path.history.initial.URL;
               Path.history.initial.popped = true;
               if (initialPop) return;
               Path.dispatch(document.location.pathname === "/" ? "" : document.location.pathname);
            },
            'listen': function (fallback) {
               Path.history.supported = !!(window.history && window.history.pushState);
               Path.history.fallback = fallback;

               if (Path.history.supported) {
                  Path.history.initial.popped = ('state' in window.history), Path.history.initial.URL = location.href;
                  window.onpopstate = Path.history.popState;
               } else {
                  if (Path.history.fallback) {
                     for (route in Path.routes.defined) {
                        if (route.charAt(0) != "#") {
                           Path.routes.defined["#" + route] = Path.routes.defined[route];
                           Path.routes.defined["#" + route].path = "#" + route;
                        }
                     }
                     Path.listen();
                  }
               }
            }
         },
         'match': function (path, parameterize) {
            var params = {}, route = null, possible_routes, slice, i, j, compare, result;
            for (route in Path.routes.defined) {
               if (route !== null && route !== undefined) {
                  route = Path.routes.defined[route];
                  possible_routes = route.partition();
                  for (j = 0; j < possible_routes.length; j++) {
                     slice = possible_routes[j];
                     compare = path;
                     if (slice.search(/:/) > 0) {
                        for (i = 0; i < slice.split("/").length; i++) {
                           if ((i < compare.split("/").length) && (slice.split("/")[i].charAt(0) === ":")) {
                              params[slice.split('/')[i].replace(/:/, '')] = compare.split("/")[i];
                              result = compare.split("/");
                              result[i] = slice.split("/")[i];
                              compare = result.join("/");
                           }
                        }
                     }
                     if (slice === compare) {
                        if (parameterize) {
                           route.params = params;
                        }
                        return route;
                     }
                  }
               }
            }
            return null;
         },
         'dispatch': function (passed_route) {
            var previous_route, matched_route;
            if (Path.routes.current !== passed_route) {
               Path.routes.previous = Path.routes.current;
               Path.routes.current = passed_route;
               matched_route = Path.match(passed_route, true);

               if (Path.routes.previous) {
                  previous_route = Path.match(Path.routes.previous);
                  if (previous_route !== null && previous_route.do_exit !== null) {
                     previous_route.do_exit();
                  }
               }

               if (matched_route !== null) {
                  matched_route.run();
                  return true;
               } else {
                  if (Path.routes.rescue !== null) {
                     Path.routes.rescue();
                  }
               }
            }
         },
         'listen': function () {
            var fn = function () { Path.dispatch(location.hash); }

            if (location.hash === "") {
               if (Path.routes.root !== null) {
                  location.hash = Path.routes.root;
               }
            }

            // The 'document.documentMode' checks below ensure that PathJS fires the right events
            // even in IE "Quirks Mode".
            if ("onhashchange" in window && (!document.documentMode || document.documentMode >= 8)) {
               window.onhashchange = fn;
            } else {
               setInterval(fn, 50);
            }

            if (location.hash !== "") {
               Path.dispatch(location.hash);
            }
         },
         'core': {
            'route': function (path) {
               this.path = path;
               this.action = null;
               this.do_enter = [];
               this.do_exit = null;
               this.params = {};
               Path.routes.defined[path] = this;
            }
         },
         'routes': {
            'current': null,
            'root': null,
            'rescue': null,
            'previous': null,
            'defined': {}
         }
      };
      Path.core.route.prototype = {
         'to': function (fn) {
            this.action = fn;
            return this;
         },
         'enter': function (fns) {
            if (fns instanceof Array) {
               this.do_enter = this.do_enter.concat(fns);
            } else {
               this.do_enter.push(fns);
            }
            return this;
         },
         'exit': function (fn) {
            this.do_exit = fn;
            return this;
         },
         'partition': function () {
            var parts = [], options = [], re = /\(([^}]+?)\)/g, text, i;
            while (text = re.exec(this.path)) {
               parts.push(text[1]);
            }
            options.push(this.path.split("(")[0]);
            for (i = 0; i < parts.length; i++) {
               options.push(options[options.length - 1] + parts[i]);
            }
            return options;
         },
         'run': function () {
            var halt_execution = false, i, result, previous;

            if (Path.routes.defined[this.path].hasOwnProperty("do_enter")) {
               if (Path.routes.defined[this.path].do_enter.length > 0) {
                  for (i = 0; i < Path.routes.defined[this.path].do_enter.length; i++) {
                     result = Path.routes.defined[this.path].do_enter[i].apply(this, null);
                     if (result === false) {
                        halt_execution = true;
                        break;
                     }
                  }
               }
            }
            if (!halt_execution) {
               Path.routes.defined[this.path].action();
            }
         }
      };

      // Add plugin functions.
      dotnetify.react.router = {
         version: "1.1.0",

         // URL path that will be parsed when performing routing.
         urlPath: document.location.pathname,

         // Initialize routing using PathJS.
         init: function () {
            if (typeof Path !== "undefined") {
               Path.history.listen(true);
               Path.routes.rescue = function () {
                  //window.location.replace(document.location.pathname);
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
         overrideUrl: function (iUrl, iTargetSelector) { return iUrl },

         // Push state to HTML history.
         pushState: function (iState, iTitle, iPath) {
            dotnetify.react.router.urlPath = "";
            if (typeof Path !== "undefined")
               Path.history.pushState(iState, iTitle, iPath);
         },

         // Redirect to the a URL.
         redirect: function (iUrl) {
            // Check first whether existing views can handle routing this URL.
            // Otherwise, do a hard browser redirect.
            dotnetify.react.router.urlPath = iUrl;
            for (var vmId in dotnetify.react.viewModels) {
               var vm = dotnetify.react.viewModels[vmId];
               if (vm != null && vm.$router.routeUrl()) {
                  if (dotnetify.debug)
                     console.log("router> redirected");
                  return;
               }
            }
            window.location.replace(iUrl);
         },

         // Used by client-side React component to get server-side rendered initial state.
         ssrState: function (iVMId) {
            if (window.ssr && window.ssr[iVMId] && !window.ssr[iVMId].fetched) {
               window.ssr[iVMId].fetched = true;
               return window.ssr[iVMId];
            }
         },

         // Called from server to configure server-side rendering.
         // iPath - initial URL path.
         // iInitialStates - serialized object literal '{ "vmName": {initialState}, ...}'.
         // iOverrideRouteFn - function to override routing URLs before the router loads them.
         // iCallbackFn - callback after the path is fully routed.
         // iTimeout - timeout in milliseconds.
         ssr: function (iPath, iInitialStates, iOverrideRouteFn, iCallbackFn, iTimeout) {
            dotnetify.ssr = true;
            dotnetify.react.router.urlPath = iPath;
            dotnetify.react.router.overrideUrl = iOverrideRouteFn;

            // Insert initial states in the head tag.
            var script = document.createElement("script");
            script.type = "text/javascript";
            script.text = "window.ssr=" + iInitialStates + ";";
            var head = document.getElementsByTagName("head")[0];
            if (head)
               head.insertBefore(script, head.firstChild);
            else
               console.error("router> document head tag is required for server-side render.");

            var routed = false;
            var fallback = iTimeout ? setTimeout(function () { if (!routed) iCallbackFn(); }, iTimeout) : 0;
            window.addEventListener('dotnetify.routed', function () {
               routed = true;
               if (fallback != 0)
                  clearTimeout(fallback);
               iCallbackFn();
            });

            // Add initial states into the window scope for the server-renderd components.
            window.ssr = JSON.parse(iInitialStates);
         },

         // Called by dotNetify when a view model is ready.
         $ready: function () {
            this.$router.initRouting();
         }
      }

      // Inject a view model with functions.
      dotnetify.react.router.$inject = function (iVM) {

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
                  endsWith: function (iStr, iValue) { return iValue == '' || iStr.toLowerCase().slice(-iValue.length) == iValue.toLowerCase(); },
                  // Dispatch event with IE polyfill.
                  dispatchEvent: function (iEvent) {
                     if (typeof Event === "function")
                        window.dispatchEvent(new Event(iEvent));
                     else {
                        var event = document.createEvent("CustomEvent");
                        event.initEvent(iEvent, true, true);
                        window.dispatchEvent(event);
                     }
                  },
                  grep: function (iArray, iFilter) { return (Array.isArray(iArray)) ? iArray.filter(iFilter) : []; }
               }
            })();

            return {
               routes: [],

               // Build the absolute root path from the "vmRoot" property on React component.
               initRoot: function () {
                  var vm = this;
                  var state = vm.State();
                  if (!state.hasOwnProperty("RoutingState") || state.RoutingState === null || state.RoutingState.Root === null)
                     return;

                  if (vm.$router._absRoot != state.RoutingState.Root) {
                     var vmRoot = vm.$component.props["vmRoot"];
                     var absRoot = utils.trim(vmRoot);
                     if (absRoot != "")
                        absRoot = "/" + absRoot;
                     var root = utils.trim(state.RoutingState.Root);
                     vm.$router._absRoot = root != "" ? absRoot + "/" + root : absRoot;
                     state.RoutingState.Root = vm.$router._absRoot;
                  }
               }.bind(iScope),

               // Initialize routing templates if the view model implements IRoutable.
               initRouting: function () {
                  var vm = this;
                  var state = vm.State();
                  if (state == null || !state.hasOwnProperty("RoutingState"))
                     return;

                  if (state.RoutingState === null) {
                     console.error("router> the RoutingState prop of '" + vm.$vmId + "' was not initialized.");
                     return;
                  }

                  var templates = state.RoutingState.Templates;
                  if (templates == null || templates.length == 0)
                     return;

                  // Initialize the router.
                  if (!dotnetify.react.router.$init) {
                     dotnetify.react.router.init();
                     dotnetify.react.router.$init = true;
                  }

                  // Build the absolute root path.
                  vm.$router.initRoot();

                  templates.forEach(function (template, idx) {
                     // If url pattern isn't given, consider Id as the pattern.
                     var urlPattern = template.UrlPattern != null ? template.UrlPattern : template.Id;
                     urlPattern = urlPattern != "" ? urlPattern : "/";
                     var mapUrl = vm.$router.toUrl(urlPattern);

                     if (dotnetify.debug)
                        console.log("router> map " + mapUrl + " to template id=" + template.Id);

                     dotnetify.react.router.mapTo(mapUrl, function (iParams) {
                        dotnetify.react.router.urlPath = "";

                        // Construct the path from the template pattern and the params passed by PathJS.
                        var path = urlPattern;
                        for (var param in iParams)
                           path = path.replace(":" + param, iParams[param]);
                        path = path.replace(/\(\/:([^)]+)\)/g, "").replace(/\(|\)/g, "");

                        vm.$router.routeTo(path, template);
                     });
                  });

                  // Route initial URL.
                  var activeUrl = vm.$router.toUrl(state.RoutingState.Active);
                  if (dotnetify.react.router.urlPath == "")
                     dotnetify.react.router.urlPath = activeUrl;
                  if (!vm.$router.routeUrl())
                     // If routing ends incomplete, raise routed event anyway.
                     vm.$router.raiseRoutedEvent(true);
               }.bind(iScope),

               // Whether a route is active.
               isActive: function (iRoute) {
                  var state = this.State();
                  if (iRoute != null && iRoute.hasOwnProperty("Path"))
                     return utils.equal(iRoute.Path, state.RoutingState.Active);
                  return false;
               }.bind(iScope),

               // Loads a view into a target element.
               loadView: function (iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, iCallbackFn) {
                  var vm = this;
                  var state = vm.State();
                  var reactProps;

                  // If the view model supports routing, add the root path to the view, to be used
                  // to build the absolute route path, and view model argument if provided.
                  if (state.hasOwnProperty("RoutingState")) {

                     if (state.RoutingState === null) {
                        console.error("router> the RoutingState prop of '" + vm.$vmId + "' was not initialized.");
                        return;
                     }

                     var root = vm.$component.props.vmRoot;
                     root = root != null ? "/" + utils.trim(state.RoutingState.Root) + "/" + utils.trim(root) : state.RoutingState.Root;
                     reactProps = { vmRoot: root, vmArg: iVmArg };
                  }

                  // Provide the opportunity to override the URL.
                  iViewUrl = dotnetify.react.router.overrideUrl(iViewUrl, iTargetSelector);
                  iJsModuleUrl = dotnetify.react.router.overrideUrl(iJsModuleUrl, iTargetSelector);

                  if (utils.endsWith(iViewUrl, "html"))
                     vm.$router.loadHtmlView(iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, iCallbackFn);
                  else
                     vm.$router.loadReactView(iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, reactProps, iCallbackFn);

               }.bind(iScope),

               // Loads an HTML view.
               loadHtmlView: function (iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, callbackFn) {
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
               }.bind(iScope),

               // Loads a React view.
               loadReactView: function (iTargetSelector, iReactClassName, iJsModuleUrl, iVmArg, iReactProps, callbackFn) {
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
                     $.when.apply($, getScripts).done(createViewFunc);
                  }
               }.bind(iScope),

               // Routes to a path.
               manualRouteTo: function (iPath, iTarget, iViewUrl, iJSModuleUrl) {
                  var template = { Id: "manual", Target: iTarget, ViewUrl: iViewUrl, JSModuleUrl: iJSModuleUrl };
                  this.$router.routeTo(iPath, template, true);
               }.bind(iScope),

               // Raise event indicating the routing process has ended.
               raiseRoutedEvent: function (force) {
                  if (dotnetify.react.router.urlPath == "" || force == true) {
                     if (dotnetify.debug)
                        console.log("router> routed");
                     utils.dispatchEvent("dotnetify.routed");
                  }
               }.bind(iScope),

               // Routes to a path.
               routeTo: function (iPath, iTemplate, iDisableEvent, iCallbackFn) {
                  var vm = this;
                  var state = vm.State();

                  if (dotnetify.debug)
                     console.log("router> route '" + iPath + "' to template id=" + iTemplate.Id);

                  // We can determine whether the view has already been loaded by matching the 'RoutingState.Origin' argument
                  // on the existing view model inside that target selector with the path.
                  for (var vmId in dotnetify.react.viewModels) {
                     var vmOther = dotnetify.react.viewModels[vmId];
                     var vmArg = vmOther.$component.props["vmArg"];
                     if (vmArg != null) {
                        if (typeof vmArg["RoutingState.Origin"] === "string" && utils.equal(vmArg["RoutingState.Origin"], iPath))
                           return;
                     }
                  }

                  // Support enter interception.
                  if (iDisableEvent != true && vm.hasOwnProperty("onRouteEnter"))
                     if (vm.onRouteEnter(iPath, iTemplate) == false)
                        return;

                  // Check if the route has valid target.
                  if (iTemplate.Target === null) {
                     console.error("router> the Target for template '" + iTemplate.Id + "' was not set.  Use vm.onRouteEnter() to set the target.");
                     return;
                  }

                  // If target DOM element isn't found, redirect URL to the path.
                  if (document.getElementById(iTemplate.Target) == null) {
                     if (dotnetify.debug)
                        console.log("router> target '" + iTemplate.Target + "' not found in DOM, use redirect instead");

                     return dotnetify.react.router.redirect(vm.$router.toUrl(iPath));
                  }

                  // Load the view associated with the route asynchronously.
                  var view = iTemplate.ViewUrl ? iTemplate.ViewUrl : iTemplate.Id;
                  vm.$router.loadView("#" + iTemplate.Target, view, iTemplate.JSModuleUrl, { "RoutingState.Origin": iPath }, function () {
                     // If load is successful, update the active route.
                     state.RoutingState.Active = iPath;
                     vm.$dispatch({ "RoutingState.Active": iPath });
                     vm.State({ "RoutingState.Active": iPath });

                     // Support exit interception.
                     if (iDisableEvent != true && vm.hasOwnProperty("onRouteExit"))
                        vm.onRouteExit(iPath, iTemplate);

                     if (typeof iCallbackFn === "function")
                        iCallbackFn.call(vm);
                  }.bind(vm));
               }.bind(iScope),

               // Routes the URL if the view model implements IRoutable.
               // Returns true if the view model handles the routing.
               routeUrl: function () {
                  var vm = this;
                  var state = vm.State();
                  if (!state.hasOwnProperty("RoutingState"))
                     return false;

                  var root = state.RoutingState.Root;
                  if (root == null)
                     return false;

                  // Get the URL path to route.
                  var urlPath = dotnetify.react.router.urlPath;

                  if (dotnetify.debug)
                     console.log("router> routing " + urlPath);

                  // If the URL path matches the root path of this view, use the template with a blank URL pattern if provided.
                  if (utils.equal(urlPath, root) || utils.equal(urlPath, root + "/") || urlPath === "/") {
                     var match = utils.grep(state.RoutingState.Templates, function (iTemplate) { return iTemplate.UrlPattern === "" });
                     if (match.length > 0) {
                        vm.$router.routeTo("", match[0]);
                        dotnetify.react.router.urlPath = "";
                        vm.$router.raiseRoutedEvent();
                        return true;
                     }
                     return false;
                  }

                  // If the URL path starts with the root path of this view, look at the next path and try to match it with the
                  // anchor tags in this view that are bound with the vmRoute binding type.  If there is match, route to that path.
                  root = root + "/";
                  if (utils.startsWith(urlPath, root)) {

                     var routeElem = null;
                     var match = utils.grep(vm.$router.routes, function (elem) {
                        return utils.startsWith(urlPath + "/", elem.Url + "/")
                     });
                     if (match.length > 0) {
                        // If more than one match, find the best match.
                        for (var i = 0; i < match.length; i++)
                           if (routeElem == null || routeElem.Url.length < match[i].Url.length)
                              routeElem = match[i];
                     }

                     if (routeElem != null) {
                        var path = routeElem.Path;
                        var template = vm.$router.hasOwnProperty("pathToRoute") && vm.$router.pathToRoute.hasOwnProperty(path) ? vm.$router.pathToRoute[path].$template : null;
                        if (template != null) {
                           // If the URL path is completely routed, clear it.
                           if (utils.equal(dotnetify.react.router.urlPath, vm.$router.toUrl(path)))
                              dotnetify.react.router.urlPath = "";

                           // If route's not already active, route to it.
                           if (!utils.equal(state.RoutingState.Active, path))
                              vm.$router.routeTo(path, template, false, function () { vm.$router.raiseRoutedEvent(); });
                           else
                              vm.$router.raiseRoutedEvent();
                           return true;
                        }
                     }
                     else if (dotnetify.react.router.match(urlPath)) {
                        // If no vmRoute binding matches, try to match with any template's URL pattern.
                        dotnetify.react.router.urlPath = "";
                        vm.$router.raiseRoutedEvent();
                        return true;
                     }
                  }
                  return false;
               }.bind(iScope),

               // Builds an absolute URL from a path.
               toUrl: function (iPath) {
                  var state = this.State();
                  var path = utils.trim(iPath);
                  if (path.charAt(0) != '(' && path != "")
                     path = "/" + path;
                  return state.hasOwnProperty("RoutingState") ? state.RoutingState.Root + path : iPath;
               }.bind(iScope),


            }
         })(iVM);

         // Returns the URL for an anchor tag.
         iVM.$route = function (iRoute, iTarget) {
            var vm = this;
            var state = vm.State();

            // No route to process. Return silently.
            if (iRoute == null)
               return;

            if (!iRoute.hasOwnProperty("Path") || !iRoute.hasOwnProperty("TemplateId"))
               throw new Error("Not a valid route");

            // Build the absolute root path.
            vm.$router.initRoot();

            // If the route path is not defined, use the URL pattern from the associated template.
            // This is so that we don't send the same data twice if both are equal.
            var path = iRoute.Path;
            var template = null;
            if (state.hasOwnProperty("RoutingState") && state.RoutingState.Templates != null) {
               var match = state.RoutingState.Templates.filter(function (iTemplate) { return iTemplate.Id == iRoute.TemplateId });
               if (match.length > 0) {
                  template = match[0];

                  if (typeof iTarget === "string")
                     template.Target = iTarget;

                  if (path == null) {
                     path = template.UrlPattern != null ? template.UrlPattern : template.Id;
                     iRoute.Path = path;
                  }
               }
               else if (iRoute.RedirectRoot == null)
                  throw new Error("vmRoute cannot find route template '" + iRoute.TemplateId);
            }

            // If the path has a redirect root, the path doesn't belong to the current root and needs to be
            // redirected to a different one.  Set the absolute path to the HREF attribute, and prevent the
            // default behavior of the anchor click event and instead do push to HTML5 history state, which 
            // would attempt to resolve the path first before resorting to hard browser redirect.
            if (iRoute.RedirectRoot != null) {

               // Combine the redirect root with the view model's root.
               var redirectRoot = iRoute.RedirectRoot;
               if (redirectRoot.charAt(0) == '/')
                  redirectRoot = redirectRoot.substr(0, redirectRoot.length - 1);
               var redirectRootPath = iRoute.RedirectRoot.split("/");

               var urlRedirect = "";
               var absRoot = vm.$component.props["vmRoot"];
               if (absRoot != null) {
                  var absRootPath = absRoot.split("/");
                  for (var i = 0; i < absRootPath.length; i++) {
                     if (absRootPath[i] != "" && absRootPath[i] == redirectRootPath[0])
                        break;
                     urlRedirect += absRootPath[i] + "/";
                  }
               }
               urlRedirect += redirectRoot + "/" + path;
               vm.$router.routes.push({ Path: path, Url: urlRedirect });
               return urlRedirect;
            }

            // For quick lookup, save the mapping between the path to the route inside the view model.
            if (template == null)
               throw new Error("vmRoute cannot find any route template");

            iRoute.$template = template;
            vm.$router.pathToRoute = vm.$router.pathToRoute || {};
            vm.$router.pathToRoute[path] = iRoute;

            // Set the absolute path to the HREF attribute, and prevent the default behavior of 
            // the anchor click event and instead do push to HTML5 history state.
            var url = vm.$router.toUrl(path);
            vm.$router.routes.push({ Path: path, Url: url });
            return url;

         }.bind(iVM);

         // Handles click event from anchor tags.
         iVM.$handleRoute = function (iEvent) {
            iEvent.preventDefault();
            var path = iEvent.currentTarget.pathname;
            if (path == null || path == "")
               throw new Error("The event passed to $handleRoute has no path name.");

            setTimeout(function () { dotnetify.react.router.pushState({}, "", path) }, 0);
         }.bind(iVM);

         // Executes the given route.
         iVM.$routeTo = function (iRoute) {
            var path = iVM.$route(iRoute);
            if (path == null || path == "")
               throw new Error("The route passed to $routeTo is invalid.");

            setTimeout(function () { dotnetify.react.router.pushState({}, "", path) }, 0);
         }.bind(iVM);
      }

      // <RouteLink> is a helper component to set anchor tags for routes.
      dotnetify.react.router.RouteLink = function (props) {
         if (props.vm == null)
            console.error("RouteLink requires 'vm' property.");

         return _React.createElement(
            "a", {
               style: props.style,
               className: props.className,
               href: props.route != null ? props.vm.$route(props.route) : "",
               onClick: function (event) {
                  event.preventDefault();
                  if (props.route == null) {
                     console.error("RouteLink requires 'route' property.");
                     return;
                  }
                  if (typeof props.onClick === "function")
                     props.onClick(event);
                  return props.vm.$handleRoute(event);
               }
            },
            props.children
         );
      }

      // <RouteTarget> is a helper component to provide DOM target for routes, and is essential for server-side rendering.
      dotnetify.react.router.RouteTarget = createReactClass({
         displayName: "RouteTarget",
         componentWillMount: function componentWillMount() {
            var elem = document.getElementById(this.props.id);
            if (elem != null && window.ssr && !window.ssr[this.props.id]) {
               window.ssr[this.props.id] = true;
               this.initialHtml = { __html: elem.innerHTML };
            } else this.initialHtml = { __html: '' };
         },
         getDOMNode: function getDOMNode() { return this.elem; },
         render: function render() {
            var _this = this;
            return _React.createElement(
               "div",
               $.extend({ id: this.props.id, ref: function (el) { return _this.elem = el; } }, this.props, { dangerouslySetInnerHTML: this.initialHtml }),
               this.props.children
            );
         }
      });

      // Register the plugin to dotNetify.
      dotnetify.react.plugins["router"] = dotnetify.react.router;

      return dotnetify.react.router;
   }))