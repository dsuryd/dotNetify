## Scale-Out

SignalR connections are persistent and will stay open as long as the application remains on the client browser, even when it's idle.  If the number of concurrent clients is high enough, your server may run out of resources and start to throw connection errors.  In this case, the solution is to scale the application horizontally to multiple servers.

If your dotNetify view models only serve one client per instance, you can use a load balancer with session affinity (or "sticky sessions") to direct the traffic to any one of the servers.  But if you are utilizing a multicast view model, this method won't work because a particular server has no visibility to the other server's connections.  To solve this, Microsoft provides [two scale-out options](https://docs.microsoft.com/en-us/aspnet/core/signalr/scale): using a backplane (typically with Redis) or the Azure SignalR Service.

Between the two options, the proxy technique used by Azure makes the most sense because with proxy servers handling the connections, you don't have to scale out the app server itself nor maintain a Redis server.  However, if deploying to Azure is not really an option for you, dotNetify provides a middleware to implement a similar techique that you can use on-prem or with other cloud providers.

#### Forwarding Middleware

The forwarding middleware is designed to facilitate forwarding of client messages from one dotNetify server to another, including passing the message responses back to the forwarding server and to the client.  With this middleware, we can create a scale-out configuration where we have multiple proxy servers that sit behind a load balancer to handle client connections and forward all communication to a dotNetify app server that host all the view models. 

A proxy server is simply a bare-bone ASP.NET Core server with the following startup configuration:

```csharp
using DotNetify;
using DotNetify.Forwarding;
...
namespace ProxyServer
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
         app.UseDotNetify(config =>
         {
            config.UseForwarding("<app_server_url>");
         });
         app.UseRouting();
         app.UseEndpoints(endpoints => endpoints.MapHub<DotNetifyHub>("/dotnetify"));
      }
   }
}
```

The **UseForwarding** API accepts the following options:

```csharp
public class ForwardingOptions
{
  // Number of connections to the app server. 
  public int ConnectionPoolSize { get; set; } = 5;

  // Predicate to decide whether to forward a message.
  public Func<DotNetifyHubContext, bool> Filter { get; set; }

  // Use the message pack protocol to connect to the app server.
  public bool UseMessagePack { get; set; } = false;

  // Prevent further message processing in this server after being forwarded.
  public bool HaltPipeline { get; set; } = true;
}
```

The __ConnectionPoolSize__ option specifies the number of persistent connections between the proxy server and the app server.  Messages from the client will be forwarded through these connections in a round-robin fashion.  If you expect a high message throughput, increase this number to avoid it becoming a bottleneck.

Forwarding middlewares can be chained together to support a more robust scale-out strategy involving multiple app servers.  With the __Filter__ option, you can have a specific app server process certain types of client requests.  For example:

```csharp
app.UseDotNetify(config =>
{
  config
    .UseForwarding("<app_server_1_url>", options => { options.Filter = context => context.VMId == "broadcast"; })
    .UseForwarding("<app_server_2_url>", options => { options.Filter = context => context.VMId == "chatroom"; })
    // otherwise, process the request here...
    ;
});
```

#### Demonstration

The following video demonstrates a cloud configuration with two proxy servers forwarding messages to an app server.  The servers were containerized and deployed to the Heroku cloud platform.  

The visualization was provided by [DotNetify-Observer](/core/dotnetify-observer), in which the servers were represented as green nodes, and the clients the smaller grey nodes.  [DotNetify-LoadTester](/core/dotnetify-loadtester) was used to simulate client connections to the proxy servers.

<div style="display:flex;justify-content:center;padding:1.5rem">
  <video width="80%" controls style="border: 1px solid #ccc; box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19)">
    <source src="https://dotnetify.net/Content/Videos/scaleout-demo.mp4" type="video/mp4">
  </video>
</div>
