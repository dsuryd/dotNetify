## Web API Mode
 
When real-time communication isn't a requirement and you don't want to use SignalR, but you still want to write your back-end code using the MVVM pattern, you can specify your UI component to use the web API mode when connecting.

The web API mode will cause your component to send requests to the DotNetify Web API endpoint instead of the usual SignalR hub.   To enable mode, you will need to add the MVC services to the _Startup.cs_, and on the client-side, include the __webApi__ flag in the _connect_ option:

```csharp
using DotNetify.WebApi;

public void ConfigureServices(IServiceCollection services)
{
   services.AddDotNetify();
   services.AddMvc();
   ...
}
public void Configure(IApplicationBuilder app)
{
   app.UseDotNetify();

   // .NET Core 2.x
   app.UseMvc();

   // .NET Core 3.x
   app.UseEndpoints(endpoints => endpoints.MapControllers());
   ...
}
```

```jsx
dotnetify.react.connect("MyVM", this, { webApi: true } });
```

Keep in mind that other than the fact that real-time functionality like _PushUpdates_ and _multicasting_ won't be working in this mode, every API request will create a new instance of the view model and dispose it on completion. Therefore, the correct way to implement a view model for this mode is to make it stateless.

The headers that you've configured in the _connect_ options will be set to the HTTP request headers. The middlewares and filters will continue to work, with the _DotNetifyHubContext_ argument using the information from _HttpContext_.

#### Cross-Origin Support

By default the API uses a relative path.  To direct it to a different domain, create a web API hub client with the new base URL, then implement the __dotnetify.connectHandler__ handler that will return the instance if the _webApi_ option is enabled:

```jsx
const callback = (url, xmlHttpRequest) => console.log("Requesting...", url, xmlHttpRequest);
const webApiHubClient = dotnetify.createWebApiHub("http://my-other-domain.net", callback);

dotnetify.connectHandler = vmConnectArgs => {
  if (vmConnectArgs.options && vmConnectArgs.options.webApi)
   return { ...vmConnectArgs, hub: webApiHubClient };
}
```
