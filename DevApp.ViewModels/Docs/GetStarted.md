## Get Started

#### SPA Template
The easiest way to get started to use the basic SPA template that contains some examples, and also include a functional login page with JWT authentication.

Prerequisites:
- Node.js
- .NET Core SDK (v2.1 and up)

Download the template from nuget from the command line, then create your project:
```js
dotnet new -i dotnetify.react.template

dotnet new dotnetify -o MyApp
cd MyApp
npm i
dotnet watch run
```

You can also perform the step-by-step instructions given in the _Overview_ to create a new project from scratch with either WebPack or script tag.

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
<br/>

#### Client-Side Library

If you use module bundler like WebPack, install __dotnetify__ from NPM: 
```js
npm i --save dotnetify
```

You must also include `react` and `react-dom`.   

If using script tags, you must include at the _React_ libraries and _SignalR_, from their respective CDNs:
```html
<script src="https://unpkg.com/react@16/umd/react.production.min.js"></script>
<script src="https://unpkg.com/react-dom@16/umd/react-dom.production.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/@aspnet/signalr@1/dist/browser/signalr.min.js"></script>
<script src="https://unpkg.com/dotnetify@3/dist/dotnetify-react.min.js"></script>
```

