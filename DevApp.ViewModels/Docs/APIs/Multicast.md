## Multicast

View models that inherit from __BaseVM__ maintain 1-to-1 association with their views.  But in some use cases, such as chat rooms, multiplayer games, IoT broadcast, and all kinds of collaborative applications, it is often desirable to allow a view model be shared among multiple views.  DotNetify facilitates this by providing the __MulticastVM__ base class.

#### Single Instance

By default, a view model that inherits from __MulticastVM__ will always produce one instance at any given time.  After an initial request for the view model from one connection, subsequent requests from other connections will be given the same instance.  Any data update that goes through __Changed__ or __PushUpdates__ will apply to all clients.  

```csharp
public class HelloWorld : MulticastVM
{
  public string Greetings => "Hello World!";

  protected override void Dispose(bool disposing) {}
}
```

When a client disconnects, the _Dispose(bool disposing)_ method will be invoked to give you the chance to process the disconnection.  The instance is only disposed  after the last client disconnects, marked by _disposing_ argument value of true.

#### Partitioned Instances

When you need to partition the view model into instances specific to a group of connections, override the __GroupName__ property.   For example, this view model will allow any authenticated user connecting from multiple devices to share the same instance:

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

Notice that the __GroupName__ value will be dynamic, depending on the user context of the calling connection.  The way this works is, when the first connection makes a request for a multicast view model, a new instance will be created and associated with the group name produced by that connection.  When a subsequent connection makes a similar request, dotNetify will first try to find an existing instance with matching group names, before it would create another instance with the new group name.

The interface __IPrincipalAccessor__ provides access to the authenticated user's information, including claims-based identity.  Another interface, __IConnectionContext__ provides access to some connection information, such as SignalR connection ID, client's IP address, and initial HTTP request headers. 

When the __GroupName__ is provided, dotNetify will use SignalR group broadcast API to simultaneously push data updates. Using groups is also important if you need to scale your application to multiple servers and take advantage of SignalR's Redis integration.

#### Sending Direct Message

With __MulticastVM__, you could also send a direct message to one or more connections by using the __Send__ APIs.  The first one takes a list of connection IDs, along with the property name and value to send.  Handle this message on the client side by listening for state change, using the API provided by the UI framework.

```csharp
   Send( new List<string> { connectionId }, propertyName, propertyValue);
```

And if this isn't enough, the second API takes the following argument which practically gives you access to most SignalR's broadcast APIs:

```csharp
public class SendEventArgs : EventArgs
{
  // Will call Clients.Client() for each list item.
  public IList<string> ConnectionIds { get; }

  // Will call either Clients.Group() or Clients.GroupExcept().
  public string GroupName { get; set; }
  public IList<string> ExcludedConnectionIds { get; };

  // Will call Clients.Users().
  public IList<string> UserIds { get; };

  // Payload.
  public IDictionary<string, object> Properties { get; set; }
}
```





