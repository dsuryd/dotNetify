## Routing

The Knockout library doesn't provide SPA client-side routing, but dotNetify provides one.  Routes are defined on the back-end inside the root (or Index) view model, and then loaded to the client on the initial connection.

#### Setting Up Route Links

Now that we have our routes all defined in the back-end, this is how they are set up in the _Index_ component:

```html
<div data-vm="Index">
    <a data-bind="vmRoute: LinkPage1">Page 1</a>
    <a data-bind="vmRoute: LinkPage2">Page 2</a>
    <div id="Content" />
</div>
```
```jsx
export default class Index {
  constructor(vm: any) {
    vm.onRouteEnter = (path: string, template: any) => (template.Target = 'Content');
  }
}
```

The routing state that have been defined in the view model will be part of the initial state that the _Index_ component will receive once connected. There are two things left to do: tell the component which target DOM element to render the route, and create the links.

To set the target DOM element, implement the __vm.onRouteEnter__ function in the constructor of the code-behind. This API will pass the URL path and the route template object to the function whenever a route is activated, and you will just set the _Target_ property to the ID of the target DOM element. It also allows you to cancel the operation by returning false.

To set up the route links, use the __vmRoute__ binding and pass in the route object that we have defined in the view model.  This binding will set the _href_ attribute with the appropriate path, and handles the _click_ event of the anchor tag.

You can register more routes inside the view model associated with the pages under _Index_, basically making nested routes. You only need to make sure that the root path of the routes registered inside the nested view model match the the component name (or URL pattern) of the parent route. For example, the routes registered in the _Page1_ view model should have "Page1" for root path. All routes defined this way support deep-linking: you can paste the URL to the browser and go directly to any deeply nested view.