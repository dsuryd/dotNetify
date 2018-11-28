## From Scratch with Visual Studio and WebPack

The following steps will create a simple real-time Hello World ASP.NET Core app from Visual Studio. It will use WebPack with hot-reload feature to build the client-side code.

Prerequisites:

- Visual Studio 2017
- Node.js

##### Create Project

Create an empty ASP.NET Core Web Application (.NET Core 2.1) project and name it _HelloWorld_.  Then use the NuGet Package Manager Console to install the dotNetify package:
```csharp
install-package DotNetify.SignalR
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
using Microsoft.AspNetCore.SpaServices.Webpack;
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

            // Optional: utilize webpack hot reload feature.
            app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
            {
                HotModuleReplacement = true,
                HotModuleReplacementClientOptions = new Dictionary<string, string> { { "reload", "true" } },
            });            

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

##### Configure NPM

Add NPM configuration file _package.json_ with the following content:
```json
{
  "name": "helloworld",
  "scripts": {
    "build": "webpack"
  },
  "babel": {
    "presets": [
      "env",
      "react"
    ]
  },
  "dependencies": {
    "dotnetify": "~3.3.0",
    "react": "~16.5.0",
    "react-dom": "~16.5.0"
  },
  "devDependencies": {
    "aspnet-webpack": "^3.0.0",
    "babel-core": "~6.26.3",
    "babel-loader": "~7.1.4",
    "babel-preset-env": "~1.7.0",
    "babel-preset-react": "~6.24.1",
    "webpack": "^4.18.0",
    "webpack-cli": "^3.1.0",
    "webpack-dev-middleware": "^3.3.0",
    "webpack-hot-middleware": "^2.23.1"
  }
}
```

The packages will be automatically downloaded by Visual Studio when the file is saved.
<br/>

##### Configure WebPack

Add _webpack.config.js_ with the following content:
```js
'use strict';

module.exports = {
  mode: 'development',
  entry: { main: './src/index.js' },
  output: {
    path: __dirname + '/wwwroot/dist',
    publicPath: '/dist/'
  },
  resolve: {
    modules: [ 'src', 'node_modules' ]
  },
  module: {
    rules: [ { test: /\.jsx?$/, use: 'babel-loader', exclude: /node_modules/ } ]
  }
};
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
    <script src="dist/main.js" charset="UTF-8"></script>
  </body>
</html>
```
<br/>

##### Add Hello World

Create a new folder _src_, and add a new file _src/index.js_ with the following content:
```jsx
import React from 'react';
import ReactDOM from 'react-dom';
import HelloWorld from './HelloWorld';

ReactDOM.render(<HelloWorld />, document.getElementById('App'));
```

Add a new file _src/HelloWorld.js_ with the following content:
```jsx
import React from 'react';
import dotnetify from 'dotnetify';

export default class HelloWorld extends React.Component {
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

Run the application.  Hello World!
