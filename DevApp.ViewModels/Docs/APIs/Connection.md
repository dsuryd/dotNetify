## Connection

DotNetify is an abstraction over SignalR, which in turn is an abstraction over some of the network transports that are required for real-time, bidirectional communication between client and server. There are currently two distinct implementations of SignalR: the original [SignalR for .NET Framework](http://asp.net/signalr), and the new [SignalR for .NET Core](https://docs.microsoft.com/en-us/aspnet/core/signalr/?view=aspnetcore-2.1). Unfortunately, they are not compatible with one another, i.e. the client of one won't be able to talk with the server of another.

While dotNetify's client-side library can be used for either one, it needs to be configured to match the server it connects with. The .NET Core library is assumed by default. To switch to the .NET Framework library, import the one included in the dotNetify's distribution package (a customized one that doesn't require the full jQuery) and set it to the __dotnetify.hubLib__ at the application's entry point:

```jsx
import signalR from 'dotnetify/dist/signalR-netfx';
dotnetify.hubLib = signalR;
```

#### Transport Fallback

SignalR connection uses WebSocket transport whenever available, and automatically falls back to older transport otherwise. By default, dotNetify will fall back to long polling when WebSocket fails. The fallback order and type can be configured through __dotnetify.hubOptions__ as below:

```jsx
dotnetify.hubOptions = { transport: ["webSockets", "serverSentEvent", "longPolling"] };
```
> Caution: server-sent event transport is sometimes exhibiting inexplicable, significant delay during the initial connection.

#### Reconnection
If the connection gets disconnected, as part of its resiliency feature, dotNetify will automatically attempt to reconnect indefinitely, with the increasing delay between attempts up to every 10 seconds. This, too, is configurable as below:

```jsx
dotnetify.hub.reconnectRetry = 10;
dotnetify.hub.reconnectDelay = [5, 30];
```

The above configuration indicates that the reconnect attempt is limited to 10 times after every disconnected event, with initial delay of 5 seconds and then every 30 seconds afterwards. When the retry attempt is exhausted, the _"terminated"_ state is triggered.
You can receive notification when the connection state changed by providing a handler function to __dotnetify.connectionStateHandler__:

```jsx
dotnetify.connectionStateHandler = (state) => { /* handle state changed event */};
```
The state's possible values are: _"connecting", "connected", "reconnecting", "disconnected"_ and _"terminated"_.

#### Cross-Origin Support

If you want to host the SignalR server on a different domain, use __dotnetify.hubServerUrl__ to specify its location; for example:

```jsx
// ASP.NET Core:
dotnetify.hubServerUrl = "http://my-other-domain.net"; 

// ASP.NET Framework:
dotnetify.hubServerUrl = "http://my-other-domain.net/signalr"; 
```

> The server must be configured to allow CORS.

The above setup applies to all connection, but if you need a more fine-grained control over more than one hub servers, use __dotnetify.connectHandler__ to intercept every _connect_ call and replace the default hub proxy with one that you created with __donetify.createHub__ and initialized to a different origin:

```jsx
const hub1 = dotnetify.createHub("http://my-domain-1.net");
const hub2 = dotnetify.createHub("http://my-domain-2.net");

dotnetify.connectHandler = vmConnectArgs => {
  if (vmConnectArgs.vmId === 'MyVM_1') return { ...vmConnectArgs, hub: hub1 };
  else if (vmConnectArgs.vmId === 'MyVM_2') return { ...vmConnectArgs, hub: hub2 };
};
```

The argument __vmConnectArgs__ is an object with the following properties which you can override: `vmId`, `options`, and `hub`.  One good use case to override the `options` is to add authentication header as in this example:

```jsx
dotnetify.connectHandler = vmConnectArgs => {
  return { 
    ...vmConnectArgs, 
    options: { 
      ...vmConnectArgs.options, 
      headers: { Authorization: 'Bearer ' + auth.getAccessToken() } 
    }
   };
};
```
#### HubConnectionBuilder
Use __hubOptions.connectionBuilder__ to gain access to the `HubConnectionBuilder` object in order to further customize SignalR hub connections.  

For example, the following code will configure the client to use the MessagePack binary protocol:
```jsx
import { MessagePackHubProtocol } from '@aspnet/signalr-protocol-msgpack';

const protocol = new MessagePackHubProtocol();
dotnetify.hubOptions.connectionBuilder = builder => builder.withHubProtocol(protocol);
```