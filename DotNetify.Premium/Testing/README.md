<p align="center"><img width="350px" src="http://dotnetify.net/content/images/dotnetify-logo.png"></p>

[![NuGet](https://img.shields.io/nuget/v/DotNetify.Testing.svg?style=flat-square)](https://www.nuget.org/packages/DotNetify.Testing/)

## DotNetify-Testing

_DotNetify-Testing_ provides a simple and effective way to unit-test dotNetify view models. It emulates the SignalR hub and client-side connections so you can write tests that closely mimic the way your application would interact with real clients in real-time.

### Hub Emulator

Start by creating a new instance of **HubEmulator** through **HubEmulatorBuilder**:

```csharp
var hubEmulator = new HubEmulatorBuilder()
  .Register<HelloWorldVM>(nameof(HelloWorldVM))
  .Build();
```

The builder methods are:

- **Register**: register the view model types to test.
- **AddServices**: add services or service stubs that are required by the registered view models.
- **UseMiddleware**: add a dotNetify middleware to the emulated hub pipeline.
- **UseFilter**: add a dotNetify filter to the emulated hub pipeline.

Example:

```csharp
var hubEmulator = new HubEmulatorBuilder()
  .Register<HelloWorldVM>(nameof(HelloWorldVM))
  .AddServices(services =>
  {
    services.AddTransient<IMyService>(provider => myServiceStub);
  })
  .UseMiddleware<JwtBearerAuthenticationMiddleware>()
  .UseFilter<AuthorizeFilter>()
  .Build();
```

### Hub Client

```csharp
var client = hubEmulator.CreateClient();
```

A client represents a single hub connection. You can optionally assign a specific connection ID or a user identity to associate with the connection.

## Connect to View Model

Connect the hub client to a view model as you would on a real client and get the initial state:

```csharp
client.Connect(nameof(HelloWorldVM));
var state = client.GetState<IHelloWorldState>();

Assert.Equal("Hello World", state.Greetings);
```

The generic method **GetState** always returns the latest client state.

### Client Dispatch

```csharp
client.Connect(nameof(SimpleListVM));
client.Dispatch(new { Add = "Clive Lewis" });

var state = client.GetState<ISimpleListState>();

Assert.Contains(state.Employees, x => x.FullName == "Clive Lewis");
```

The **Dispatch** method has several overloads so it can accept either a dynamic object, a dictionary of objects, or a JSON-serialized string.

### Checking Actual Responses

The previous example asserts on the client state after a dispatch. But if you want to assert on the actual server response, use the return value of the **Dispatch**:

```csharp
var responses = client.Dispatch(new { Name = "Clive Lewis" });
var firstResponse = responses.As<IHelloWorldState>();

Assert("Hello Clive Lewis", firstResponse.Greetings);
```

Notice that the return type is actually a list of responses. Even though the call is synchronous, the underlying communication is not. It is possible that a view model can return multiple responses in a span of time the emulator is set to wait for responses. **_The default wait time is 1 second with maximum responses of 1_**, so by default it will always return with 0 or 1 response. You can change these settings on the client object itself:

```csharp
client.ResponseTimeout = 2000; // in milliseconds.
client.MaxResponses = 10;
```

A response object contains raw data as produced by the server, which you can convert to a type with the **As** generic method.

### Listening for Server Updates

If the responses are initiated by the server through _PushUpdates_, use the **Listen** method:

```csharp
client.Connect(nameof(LiveChartVM));
var updates = client.Listen(5500);

Assert.Equal(5, updates.Count);
```

This method allows you to aggregate updates from the server with the span of time specified by the argument.

In some cases involving _MulticastVM_, you may want to check whether one client's action will trigger server updates to another client. Calling **Listen** right after the action runs a risk of a race condition. To ensure that it starts listening before the action takes place, you hae two options:

1. Use the overload that accepts an action argument:

```csharp
var client1 = _hubEmulator.CreateClient();
var client2 = _hubEmulator.CreateClient();

client1.Connect(nameof(ChatRoomVM));
client2.Connect(nameof(ChatRoomVM));

var client1Responses = client1.Listen(() =>
{
  client2.Dispatch(new { SendMessage = new { Text = "Hi", UserName = "Billy" } });
});

var response = client1Responses.As<IChatRoomState>();
Assert.Equal("Hi", response.Message.Text);
Assert.Equal("Billy", response.Message.UserName);
```

2. Use the asynchronous overload:

```csharp
var client1ResponsesTask = client1.ListenAsync();

client2.Dispatch(new { SendMessage = new { Text = "Hi", UserName = "Billy" } });

var client1Response = await client1ResponsesTask;
```

### Client Destroy

To remove a client from the hub emulator, use the _Destroy_ method.

```csharp
client.Destroy();
```

### Connect to Live Hub

You can use the same client API to connect to a live hub with **LiveHub**:

```csharp
using (var client = await LiveHub.CreateClientAsync("https://my-webapp"))
{
  var responses = client.Connect(nameof(HelloWorldVM));
  var response = responses.As<IHelloWorldState>();
  Assert.Equal("Hello World", response.FullName);
}
```
