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
import dotnetify from './dotnetify-ko';
import dotnetifyRouter from '../core/dotnetify-router';
import dotnetifyKoVMRouter from './dotnetify-ko.vm-router';
import $ from 'jquery';
import * as ko from 'knockout';

// Add plugin functions.
dotnetify.ko.router = new dotnetifyRouter(dotnetify.debug);

// Inject a view model with functions.
dotnetify.ko.router.$inject = function(iVM) {
  const router = new dotnetifyKoVMRouter(iVM, dotnetify.ko.router);

  // Put functions inside $router namespace.
  iVM.$router = router;
};

// Custom knockout binding to do routing.
ko.bindingHandlers.vmRoute = {
  update: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
    var vm = bindingContext.$root;
    var route = ko.unwrap(valueAccessor());

    if (!route.hasOwnProperty('Path') || !route.hasOwnProperty('TemplateId'))
      throw new Error('invalid vmRoute data at ' + element.outerHTML);

    // Build the absolute root path.
    vm.$router.initRoot();

    // If the route path is not defined, use the URL pattern from the associated template.
    // This is so that we don't send the same data twice if both are equal.
    let path = route.Path();
    var template = null;
    if (vm.hasOwnProperty('RoutingState') && typeof vm.RoutingState.Templates === 'function' && vm.RoutingState.Templates() != null) {
      var match = $.grep(vm.RoutingState.Templates(), function(iTemplate) {
        return iTemplate.Id() == route.TemplateId();
      });
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
      if (redirectRoot.charAt(0) == '/') redirectRoot = redirectRoot.substr(0, redirectRoot.length - 1);
      var redirectRootPath = route.RedirectRoot().split('/');

      var url = '';
      var absRoot = vm.$element.attr('data-vm-root');
      if (absRoot != null) {
        var absRootPath = absRoot.split('/');
        for (i = 0; i < absRootPath.length; i++) {
          if (absRootPath[i] != '' && absRootPath[i] == redirectRootPath[0]) break;
          url += absRootPath[i] + '/';
        }
      }
      url += redirectRoot + '/' + path;

      $(element).attr('href', url).attr('data-vm-route', path).click(function(iEvent) {
        iEvent.preventDefault();
        dotnetify.ko.router.pushState({}, '', $(this).attr('href'));
      });
      return;
    }

    // For quick lookup, save the mapping between the path to the route inside the view model.
    if (template == null) throw new Error('vmRoute cannot find any route template at ' + element.outerHTML);

    route.$template = template;
    vm.$router.pathToRoute = vm.$router.pathToRoute || {};
    vm.$router.pathToRoute[path] = route;

    // Set the absolute path to the HREF attribute, and prevent the default behavior of
    // the anchor click event and instead do push to HTML5 history state.
    $(element).attr('href', vm.$router.toUrl(path)).attr('data-vm-route', path).click(function(iEvent) {
      iEvent.preventDefault();
      dotnetify.ko.router.pushState({}, '', $(this).attr('href'));
    });
  }
};

// Register the plugin to dotNetify.
dotnetify.ko.plugins['router'] = dotnetify.ko.router;
