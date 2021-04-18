## From Scratch with Visual Studio and WebPack

The following steps will create a simple real-time Hello World ASP.NET Core app from Visual Studio. It will use WebPack with hot-reload feature to build the client-side code.

Prerequisites:

- Visual Studio 2019 with .NET 5 SDK
- Node.js

##### Create Project

Create an empty ASP.NET 5 Web Application project and name it _HelloWorld_. Then use the NuGet Package Manager Console to install the dotNetify package:

```csharp
install-package DotNetify.SignalR
install-package BrunoLau.SpaServices
```

<br/>

##### Configure Startup

Open _Startup.cs_ file and replace the content with the following:

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using BrunoLau.SpaServices.Webpack;
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

            if (env.IsDevelopment())
              app.UseWebpackDevMiddlewareEx(new WebpackDevMiddlewareOptions { HotModuleReplacement = true });


            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<DotNetifyHub>("/dotnetify");
                endpoints.MapFallbackToFile("index.html");
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
  "private": true,
  "scripts": {
    "build": "webpack"
  },
  "babel": {
    "presets": ["env", "react"]
  },
  "dependencies": {
    "dotnetify": "^5.0.1",
    "react": "^17.0.2",
    "react-dom": "^17.0.2"
  },
  "devDependencies": {
    "@types/react": "^17.0.3",
    "@types/react-dom": "^17.0.3",
    "babel-core": "~6.26.3",
    "babel-loader": "~7.1.4",
    "babel-preset-env": "~1.7.0",
    "babel-preset-react": "~6.24.1",
    "ts-loader": "~7.0.4",
    "typescript": "~3.9.3",
    "webpack": "~4.18.0",
    "webpack-cli": "~3.3.11",
    "webpack-dev-middleware": "~3.3.0",
    "webpack-hot-middleware": "~2.23.1"
  }
}
```

The packages will be automatically downloaded by Visual Studio when the file is saved.
<br/>

##### Configure WebPack

Add _webpack.config.js_ with the following content:

```js
"use strict";

module.exports = {
  mode: "development",
  entry: { main: "./src/index" },
  output: {
    path: __dirname + "/wwwroot/dist",
    publicPath: "/dist/"
  },
  devtool: "source-map",
  resolve: {
    extensions: [".tsx", ".ts", ".js"],
    modules: ["src", "node_modules"]
  },
  module: {
    rules: [
      { test: /\.jsx?$/, use: "babel-loader", exclude: /node_modules/ },
      { test: /\.tsx?$/, use: "ts-loader", exclude: /node_modules/ }
    ]
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
    <meta charset="utf-8" />
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
import React from "react";
import ReactDOM from "react-dom";
import HelloWorld from "./HelloWorld";

ReactDOM.render(<HelloWorld />, document.getElementById("App"));
```

Add a new file _src/HelloWorld.tsx_ with the following content:

```tsx
import React from "react";
import { useConnect } from "dotnetify";

interface State {
  Greetings: string;
  ServerTime: string;
}

export const HelloWorld = () => {
  const { state } = useConnect<State>("HelloWorld", this);
  return (
    <div>
      <p>{state.Greetings}</p>
      <p>Server time is: {state.ServerTime}</p>
    </div>
  );
};
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

Run the application. Hello World!
