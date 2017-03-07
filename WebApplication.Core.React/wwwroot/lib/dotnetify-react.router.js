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
   if (typeof define === "function" && define["amd"]) {
      define(['jquery', 'dotnetify'], factory);
   }
   else if (typeof exports === "object" && typeof module === "object") {
      module.exports = factory(require('jquery'), require('dotnetify'));
   }
   else {
      factory(jQuery, dotnetify);
   }
}
(function ($, dotnetify) {

   // PathJS. Need this specific version, because latest version is causing issue.
   var Path = { version: "0.8.4", map: function (a) { if (Path.routes.defined.hasOwnProperty(a)) { return Path.routes.defined[a] } else { return new Path.core.route(a) } }, root: function (a) { Path.routes.root = a }, rescue: function (a) { Path.routes.rescue = a }, history: { initial: {}, pushState: function (a, b, c) { if (Path.history.supported) { if (Path.dispatch(c)) { history.pushState(a, b, c) } } else { if (Path.history.fallback) { window.location.hash = "#" + c } } }, popState: function (a) { var b = !Path.history.initial.popped && location.href == Path.history.initial.URL; Path.history.initial.popped = true; if (b) return; Path.dispatch(document.location.pathname) }, listen: function (a) { Path.history.supported = !!(window.history && window.history.pushState); Path.history.fallback = a; if (Path.history.supported) { Path.history.initial.popped = "state" in window.history, Path.history.initial.URL = location.href; window.onpopstate = Path.history.popState } else { if (Path.history.fallback) { for (route in Path.routes.defined) { if (route.charAt(0) != "#") { Path.routes.defined["#" + route] = Path.routes.defined[route]; Path.routes.defined["#" + route].path = "#" + route } } Path.listen() } } } }, match: function (a, b) { var c = {}, d = null, e, f, g, h, i; for (d in Path.routes.defined) { if (d !== null && d !== undefined) { d = Path.routes.defined[d]; e = d.partition(); for (h = 0; h < e.length; h++) { f = e[h]; i = a; if (f.search(/:/) > 0) { for (g = 0; g < f.split("/").length; g++) { if (g < i.split("/").length && f.split("/")[g].charAt(0) === ":") { c[f.split("/")[g].replace(/:/, "")] = i.split("/")[g]; i = i.replace(i.split("/")[g], f.split("/")[g]) } } } if (f === i) { if (b) { d.params = c } return d } } } } return null }, dispatch: function (a) { var b, c; if (Path.routes.current !== a) { Path.routes.previous = Path.routes.current; Path.routes.current = a; c = Path.match(a, true); if (Path.routes.previous) { b = Path.match(Path.routes.previous); if (b !== null && b.do_exit !== null) { b.do_exit() } } if (c !== null) { c.run(); return true } else { if (Path.routes.rescue !== null) { Path.routes.rescue() } } } }, listen: function () { var a = function () { Path.dispatch(location.hash) }; if (location.hash === "") { if (Path.routes.root !== null) { location.hash = Path.routes.root } } if ("onhashchange" in window && (!document.documentMode || document.documentMode >= 8)) { window.onhashchange = a } else { setInterval(a, 50) } if (location.hash !== "") { Path.dispatch(location.hash) } }, core: { route: function (a) { this.path = a; this.action = null; this.do_enter = []; this.do_exit = null; this.params = {}; Path.routes.defined[a] = this } }, routes: { current: null, root: null, rescue: null, previous: null, defined: {} } }; Path.core.route.prototype = { to: function (a) { this.action = a; return this }, enter: function (a) { if (a instanceof Array) { this.do_enter = this.do_enter.concat(a) } else { this.do_enter.push(a) } return this }, exit: function (a) { this.do_exit = a; return this }, partition: function () { var a = [], b = [], c = /\(([^}]+?)\)/g, d, e; while (d = c.exec(this.path)) { a.push(d[1]) } b.push(this.path.split("(")[0]); for (e = 0; e < a.length; e++) { b.push(b[b.length - 1] + a[e]) } return b }, run: function () { var a = false, b, c, d; if (Path.routes.defined[this.path].hasOwnProperty("do_enter")) { if (Path.routes.defined[this.path].do_enter.length > 0) { for (b = 0; b < Path.routes.defined[this.path].do_enter.length; b++) { c = Path.routes.defined[this.path].do_enter[b](); if (c === false) { a = true; break } } } } if (!a) { Path.routes.defined[this.path].action() } } }

   // Add plugin functions.
   dotnetify.react.router =
      {
         version: "1.0.0",

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
         overrideUrl: function (iUrl) { return iUrl },

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
               if (vm != null && vm.$router.routeUrl())
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
               endsWith: function (iStr, iValue) { return iValue == '' || iStr.toLowerCase().slice(-iValue.length) == iValue.toLowerCase(); }
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

               $.each(templates, function (idx, template) {
                  // If url pattern isn't given, consider Id as the pattern.
                  var urlPattern = template.UrlPattern != null ? template.UrlPattern : template.Id;
                  var mapUrl = vm.$router.toUrl(urlPattern);

                  if (dotnetify.debug)
                     console.log("router> map " + mapUrl + " to template id=" + template.Id);

                  dotnetify.react.router.mapTo(mapUrl, function (iParams) {
                     dotnetify.react.router.urlPath = "";

                     // Construct the path from the template pattern and the params passed by PathJS.
                     var path = urlPattern;
                     for (param in iParams)
                        path = path.replace(":" + param, iParams[param]);
                     path = path.replace(/\(\/:([^)]+)\)/g, "").replace(/\(|\)/g, "");

                     vm.$router.routeTo(path, template);
                  });
               });

               // Route initial URL.
               var activeUrl = vm.$router.toUrl(state.RoutingState.Active);
               if (dotnetify.react.router.urlPath == "")
                  dotnetify.react.router.urlPath = activeUrl;
               vm.$router.routeUrl();

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
               iJsModuleUrl = dotnetify.react.router.overrideUrl(iJsModuleUrl);

               if (iViewUrl.endsWith("html"))
                  vm.$loadHtmlView(iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, iCallbackFn);
               else
                  vm.$loadReactView(iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, reactProps, iCallbackFn);

            }.bind(iScope),

            // Routes to a path.
            manualRouteTo: function (iPath, iTarget, iViewUrl, iJSModuleUrl) {
               var template = { Id: "manual", Target: iTarget, ViewUrl: iViewUrl, JSModuleUrl: iJSModuleUrl };
               this.$router.routeTo(iPath, template, true);
            }.bind(iScope),

            // Routes to a path.
            routeTo: function (iPath, iTemplate, iDisableEvent) {
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

                  // Support exit interception.
                  if (iDisableEvent != true && vm.hasOwnProperty("onRouteExit"))
                     vm.onRouteExit(iPath, iTemplate);
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
                  var match = $.grep(state.RoutingState.Templates, function (iTemplate) { return iTemplate.UrlPattern === "" });
                  if (match.length > 0)
                     vm.$router.routeTo("", match[0]);
                  return;
               }

               // If the URL path starts with the root path of this view, look at the next path and try to match it with the
               // anchor tags in this view that are bound with the vmRoute binding type.  If there is match, route to that path.
               root = root + "/";
               if (utils.startsWith(urlPath, root)) {

                  var routeElem = null;
                  var match = $.grep(vm.$router.routes, function (elem) {
                     return utils.startsWith(urlPath, elem.Url)
                  });
                  if (match.length > 0) {
                     // If more than one match, find the best match.
                     for (i = 0; i < match.length; i++)
                        if (routeElem == null || routeElem.Url.length < $(match[i]).get(0).Url.length)
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
                           vm.$router.routeTo(path, template);

                        return true;
                     }
                  }
                  else if (dotnetify.react.router.match(urlPath)) {
                     // If no vmRoute binding matches, try to match with any template's URL pattern.
                     dotnetify.react.router.urlPath = "";
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
         path = iRoute.Path;
         var template = null;
         if (state.hasOwnProperty("RoutingState") && state.RoutingState.Templates != null) {
            var match = $.grep(state.RoutingState.Templates, function (iTemplate) { return iTemplate.Id == iRoute.TemplateId });
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

            let url = "";
            var absRoot = vm.$component.props["vmRoot"];
            if (absRoot != null) {
               var absRootPath = absRoot.split("/");
               for (i = 0; i < absRootPath.length; i++) {
                  if (absRootPath[i] != "" && absRootPath[i] == redirectRootPath[0])
                     break;
                  url += absRootPath[i] + "/";
               }
            }
            url += redirectRoot + "/" + path;
            vm.$router.routes.push({ Path: path, Url: url });
            return url;
         }

         // For quick lookup, save the mapping between the path to the route inside the view model.
         if (template == null)
            throw new Error("vmRoute cannot find any route template");

         iRoute.$template = template;
         vm.$router.pathToRoute = vm.$router.pathToRoute || {};
         vm.$router.pathToRoute[path] = iRoute;

         // Set the absolute path to the HREF attribute, and prevent the default behavior of 
         // the anchor click event and instead do push to HTML5 history state.
         let url = vm.$router.toUrl(path);
         vm.$router.routes.push({ Path: path, Url: url });
         return url;

      }.bind(iVM);

      iVM.$handleRoute = function (iEvent) {
         iEvent.preventDefault();
         var path = iEvent.currentTarget.pathname;
         if (path == null || path == "")
            throw new Error("The event passed to $handleRoute has no path name.");

         dotnetify.react.router.pushState({}, "", path);
      }.bind(iVM);
   }

   // Register the plugin to dotNetify.
   dotnetify.react.plugins["router"] = dotnetify.react.router;

   // <RouteLink> is a helper component to set anchor tags for routes.
   dotnetify.react.router.RouteLink = function (props) {
      if (props.vm == null)
         console.error("RouteLink requires 'vm' property.");
      else if (props.route == null)
         console.error("RouteLink requires 'route' property.")

      return React.createElement(
         "a", {
            style: props.style,
            className: props.className,
            href: props.vm.$route(props.route),
            onClick: function (event) {
               event.preventDefault();
               if (typeof props.onClick === "function")
                  props.onClick();
               return props.vm.$handleRoute(event);
            }
         },
         props.children
      );
   }
}))