## Multicast

View models that inherit from __BaseVM__ maintains 1-to-1 association with their views.  But in some use cases, such as chat rooms, multiplayer games, IoT broadcast, and any other kinds of collaborative applications, it is often desirable to allow a view model be shared among multiple views.  DotNetify facilitates this by providing the __MulticastVM__ base class.

#### Single Instance

By default, a view model that inherits from __MulticastVM__ will always produce one instance at any given time.  After an initial request for the view model from one connection, subsequent requests from other connections will be given the same instance.  The instance will be disposed after the last one disconnects.

```csharp
public class HelloWorld : MulticastVM
{
  public string Greetings => "Hello World!";
}
```

Any data update that goes through __Changed__ or __PushUpdates__ will apply to all clients.


#### Partitioned Instances

When you need to partition the view model into instances specific to a group of connections, override the __GroupName__ property.   For example, this view model will allow any authenticated user to use the same instance when making connections from multiple devices:

```csharp
public class PerUserVM : MulticastVM
{
  private readonly IPrincipalAccessor _principalAccessor;

  public override string GroupName => _principalAccessor.Principal.Identity.Name;

  public PerUserVM(IPrincipalAccessor principalAccessor)
  {
    _principalAccessor = principalAccessor;
  }
}
```

Notice that the __GroupName__ value is dynamic, based on the user context of the calling connection.  The way this works is, when the first connection makes a request for a multicast view model, a new instance will be created and associated with the group name that is produced by that connection.  When subsequent connections make a similar request, dotNetify will first try to find an existing instance with matching group names for it, before it creates another instance with a different group name.

The interface __IPrincipalAccessor__ provides access to the authenticated user's information, including claims-based identity.  Another interface, __IConnectionContext__ provides access to some connection information, such as SignalR connection ID, client's IP address, and initial HTTP request headers. 

When the __GroupName__ is provided, dotNetify will use SignalR group broadcast API to simultaneously push data updates. Using groups is also important if you need to scale your application to multiple servers and take advantage of SignalR's Redis integration.

#### Sending Direct Message

With __MulticastVM__, you could also send a direct message to one or more connections by using the __Send__ APIs.  The first one takes a list of connection IDs, along with the property name and value to send.  Handle this message on the client side by listening for state change, using the API provided by the UI framework.

```csharp
   Send( new List<string> { connectionId }, propertyName, propertyValue);
```

And if it isn't enough, the second API takes the following argument which practically gives you full access to SignalR's broadcast APIs:

```csharp
public class SendEventArgs : EventArgs
{
  public IList<string> ConnectionIds { get; }
  public IList<string> ExcludedConnectionIds { get; };
  public string GroupName { get; set; }
  public IList<string> UserIds { get; };
  public IDictionary<string, object> Properties { get; set; }
}
```





