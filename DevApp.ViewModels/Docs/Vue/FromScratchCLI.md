## From Scratch with Vue CLI

The following steps will create a simple real-time Hello World ASP.NET Core app from Vue CLI.

Prerequisites:

- Node.js
- Vue CLI (`npm install -g @vue/cli`)

[inset]

##### Create Project

From the command line, run the following:

```jsx
vue create helloworld
cd helloworld

npm i --save dotnetify

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
        .WithOrigins("http://localhost:8080")
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

##### Configure Vue

Add a new file _vue.config.js_ with the following content:

```jsx
module.exports = {
  devServer: {
    proxy: {
      "/dotnetify": { target: "http://localhost:5000" }
    }
  }
};
```

<br/>

##### Add Hello World

Replace the content of _src/components/HelloWorld.vue_ with the following:

```html
<template>
  <div class="hello">
    <h1>{{ msg }}</h1>
    <div>
      <h3>{{ state.Greetings }}</h3>
      <p>Server time is: {{ state.ServerTime }}</p>
    </div>
  </div>
</template>

<script>
  import dotnetify from "dotnetify/vue";
  export default dotnetify.vue.component(
    {
      name: "hello-world",
      props: { msg: String }
    },
    "HelloWorld"
  );
</script>

<style scoped>
  h3 {
    font-weight: 500;
  }
</style>
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

Open a new terminal and run `dotnet run`. On another terminal, run `npm run serve`. Hello World!
