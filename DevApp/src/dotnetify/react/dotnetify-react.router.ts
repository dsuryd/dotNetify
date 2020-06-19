/* 
Copyright 2017-2020 Dicky Suryadi

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
import dotnetify from "./dotnetify-react";
import DotnetifyRouter from "../core/dotnetify-router";
import DotnetifyReactVMRouter from "./dotnetify-react.vm-router";
import DotnetifyVM from "../core/dotnetify-vm";

// Add plugin functions.
dotnetify.react.router = new DotnetifyRouter(dotnetify.debug);

// Inject a view model with functions.
dotnetify.react.router.$inject = function (iVM: DotnetifyVM) {
  const router = new DotnetifyReactVMRouter(iVM, dotnetify.react.router);
  const vm: any = iVM;

  // Put functions inside $router namespace.
  vm.$router = router;

  // Handles click event from anchor tags.  Argument can be event object or path string.
  vm.$handleRoute = iArg => router.handleRoute(iArg);

  // Returns the URL for an anchor tag.
  vm.$route = (iRoute, iTarget) => router.route(iRoute, iTarget);

  // Executes the given route.
  vm.$routeTo = iRoute => router.routeToRoute(iRoute);
};

// Provide function to load a view.
dotnetify.react.router.$mount = function (
  iTargetSelector: string,
  iComponent: any,
  iProps?: any,
  iCallbackFn?: Function
) {
  return DotnetifyReactVMRouter.prototype.loadReactView(
    iTargetSelector,
    iComponent,
    null,
    iProps,
    iCallbackFn
  );
};

// Register the plugin to dotNetify.
dotnetify.react.plugins["router"] = dotnetify.react.router;
