## Web API Mode
 
When real-time communication is not a requirement and you don't want to use SignalR, but you still want to write your back-end code using the MVVM (or Reactive MVVM) pattern, you can specify your UI component to use the web API mode when connecting.

The Web API mode will cause your component to send requests to the DotNetify Web API endpoint instead of the usual SignalR hub:

```jsx
dotnetify.react.connect("MyVM", this, { webApi: true } });
```

You'll need to enable the mode and add the MVC services to your server with the following configuration in the _Startup.cs_:

```csharp
using DotNetify.WebApi;

public void ConfigureServices(IServiceCollection services)
{
   services.AddDotNetify();
   services.AddDotNetifyWebApi();
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

It's important to understand that, other than the obvious fact that real-time functionality like _PushUpdates_ and _multicasting_ won't be working in this mode, every API request will create a new instance of the view model and dispose it on completion. Therefore, the correct way to implement a view model for this mode is to make it stateless.

#### Custom Base URL

By default the API uses a relative path.  To give it a different base URL, create a web API hub client, initialized to the new base URL, then implement the __dotnetify.connectHandler__ handler in which you return the instance:

```jsx
import dotNetify, { webApiHub } from 'dotnetify';

const webApiHubClient = new webApiHub("http://my-domain.com");

dotNetify.connectHandler = vmConnectArgs => {
  if (vmConnectArgs.options && vmConnectArgs.options.webApi)
    return webApiHubClient;
}
```
