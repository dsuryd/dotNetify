## Web API Mode
 
When real-time communication is not a requirement and you don't want to use SignalR, but you still want to write your back-end code using the MVVM (or Reactive MVVM) pattern, you can specify your UI component to use the web API mode when connecting.

The Web API mode will cause your component to send requests to the DotNetify Web API endpoint instead of the usual SignalR hub.   To enable mode, you will need to add the MVC services to the _Startup.cs_, and on the client-side, include the __webApi__ flag in the _connect_ option:

```csharp
using DotNetify.WebApi;

public void ConfigureServices(IServiceCollection services)
{
   services.AddDotNetify();
   services.AddMvc();
   ...
}
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
   app.UseDotNetify();
   app.UseMvc();
   ...
}
```

```jsx
dotnetify.react.connect("MyVM", this, { webApi: true } });
```

It's important to understand that, other than the obvious fact that real-time functionality like _PushUpdates_ and _multicasting_ won't be working in this mode, every API request will create a new instance of the view model and dispose it on completion. Therefore, the correct way to implement a view model for this mode is to make it stateless.

The headers that you've configured in the _connect_ options will be set to the HTTP request headers. The middlewares and filters will continue to work, with the _DotNetifyHubContext_ argument using the information from _HTTPContext_.

#### Cross-Domain Support

By default the API uses a relative path.  To direct it to a different domain, create a web API hub client with the new base URL, then implement the __dotnetify.connectHandler__ handler that will return the instance if the _webApi_ option is enabled:

```jsx
const callback = (url, request /*XMLHttpRequest*/) => console.log("Requesting...", url, request)
const webApiHubClient = dotNetify.createWebApiHub("http://my-domain.com", callback);

dotNetify.connectHandler = vmConnectArgs => {
  if (vmConnectArgs.options && vmConnectArgs.options.webApi)
    return webApiHubClient;
}
```
