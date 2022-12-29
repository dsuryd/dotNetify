## DotNetify-ResiliencyAddon

_DotNetify-ResiliencyAddon_ allows your dotNetify app server to be more resilient when serving as an HTTP integration backend to the Amazon WebSocket API gateway. When configured with a distributed cache such as Redis, existing connections will be able to survive a server restart. Any active view model instance will be recreated and can resume its activities with connected clients when the server is restored.

<d-alert info="true">

<b>This is a closed-source library for Pro, Team, and Enterprise sponsors.</b> If you are one, send an email to _admin@dotnetify.net_ with your username to get your license key.

- Pro sponsor: single developer, up to 100 client connections.
- Team sponsor: up to 10 developers, unlimited usage + private email support.
- Enterprise sponsor: unlimited usage + private email support.

</d-alert>

#### Installation

Add **DotNetify.ResiliencyAddon** from NuGet to your project.

#### Setup

Add the following in the _Startup.cs_:

```csharp
using DotNetify.WebApi;

...
public void ConfigureServices(IServiceCollection services)
{
   ...
   /* Place this after services.AddDotNetifyIntegrationWebApi() */
   services.AddDotNetifyResiliencyAddon();
   services.AddStackExchangeRedisCache(options => options.Configuration = "<redis-connection-string>");
   ...
}

public void Configure(IApplicationBuilder app)
{
   ...
   app.UseDotNetify();
   app.UseDotNetifyResiliencyAddon();
   ...
}
```
