## Get Started

The easiest way to get started is to perform the following step-by-step instructions given in the _Overview_ to create a new project from scratch with either WebPack or script tag.  
[inset]

#### NuGet Packages

There are three published NuGet packages:
- __DotNetify.SignalR__: The essential library for your `ASP.NET Core` web project.  
- __DotNetify.SignalR.Owin__: The essential library for your `ASP.NET Framework` web project.
- __DotNetify.Core__: core .NET Standard library; only for C# class libraries where you plan to group your view models.

#### ASP.NET Core Startup

The following are required in the _Startup.cs_:
```csharp
public void ConfigureServices(IServiceCollection services)
{
   services.AddMemoryCache();
   services.AddSignalR();
   services.AddDotNetify();
   ...
}
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
   app.UseWebSockets();
   app.UseSignalR(routes => routes.MapDotNetifyHub());
   app.UseDotNetify();
   ...
}
```
<br/>

The default configuration assumes all view model classes are in the web project, and uses built-in ASP.NET Core dependency injection. To override the configuration:
```csharp
app.UseDotNetify(config => {

    config.RegisterAssembly(/* name of the assembly where the view model classes are located */);
    config.SetFactoryMethod((type, args) => /* let your favorite IoC library creates the view model instance */);
});
```

#### ASP.NET Framework Startup

ASP.NET Framework does not come with _Startup.cs_, so will need to create one. Add an OWIN Startup Class item, and name it _Startup_.  Then replace the content with the following:
```csharp
using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(MyProject.Startup))]
namespace MyProject
{
  public class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      var vmAssembly = /* assembly where your view models are */;
      app.MapSignalR();
      app.UseDotNetify(config => config.RegisterAssembly(vmAssembly));
    }
  }
}
```

#### Client-Side Library

If you use module bundler like WebPack, install __dotnetify__ and `knockout` from NPM: 
```js
npm i --save dotnetify
npm i --save knockout
```

If using script tags, include _dotNetify_, _jQuery_, _knockout_ and _signalR_ from their respective CDNs:
```html
<script src="https://code.jquery.com/jquery-3.1.1.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/knockout/3.4.2/knockout-min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/@aspnet/signalr@1/dist/browser/signalr.min.js"></script>
<script src="https://unpkg.com/dotnetify@3/dist/dotnetify-ko.min.js"></script>
```

