<p align="center"><img width="350px" src="http://dotnetify.net/content/images/dotnetify-logo.png"></p>

[![NuGet](https://img.shields.io/nuget/v/DotNetify.ResiliencyAddon.svg?style=flat-square)](https://www.nuget.org/packages/DotNetify.ResiliencyAddon/)

## DotNetify-ResiliencyAddon

_DotNetify-ResiliencyAddon_ allows dotNetify server to be more resilient when serving as an HTTP integration backend to an external Websocket API gateway. When configured with a distributed cache such as Redis, existing connections will be able to survive a server restart. Any active view model instance will be recreated and can resume its activities with connected clients when the server is restored.

### Installation

Add the following library from NuGet: **DotNetify.ResiliencyAddon**.

### Setup

Add the following in the _Startup.cs_:

```c#
using DotNetify.WebApi;

...
public void ConfigureServices(IServiceCollection services)
{
   ...
   services.AddDotNetify();
   services.AddDotNetifyIntegrationWebApi();
   services.AddDotNetifyResiliencyAddon();
   services.AddStackExchangeRedisCache(options => options.Configuration = builder.Configuration["Redis:ConnectionString"]);
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
