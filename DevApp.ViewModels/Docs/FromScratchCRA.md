## From Scratch with CRA

The following steps will create a simple real-time Hello World ASP.NET Core app from `create-react-app`.

Prerequisites:

- Node.js

[inset]

##### Create Project

From the command line, run the following:

```jsx
npx create-react-app helloworld
cd helloworld

npm i --save dotnetify
npm i --save tslib

dotnet new web
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

namespace helloworld
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();
        services.AddMemoryCache();
        services.AddSignalR();
        services.AddDotNetify();
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseCors(builder => builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithOrigins("http://localhost:3000")
        .AllowCredentials());

      app.UseWebSockets();
      app.UseSignalR(routes => routes.MapDotNetifyHub());
      app.UseDotNetify();

      app.Run(async (context) =>
      {
        await context.Response.WriteAsync("Hello World!");
      });
    }
  }
}
```

<br/>

##### Configure NPM

Add the following settings to _package.json_:

```js
  "proxy": "http://localhost:5000/"
```

<br/>

##### Add Hello World

Add a new file _src/HelloWorld.js_ with the following content:

```jsx
import React from "react";
import dotnetify from "dotnetify";

dotnetify.hubServerUrl = "http://localhost:5000";

export default class HelloWorld extends React.Component {
  constructor(props) {
    super(props);
    dotnetify.react.connect("HelloWorld", this);
    this.state = { Greetings: "", ServerTime: "" };
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

Add the _HelloWorld_ component in _src/App.js_:

```jsx
import React, { Component } from "react";
import logo from "./logo.svg";
import "./App.css";
import HelloWorld from "./HelloWorld";

class App extends Component {
  render() {
    return (
      <div className="App">
        {/* ... */}
        <HelloWorld />
      </div>
    );
  }
}

export default App;
```

Add a new file _HelloWorld.cs_ with the following content:

```csharp
using System;
using DotNetify;
using System.Threading;

namespace helloworld
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

Open a new terminal and run `dotnet run`. On another terminal, run `npm start`. Hello World!
