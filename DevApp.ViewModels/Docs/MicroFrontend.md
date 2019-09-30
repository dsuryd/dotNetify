## Micro-Frontend

Micro-frontend is a technique that decomposes an otherwise monolithic web application into smaller, isolated, and independently deployable sub-applications.  Each sub-application provides a subset of the UI, which will then be stitched together by a light orchestrator to present a single, cohesive front-end to its users.

While this technique does increase development complexity and brings its own challenges, many large organizations find merits to this approach, as it allows them to better scale their development and deployment process across autonomous, geographically-dispersed teams.

There are many different ways in implementing micro-frontend. What dotNetify proposes is closely aligned to the microservices approach, where aspects of functionality is encapsulated within independent web services (dotNetify on ASP.NET Core applications), each with its own build process and deployment pipeline.  The domain-driven design approach can be employed here on deciding how to slice the problem domain into these distinct services.

The entry to the application is provided by a portal service that performs the front-end orchestration.  That process involves discovering the services, then dynamically loading the static UI script bundles from the running web services, instantiating and composing their root UI components into a single-page application with client-side routing (can be provided by dotNetify), and redirecting the network traffic to appropriate service ports.  

The portal service also takes care of the authentication and authorization for the services, i.e. communicate with an identity server to generate security tokens, then inject the tokens through request interception.  In production environment, we want to put these services behind an API gateway to secure them and expose just a single entry point into the system.

Below is the diagram depicting the architecture:
[inset]

A simple reference implementation covering the application layer of this architecture is provided in the nuget:
```
dotnet new -i dotnetify.react.template

dotnet new dotnetify-mfe -o MyApp

// To run the app, follow the instructions in README.md
```

### App Services

Each app service is built from ASP.NET Core application that's been configured to build the UI scripts with Webpack on compile time (dev build on `dotnet run`, prod build on `dotnet publish`).  The main script that serves as the Webpack's entry point is designed to expose the root UI component as a native Web Component, instead of the normal mounting to a DOM tag.   This will allow the portal service later on to create the application's root component at will as a framework-agnostic HTML element and mount it anywhere it likes.

Here is a sample implementation of the main script.  For React, you can use the function provided by dotNetify-Elements to wrap your React component in a HTML Web Component:

```
import { createWebComponent } from 'dotnetify-elements/web-components/core';
import MyApp from './components/MyApp';

const elementName = 'my-app';
createWebComponent(TodoList, elementName);

export default document.createElement(elementName);
```

You can still run the app independently without the portal by having the custom element tag in the index.html.  

### Portal 

The portal service is similar to an app service, but has special logic to discover running app services, load the root UI components from them, and assemble them as an ordinary single-page application.  The demo simplifies the discovery process (for demonstration purpose) by having the service URLs hardcoded and periodically pinged.  When ther is a response, the portal uses SystemJS module loader to load the app's UI script bundle and execute the instantiation of its root UI component.

As each app's UI will be communicating with their own backend service, the portal will be responsible for making sure that the dotNetify connection in every app will go to the correct hub server.  This is done by having the portal intercept every dotNetify connect call by implementing the connectHandler function, and override the app's default hub proxy with a new one initialized to the known service address.

#### Dynamic Routing

The demo portal employs dotNetify-Elements Nav component and a local view model to dynamically build and apply the client-side routing.  Apps that use dotNetify routing system will be able to integrate and nest their routes within the portal's root path.

### Authentication

Authenticating users are the responsibility of the portal.  The demo portal provides a login page, and uses IdentityServer4 library to generate a security token on successful login.  Since the portal overrides the hub proxies of all apps, it can inject the token into every dotNetify connect call.  The app's back-end authenticates the token by calling the IdentityServer4 AP
