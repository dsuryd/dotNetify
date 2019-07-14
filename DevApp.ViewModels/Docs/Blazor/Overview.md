## Overview

DotNetify makes it super easy to connect your client-side [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/client) app to a .NET back-end in a declarative, real-time and reactive manner.  It uses [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/?view=aspnetcore-2.1) to communicate with the server through an MVVM-styled abstraction, while making sure that you still have control over what gets sent over the network.

DotNetify is a good fit for building complex web applications that requires clear separation of concerns between client-side UI, presentation, and domain logic for long-term testability, maintainability and extensiblity.  Furthermore, its integration with SignalR allows you to easily implement asynchronous data updates to multiple clients in real-time.

DotNetify also comes with a library of HTML native web components called __DotNetify-Elements__.  Some are pre-wired to talk directly to your server-side view model, which make them great for quickly implementing input forms.  Others provide UI elements such as tabs, modals, collapsibles and more --- all supporting scoped CSS customization.

#### Hello World

In the most basic form, you can just use dotNetify to quickly fetch from the server the initial state of your Blazor component. You do this by nesting your component's HTML inside a __VMContext__ component, and specify its __VM__ attribute value with the name of the C# view model class that will provide the state.

```jsx
<VMContext VM="HelloWorld" OnStateChanged="@HandleStateChanged">
    <div>@state?.Greetings</div>
</VMContext>

@code {
    HelloWorldState state;

    class HelloWorldState
    {
        public string Greetings { get; set; }
    }

    void HandleStateChanged(object newState)
    {
        state = newState.As<HelloWorldState>();
        StateHasChanged();
    }
}
```
```csharp
public class HelloWorld : BaseVM
{
   public string Greetings => "Hello World!";
}
```

Write a C# class that inherits from __BaseVM__ in your ASP.NET project, and add public properties for all the data your component will need. When the connection is established, the class instance will be serialized to JSON and sent as the initial state for the component.

#### Real-Time Push

With very little effort, you can make your app gets real-time data update from the back-end:

```jsx
<VMContext VM="HelloWorld" OnStateChanged="@HandleStateChanged">
    <div>
      <p>@state?.Greetings</p>
      <p>Server time is: @state?ServerTime</p>
    </div>    
</VMContext>

@code {
    HelloWorldState state;

    class HelloWorldState
    {
        public string Greetings { get; set; }
        public string ServerTime { get; set; }
    }

    void HandleStateChanged(object newState)
    {
        state = newState.As<HelloWorldState>();
        StateHasChanged();
    }
}
```
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
We added two new back-end APIs, __Changed__ that accepts the name of the property we want to update, and __PushUpdates__ to do the actual push of all changed properties to the front-end.

#### Server Update

At some point in your app, you probably want to send data back to the server to be persisted. Let's add to this example something to submit:

```jsx
<VMContext @ref="@vm" VM="ServerUpdate" OnStateChanged="@HandleStateChanged">
    <div>
        <p>@state?.Greetings</p>
        <input type="text" bind="@person.FirstName" />
        <input type="text" bind="@person.LastName" />
        <button onclick="@HandleSubmit">Submit</button>
    </div>
</VMContext>

@code {
    VMContext vm;
    ServerUpdateState state;
    Person person = new Person();

    class ServerUpdateState
    {
        public string Greetings { get; set; }
    }

    class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    void HandleStateChanged(object newState)
    {
        state = newState.As<ServerUpdateState>();
        StateHasChanged();
    }

    void HandleSubmit()
    {
        vm.DispatchAsync("Submit", person);
    }
}
```
```csharp
public class HelloWorld : BaseVM
{
   public class Person
   {
      public string FirstName { get; set; }
      public string LastName { get; set; }
   }

   public string Greetings { get; set; } = "Hello World!";
   public Action<Person> Submit => person =>
   {
      Greetings = $"Hello {person.FirstName} {person.LastName}!";
      Changed(nameof(Greetings));
   };
}
```
[inset]
Notice that we now uses the _ref_ attribute on the _VMContext_ component to capture the reference.  This reference serves as a proxy to the back-end class instance that's connecting to the component. Through this proxy, we have access to the __DispatchAsync__ API, which we use to send the state to the back-end instance. The state value will be set to the property name matching the state name.

On the back-end, we set up the Submit property to be of an Action delegate type since we don't expect it to send any value to the front-end. Invoking the __DispatchAsync__ Submit on the front-end will cause that action to be invoked, with the dispatched state value being the argument. After the action modifies the Greetings property, it marks it as changed so that the new text will get sent to the front-end.

Notice that we don't need to use __PushUpdates__ to get the new greetings sent. That's because dotNetify employs something similar to the request-response cycle, but in this case it's _action-reaction cycle_: the front-end initiates an action that mutates the state, the back-end processes the action and then sends back to the front-end any other states that mutate as the reaction to that action.


#### Object Lifetime

You probably think of those back-end classes as models, but in dotNetify's scheme of things, they are considered view models. Whereas a model represents your app's business domain, a view model is an abstraction of a view, which embodies data and behavior necessary for the view to fulfill its use case.

View-models gets their model data from the back-end service layer and shape them to meet the needs of the views. This enforces separation of concerns, where you can keep the views dealing only with UI presentation and leave all data-driven operations to the view models. It's great for testing too, because you can test your use case independent of any UI concerns.

View model objects stay alive on the back-end until the browser page is closed, reloaded, navigated away, or the session times out.


#### API Essentials

##### Client APIs

__VMContext__ component attributes:
- __VM__:  the name of the view model type to connect to.
- __OnStateChanged__: callback when a new state is received from the back-end.
- __ref__: captures the VMContext reference to access the __DispatchAsync__ method.

__As(_type_): helper extension method to deserialize the state object.

##### Server APIs
On the back-end, inherit from __BaseVM__ and use:
- __Changed__(_propName_)
- __PushUpdates__(): for real-time push.

Not essential, but these property accessor/mutator helper __Get/Set__ can make your code more concise:

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
