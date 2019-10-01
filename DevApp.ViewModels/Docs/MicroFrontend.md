## Micro-Frontend

Micro-frontend is a technique that decomposes an otherwise monolithic web application into smaller, isolated, and independently deployable sub-applications.  Each sub-application provides a subset of the UI, which will then be stitched together by a light orchestrator to present a single, cohesive front-end to its users.

While this technique does increase development complexity and brings its own challenges, many large organizations find merits to this approach, as it allows them to better scale their development and deployment process across autonomous, geographically-dispersed teams.

There are many different ways in implementing micro-frontend. What dotNetify proposes is closely aligned to the microservices approach, where aspects of functionality is encapsulated within independent web services (dotNetify on ASP.NET Core applications), each with its own build process and deployment pipeline.  The domain-driven design approach can be employed here on deciding how to slice the problem domain into these distinct services.  

The diagram below depicts the proposed architecture:
[inset]

The entry to the application is provided by what is referred to as a portal service.  This service is responsible for performing front-end orchestration, which involves:
- discovering the app services,
- loading the static UI script bundles dynamically, 
- instantiating and composing their root UI components into a single-page application with client-side routing (can be done with dotNetify routing system), 
- and redirecting the network traffic to the appropriate service address.  

The portal service is also responsible for authentication/authorization.  It communicates with an identity provider to generate security tokens, and intercept outgoing service requests to injects the tokens into the auth header. In production environment, these services should be put behind an API gateway to secure them, and only expose a single entry point into the system.

#### Reference Implementation

A simple reference implementation covering the application layer of the above architecture is provided in the `DotNetify.React.Template` nuget:

```js
dotnet new -i dotnetify.react.template

dotnet new dotnetify-mfe -o MyApp
// To run the app, follow the instructions in README.md
```
<br/>

##### App Services

Each app service is an ASP.NET Core web application that's been configured to build the UI scripts using Webpack when the application is compiled (dev build on _dotnet run_, prod build on _dotnet publish_).  The main UI script that serves as the Webpack's entry point is made to provide a function that will return the root UI component as a native Web Component. This will allow the portal service that will load the app's UI script bundle to create and mount the app's root component in a framework-agnostic manner.

The following is a sample implementation of the main script.  The _createWebComponent_ function is provided by `dotNetify-Elements` to wrap the React component inside a standard HTML custom element:

```jsx
import { createWebComponent } from 'dotnetify-elements/web-components/core';
import MyApp from './components/MyApp';

const elementName = 'app1-element';
createWebComponent(TodoList, elementName);

export default () => document.createElement(elementName);
```
<br/>

##### Portal Service

The portal service is also an ASP.NET Core web application combined with Webpack.  The UI script contains a function called `loader` that will be executed as soon as the portal starts.  This function is given a list containing information on all the app services, including their addresses.  For demo purpose, this list is hardcoded.   

```jsx
loader(
  [
    {
      id: 'app1',
      label: 'App 1',
      routePath: 'app1',
      baseUrl: '//localhost:8080/app1',
      moduleUrl: '/dist/app.js'
    },
    ...
  ],
  // External dependencies required by the apps.
  [ 'dotnetify', 'dotNetifyElements' ]
);
```

The `loader` function will ping each app service addresses, and on getting a response, use `SystemJS` module loader to load the app, and pass the module function that creates the app's root component to an object that manages and displays these components.

```jsx
function importApp(app) {
  const appUrl = app.baseUrl + app.moduleUrl;
  return SystemJS.import(appUrl)
    .then(module => module.default && updatePortal({ ...app, rootComponent: module.default }));
}
```

When the app's root component is mounted, it will communicate to its own hub server.  Since the address is relative, the portal needs to intercept every connect request and redirect it to the correct address.  The portal can do this by implementing __dotnetify.connectHandler__ and use the special identifier that each app will need to provide when its component makes the _connect_ call to forward the request appropriately:

```jsx
dotnetify.connectHandler = vmConnectArgs => {
  const appId = vmConnectArgs.options && vmConnectArgs.options.appId;
  const app = apps.find(x => x.id === appId);
  if (app) {
    app.hub = app.hub || dotnetify.createHub(app.baseUrl);
    return {
      ...vmConnectArgs,
      hub: app.hub,
      options: { ...vmConnectArgs.options, headers: { Authorization: 'Bearer ' + getAccessToken() } }
    };
  }
};
```

Along with the hub change, it too injects the access token to the _connect_ request.  The service's back-end is set up to get the JWT signing keys from the identity provider (`IdentityServer4`) to authenticate the token. 

Finally, the portal uses the Nav component from `dotNetify-Elements` to dynamically build and run the client-side routing.  Any app that use dotNetify routing system will be able to integrate and nest their routes within the portal's root path.

##### Shared UI Component Library

With each micro-frontend app being developed and deployed independently, naturally there are concerns over visual inconsistencies, and ballooning size due to duplicated dependencies.  This can be mitigated by agreeing to use a shared set of core UI components as basic building blocks for the app's more domain-specific components.  This UI library will then be declared external when bundling the apps, and to load the module during the portal's initialization so it can be made available to all apps.

The reference implementation uses `dotNetify-Elements` for this common library.  While it's built with React, it exposes the same components as HTML custom elements, making them reusable in other non-React frameworks.
