## Middleware

Middlewares are essentially objects that have access to raw client requests before they are resolved, server responses before they are sent to the client, and can either pass control to the next middleware in the pipeline, halt the execution, or throw an exception that gets delivered back to the client.

Middlewares can play many useful roles, such as creating audit logs, preprocessing data, and authenticating requests. In fact, dotNetify employs its own internal middleware to extract request headers out of the payload, and provides _JwtBearerAuthenticationMiddleware_ to perform token based authentication, and _DeveloperLoggingMiddleware_ to output logs of all incoming and outgoing messages.

To write your own middleware, create a class that inherits from __IMiddleware__. It can be injected with registered services, or passed arguments. It will be executed every time the server receives a request to create or update a view model, and when a response or push update is sent. Here's one example:

```csharp
public class MyLoggingMiddleware : IMiddleware
{
   private readonly ILoggingService _loggingService;

   public MyLoggingMiddleware(ILoggingService loggingService)
   {
      _loggingService = loggingService;
   }

   public Task Invoke(DotNetifyHubContext hubContext, NextDelegate next)
   {
      _loggingService.WriteLine($"{hubContext.CallType} vmId={hubContext.VMId}");

      return next(hubContext);
   }
}
```

The Invoke method receives a __DotNetifyHubContext__ object with the following properties:

- __CallerContext__: SignalR _HubCallerContext_ object.
- __CallType__: indicates whether it's a view model request, update, or server response.
- __VMId__: identifies the view model.
- __Data__: data sent from the client.
- __Headers__: headers sent from the client, if set in the __connect__ API.
- __PipelineData__: a dictionary for passing data from a middleware or filter to the next one in the pipeline.

A middleware must call next to pass control to the next middleware, or the execution will halt. If the middleware throws an exception other than _OperationCanceledException_, the exception type name and message will be sent back to the client, giving the opportunity for the UI to react according to the exception type.

#### Middleware Registration

To register the middleware to the pipeline, add it to dotNetify's configuration with __UseMiddleware<T>__.

```csharp
public void Configuration(IAppBuilder app)
{
   ...
   app.UseDotNetify(config => {
      config.UseMiddleware<MyLoggingMiddleware>();
   });
}
```

The order of registrations determine the order of their executions in the pipeline. If the middleware requires a certain argument, it's recommended to create an extension method to strong-type the argument such as this:

```csharp
public static class MyMiddlewareExtension
{
   public static void UseMyMiddleware(this IDotNetifyConfiguration config, MyArgument arg)
   {
      config.UseMiddleware<MyMiddleware>(arg);
   }
}
```

#### Exception Middleware

An exception middleware is an object that inherits from __IExceptionMiddleware__, and is executed when an exception is thrown from anywhere in the pipeline. The purpose is to provide the opportunity to implement an exception handling strategy, by allowing the middleware to transform the type of exception before it reaches the client. For example:

```csharp
public class MyExceptionMiddleware : IExceptionMiddleware
{
   public Task<Exception> OnException(HubCallerContext context, Exception exception)
   {
      var newException = exception is SqlException ? new ApplicationException("Database error") : exception;
      return Task.FromResult(newException);
   }
}
```

#### Disconnection Middleware

Another type of middleware inherits from __IDisconnectionMiddleware__, and is executed when a disconnected event occurs:

```csharp
public class MyDisconnectionMiddleware : IDisconnectionMiddleware
{
   public Task OnDisconnected(HubCallerContext context)
   {
      Trace.Writeline($"{context.ConnectionId} is disconnected");
      return Task.CompletedTask;
   }
}
```