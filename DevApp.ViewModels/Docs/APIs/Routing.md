## Routing

While you certainly can use _React-Router_ to handle all your app's routing needs, dotNetify provides you with an option to declare all the routes in the back-end if you prefer to have your components decoupled from routing concerns. 
> Demo source code is on GitHub at [dotnetify-react-demo-vs2017](https://github.com/dsuryd/dotnetify-react-demo-vs2017).

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
      this.RegisterRoutes("index", new List<RouteTemplate>
      {
         new RouteTemplate("Home",  "/home.js")  { UrlPattern = "" },
         new RouteTemplate("Page1", "/page1.js") { UrlPattern = "page1" },
         new RouteTemplate("Page2", "/page2.js") { UrlPattern = "page2" },
      });
   }
}
```

Any view model involved with routing must implement __IRoutable__ interface from the namespace __DotNetify.Routing__. It only has one property, the __RoutingState__, and you only need to implement it as auto-property.

In the constructor, create the route templates for _Page1_ and _Page2_ and register them using the __RegisterRoutes__ API. You are required to specify the root path for all the routes.

The route template object, __RouteTemplate__ defines which component to render (e.g. _Page1_) when the route is activated, where to get the script (e.g. _/page1.js_), and maps a URL to a specific route. By default it assumes the URL will match the component name, but you can use the UrlPattern property to change the search pattern, such as mapping URL with only the root path to the Home route. 

The nav-bar will need links to the pages, so we add some properties that return the Route objects for _Page1_ and _Page2_. To get the object, use the __GetRoute__ API, which will find it from the list of registered routes.

You can leave out the script location if you plan to bundle the components, in which case you must add them to the _window_ variable during your application's bootstrap: 
```jsx
import Page1 from './page1.js';
import Page2 from './page1.js';

Object.assign(window, { Page1, Page2 });
```

#### Setting Up Route Links

Now that we have our routes all defined in the back-end, this is how they are set up in the _Index_ component:

```jsx
import React from 'react';
import dotnetify from 'dotnetify';
import { RouteLink } from 'dotnetify/react';

class Index extends React.Component {
  constructor() {
     super();
     this.vm = dotnetify.react.connect("Index", this);
     this.vm.onRouteEnter = (path, template) => template.Target = "Content";
     this.state = {};
  }
  componentWillUnmount() {
     this.vm.$destroy();
  }
  render() {
    return (
        <div>
          <RouteLink vm={this.vm} route={this.state.LinkPage1}>Page 1</RouteLink>
          <RouteLink vm={this.vm} route={this.state.LinkPage2}>Page 2</RouteLink>
          <div id="Content" />
        </div>
    );
  }
}
```

The routing state that have been defined in the view model will be part of the initial state that the _Index_ component will receive once connected. There are two things left to do: tell the component which target DOM element to render the route, and create the links.

To set the target DOM element, implement the __vm.onRouteEnter__ function in the constructor. This API will pass the URL path and the route template object to the function whenever a route is activated, and you will just set the _Target_ property to the ID of the target DOM element. It also allows you to cancel the operation by returning false.

To set up the route links, import the __RouteLink__ component. It's basically a wrapper of the a tag, and you just need to pass as properties the _vm_ object, and the route from the initial state. If you need to handle the click event before the route is activated, just pass your function to its _onClick_ property.

You can register more routes inside the view model associated with the pages under _Index_, basically making nested routes. You only need to make sure that the root path of the routes registered inside the nested view model match the the component name (or URL pattern) of the parent route. For example, the routes registered in the _Page1_ view model should have "Page1" for root path. All routes defined this way support deep-linking: you can paste the URL to the browser and go directly to any deeply nested view.

As an alternative to route links, you can also call __vm.$routeTo__ from your script and pass it the route object.

#### Redirection

A view can navigate to another view that's not part of its registered route templates by creating a redirect route. This type of route is specified by using __Redirect__ API. The first parameter specifies the root path, which doesn't have to start from the top since the routing logic will attempt to find the best match. The second parameter is optional, and is used only when the path requires parameters, in which case it should be set to the parameter value.

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

#### Server-Side Rendering

[Server-side Rendering of Deep Links with React and .NET Core](https://hackernoon.com/server-side-rendering-of-deep-links-with-react-and-net-core-882830ca663)

