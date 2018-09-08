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
import React from 'react';
import ReactDOM from 'react-dom';
import $ from '../jquery-shim';
import dotnetify from './dotnetify-react';
import Path from '../path';
import utils from '../utils';

const _React = React;
const _ReactDOM = ReactDOM;

// Add plugin functions.
dotnetify.react.router = {
  version: '2.0.0',

  // URL path that will be parsed when performing routing.
  urlPath: document.location.pathname,

  // Initialize routing using PathJS.
  init: function() {
    if (typeof Path !== 'undefined') {
      Path.history.listen(true);
      Path.routes.rescue = function() {
        //window.location.replace(document.location.pathname);
      };
    }
    else throw new Error('Pathjs library is required for routing.');
  },

  // Map a route to an action.
  mapTo: function(iPath, iFn) {
    if (typeof Path !== 'undefined')
      Path.map(iPath).to(function() {
        iFn(this.params);
      });
  },

  // Match a URL path to a route and run the action.
  match: function(iUrlPath) {
    if (typeof Path !== 'undefined') {
      var matched = Path.match(iUrlPath, true);
      if (matched != null) {
        matched.run();
        return true;
      }
    }
    return false;
  },

  // Optional callback to override a URL before performing routing.
  overrideUrl: function(iUrl, iTargetSelector) {
    return iUrl;
  },

  // Push state to HTML history.
  pushState: function(iState, iTitle, iPath) {
    dotnetify.react.router.urlPath = '';
    if (typeof Path !== 'undefined') Path.history.pushState(iState, iTitle, iPath);
  },

  // Redirect to the a URL.
  redirect: function(iUrl, viewModels) {
    // Check first whether existing views can handle routing this URL.
    // Otherwise, do a hard browser redirect.
    dotnetify.react.router.urlPath = iUrl;
    for (var vm in viewModels) {
      if (vm.$router.routeUrl()) {
        if (dotnetify.debug) console.log('router> redirected');
        return;
      }
    }
    window.location.replace(iUrl);
  },

  // Called by dotNetify when a view model is ready.
  $ready: function() {
    this.$router.initRouting();
  }
};

// Inject a view model with functions.
dotnetify.react.router.$inject = function(iVM) {
  // Put functions inside $router namespace.
  iVM['$router'] = (function(iScope) {
    return {
      routes: [],

      // Build the absolute root path from the "vmRoot" property on React component.
      initRoot: function() {
        var vm = this;
        var state = vm.State();
        if (!state.hasOwnProperty('RoutingState') || state.RoutingState === null || state.RoutingState.Root === null) return;

        if (vm.$router._absRoot != state.RoutingState.Root) {
          var vmRoot = vm.Props('vmRoot');
          var absRoot = utils.trim(vmRoot);
          if (absRoot != '') absRoot = '/' + absRoot;
          var root = utils.trim(state.RoutingState.Root);
          vm.$router._absRoot = root != '' ? absRoot + '/' + root : absRoot;
          state.RoutingState.Root = vm.$router._absRoot;
        }
      }.bind(iScope),

      // Initialize routing templates if the view model implements IRoutable.
      initRouting: function() {
        var vm = this;
        var state = vm.State();
        if (state == null || !state.hasOwnProperty('RoutingState')) return;

        if (state.RoutingState === null) {
          console.error("router> the RoutingState prop of '" + vm.$vmId + "' was not initialized.");
          return;
        }

        var templates = state.RoutingState.Templates;
        if (templates == null || templates.length == 0) return;

        // Initialize the router.
        if (!dotnetify.react.router.$init) {
          dotnetify.react.router.init();
          dotnetify.react.router.$init = true;
        }

        // Build the absolute root path.
        vm.$router.initRoot();

        templates.forEach(function(template, idx) {
          // If url pattern isn't given, consider Id as the pattern.
          var urlPattern = template.UrlPattern != null ? template.UrlPattern : template.Id;
          urlPattern = urlPattern != '' ? urlPattern : '/';
          var mapUrl = vm.$router.toUrl(urlPattern);

          if (dotnetify.debug) console.log('router> map ' + mapUrl + ' to template id=' + template.Id);

          dotnetify.react.router.mapTo(mapUrl, function(iParams) {
            dotnetify.react.router.urlPath = '';

            // Construct the path from the template pattern and the params passed by PathJS.
            var path = urlPattern;
            for (var param in iParams) path = path.replace(':' + param, iParams[param]);
            path = path.replace(/\(\/:([^)]+)\)/g, '').replace(/\(|\)/g, '');

            vm.$router.routeTo(path, template);
          });
        });

        // Route initial URL.
        var activeUrl = vm.$router.toUrl(state.RoutingState.Active);
        if (dotnetify.react.router.urlPath == '') dotnetify.react.router.urlPath = activeUrl;
        if (!vm.$router.routeUrl())
          // If routing ends incomplete, raise routed event anyway.
          vm.$router.raiseRoutedEvent(true);
      }.bind(iScope),

      // Whether a route is active.
      isActive: function(iRoute) {
        var state = this.State();
        if (iRoute != null && iRoute.hasOwnProperty('Path')) return utils.equal(iRoute.Path, state.RoutingState.Active);
        return false;
      }.bind(iScope),

      // Loads a view into a target element.
      loadView: function(iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, iCallbackFn) {
        var vm = this;
        var state = vm.State();
        var reactProps;

        // If the view model supports routing, add the root path to the view, to be used
        // to build the absolute route path, and view model argument if provided.
        if (state.hasOwnProperty('RoutingState')) {
          if (state.RoutingState === null) {
            console.error("router> the RoutingState prop of '" + vm.$vmId + "' was not initialized.");
            return;
          }

          var root = vm.Props('vmRoot');
          root = root != null ? '/' + utils.trim(state.RoutingState.Root) + '/' + utils.trim(root) : state.RoutingState.Root;
          reactProps = { vmRoot: root, vmArg: iVmArg };
        }

        // Provide the opportunity to override the URL.
        iViewUrl = dotnetify.react.router.overrideUrl(iViewUrl, iTargetSelector);
        iJsModuleUrl = dotnetify.react.router.overrideUrl(iJsModuleUrl, iTargetSelector);

        if (utils.endsWith(iViewUrl, 'html')) vm.$router.loadHtmlView(iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, iCallbackFn);
        else vm.$router.loadReactView(iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, reactProps, iCallbackFn);
      }.bind(iScope),

      // Loads an HTML view.
      loadHtmlView: function(iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, callbackFn) {
        var vm = this;

        try {
          // Unmount any React component before replacing with a new DOM.
          _ReactDOM.unmountComponentAtNode(document.querySelector(iTargetSelector));
        } catch (e) {
          console.error(e);
        }

        // Load the HTML view.
        $(iTargetSelector).load(iViewUrl, null, function() {
          if (iJsModuleUrl != null) {
            $.getScript(iJsModuleUrl, function() {
              if (typeof callbackFn === 'function') callbackFn.call(vm);
            });
          }
          else if (typeof callbackFn === 'function') callbackFn.call(vm);
        });
      }.bind(iScope),

      // Loads a React view.
      loadReactView: function(iTargetSelector, iReactClassName, iJsModuleUrl, iVmArg, iReactProps, callbackFn) {
        var vm = this;
        var createViewFunc = function() {
          if (!window.hasOwnProperty(iReactClassName)) {
            console.error('[' + vm.$vmId + "] failed to load view '" + iReactClassName + "' because it's not a React element.");
            return;
          }

          try {
            _ReactDOM.unmountComponentAtNode(document.querySelector(iTargetSelector));
          } catch (e) {
            console.error(e);
          }

          try {
            var reactElement = _React.createElement(window[iReactClassName], iReactProps);
            _ReactDOM.render(reactElement, document.querySelector(iTargetSelector));
          } catch (e) {
            console.error(e);
          }
          if (typeof callbackFn === 'function') callbackFn.call(vm, reactElement);
        };

        if (iJsModuleUrl == null) createViewFunc();
        else {
          // Load all javascripts first. Multiple files can be specified with comma delimiter.
          var getScripts = iJsModuleUrl.split(',').map(function(i) {
            return $.getScript(i);
          });
          $.when.apply($, getScripts).done(createViewFunc);
        }
      }.bind(iScope),

      // Routes to a path.
      manualRouteTo: function(iPath, iTarget, iViewUrl, iJSModuleUrl) {
        var template = { Id: 'manual', Target: iTarget, ViewUrl: iViewUrl, JSModuleUrl: iJSModuleUrl };
        this.$router.routeTo(iPath, template, true);
      }.bind(iScope),

      // Raise event indicating the routing process has ended.
      raiseRoutedEvent: function(force) {
        if (dotnetify.react.router.urlPath == '' || force == true) {
          if (dotnetify.debug) console.log('router> routed');
          utils.dispatchEvent('dotnetify.routed');
        }
      }.bind(iScope),

      // Routes to a path.
      routeTo: function(iPath, iTemplate, iDisableEvent, iCallbackFn) {
        const vm = this;
        const state = vm.State();
        const viewModels = vm.$lib.getViewModels();

        if (dotnetify.debug) console.log("router> route '" + iPath + "' to template id=" + iTemplate.Id);

        // We can determine whether the view has already been loaded by matching the 'RoutingState.Origin' argument
        // on the existing view model inside that target selector with the path.
        for (let i = 0; i < viewModels.length; i++) {
          var vmOther = viewModels[i];
          var vmArg = vmOther.Props('vmArg');
          if (vmArg != null) {
            if (typeof vmArg['RoutingState.Origin'] === 'string' && utils.equal(vmArg['RoutingState.Origin'], iPath)) return;
          }
        }

        // Support enter interception.
        if (iDisableEvent != true && vm.hasOwnProperty('onRouteEnter')) if (vm.onRouteEnter(iPath, iTemplate) == false) return;

        // Check if the route has valid target.
        if (iTemplate.Target === null) {
          console.error("router> the Target for template '" + iTemplate.Id + "' was not set.  Use vm.onRouteEnter() to set the target.");
          return;
        }

        // If target DOM element isn't found, redirect URL to the path.
        if (document.getElementById(iTemplate.Target) == null) {
          if (dotnetify.debug) console.log("router> target '" + iTemplate.Target + "' not found in DOM, use redirect instead");

          return dotnetify.react.router.redirect(vm.$router.toUrl(iPath), viewModels);
        }

        // Load the view associated with the route asynchronously.
        var view = iTemplate.ViewUrl ? iTemplate.ViewUrl : iTemplate.Id;
        vm.$router.loadView(
          '#' + iTemplate.Target,
          view,
          iTemplate.JSModuleUrl,
          { 'RoutingState.Origin': iPath },
          function() {
            // If load is successful, update the active route.
            state.RoutingState.Active = iPath;
            vm.$dispatch({ 'RoutingState.Active': iPath });
            vm.State({ 'RoutingState.Active': iPath });

            // Support exit interception.
            if (iDisableEvent != true && vm.hasOwnProperty('onRouteExit')) vm.onRouteExit(iPath, iTemplate);

            if (typeof iCallbackFn === 'function') iCallbackFn.call(vm);
          }.bind(vm)
        );
      }.bind(iScope),

      // Routes the URL if the view model implements IRoutable.
      // Returns true if the view model handles the routing.
      routeUrl: function() {
        var vm = this;
        var state = vm.State();
        if (!state.hasOwnProperty('RoutingState')) return false;

        var root = state.RoutingState.Root;
        if (root == null) return false;

        // Get the URL path to route.
        var urlPath = dotnetify.react.router.urlPath;

        if (dotnetify.debug) console.log('router> routing ' + urlPath);

        // If the URL path matches the root path of this view, use the template with a blank URL pattern if provided.
        if (utils.equal(urlPath, root) || utils.equal(urlPath, root + '/') || urlPath === '/') {
          var match = utils.grep(state.RoutingState.Templates, function(iTemplate) {
            return iTemplate.UrlPattern === '';
          });
          if (match.length > 0) {
            vm.$router.routeTo('', match[0]);
            dotnetify.react.router.urlPath = '';
            vm.$router.raiseRoutedEvent();
            return true;
          }
          return false;
        }

        // If the URL path starts with the root path of this view, look at the next path and try to match it with the
        // anchor tags in this view that are bound with the vmRoute binding type.  If there is match, route to that path.
        root = root + '/';
        if (utils.startsWith(urlPath, root)) {
          var routeElem = null;
          var match = utils.grep(vm.$router.routes, function(elem) {
            return utils.startsWith(urlPath + '/', elem.Url + '/');
          });
          if (match.length > 0) {
            // If more than one match, find the best match.
            for (var i = 0; i < match.length; i++)
              if (routeElem == null || routeElem.Url.length < match[i].Url.length) routeElem = match[i];
          }

          if (routeElem != null) {
            var path = routeElem.Path;
            var template =
              vm.$router.hasOwnProperty('pathToRoute') && vm.$router.pathToRoute.hasOwnProperty(path)
                ? vm.$router.pathToRoute[path].$template
                : null;
            if (template != null) {
              // If the URL path is completely routed, clear it.
              if (utils.equal(dotnetify.react.router.urlPath, vm.$router.toUrl(path))) dotnetify.react.router.urlPath = '';

              // If route's not already active, route to it.
              if (!utils.equal(state.RoutingState.Active, path))
                vm.$router.routeTo(path, template, false, function() {
                  vm.$router.raiseRoutedEvent();
                });
              else vm.$router.raiseRoutedEvent();
              return true;
            }
          }
          else if (dotnetify.react.router.match(urlPath)) {
            // If no vmRoute binding matches, try to match with any template's URL pattern.
            dotnetify.react.router.urlPath = '';
            vm.$router.raiseRoutedEvent();
            return true;
          }
        }
        return false;
      }.bind(iScope),

      // Builds an absolute URL from a path.
      toUrl: function(iPath) {
        var state = this.State();
        var path = utils.trim(iPath);
        if (path.charAt(0) != '(' && path != '') path = '/' + path;
        return state.hasOwnProperty('RoutingState') ? state.RoutingState.Root + path : iPath;
      }.bind(iScope)
    };
  })(iVM);

  // Returns the URL for an anchor tag.
  iVM.$route = function(iRoute, iTarget) {
    var vm = this;
    var state = vm.State();

    // No route to process. Return silently.
    if (iRoute == null) return;

    if (!iRoute.hasOwnProperty('Path') || !iRoute.hasOwnProperty('TemplateId')) throw new Error('Not a valid route');

    // Build the absolute root path.
    vm.$router.initRoot();

    // If the route path is not defined, use the URL pattern from the associated template.
    // This is so that we don't send the same data twice if both are equal.
    var path = iRoute.Path;
    var template = null;
    if (state.hasOwnProperty('RoutingState') && state.RoutingState.Templates != null) {
      var match = state.RoutingState.Templates.filter(function(iTemplate) {
        return iTemplate.Id == iRoute.TemplateId;
      });
      if (match.length > 0) {
        template = match[0];

        if (typeof iTarget === 'string') template.Target = iTarget;

        if (path == null) {
          path = template.UrlPattern != null ? template.UrlPattern : template.Id;
          iRoute.Path = path;
        }
      }
      else if (iRoute.RedirectRoot == null) throw new Error("vmRoute cannot find route template '" + iRoute.TemplateId);
    }

    // If the path has a redirect root, the path doesn't belong to the current root and needs to be
    // redirected to a different one.  Set the absolute path to the HREF attribute, and prevent the
    // default behavior of the anchor click event and instead do push to HTML5 history state, which
    // would attempt to resolve the path first before resorting to hard browser redirect.
    if (iRoute.RedirectRoot != null) {
      // Combine the redirect root with the view model's root.
      var redirectRoot = iRoute.RedirectRoot;
      if (redirectRoot.charAt(0) == '/') redirectRoot = redirectRoot.substr(0, redirectRoot.length - 1);
      var redirectRootPath = iRoute.RedirectRoot.split('/');

      var urlRedirect = '';
      var absRoot = vm.Props('vmRoot');
      if (absRoot != null) {
        var absRootPath = absRoot.split('/');
        for (var i = 0; i < absRootPath.length; i++) {
          if (absRootPath[i] != '' && absRootPath[i] == redirectRootPath[0]) break;
          urlRedirect += absRootPath[i] + '/';
        }
      }
      urlRedirect += redirectRoot + '/' + path;
      vm.$router.routes.push({ Path: path, Url: urlRedirect });
      return urlRedirect;
    }

    // For quick lookup, save the mapping between the path to the route inside the view model.
    if (template == null) throw new Error('vmRoute cannot find any route template');

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
  iVM.$handleRoute = function(iEvent) {
    iEvent.preventDefault();
    var path = iEvent.currentTarget.pathname;
    if (path == null || path == '') throw new Error('The event passed to $handleRoute has no path name.');

    setTimeout(function() {
      dotnetify.react.router.pushState({}, '', path);
    }, 0);
  }.bind(iVM);

  // Executes the given route.
  iVM.$routeTo = function(iRoute) {
    var path = iVM.$route(iRoute);
    if (path == null || path == '') throw new Error('The route passed to $routeTo is invalid.');

    setTimeout(function() {
      dotnetify.react.router.pushState({}, '', path);
    }, 0);
  }.bind(iVM);
};

// Register the plugin to dotNetify.
dotnetify.react.plugins['router'] = dotnetify.react.router;
