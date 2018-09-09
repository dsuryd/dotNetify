import utils from './utils';

export default class dotnetifyRouter {
  routes = [];
  vm = null;
  debug = false;

  static $init = false;
  static urlPath = document.location.pathname;

  constructor(vm) {
    this.vm = vm;
  }

  // Build the absolute root path from the "vmRoot" property on React component.
  initRoot() {
    const vm = this.vm;
    const state = vm.State();
    if (!state.hasOwnProperty('RoutingState') || state.RoutingState === null || state.RoutingState.Root === null) return;

    if (this._absRoot != state.RoutingState.Root) {
      const vmRoot = vm.Props('vmRoot');
      let absRoot = utils.trim(vmRoot);
      if (absRoot != '') absRoot = '/' + absRoot;
      const root = utils.trim(state.RoutingState.Root);
      this._absRoot = root != '' ? absRoot + '/' + root : absRoot;
      state.RoutingState.Root = this._absRoot;
    }
  }

  initRouting() {
    const vm = this.vm;
    const state = vm.State();
    if (state == null || !state.hasOwnProperty('RoutingState')) return;

    if (state.RoutingState === null) {
      console.error("router> the RoutingState prop of '" + vm.$vmId + "' was not initialized.");
      return;
    }

    const templates = state.RoutingState.Templates;
    if (templates == null || templates.length == 0) return;

    // Initialize the router.
    dotnetifyRouter.init();

    // Build the absolute root path.
    this.initRoot();

    templates.forEach(template => {
      // If url pattern isn't given, consider Id as the pattern.
      var urlPattern = template.UrlPattern != null ? template.UrlPattern : template.Id;
      urlPattern = urlPattern != '' ? urlPattern : '/';
      var mapUrl = this.toUrl(urlPattern);

      if (this.debug) console.log('router> map ' + mapUrl + ' to template id=' + template.Id);

      this.mapTo(mapUrl, function(iParams) {
        this.urlPath = '';

        // Construct the path from the template pattern and the params passed by PathJS.
        let path = urlPattern;
        for (const param in iParams) path = path.replace(':' + param, iParams[param]);
        path = path.replace(/\(\/:([^)]+)\)/g, '').replace(/\(|\)/g, '');

        this.routeTo(path, template);
      });
    });

    // Route initial URL.
    var activeUrl = this.toUrl(state.RoutingState.Active);
    if (this.urlPath == '') this.urlPath = activeUrl;
    if (!this.routeUrl())
      // If routing ends incomplete, raise routed event anyway.
      this.raiseRoutedEvent(true);
  }

  // Routes to a path.
  routeTo(iPath, iTemplate, iDisableEvent, iCallbackFn) {
    var vm = this;
    var state = vm.State();

    if (dotnetify.debug) console.log("router> route '" + iPath + "' to template id=" + iTemplate.Id);

    // We can determine whether the view has already been loaded by matching the 'RoutingState.Origin' argument
    // on the existing view model inside that target selector with the path.
    for (var vmId in dotnetify.react.viewModels) {
      var vmOther = dotnetify.react.viewModels[vmId];
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

      return dotnetify.react.router.redirect(vm.$router.toUrl(iPath));
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
  }

  // Builds an absolute URL from a path.
  toUrl(iPath) {
    const state = this.vm.State();
    var path = utils.trim(iPath);
    if (path.charAt(0) != '(' && path != '') path = '/' + path;
    return state.hasOwnProperty('RoutingState') ? state.RoutingState.Root + path : iPath;
  }

  // Initialize routing using PathJS.
  static init() {
    if (dotnetifyRouter.$init) return;

    if (typeof Path !== 'undefined') {
      Path.history.listen(true);
      Path.routes.rescue = function() {
        //window.location.replace(document.location.pathname);
      };
      dotnetifyRouter.$init = true;
    }
    else throw new Error('Pathjs library is required for routing.');
  }

  // Map a route to an action.
  static mapTo(iPath, iFn) {
    if (typeof Path !== 'undefined')
      Path.map(iPath).to(function() {
        iFn(this.params);
      });
  }
}
