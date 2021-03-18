## DotNetify-Observer

_DotNetify-Observer_ is a visualization dashboard that lets you see and inspect all active connections to your dotNetify applications in real-time.  

<div style="display:flex;justify-content:center;padding:1.5rem">
  <video width="80%" controls style="border: 1px solid #ccc; box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19)">
    <source src="https://dotnetify.net/Content/Videos/observer-demo.mp4" type="video/mp4">
  </video>
</div>

<d-alert info="true">

<b>This is a closed source library for Pro, Team, and Enterprise sponsors.</b> If you are one, send an email to _admin@dotnetify.net_ with your username to get your license key.  

- Pro sponsor: single developer, web endpoint only.
- Team sponsor: up to 10 developers, up to 5 server nodes + private email support.
- Enterprise sponsor: up to 25 server nodes + private email support.

</d-alert>

#### Installation

_DotNetify-Observer_ supports two configuration types:
- as a web endpoint on your dotNetify app server (live demo: https://dotnetify.net/observer)
- as a standalone web service for visualizing multiple servers (see the video demo in the [Scale-Out page](/core/scaleout)).

<br/>

##### Web Endpoint

Add **DotNetify.Observer** from NuGet to your dotNetify server project, then update the _Startup.cs__ as below:

```csharp
using DotNetify.Observer;
...

public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
      ...
      services.AddDotNetifyObserver();
  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    ...
    app.UseDotNetify(config =>
    {
      config.ConfigureObserver();  /* Place this before any middleware */
    });
    
    app.UseRouting();
    app.UseStaticFiles();
    app.UseEndpoints(endpoints =>
    {
      ...
      endpoints.MapObserver("/observer"); /* Endpoint path */
    });
  }
}
```

<br/>

##### Web Service

The first step is to create a new ASP.NET Core project with **DotNetify.Observer** NuGet package, and update the _Startup.cs_ as below:

```csharp
using DotNetify;
using DotNetify.Observer; 
...

public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
      services.AddSignalR();
      services.AddDotNetify();
      services.AddDotNetifyObserver();
  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
      app.UseWebSockets();
      app.UseDotNetify(config =>
      {
        config.ConfigureObserverHost();
      });

      app.UseRouting();
      app.UseStaticFiles();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapHub<DotNetifyHub>("/dotnetify");
        endpoints.MapObserver("/");
      });
  }
}
```

The next step is to configure your dotNetify server so it will forward messages to the above web service.  Add **DotNetify.Observer** NuGet package and update the _Startup.cs_ as below: 

```csharp
public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
      ...
      services.AddDotNetifyObserverClient();
  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    ...
    app.UseDotNetify(config =>
    {
      config.ConfigureObserverClient("<observer_web_service_url>");  
    });
  }
}
```


