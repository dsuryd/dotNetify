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
import dotnetify from './dotnetify-react';
import dotnetifyRouter from '../core/dotnetify-router';
import dotnetifyReactVMRouter from './dotnetify-react.vm-router';

// Add plugin functions.
dotnetify.react.router = new dotnetifyRouter(dotnetify.debug);

// Inject a view model with functions.
dotnetify.react.router.$inject = function(iVM) {
  const router = new dotnetifyReactVMRouter(iVM, dotnetify.react.router);

  // Put functions inside $router namespace.
  iVM.$router = router;

  // Handles click event from anchor tags.  Argument can be event object or path string.
  iVM.$handleRoute = iArg => router.handleRoute(iArg);

  // Returns the URL for an anchor tag.
  iVM.$route = (iRoute, iTarget) => router.route(iRoute, iTarget);

  // Executes the given route.
  iVM.$routeTo = iRoute => router.routeToRoute(iRoute);
};

// Provide function to load a view.
dotnetify.react.router.$mount = function(iTargetSelector, iComponent, iProps, iCallbackFn) {
  return dotnetifyReactVMRouter.prototype.loadReactView(iTargetSelector, iComponent, null, iProps, iCallbackFn);
};

// Register the plugin to dotNetify.
dotnetify.react.plugins['router'] = dotnetify.react.router;
