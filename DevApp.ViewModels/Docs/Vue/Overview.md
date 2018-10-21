## Overview

DotNetify makes it super easy to connect your [Vue](https://vuejs.org) app to a cross-platform .NET back-end. It frees you up from having to write AJAX requests or Web APIs, and your app gets real-time two-way communication through WebSockets with [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/?view=aspnetcore-2.1) for free.

Whether in a brand-new ASP.NET Core project that runs on Windows, Linux, and Mac, or in an existing ASP.NET MVC codebase, you can use it right away with only a tiny footprint and get your app running in minutes! Need proof? Pick one and try it out:

[inset]

#### Hello World

In the most basic form, you can just use dotNetify to quickly set up a back-end data store to get the initial state of your Vue component. Simply add the __connect__ API inside the _created_ method, specify the name of the C# class that will provide the state, and pass the component itself.

```html
<template>
	<div>{{Greetings}}</div>
</template>

<script>
import dotnetify from 'dotnetify/vue';

export default {
  name: "MyApp",
  created() { 
    dotnetify.vue.connect("HelloWorld", this);
  },
  data() {	
    return { Greetings: '' } 
  }
}
</script>
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

```html
<template>
  <div>
    <p>{{Greetings}}</p>
    <p>Server time is: {{ServerTime}}</p>
  </div>
</template>

<script>
import dotnetify from 'dotnetify/vue';

export default {
  name: 'MyApp',
  created() {
    dotnetify.vue.connect("HelloWorld", this);
  },
  data() {
    return { Greetings: '', ServerTime: '' }
  }
}
</script>
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

```html
<template>
  <div>
    <div>{{Greetings}}</div>
    <input type="text" v-model="firstName" />
    <input type="text" v-model="lastName" />
    <button @click="onSubmit">Submit</button>
  </div>
</template>

<script>
import dotnetify from 'dotnetify/vue';

export default {
  name: 'MyApp',
  created() {
    this.vm = dotnetify.vue.connect("HelloWorld", this);
  },
  data() {
    return { Greetings: '', firstName: '', lastName: '' }
  },
  methods: {
    onSubmit() {
      this.vm.$dispatch({ 
        Submit: { FirstName: this.firstName, LastName: this.lastName } 
      });
    }
  }
}
</script>
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
The Vue component now stores the return value of the __connect__ API, which is the proxy for the back-end class instance that's connecting to the component. Through this proxy, we have access to the __$dispatch__ API that we use to send the state to the back-end instance. The state value will be set to the property name matching the state name.

On the back-end, we set up the Submit property to be of an Action delegate type since we don't expect it to send any value to the front-end. Invoking the __$dispatch__ Submit on the front-end will cause that action to be invoked, with the dispatched state value being the argument. After the action modifies the Greetings property, it marks it as changed so that the new text will get sent to the front-end.

Notice that we don't need to use __PushUpdates__ to get the new greetings sent. That's because dotNetify employs something similar to the request-response cycle, but in this case it's _action-reaction cycle_: the front-end initiates an action that mutates the state, the back-end processes the action and then sends back to the front-end any other states that mutate as the reaction to that action.

#### Object Lifetime

You probably think of those back-end classes as models, but in dotNetify's scheme of things, they are considered view models. Whereas a model represents your app's business domain, a view model is an abstraction of a view, which embodies data and behavior necessary for the view to fulfill its use case.

View-models gets their model data from the back-end service layer and shape them to meet the needs of the views. This enforces separation of concerns, where you can keep the views dealing only with UI presentation and leave all data-driven operations to the view models. It's great for testing too, because you can test your use case independent of any UI concerns.

View model objects stay alive on the back-end until the browser page is closed, reloaded, navigated away, or the session times out. On a single-page app, when a component can be mounted and dismounted repeatedly, it is important that you manually destroy the view model when your Vue component dismounts by calling the __$destroy__ API.

```jsx
destroyed() {
   this.vm.$destroy();
}
```
> When the browser is closing or reloading, SignalR will normally send a disconnection event to the server, which signals dotNetify to dispose the view models associated with the connection.  However, this event won't occur when the SignalR transport falls back to HTTP long polling.  Orphan view models will eventually be cleared, but for best practice, use the window's `beforeUnload` event as fallback to invoke `this.vm.$destroy()`.  

#### Component Shorthand

To make your code even more concise, dotNetify provides the __component__ API that produces a preconfigured Vue object.  Here's how it's used to refactor the real-time push example:
```html
<template>
  <div>
    <p>{{state.Greetings}}</p>
    <p>Server time is: {{state.ServerTime}}</p>
  </div>
</template>

<script>
import dotnetify from 'dotnetify/vue';
export default dotnetify.vue.component('MyApp', 'HelloWorld');
</script>
```

The API's first argument assigns a name to the component; the second argument is the C# view model class to connect with; the optional third one is for the connection options.  Notice that the view model properties are made available from the _state_ property so that you won't need to explicitly define them in the _data_ function.  

#### Two-Way Binding

To keep the properties in the client in sync with the server without explicit __$dispatch__ call, include those property names in the  __watch__ property of the connection options argument:

```html
<template>
  <div>
    <div>{{state.Greetings}}</div>
    <input type="text" v-model="state.Name" />
  </div>`
</template>

<script>
  import dotnetify from 'dotnetify/vue';
  export default dotnetify.vue.component('MyApp', 'HelloWorld', { watch: [ 'Name' ] }),
</script>
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
To get started, you only need these essential APIs to add to your Vue component:

- _vm_ = __dotnetify.vue.connect__(_vmName, component, options_)<br/>

   The options argument is an object with one or more of the following properties:
   - __getState__: function, provide your own component's state accessor.
   - __setState__: function, provide your own component's state mutator.
   - __watch__: array of properties you want to set up two-way binding with the server.
   - __vmArg__: object, initialize view model properties. 
      For example:
      ```jsx
      let initialState = { Person: { FirstName: 'John', LastName: 'Hancock' } };
      dotnetify.vue.connect("HelloWorld", this, { vmArg: initialState });
      ```
   - __headers__: object, pass request headers, e.g. for authentication purpose.
   - __exceptionHandler__: function, handler for server exceptions.


- _vm_.__$dispatch__(_state_)
- _vm_.__$destroy__()

Custom directive:
- __vmOn__: calls a Vue method on server property update.  For example: `data-bind="vmOn: {ServerTime: doSomething}`.

To output debug logs to the browser's Console tab, add `dotnetify.debug = true`.

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