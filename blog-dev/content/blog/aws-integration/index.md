---
title: Scaling DotNetify Apps with AWS WebSockets
date: "2023-03-06T00:00:00Z"
description: "Use Amazon API Gateway and WebSocket API to scale your dotNetify app and handle large-scale WebSocket connections."
---

If you're a dotNetify user, you might be interested to know that the AWS WebSocket API integration capability was just added to the framework early this year. By default, dotNetify uses SignalR for client-server communication, but with this update, it can also use the browser's native WebSocket API to communicate with other WebSocket server implementations, such as Amazon API Gateway.

Amazon API Gateway is a great option for scaling out. It is a fully-managed service that maintains persistent connections and handles the message transfer between the clients and your application server through built-in HTTP integration endpoints. With Amazon API Gateway, you won't have to worry about self-hosting a web server that can handle large-scale WebSocket connections.

<img src="./diagram.svg" width="630px" style="margin-bottom:2rem" />

So, how can you get your dotNetify application communicating through Amazon API Gateway? Well, the first thing you need to do is create, configure, and deploy a WebSocket API on Amazon API Gateway. You'll also need to create an IAM authorization for allowing access to the API, and configure the HTTP integration endpoints and the IAM credentials on the dotNetify app server. Lastly, you'll need to point the dotNetify client to the API's WebSocket URL.

## Amazon WebSocket API

Setting up a WebSocket API in AWS involves many steps if you do it manually. To make things easier, [a CloudFormation template](https://github.com/dsuryd/dotNetify/blob/master/dotnetify-cloudformation-template.yaml) is provided to define an ApiGatewayV2 API of WebSocket protocol type, the routes and integration contracts with dotNetify HTTP endpoints. All you need to do is copy and edit the template to replace `<your-domain-url>` with the domain name of your deployed dotNetify app server.

After the API has been created, go to the API Gateway service and deploy it. Notice the two URLs that are provided on the Deployment Stage page: the _WebSocket URL_ and the _Connection URL_. Use the [wscat](https://github.com/websockets/wscat) tool to verify the WebSocket URL is accepting connections.

Next, create an IAM user with sufficient permission for the dotNetify server to call the Connection URL so it can push data to clients. You can use an AWS policy `AmazonAPIGatewayInvokeFullAccess` for this. Once it's created, generate an access key for our next step.

## DotNetify App Server

Now, let's configure the dotNetify app server to enable the integration endpoints and set the IAM access key. To sign the HTTP requests with the AWS signature, we'll use a NuGet package called [AwsSignatureVersion4](https://www.nuget.org/packages/AwsSignatureVersion4). We'll also need to configure the startup class, which is pretty straightforward.

```csharp
using Amazon.Runtime;
using AwsSignatureVersion4;
using DotNetify;
...

public void ConfigureServices(IServiceCollection services)
{
  ...
  var awsCredentials = new ImmutableCredentials
     ("<aws-access-key-id>", "<aws-secret-access-key>", null);

  services
    .AddTransient<AwsSignatureHandler>()
    .AddTransient(_ => new AwsSignatureHandlerSettings(
       "<aws-region>", "execute-api", awsCredentials))
    .AddDotNetifyIntegrationWebApi(client =>
       client.BaseAddress = new Uri("<api-connection-url>/"))
    .AddHttpMessageHandler<AwsSignatureHandler>();
}

public void Configure(IApplicationBuilder app)
{
  ...
  app.MapControllers();
}
```

## DotNetify Client

Finally, the last and simplest setup step is to configure your dotNetify client to point to the WebSocket URL.

```js
import dotnetify from "dotnetify"

dotnetify.hub = dotnetify.createWebSocketHub("<aws-api-websocket-url>")
```

And just like that, your dotNetify application server is now scalable and able to handle large-scale WebSocket connections.

Of course, there may be some troubleshooting involved along the way, but don't worry, it's not too difficult to figure out. Just make sure to enable debug logging on the client and check the app server logs if you're not seeing data being pushed to the client.

In conclusion, using Amazon API Gateway to scale out your dotNetify application server is a great option that can save you a lot of time and effort. It's not too difficult to set up, and once it's done, you can sit back, relax, and enjoy your scalable real-time, reactive web applications.
