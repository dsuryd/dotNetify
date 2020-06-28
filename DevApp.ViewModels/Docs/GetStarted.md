## Get Started

#### SPA Template

The easiest way to get started is to use the basic SPA template that contains some examples, and also include a functional login page with JWT authentication.

Prerequisites:

- Node.js
- .NET Core SDK (2.x or 3.x)

Download the template from nuget from the command line, then create your project:

```js
dotnet new -i dotnetify.react.template

dotnet new dotnetify -o MyApp
cd MyApp
npm i
dotnet watch run
```

You can also perform the following step-by-step instructions to create a new project from scratch with either WebPack or script tag.
[inset]

#### NuGet Packages

- **DotNetify.SignalR**: The essential library for your `ASP.NET Core` web project.
- **DotNetify.Core**: core .NET Standard library; only for C# class libraries where you plan to group your view models.

#### Server Setup

Add the following in the _Startup.cs_:

```csharp
public void ConfigureServices(IServiceCollection services)
{
   services.AddSignalR();
   services.AddDotNetify();
   ...
}
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
   app.UseWebSockets();
   app.UseDotNetify();

   // .NET Core 2.x:
   app.UseSignalR(routes => routes.MapDotNetifyHub());

   // .NET Core 3.x:
   app.UseRouting();
   app.UseEndpoints(endpoints => endpoints.MapHub<DotNetifyHub>("/dotnetify"));

   ...
}
```

<br/>

The default configuration assumes all view model classes are in the web project, and uses built-in .NET dependency injection. To override the configuration:

```csharp
app.UseDotNetify(config => {

    config.RegisterAssembly(/* name of the assembly where the view model classes are located */);
    config.SetFactoryMethod((type, args) => /* let your favorite IoC library creates the view model instance */);
});
```

#### Client Setup

If you use module bundler like WebPack, install **dotnetify**, `react`, and `react-dom` from NPM:

```jsx
npm i --save dotnetify react react-dom
```

If using script tags, include _React_, _signalR_, and _dotNetify_ from their respective CDNs:

```html
<script src="https://unpkg.com/react@16/umd/react.production.min.js"></script>
<script src="https://unpkg.com/react-dom@16/umd/react-dom.production.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/@aspnet/signalr@1/dist/browser/signalr.min.js"></script>
<script src="https://unpkg.com/dotnetify@latest/dist/dotnetify-react.min.js"></script>
```

#### .NET Framework

> The library for .NET Framework is no longer maintained. All features and bug fixes introduced after v3.6.1 will only apply to the library for .NET Core. However, private support is possible through sponsorship.

Add the NuGet package - **DotNetify.SignalR.Owin**.

.NET Framework does not come with _Startup.cs_, so will need to create one. Add an OWIN Startup Class item, and name it _Startup_. Then replace the content with the following:

```csharp
using Microsoft.Owin;
using Owin;
using DotNetify;

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

The library uses SignalR .NET Core client by default. To switch to .NET Framework client, add:

```jsx
import signalR from 'dotnetify/dist/signalR-netfx';
dotnetify.hubLib = signalR;
```

If using script tags, add the following:

```html
<script src="https://code.jquery.com/jquery-3.1.1.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/signalr@2/jquery.signalR.min.js"></script>
```
