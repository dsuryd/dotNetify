## Minimal API

If your use case doesn't require much view model logic, e.g. you just want to do a simple data pass-through from an event source to your clients, then you may skip **BaseVM** and apply the minimal API directly to your ASP.NET Core pipeline as below:

```csharp
using DotNetify;
using System.Reactive.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDotNetify().AddSignalR();

var app = builder.Build();

app.MapHub<DotNetifyHub>("/dotnetify");
app.MapVM("TimerVM", () => new { Tick = Observable.Interval(TimeSpan.FromSeconds(1) });

app.Run();
```

Use **MapVM** (or **MapMulticastVM**) to register a named view model. The second parameter accepts an action delegate that returns an anonymous object representing the state to be pushed to the client. You can assign to a property of this object either a primitive value, an observable, or an action with 0 or 1 argument.

In the above example, the `Tick` property is set to an observable that produces an event every second. When you connect this to a client, the event will also be pushed to the client every second.

The action delegate supports dependency injection, and you can use it to inject a service for your view model to subscribe to. For example:

```csharp
app.MapVM("TimerVM", (ITimerService service) => new { Tick = service.GetObservable() };
```

The delegate can be asynchronous:

```csharp
app.MapVM("TimerVM", async (ITimerService service) => new { Tick = await service.GetObservableAsync() };
```

Use the action property type to allow the client to send commands (through `vm.$dispatch`) to your server:

```csharp
app.MapVM("TimerVM", async (ITimerService service) => new
{
  Tick = await service.GetObservableAsync() },
  Reset = new Action(async () => await service.ResetAsync())
}
```

#### Authorization

View models registered by the minimal API can be protected using the **[Authorize]** attribute:

```csharp
app.MapVM("TimerVM", [Authorize] (ITimerService service) => new { Tick = service.GetObservable() };
```
