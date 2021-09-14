## From Scratch with .NET Core CLI and Script Tag

The following steps will create a simple real-time Hello World ASP.NET Core app using the .NET Core CLI, and use only script tags to include libraries from CDN locations.

Prerequisites:

- .NET Core 2.1 SDK.
<br/>

##### Create Project

From the command line, run the following:
```js
dotnet new web -o HelloWorld
cd HelloWorld
dotnet add package DotNetify.SignalR
```
<br/>

##### Configure Startup

Open _Startup.cs_ file and replace the content with the following:
```csharp
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using DotNetify;

namespace HelloWorld
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSignalR();
            services.AddDotNetify();          
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.UseSignalR(routes => routes.MapDotNetifyHub());
            app.UseDotNetify();          

            app.UseStaticFiles();
            app.Run(async (context) =>
            {
                using (var reader = new StreamReader(File.OpenRead("wwwroot/index.html")))
                    await context.Response.WriteAsync(reader.ReadToEnd());
            });
        }
    }
}
```
<br/>


##### Add Index Page

Add a new file _wwwroot/index.html_ with the following content:
```html
<html>
  <head>
    <title>DotNetify</title>
    <meta charset="utf-8">
    <meta name="viewport" content="initial-scale=1, width=device-width" />
  </head>
  <body>
    <div data-vm="HelloWorld">
        <p data-bind="text: Greetings" />
        <p data-bind="text: ServerTime" />
    </div>

    <!-- Polyfills for IE 11 -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/babel-polyfill/7.0.0/polyfill.min.js"></script>

    <script src="https://code.jquery.com/jquery-3.1.1.min.js"></script>
    <script src="https://code.jquery.com/ui/1.11.3/jquery-ui.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/knockout/3.4.2/knockout-min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@microsoft/signalr@5/dist/browser/signalr.min.js"></script>
    <script src="https://unpkg.com/dotnetify@3/dist/dotnetify-ko.min.js"></script>
  </body>
</html>
```
<br/>

##### Add Hello World

Add a new file _HelloWorld.cs_ with the following content:
```csharp
using System;
using DotNetify;
using System.Threading;

namespace HelloWorld
{
    public class HelloWorld : BaseVM
    {
        private Timer _timer;
        public string Greetings => "Hello World!";
        public DateTime ServerTime => DateTime.Now;

        public HelloWorld()
        {
            _timer = new Timer(state =>
            {
                Changed(nameof(ServerTime));
                PushUpdates();
            }, null, 0, 1000);
        }

        public override void Dispose() => _timer.Dispose();
    }
}
```
<br/>

##### Build and Run

Run `dotnet run`.  Hello World!
