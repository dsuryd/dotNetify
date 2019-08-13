## Local Mode

When writing client-side unit tests, there's often a need to mock your component's connection so it won't talk to the real server.  DotNetify facilitates this by supporting "local" mode connection option.   You would do this by passing an object as the __connect__ API's _options_ parameter with the following properties:
- __mode__: "local".
- __initialState__: initial state for your component.
- __onDispatch__: optional callback for when the component is dispatching something. You can respond by calling __this.update__ with the data you want to pass back to the component.

[inset]
[inset]

#### Local View Model

Of course this feature can go beyond unit tests, and be used in other scenarios, for example, providing elements from _dotNetify-Elements_ with a local __VMContext__:

```jsx
const localNav = {
  mode: 'local',
  initialState: {
    RoutingState: {},
    NavMenu: [
      {
        Route: { TemplateId: null, Path: 'core/overview', RedirectRoot: '' },
        Label: 'Introduction'
      },
      {
        IsExpanded: true,
        Routes: [
          {
            Route: { TemplateId: null, Path: 'core/examples/helloworld', RedirectRoot: '' },
            Label: 'Hello World',
            Icon: 'material-icons web'
          },
          {
            Route: { TemplateId: null, Path: 'core/examples/livechart', RedirectRoot: '' },
            Label: 'Live Chart',
            Icon: 'material-icons show_chart'
          }
        ],
        Label: 'Examples'
      }
    ]
  }
};

<VMContext vm="localNav" options={localNav}>
  <NavMenu id="NavMenu" />
</VMContext>
```

[inset]

