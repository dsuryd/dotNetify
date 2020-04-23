/* 
Copyright 2017-2018 Dicky Suryadi

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
import utils from '../libs/utils';
import $ from '../libs/jquery-shim';

export default class dotnetifyVMRouter {
  routes = [];

  get RoutingState() {
    throw new Error('Not implemented');
  }
  get VMRoot() {
    throw new Error('Not implemented');
  }
  get VMArg() {
    throw new Error('Not implemented');
  }

  constructor(vm, router) {
    this.vm = vm;
    this.router = router;
    this.debug = vm.$dotnetify.controller.debug;

    // Handle 'onRouteEnter' callback being given in the VM options.
    if (vm.$options && vm.$options.onRouteEnter) vm.onRouteEnter = vm.$options.onRouteEnter;
  }

  // Dispatch the active routing state to the server.
  dispatchActiveRoutingState(iPath) {
    this.vm.$dispatch({ 'RoutingState.Active': iPath });
    let { RoutingState } = this.vm.State();
    RoutingState = Object.assign(RoutingState || {}, { Active: iPath });
    this.vm.State({ RoutingState });
  }

  // Handles click event from anchor tags.  Argument can be event object or path string.
  handleRoute(iArg) {
    let path = null;
    if (typeof iArg === 'object') {
      iArg.preventDefault();
      path = iArg.currentTarget.pathname;
    }
    else if (typeof iArg === 'string') path = iArg;

    if (path == null || path == '') throw new Error('$handleRoute requires path argument or event with pathname.');
    setTimeout(() => this.router.pushState({}, '', path));
  }

  // Build the absolute root path from the "vmRoot" property on React component.
  initRoot() {
    if (!this.hasRoutingState || this.RoutingState === null || this.RoutingState.Root === null) return;

    if (this._absRoot != this.RoutingState.Root) {
      var absRoot = utils.trim(this.VMRoot);
      if (absRoot != '') absRoot = '/' + absRoot;
      var root = utils.trim(this.RoutingState.Root);
      this._absRoot = root != '' ? absRoot + '/' + root : absRoot;
      this.RoutingState.Root = this._absRoot;
    }
  }

  // Initialize routing templates if the view model implements IRoutable.
  initRouting() {
    const vm = this.vm;
    if (!this.hasRoutingState) return;

    if (this.RoutingState === null) {
      console.error("router> the RoutingState prop of '" + vm.$vmId + "' was not initialized.");
      return;
    }

    const templates = this.RoutingState.Templates;
    if (templates == null || templates.length == 0) return;

    // Initialize the router.
    if (!this.router.$init) {
      this.router.init();
      this.router.$init = true;
    }

    // Build the absolute root path.
    this.initRoot();

    templates.forEach(template => {
      // If url pattern isn't given, consider Id as the pattern.
      let urlPattern = template.UrlPattern != null ? template.UrlPattern : template.Id;
      urlPattern = urlPattern != '' ? urlPattern : '/';
      const mapUrl = this.toUrl(urlPattern);

      if (this.debug) console.log('router> map ' + mapUrl + ' to template id=' + template.Id);

      this.router.mapTo(mapUrl, iParams => {
        this.router.urlPath = '';

        // Construct the path from the template pattern and the params passed by PathJS.
        let path = urlPattern;
        for (var param in iParams) path = path.replace(':' + param, iParams[param]);
        path = path.replace(/\(\/:([^)]+)\)/g, '').replace(/\(|\)/g, '');

        this.routeTo(path, template);
      });
    });

    // Route initial URL.
    let activeUrl = this.toUrl(this.RoutingState.Active);
    if (this.router.urlPath == '') this.router.urlPath = activeUrl;
    if (!this.routeUrl())
      // If routing ends incomplete, raise routed event anyway.
      this.raiseRoutedEvent(true);
  }

  // Whether a route is active.
  isActive(iRoute) {
    if (iRoute != null && iRoute.hasOwnProperty('Path')) {
      return utils.equal(iRoute.Path, this.RoutingState.Active);
    }
    return false;
  }

  // Loads an HTML view.
  loadHtmlView(iTargetSelector, iViewUrl, iJsModuleUrl, iCallbackFn) {
    const vm = this.vm;
    this.unmountView(iTargetSelector);

    // Load the HTML view.
    $(iTargetSelector).load(iViewUrl, null, function() {
      if (iJsModuleUrl != null) {
        const getScripts = iJsModuleUrl.split(',').map(i => $.getScript(i));
        $.when.apply($, getScripts).done(() => typeof callbackFn === 'function' && iCallbackFn.call(vm));
      }
      else if (typeof callbackFn === 'function') iCallbackFn.call(vm);
    });
  }

  loadHtmlElementView(iTargetSelector, iHtmlElement, iJsModuleUrl, iVmArg, iCallbackFn) {
    const vm = this.vm;
    const mountViewFunc = () => {
      this.unmountView(iTargetSelector);

      const target = document.querySelector(iTargetSelector);
      while (target.firstChild) target.removeChild(target.firstChild);
      target.appendChild(iHtmlElement);

      if (typeof callbackFn === 'function') iCallbackFn.call(vm);
    };

    if (iJsModuleUrl == null) mountViewFunc();
    else {
      // Load all javascripts first. Multiple files can be specified with comma delimiter.
      const getScripts = iJsModuleUrl.split(',').map(i => $.getScript(i));
      $.when.apply($, getScripts).done(mountViewFunc);
    }
  }

  // Loads a view into a target element.
  loadView(iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, iCallbackFn) {
    throw new Error('Not implemented');
  }

  // Routes to a path.
  manualRouteTo(iPath, iTarget, iViewUrl, iJSModuleUrl) {
    const vm = this.vm;
    var template = { Id: 'manual', Target: iTarget, ViewUrl: iViewUrl, JSModuleUrl: iJSModuleUrl };
    this.routeTo(iPath, template, true);
  }

  // Handles route enter event.
  onRouteEnter(iPath, iTemplate) {
    return true;
  }

  // Raise event indicating the routing process has ended.
  raiseRoutedEvent(force) {
    const vm = this.vm;
    if (this.router.urlPath == '' || force == true) {
      if (this.debug) console.log('router> routed');
      this.router.routedEvent.emit();
    }
  }

  // Returns the URL for an anchor tag.
  route(iRoute, iTarget) {
    // No route to process. Return silently.
    if (iRoute == null) return;

    if (!iRoute.hasOwnProperty('Path') && !iRoute.hasOwnProperty('TemplateId')) throw new Error('Not a valid route');

    // Build the absolute root path.
    this.initRoot();

    // If the route path is not defined, use the URL pattern from the associated template.
    // This is so that we don't send the same data twice if both are equal.
    let path = iRoute.Path;
    let template = null;
    if (this.hasRoutingState && this.RoutingState.Templates != null && iRoute.TemplateId != null) {
      let match = this.RoutingState.Templates.filter(function(iTemplate) {
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
      else if (iRoute.RedirectRoot == null) throw new Error(`vmRoute cannot find route template ${iRoute.TemplateId}`);
    }

    // If the path has a redirect root, the path doesn't belong to the current root and needs to be
    // redirected to a different one.  Set the absolute path to the HREF attribute, and prevent the
    // default behavior of the anchor click event and instead do push to HTML5 history state, which
    // would attempt to resolve the path first before resorting to hard browser redirect.
    if (iRoute.RedirectRoot != null) {
      // Combine the redirect root with the view model's root.
      let redirectRoot = iRoute.RedirectRoot;
      if (redirectRoot.charAt(0) == '/') redirectRoot = redirectRoot.substr(0, redirectRoot.length - 1);
      let redirectRootPath = iRoute.RedirectRoot.split('/');

      let urlRedirect = '';
      let absRoot = this.VMRoot;
      if (absRoot != null) {
        let absRootPath = absRoot.split('/');
        for (let i = 0; i < absRootPath.length; i++) {
          if (absRootPath[i] != '' && absRootPath[i] == redirectRootPath[0]) break;
          urlRedirect += absRootPath[i] + '/';
        }
      }

      urlRedirect += redirectRoot + '/' + path;
      urlRedirect = urlRedirect.replace(/\/\/+/g, '/');
      if (!this.routes.some(x => x.Path === path)) this.routes.push({ Path: path, Url: urlRedirect });
      return urlRedirect;
    }

    // For quick lookup, save the mapping between the path to the route inside the view model.
    if (template == null) throw new Error('vmRoute cannot find any route template');

    iRoute.$template = template;
    this.pathToRoute = this.pathToRoute || {};
    this.pathToRoute[path] = iRoute;

    // Set the absolute path to the HREF attribute, and prevent the default behavior of
    // the anchor click event and instead do push to HTML5 history state.
    let url = this.toUrl(path);
    url = url.length > 0 ? url : '/';
    if (!this.routes.some(x => x.Path === path)) this.routes.push({ Path: path, Url: url });
    return url;
  }

  // Routes to a path.
  routeTo(iPath, iTemplate, iDisableEvent, iCallbackFn, isRedirect) {
    const vm = this.vm;
    const viewModels = vm.$dotnetify.getViewModels();

    if (this.debug) console.log("router> route '" + iPath + "' to template id=" + iTemplate.Id);

    // We can determine whether the view has already been loaded by matching the 'RoutingState.Origin' argument
    // on the existing view model inside that target selector with the path.
    const pathUrl = this.toUrl(iPath);
    for (let i = 0; i < viewModels.length; i++) {
      let vmOther = viewModels[i];
      let vmArg = vmOther.$router.VMArg;
      if (vmArg != null) {
        if (typeof vmArg['RoutingState.Origin'] === 'string' && utils.equal(vmArg['RoutingState.Origin'], pathUrl)) return;
      }
    }

    const activateRoute = () => {
      // Check if the route has valid target.
      //iTemplate.Target = iTemplate.Target || vm.$options.routeTarget;
      if (iTemplate.Target == null) {
        console.error("router> the Target for template '" + iTemplate.Id + "' was not set.  Use vm.onRouteEnter() to set the target.");
        return;
      }

      // If target DOM element isn't found, redirect URL to the path.
      if (document.getElementById(iTemplate.Target) == null) {
        if (isRedirect === true) {
          if (this.debug) console.log("router> target '" + iTemplate.Target + "' not found in DOM");
          return;
        }
        else {
          if (this.debug) console.log("router> target '" + iTemplate.Target + "' not found in DOM, use redirect instead");
          return this.router.redirect(this.toUrl(iPath), [ ...viewModels, ...vm.$dotnetify.controller.getViewModels() ]);
        }
      }

      // Load the view associated with the route asynchronously.
      this.loadView('#' + iTemplate.Target, iTemplate.ViewUrl, iTemplate.JSModuleUrl, { 'RoutingState.Origin': iPath }, () => {
        // If load is successful, update the active route.
        this.dispatchActiveRoutingState(iPath);

        // Support exit interception.
        if (iDisableEvent != true && vm.hasOwnProperty('onRouteExit')) vm.onRouteExit(iPath, iTemplate);

        if (typeof iCallbackFn === 'function') iCallbackFn.call(vm);
      });
    };

    // Support enter interception.
    if (iDisableEvent != true && vm.hasOwnProperty('onRouteEnter')) {
      if (this.onRouteEnter(iPath, iTemplate) === false) return;

      const result = vm.onRouteEnter(iPath, iTemplate);
      if (result === false) return;
      else if (result && typeof result.then == 'function') {
        // If returning a promise, wait until it's resolved.
        result.then(res => res !== false && activateRoute());
        return;
      }
    }

    activateRoute();
  }

  routeToRoute(iRoute) {
    var path = this.vm.$route(iRoute);
    if (path == null || path == '') throw new Error('The route passed to $routeTo is invalid.');

    setTimeout(() => this.router.pushState({}, '', path));
  }

  // Routes the URL if the view model implements IRoutable.
  // Returns true if the view model handles the routing.
  routeUrl(redirectUrlPath) {
    if (!this.hasRoutingState) return false;

    const isRedirect = !!redirectUrlPath;
    let root = this.RoutingState.Root;
    if (root == null) return false;

    // Get the URL path to route.
    let urlPath = isRedirect ? redirectUrlPath : this.router.urlPath;

    if (this.debug) console.log('router> routing ' + urlPath);

    // If the URL path matches the root path of this view, use the template with a blank URL pattern if provided.
    if (utils.equal(urlPath, root) || utils.equal(urlPath, root + '/') || urlPath === '/') {
      let match = utils.grep(this.RoutingState.Templates, function(iTemplate) {
        return iTemplate.UrlPattern === '';
      });
      if (match.length > 0) {
        this.routeTo('', match[0], null, null, isRedirect);
        this.router.urlPath = '';
        this.raiseRoutedEvent();
        return true;
      }
      return false;
    }

    // If the URL path starts with the root path of this view, look at the next path and try to match it with the
    // anchor tags in this view that are bound with the vmRoute binding type.  If there is match, route to that path.
    root = root + '/';
    if (utils.startsWith(urlPath, root)) {
      let routeElem = null;
      let match = utils.grep(this.routes, function(elem) {
        return utils.startsWith(urlPath + '/', elem.Url + '/');
      });
      if (match.length > 0) {
        // If more than one match, find the best match.
        for (let i = 0; i < match.length; i++) if (routeElem == null || routeElem.Url.length < match[i].Url.length) routeElem = match[i];
      }

      if (routeElem != null) {
        let path = routeElem.Path;
        let template =
          this.hasOwnProperty('pathToRoute') && this.pathToRoute.hasOwnProperty(path) ? this.pathToRoute[path].$template : null;
        if (template != null) {
          // If the URL path is completely routed, clear it.
          if (utils.equal(this.router.urlPath, this.toUrl(path))) this.router.urlPath = '';

          // If route's not already active, route to it.
          if (!utils.equal(this.RoutingState.Active, path)) {
            this.routeTo(path, template, false, () => this.raiseRoutedEvent(), isRedirect);
          }
          else this.raiseRoutedEvent();
          return true;
        }
      }
      else if (this.router.match(urlPath)) {
        // If no vmRoute binding matches, try to match with any template's URL pattern.
        this.router.urlPath = '';
        this.raiseRoutedEvent();
        return true;
      }
    }
    return false;
  }

  // Builds an absolute URL from a path.
  toUrl(iPath) {
    let path = utils.trim(iPath);
    if (path.charAt(0) != '(' && path != '') path = '/' + path;
    return this.hasRoutingState ? this.RoutingState.Root + path : iPath;
  }

  // Unmount a view if there's one on the target selector.
  unmountView(iTargetSelector) {
    throw new Error('Not implemented');
  }
}
