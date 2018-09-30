## Reactive Programming

Most view model examples you see here use the imperative style, in which the properties that receive data from the client would also perform side-effects in response to the data, like mutating other properties or performing some actions. Here's one such example:

```csharp
public class HelloWorldVM : BaseVM
{
   public string FirstName
   {
      get { return Get<string>() ?? "Hello"; }
      set { Set(value); Changed(nameof(FullName)); }
   }
   public string LastName
   {
      get { return Get<string>() ?? "World"; }
      set { Set(value); Changed(nameof(FullName)); }
   }
   public string FullName => $"{FirstName} {LastName}";
}
```

Reactive programming offers another paradigm that could make your code cleaner, more declarative and more maintainable in the face of rising complexity. In this paradigm, data mutation is viewed as a stream of discrete events that can be subscribed and acted on by independent observers. The same example is now refactored into reactive style, utilizing the _System.Reactive_ library:

```csharp
public class HelloWorldVM : BaseVM
{
   public HelloWorldVM()
   {
      ReactiveProperty<string> firstName = AddProperty("FirstName", "Hello");
      ReactiveProperty<string> lastName = AddProperty("LastName", "World");

      AddProperty<string>("FullName")
        .SubscribeTo(Observable.CombineLatest(firstName, lastName, (fn, ln) => $"{fn} {ln}"));
   }
}
```

We turn the _FirstName_ and _LastName_ properties into _ReactiveProperty_ objects, which means they can be subscribed on. They are dynamically added into the view model with the __AddProperty__ method.

The property _FullName_ doesn't directly receive data from the client, but it subscribes to the other reactive properties using __SubscribeTo__, and does further transformation with them with the APIs provided by _System.Reactive.LINQ_.

There are several observed benefits to using this style:

- separation is maintained between the data publishers (observables) and the subscribers (observers),
- there is no more need to expose view model state through property getters/setters,
- explicit subscriptions clearly show dependencies and side-effects, making things easy to track, and
- arguably the greatest benefit: the ability for an observer to subscribe to multiple asynchronous data streams and manipulate them declaratively through a powerful set of LINQ functions that can transform, combine, filter and create derivative stream.

For more information on reactive programming, I recommend to start here: [Intro to Rx](http://introtorx.com), and the [Reactive Extensions](http://reactivex.io).

#### Reactive API Summary

- this.__AddProperty<T>__(_propName, propValue_): add a reactive property.
- this.__AddInternalProperty<T>__(_propName, propValue_): add a server-only reactive property; the value won't be sent to the client.
- _ReactiveProperty_.__SubscribeTo__(_observable_)<br/>_ReactiveProperty_.__SubscribeToAsync__: subscribe the reactive property to an observable.
- _ReactiveProperty_.__SubscribedBy__(_reactiveProp, mapper_)<br/>_ReactiveProperty_.__SubscribedByAsync__: subscribe the reactive property in the argument to self.

> Caution: values produced by the async methods require explicit call to __PushUpdates__ to be sent to the client.

#### Dynamic Registration

Given the reactive properties can be declared at runtime, you can also register your reactive view models at runtime, although it's recommended only if your use case requires it:

```csharp
app.UseDotNetify(config =>
{
   config.Register("HelloWorldVM", _ => 
   {
      var vm = new BaseVM();
      var firstName = vm.AddProperty("FirstName", "Hello");
      var lastName = vm.AddProperty("LastName", "World");
      vm.AddProperty<string>("FullName")
         .SubscribeTo(Observable.CombineLatest(firstName, lastName, (fn, ln) => $"{fn} {ln}"));
      return vm;      
   });
}
```