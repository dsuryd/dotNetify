## Overview

DotNetify makes it super easy to connect your client-side [Blazor](https://docs.microsoft.com/en-us/aspnet/core/blazor/) app to a .NET back-end in a declarative, real-time and reactive manner. It uses [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/) to communicate with the server through an MVVM-styled abstraction, while making sure that you still have control over what gets sent over the network.

DotNetify is a good fit for building complex web applications that requires clear separation of concerns between client-side UI, presentation, and domain logic for long-term testability, maintainability and extensiblity. Furthermore, its integration with SignalR allows you to easily implement asynchronous data updates to multiple clients in real-time.

DotNetify also comes with a library of HTML native web components called **DotNetify-Elements**. Some are pre-wired to talk directly to your server-side view model, which make them great for quickly implementing input forms. Others provide UI elements such as tabs, modals, collapsibles and more --- all supporting scoped CSS customization.

#### Hello World

In the most basic form, you can just use dotNetify to quickly fetch from the server the initial state of your Blazor component. You do this by nesting your component's HTML inside a **VMContext** component, and specify its **VM** attribute value with the name of the C# view model class that will provide the state.

```jsx
<VMContext VM="HelloWorld" OnStateChange="(IHelloWorldState newState) => { state = newState; StateHasChanged(); }">
    <div>@state?.Greetings</div>
</VMContext>

@code {
    IHelloWorldState state;

    public interface HelloWorldState
    {
        string Greetings { get; set; }
    }
```

```csharp
public class HelloWorld : BaseVM
{
   public string Greetings => "Hello World!";
}
```

Write a C# class that inherits from **BaseVM** in your ASP.NET project, and add public properties for all the data your component will need. When the connection is established, the class instance will be serialized to JSON and sent as the initial state for the component.

#### Real-Time Push

With very little effort, you can make your app gets real-time data update from the server:

```jsx
<VMContext VM="HelloWorld" OnStateChange="(IHelloWorldState newState) => { state = newState; StateHasChanged(); }">
    <div>
      <p>@state?.Greetings</p>
      <p>Server time is: @state?ServerTime</p>
    </div>
</VMContext>

@code {
    IHelloWorldState state;

    public interface IHelloWorldState
    {
        string Greetings { get; set; }
        string ServerTime { get; set; }
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
We added two new server APIs, **Changed** that accepts the name of the property we want to update, and **PushUpdates** to do the actual push of all changed properties to the front-end.

#### Server Update

At some point in your app, you probably want to send data back to the server to be persisted. Let's add to this example something to submit:

```jsx
<VMContext VM="ServerUpdate" OnStateChange="(IServerUpdateState newState) => { state = newState; StateHasChanged(); }">
    <div>
        <p>@state?.Greetings</p>
        <input type="text" bind="@person.FirstName" />
        <input type="text" bind="@person.LastName" />
        <button onclick="@HandleSubmit">Submit</button>
    </div>
</VMContext>

@code {
    IServerUpdateState state;
    Person person = new Person();

    public interface IServerUpdateState
    {
        string Greetings { get; set; }
        void Submit(Person person);
    }

    class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    private void HandleSubmit() => state.Submit(person);
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
Notice the `Submit` method on the client-side interface, which serves as proxy to the method of the same name in the view model. When that method is called, the argument will be dispatched to the server and used to invoke the server method. After the action modifies the Greetings property, it marks it as changed so that the new text will get sent to the front-end.

Notice that we don't need to use **PushUpdates** to get the new greetings sent. That's because dotNetify employs something similar to the request-response cycle, but in this case it's _action-reaction cycle_: the front-end initiates an action that mutates the state, the server processes the action and then sends back to the front-end any other states that mutate as the reaction to that action.

#### Object Lifetime

You probably think of those server classes as models, but in dotNetify's scheme of things, they are considered view models. Whereas a model represents your app's business domain, a view model is an abstraction of a view, which embodies data and behavior necessary for the view to fulfill its use case.

View-models gets their model data from the back-end service layer and shape them to meet the needs of the views. This enforces separation of concerns, where you can keep the views dealing only with UI presentation and leave all data-driven operations to the view models. It's great for testing too, because you can test your use case independent of any UI concerns.

View model objects stay alive on the server until the browser page is closed, reloaded, navigated away, or the session times out.

#### Two-Way Binding

To keep the properties in the client in sync with the server without explicit method call, add the **[Watch]** attribute on the interface property:

```jsx
<VMContext VM="HelloWorld" OnStateChange="(IHelloWorldState newState) => { state = newState; StateHasChanged(); }">
  <div>
    <p>@state.Greetings</p>
    <input type="text" bind="@state.Name" />
  </div>
</VMContext>

@code {
    IHelloWorldState state;
    Person person = new Person();

    public interface IHelloWorldState
    {
        string Greetings { get; set; }
        [Watch] string Name { get; set; }
    }
}
```

```csharp
public class HelloWorld : BaseVM
{
  public string Greetings => $"Hello {Name}";

  private string _name = "World";
  public string Name
  {
    get => _name;
    set
    {
      _name = value;
      Changed(nameof(Greetings));
    }
  }
}
```

[inset]

#### API Essentials

##### Client APIs

**VMContext** component attributes:

- **VM**: the name of the view model type to connect to.
- **OnStateChange**: callback when a new state is received from the server.

\__As(\_type_): helper extension method to deserialize the state object.

##### Server APIs

On the server, inherit from **BaseVM** and use:

- **Changed**(_propName_)
- **PushUpdates**(): for real-time push.

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
