## Routing

While you certainly can use standard client-side routing such as _React-Router_, dotNetify provides you with an option to declare all the routes in the back-end if you prefer to have your components decoupled from routing concerns.  DotNetify routing supports nesting, lazy-loading, and can be easily configured for server-side rendering.

#### Defining the Routes

Let's start with a simple _Index_ view that provides a nav-bar to navigate to Page1 and Page2. We'll define all the routes in the Index view model:

```csharp
public class Index : BaseVM, IRoutable
{
   public RoutingState RoutingState { get; set; }
   public Route LinkPage1 => this.GetRoute("Page1");
   public Route LinkPage2 => this.GetRoute("Page2");

   public Index()
   {
      this.RegisterRoutes("" /*base path*/, new List<RouteTemplate>
      {
         new RouteTemplate("Home")  { UrlPattern = "", ViewUrl = "Page1" },
         new RouteTemplate("Page1") { UrlPattern = "page1" },
         new RouteTemplate("Page2") { UrlPattern = "page2" },
      });
   }
}
```

Any view model involved with routing must implement __IRoutable__ interface from the namespace __DotNetify.Routing__. It only has one property, the __RoutingState__, and you only need to implement it as auto-property.

In the constructor, create the route templates for _Page1_ and _Page2_ and register them using the __RegisterRoutes__ API. You are required to specify the base path for all the routes.

The route template object, __RouteTemplate__ defines which component to render (e.g. _Page1_) when the route is activated, and maps a URL to a specific route.  It is assumed that the component name will match the template ID, but if you want to give it a different component name or even a plain HTML page, use the __ViewUrl__ property to set it.  The other property, __UrlPattern__, is used to specify the URL that will be matched to the template, and can accept parameterized URL, such as "page/:id".

The nav-bar will need links to the pages, so we add some properties that return the Route objects for _Page1_ and _Page2_. To get the object, use the __GetRoute__ API, which will find it from the list of registered routes.

On the client side, add the components to the _window_ variable during your application's bootstrap: 
```jsx
import Page1 from './page1';
import Page2 from './page2';

Object.assign(window, { Page1, Page2 });
```

#### Setting Up Route Links

Now that we have our routes all defined in the back-end, this is how they are set up in the _Index_ component:

```jsx
import React from 'react';
import dotnetify, { RouteLink, RouteTarget } from 'dotnetify';

const onRouteEnter = (path, template) => (template.Target = "Content");

export const Index = props => {
  const { vm, state } = useConnect("Index", { props }, { onRouteEnter });

  return (
    <div>
        <RouteLink vm={vm} route={state.LinkPage1}>Page 1</RouteLink>
        <RouteLink vm={vm} route={state.LinkPage2}>Page 2</RouteLink>
        <RouteTarget id="Content" />
    </div>
  );
}
```

The routing state that have been defined in the view model will be part of the initial state that the _Index_ component will receive once connected. There are two things left to do: tell the component which target DOM element to render the route, and create the links.

To set the target DOM element, pass the __onRouteEnter__ function in the _options_ argument of the _connect_ function. This API will pass the URL path and the route template object to the function whenever a route is activated, and you will just set the _Target_ property to the ID of the target DOM element. It also allows you to cancel the operation by returning false.

To set up the route links, import the __RouteLink__ component. It's basically a wrapper of the a tag, and you just need to pass as properties the _vm_ object, and the route from the initial state. If you need to handle the click event before the route is activated, just pass your function to its _onClick_ property.

The __RouteTarget__ represents the DOM element where the routed component will be rendered.  Make sure the ID matches the one specified in the _onRouteEnter_.

You can register more routes inside the view model associated with the pages under _Index_, basically making nested routes. You only need to make sure that the root path of the routes registered inside the nested view model match the the component name (or URL pattern) of the parent route. For example, the routes registered in the _Page1_ view model should have "Page1" for root path. All routes defined this way support deep-linking: you can paste the URL to the browser and go directly to any deeply nested view.

As an alternative to route links, you can also call __vm.$routeTo__ from your script and pass it the route object.

#### Redirection

A view can navigate to another view that's not part of its registered route templates by creating a redirect route. This type of route is specified by using __Redirect__ API:
```csharp
public class Page3 : BaseVM, IRoutable
{
   public RoutingState RoutingState { get; set; }
   public Route LinkPage1 => this.Redirect("index", "Page1");
}
```

 The first parameter specifies the root path, which doesn't have to start from the top since the routing logic will attempt to find the best match. The second parameter is optional, and is used only when the path requires parameters, in which case it should be set to the parameter value.

#### Back-End Routing Events

There are two APIs your view model can use to receive routing events:

- this.__OnActivated__: occurs when any route that's registered with the view model is activated.
- this.__OnRouted__: occurs when the route that's connected with the view model is activated.

To illustrate, if we click the "Page 1" link, the _Index_ view model where the "Page1" route was registered will receive the __Activated__ event, and the view model connected to Page1 component will receive the __Routed__ event. Both events carry the activated URL path in their event arguments.

#### Getting Initial State

To improve user experience, you can deliver the initial state together with the component in one HTTP get request. We will need to write an ASP.NET Controller to handle such request:

```csharp
public class HomeController : Controller
{
   [Route("module/get/{view}/{vm?}")]
   public ActionResult Module( string view, string vm )
   {
      var component = System.IO.File.ReadAllText(Server.MapPath($"{view}.js"));
      var initialState = $"window.vmStates = window.vmStates || {{}}; window.vmStates['{vm}'] = {VMController.GetInitialState(vm) ?? "{}"};";
      return Content(initialState + component, "text/js");
   }
}
```

The __VMController.GetInitialState__(_viewModelName_) API is used to get the JSON-serialized state of the view model, which then set to global window variable for the component to use when it's rendered. We can now define the routes and the component's constructor as follows:

```csharp
   new RouteTemplate("Page1", "module/get/Page1/Page1VM"),
   new RouteTemplate("Page2", "module/get/Page2/Page2VM")
```

```jsx
class Page1 extends React.Component{
   constructor() {
      ...
      this.state = window.vmStates.Page1VM;
   }
}
```

#### Handling 404

When the routing cannot resolve a URL, it will redirect the browser to `"/404.html"` which you will need to provide.  

If you want it to render a component, or if you have deep nesting and want better control of what to display, then add a _RouteTemplate_ with a wildcard (*) URL pattern:
```csharp
   new RouteTemplate("NotFoundPage") { UrlPattern = "*" }
```


#### Lazy-Loading

When your bundle size is getting too big, you would want to break the components into several chunks that only gets loaded when the route that displays them is traversed.  We can do this by leveraging the Webpack's code splitting feature.

First, let's change the static import of the page components into dynamic import:
```jsx
const importPromise = (id, dynamicImport) =>
  new Promise(resolve =>
    dynamicImport.then(module => {
      window[id] = module.default;
      resolve();
    })
  );

const lazyLoad = id => {
  if (id === 'Page1') {
    return importPromise(id, import(/* webpackChunkName: "page1" */ './page1'));
  }
  else if (id === 'Page2') {
    return importPromise(id, import(/* webpackChunkName: "page2" */ './page2'));
  }
};
```

Note the use of `webpackChunkName` in the comment. This is one of a so-called Webpack's magic comments that's used to assign specific names to the generated chunk files.

Next, update the _onRouteEnter_ function to call that _lazyLoad_ function:
```jsx
const onRouteEnter = (path, template) => {
  template.Target = "Content"; 
  return lazyLoad(template.Id);
}
```

And last, configure the Webpack to generate and name our component chunk files:
```js
module.exports = {
  /*...*/
  output: {
    chunkFilename: '[id].js',
    path: __dirname + '/wwwroot'
  },
/*...*/
```

For complete example, see [Lazy-Load Routing Demo](https://github.com/dsuryd/dotNetify/tree/master/Demo/React/LazyLoadRouting).

#### Server-Side Rendering

DotNetify's strategy for server-side rendering is to use `jsdom` headless browser, which runs on NodeJS environment, to render the page on the server.  When the ASP.NET Core server receives a page request, instead of returning the usual _index.html_, it will pass the request to a special NodeJS script to render the root component with `jsdom` and returns the complete HTML output to the browser.  

To start with, let's install `jsdom` through _npm_. Then add the following script under _wwwroot_ and name it `ssr.js`:
```jsx
const { JSDOM, ResourceLoader } = require('jsdom'); /*at least v16.2*/
module.exports = function(callback, userAgent, ...args) {
  JSDOM.fromFile('./wwwroot/index.html', {
    url: 'http://localhost:5000', /*update this to your application URL*/
    resources: new ResourceLoader({ userAgent }),
    runScripts: 'dangerously',
    beforeParse(window) {
      window.__dotnetify_ssr__ = function(ssr) {
        ssr(callback, ...args, 2000 /*timeout*/);
      };
    }
  });
};
```

In your application bootstrap, call __enableSsr__ and use __ReactDOM.hydrate__ to ensure that the client-side React won't just toss away the server-side rendered page:
```jsx
import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';
import { enableSsr } from 'dotnetify';

enableSsr();
ReactDOM.hydrate(<App />, document.getElementById('App'));
```

The last step is to update the _Startup.cs_ to use __useSsr__ request handler from DotNetify namespace.  This method requires you to provide a delegate to execute NodeJS script from inside ASP.NET Core, so for this we'll use a nuget library called `Jering.Javascript.NodeJS`.  
```csharp
...
using DotNetify;
using Jering.Javascript.NodeJS;

public class Startup
{
  ...
  public void Configure(IApplicationBuilder app)
  {
    app.UseWebSockets();
    app.UseDotNetify();
    app.UseRouting();
    app.UseEndpoints(endpoints => endpoints.MapHub<DotNetifyHub>("/dotnetify"));

    app.UseSsr(typeof(App), 
      (string[] args) => StaticNodeJSService.InvokeFromFileAsync<string>("wwwroot/ssr", null, args), 
      DefaultRequestHandler);

    app.Run(DefaultRequestHandler);

  }

  private static async Task DefaultRequestHandler(HttpContext context)
  {
      using (var reader = new StreamReader(File.OpenRead("wwwroot/index.html")))
        await context.Response.WriteAsync(reader.ReadToEnd());
  }
}
```

For complete example, see [Lazy-Load Routing Demo](https://github.com/dsuryd/dotNetify/tree/master/Demo/React/LazyLoadRouting).