## AWS Integration

Even though by default DotNetify uses SignalR for client-server communication, it can switch to use the browser's native WebSocket API and communicate to other WebSocket server implementations, most notably Amazon API Gateway, which is a great option for scaling out.

With Amazon API Gateway, you won't have to worry about self-hosting a web server that can handle large-scale WebSocket connections. This fully-managed service will maintain persistent connections and handle the message transfer between the clients and your dotNetify application server through built-in HTTP integration endpoints.

[inset]

<br/>

To get your dotNetify application communicating through Amazon API Gateway, first you need to do the following tasks:

- Create, configure, and deploy a WebSocket API on Amazon API Gateway.
- Create an IAM authorization for allowing access to the API.
- Configure the HTTP integration endpoints and the IAM credentials on the dotNetify app server.
- Point the dotNetify client to the API's WebSocket URL.

#### AWS Setup

Go to your AWS Management Console, and use the below template to add a new WebSocket API through the CloudFormation service.

[**dotnetify-cloudformation-template.yaml**](https://github.com/dsuryd/dotNetify/blob/master/dotnetify-cloudformation-template.yaml)

The template defines an ApiGatewayV2 API of WebSocket protocol type, the routes and integration contracts with dotNetify HTTP endpoints. Copy and edit the template to replace `<your-domain-url>` with the domain name of your deployed dotNetify app server.

After the API has been created, go to the API Gateway service and deploy it. Notice the two URLs that are provided on the Deployment Stage page: _the WebSocket URL_ and the _Connection URL_. Use the [wscat](https://github.com/websockets/wscat) tool to verify the _WebSocket URL_ is accepting connections.

Finally, create an IAM user with sufficient permission for the dotNetify server to call the _Connection URL_ so it can push data to clients. You can use an AWS policy `AmazonAPIGatewayInvokeFullAccess` for this. Once it's created, generate an access key for our next step.

#### DotNetify Server Setup

We will now configure the dotNetify app server to enable the integration endpoints and set the IAM access key. To sign the HTTP requests with the AWS signature, we'll use a NuGet package called _[AwsSignatureVersion4](https://www.nuget.org/packages/AwsSignatureVersion4)_.

Configure the startup class as follows:

```csharp
using Amazon.Runtime;
using AwsSignatureVersion4;
using DotNetify;
...

public void ConfigureServices(IServiceCollection services)
{
  ...
  var awsCredentials = new ImmutableCredentials("<aws-access-key-id>", "<aws-secret-access-key>", null);

  services
    .AddTransient<AwsSignatureHandler>()
    .AddTransient(_ => new AwsSignatureHandlerSettings("<aws-region>", "execute-api", awsCredentials))
    .AddDotNetifyIntegrationWebApi(client =>
      client.BaseAddress = new Uri("<api-connection-url>/") /* Uri string must end with a slash! */
    )
    .AddHttpMessageHandler<AwsSignatureHandler>();
}

public void Configure(IApplicationBuilder app)
{
  ...
  app.MapControllers();
}
```

#### DotNetify Client Setup

The last and also the simplest setup step is to configure your dotNetify client to point to the _WebSocket URL_. In your main entry file, add the following:

```jsx
import dotnetify from "dotnetify";

dotnetify.hub = dotnetify.createWebSocketHub("<aws-api-websocket-url>");
```

To handle network interruption, the client employs an automatic reconnection mechanism with a backoff period of 2, 5, and then every 10 seconds. This can be overridden by setting `donetify.hub.reconnectDelay` with an array of custom backoff number of seconds.

#### Troubleshooting Tips

For troubleshooting, it's recommended to enable debug logging on the client: `dotnetify.debug = true`. If the client is successfully connected to Amazon API Gateway, you should see this browser console output:

```
DotNetifyHub: connecting to <aws-api-websocket-url>
DotNetifyHub: connected
```

In the case of dotNetify server URL misconfiguration in Amazon API Gateway, you will see this browser console error:

```
The websocket server couldn't reach the dotNetify server's integration endpoint.
```

If you don't see data being pushed to the client, check the app server logs. If the given AWS credentials aren't valid or the connection URL doesn't end with a slash, you will see this error:

```csharp
DotNetify: Error: Integration callback responds with 403 Forbidden
```

#### Server Resiliency

See [DotNetify-ResiliencyAddon](/core/dotnetify-resiliencyaddon).
