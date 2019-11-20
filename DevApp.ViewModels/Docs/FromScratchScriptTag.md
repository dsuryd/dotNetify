## From Scratch with .NET Core CLI and Script Tag

The following steps will create a simple real-time Hello World ASP.NET Core app using the .NET Core CLI, and use only script tags to include libraries from CDN locations.

Prerequisites:

- .NET Core 3.x SDK.
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
            services.AddSignalR();
            services.AddDotNetify();          
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseWebSockets();
			app.UseDotNetify();          

            app.UseStaticFiles();
			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHub<DotNetifyHub>("/dotnetify");
			});

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
    <div id="App"></div>

    <!-- Polyfills for IE 11 -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/babel-polyfill/7.0.0/polyfill.min.js"></script>

    <script src="https://unpkg.com/react@16/umd/react.production.min.js"></script>
    <script src="https://unpkg.com/react-dom@16/umd/react-dom.production.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/babel-core/5.8.23/browser.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@aspnet/signalr@1/dist/browser/signalr.min.js"></script>
    <script src="https://unpkg.com/dotnetify@latest/dist/dotnetify-react.min.js"></script>

    <script src="HelloWorld.js" type="text/babel"></script>
  </body>
</html>
```
<br/>

##### Add Hello World

Add a new file _wwwroot/HelloWorld.js_ with the following content:
```jsx
class HelloWorld extends React.Component {
	constructor(props) {
		super(props);
		dotnetify.react.connect('HelloWorld', this);
		this.state = { Greetings: '', ServerTime: '' };
	}

	render() {
		return (
			<div>
				<p>{this.state.Greetings}</p>
				<p>Server time is: {this.state.ServerTime}</p>
			</div>
		);
	}
}

ReactDOM.render(<HelloWorld />, document.getElementById('App'));
```

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
