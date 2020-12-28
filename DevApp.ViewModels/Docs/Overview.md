## Overview

DotNetify makes it super easy to connect your [React](http://reactjs.org)/[React Native](https://facebook.github.io/react-native/) app to a cross-platform .NET back-end. It frees you up from having to write AJAX requests or Web APIs, and your app gets real-time two-way communication through WebSockets with [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/?view=aspnetcore-2.1) for free.

Whether in a brand-new ASP.NET Core project that runs on Windows, Linux, and Mac, or in an existing ASP.NET MVC codebase, you can use it right away with only a tiny footprint and get your app running in minutes! Need proof? Pick one and try it out:

[inset]

#### Hello World

In the most basic form, you can just use dotNetify to quickly set up a back-end data store to get the initial state of your React component. Simply add the **connect** API inside the constructor (or getInitialState if using ES5), specify the name of the C# class that will provide the state, and pass the component itself.

[inset]

```csharp
public class HelloWorld : BaseVM
{
   public string Greetings => "Hello World!";
}
```

Write a C# class that inherits from **BaseVM** in your ASP.NET project, and add public properties for all the data your component will need. When the connection is established, the class instance will be serialized to JSON and sent as the initial state for the component.

#### Real-Time Push

With very little effort, you can make your app gets real-time data update from the back-end:

[inset]

```csharp
public class HelloWorld : BaseVM
{
   private Timer _timer;
   public string Greetings => "Hello World!";
   public DateTime ServerTime => DateTime.Now;

   public HelloWorld()
   {
      _timer = new Timer(state =>
      {
         Changed(nameof(ServerTime));
         PushUpdates();
      }, null, 0, 1000); // every 1000 ms.
   }
   public override void Dispose() => _timer.Dispose();
}
```

[inset]
We added two new back-end APIs, **Changed** that accepts the name of the property we want to update, and **PushUpdates** to do the actual push of all changed properties to the front-end.

#### Server Update

At some point in your app, you probably want to send data back to the server to be persisted. Let's add to this example something to submit:

[inset]

```csharp
public class HelloWorld : BaseVM
{
   public class Person
   {
      public string FirstName { get; set; }
      public string LastName { get; set; }
   }

   public string Greetings { get; set; } = "Hello World!";

   public void Submit(Person person)
   {
      Greetings = $"Hello {person.FirstName} {person.LastName}!";
      Changed(nameof(Greetings));
   };
}
```

[inset]
The React component now stores the return value of the **connect** API, which is the proxy for the back-end class instance that's connecting to the component. Through this proxy, we have access to the **\$dispatch** API that we use to send the state to the back-end instance. The state value will be set to the property name matching the state name.

Invoking the **\$dispatch** Submit on the front-end will cause the view model method of the same name to be invoked, with the dispatched state value being the argument. After the action modifies the Greetings property, it marks it as changed so that the new text will get sent to the front-end.

Notice that we don't need to use **PushUpdates** to get the new greetings sent. That's because dotNetify employs something similar to the request-response cycle, but in this case it's _action-reaction cycle_: the front-end initiates an action that mutates the state, the back-end processes the action and then sends back to the front-end any other states that mutate as the reaction to that action.

### Asynchronous Methods

The _Submit_ method can be awaited by returning the _Task_ type:
```csharp
   public async Task Submit(Person person)
   {
      await SomeAsyncMethod(person);
   }
```

And if you need to call asynchronous methods to initialize your view model, override the **OnCreatedAsync** method:
```csharp
public class HelloWorld: BaseVM
{
   public string Greetings { get; set; }

   public override async Task OnCreatedAsync()
   {
      Greetings = await BuildGreetingsAsync();
   }
}
```

#### Object Lifetime

You probably think of those back-end classes as models, but in dotNetify's scheme of things, they are considered view models. Whereas a model represents your app's business domain, a view model is an abstraction of a view, which embodies data and behavior necessary for the view to fulfill its use case.

View-models gets their model data from the back-end service layer and shape them to meet the needs of the views. This enforces separation of concerns, where you can keep the views dealing only with UI presentation and leave all data-driven operations to the view models. It's great for testing too, because you can test your use case independent of any UI concerns.

View model objects stay alive on the back-end until the browser page is closed, reloaded, navigated away, or the session times out. On a single-page app, when a component can be mounted and dismounted repeatedly, it is important that you manually destroy the view model when your React component dismounts by calling the **\$destroy** API.

> Note: if you're using **useConnect** hook, this is already built-in.

```jsx
componentWillUnmount() {
   this.vm.$destroy();
}
```

When the browser is closing or reloading, SignalR will normally send a disconnection event to the server, which signals dotNetify to dispose the view models associated with the connection. However, this event won't occur when the SignalR transport falls back to HTTP long polling. Orphan view models will eventually be cleared, but for best practice, use the window's `beforeUnload` event as fallback to invoke `this.vm.$destroy()`.

#### API Essentials

##### Client APIs

To get started, you only need these essential APIs to add to your React component:

- _vm_ = **dotnetify.react.connect**(_vmName, component, options_)<br/>
- `Custom hook:` { _vm, state_ } = **useConnect**(_vmName, state | { state, props }, options_)

  The options argument is an object with one or more of the following properties:

  - **getState**: function, provide your own component's state accessor.
  - **setState**: function, provide your own component's state mutator.
  - **vmArg**: object, initialize view model properties.
    For example:
    ```jsx
    let initialState = { Person: { FirstName: 'John', LastName: 'Hancock' } };
    dotnetify.react.connect('HelloWorld', this, { vmArg: initialState });
    ```
  - **headers**: object, pass request headers, e.g. for authentication purpose.
  - **exceptionHandler**: function, handler for server exceptions.
  - **webApi**: use HTTP Web API endpoint instead of SignalR hub.

- _vm_.**\$dispatch**(_state_)
- _vm_.**\$destroy**()

- To output debug logs to the browser's Console tab, add **dotnetify.debug**= _true_.
  <br/><br/>

##### Server APIs

On the back-end, inherit from **BaseVM** and use:

- **Changed**(_propName_)
- **PushUpdates**(): for real-time push.

To asynchronously initialize view model properties, override **OnCreateAsync**.<br>
Methods that are called from the client can be made awaitable by having the return type `Task`.

Not essential, but these property accessor/mutator helper **Get/Set** can make your code more concise:

```csharp
public string Greetings
{
   get => Get<string>();
   set => Set(value);
}
// Above property is equivalent to this:
private string _greetings;
public string Greetings
{
   get { return _greetings; }
   set { _greetings = value; Changed(nameof(Greetings)); }
}
```
