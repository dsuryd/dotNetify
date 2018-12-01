## .NET Client

DotNetify can be used to let your .NET apps, such as a WPF desktop app, communicate with view models hosted on an ASP.NET Core server.  To do this, add the __DotNetify.SignalR__ package to your project, then use the __DotNetify.Client__ namespace to get to the __DotNetifyClient__ class.

#### Dependency Injection

You can manually instantiate __DotNetifyClient__ and its dependencies, but it is recommended to use the DI container from the _Microsoft.Extensions.DependencyInjection_ assembly:

```csharp
using System;
using DotNetify;
using DotNetify.Client;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp
{
   public static class ServiceProvider
   {
      private static IServiceProvider _serviceProvider;

      static ServiceProvider()
      {
         _serviceProvider = new ServiceCollection()
            .AddDotNetifyClient()
            .BuildServiceProvider();
      }

      public static T Resolve() => _serviceProvider.GetRequiredService<T>();
   }
}
```

Then get a dotNetify client instance by: 
```csharp
IDotNetifyClient dotnetify = ServiceProvider.Resolve<IDotNetifyClient>();
```

#### Usage

Start by creating a class that will serve as a proxy to the server-side view model.  It needs to implement _INotifyPropertyChanged_ and _IDisposable_ interfaces, and provide public properties with the same names and types to its counterpart:

```csharp
public class HelloWorldProxy : INotifyPropertyChanged, IDisposable
{
   private readonly IDotNetifyClient _dotnetify;

   public string Greetings { get; set; }
   public string ServerTime { get; set; }

   public HelloWorldProxy(IDotNetifyClient dotnetify)
   {
      _dotnetify = dotnetify;
      _dotnetify.ConnectAsync("HelloWorld", this);
   }

   public void Dispose() => _dotnetify.Dispose();
}
```

The APIs are similar to the Javascript client's:
- __ConnectAsync__(_string vmName, INotifyPropertyChanged proxy, VMConnectOptions options_)<br/>

   The options object provide the following properties:
   - __VMArg__: object to initialize view model properties. 
      For example:
      ```csharp
      _dotnetify.ConnectAsync("HelloWorld", this, 
         new VMConnectOptions { VMArg = new { Greetings = "Hello!" } });
      ```
   - __Headers__: object, pass request headers, e.g. for authentication purpose.


- __DispatchAsync__(_string propertyName, object propertyValue_)
- __DispatchAsync__(_Dictionary<string, object> propertyValues_)
- __DisposeAsync__()

#### UI Dispatcher

If you use this with a UI framework, update to _ObservableCollection_ objects may require the operation to be conducted in the UI thread.  You need to provide your own implementation of the __IUIDispatcher__ interface and include it in the service registration.  For example:

```csharp
// WPF
public class WpfUIThreadDispatcher : IUIThreadDispatcher
{
   public Task InvokeAsync(Action action) => Application.Current.Dispatcher.InvokeAsync(action);
}

// Avalonia
public class AvaloniaUIThreadDispatcher : IUIThreadDispatcher
{
   public Task InvokeAsync(Action action) => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(action);
}
```
