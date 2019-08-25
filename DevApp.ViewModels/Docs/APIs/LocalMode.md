## Local Mode

When writing client-side unit tests, there's often a need to mock your component's connection so it won't talk to the real server.  DotNetify facilitates this by supporting "local" view model mode, where you can override the server-side view model with a client-side Javascript object.  

You would do this by adding an object with the same name as the view model ID to the `window` variable, and with the following properties:

- __onConnect__(_vmArgs_): initial connection callback; expects initial state to be returned.
- __onDispatch__(_state_): dispatch callback; you can optionally return the data you want to pass back to the component.
- __onDestroy__(): optional destroy callback.

[inset]
[inset]


#### Local VMContext

Beyond unit testing, the local mode provides you with the option to hydrate some of your presentational components purely from client-side. For example, the following code shows how to provide components from _dotNetify-Elements_ with a _VMContext_ element that gets its data from an ES6 class instance:

```jsx
// Route components.
window.LocalPage1 = _ => <Alert>You selected Page 1</Alert>;
window.LocalPage2 = _ => <Alert danger>You selected Page 2</Alert>;

window.LocalVM = new class {
  initialState = {
    RoutingState: {
      Templates: [
        { Id: 'Page1', Root: '', UrlPattern: 'page1', ViewUrl: 'LocalPage1' },
        { Id: 'Page2', Root: '', UrlPattern: 'page2', ViewUrl: 'LocalPage2' }
      ],
      Root: 'core/api/localmode'
    },
    NavMenu: [
      { Route: { TemplateId: 'Page1', Path: 'page1' }, Label: 'Page 1' },
      { Route: { TemplateId: 'Page2', Path: 'page2' }, Label: 'Page 2' }
    ]
  };

  onConnect() {
    return this.initialState;
  }
}();

<VMContext vm="LocalVM">
  <Panel horizontal>
    <Panel flex="30%">
      <NavMenu id="NavMenu" target="localTarget" />
    </Panel>
    <Panel flex="70%" css="padding-top: .5rem">
      <div id="localTarget" />
    </Panel>
  </Panel>
</VMContext>
```
[inset]

Note that if the _VMContext_ is nested under a parent _VMContext_, the local view model name must be in the form a path of the view model IDs, delimited with underscore.  For example: ```window.ParentVM_ChildVM = { /*...*/ }```.