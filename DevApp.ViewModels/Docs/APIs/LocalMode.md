## Local Mode

When writing client-side unit tests, there's often a need to mock your component's connection so it won't talk to the real server.  DotNetify facilitates this by supporting "local" mode connection option.   You would do this by passing an object as the __connect__ API's _options_ parameter with the following properties:
- __mode__: "local".
- __initialState__: initial state for your component.
- __onDispatch__: optional callback for when the component is dispatching something. You can respond by calling __this.$update__ with the data you want to pass back to the component.

[inset]
[inset]

#### Local View Model

Beyond unit testing, you can completely replace the server-side view model with a local one by defining the object inside the `window` variable, and setting the connect options with __{mode: "local"}__.
For example, the following code show show to provide _dotNetify-Elements_ with a local view model context:

```jsx
window.LocalPage1 = _ => <Alert>You selected Page 1</Alert>;
window.LocalPage2 = _ => <Alert danger>You selected Page 2</Alert>;

window.LocalVM = {
  initialState: {
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
  }
};

<VMContext vm="LocalVM" options={{mode: "local"}}>
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

