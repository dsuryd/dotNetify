using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DotNetify.Client;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetify.Testing
{
   public interface IDotNetifyHubEmulatorProxy : IDisposable
   {
      DotNetifyHub Hub { get; }

      HubConnectionState ConnectionState { get; }

      event EventHandler<ResponseVMEventArgs> Response_VM;

      event EventHandler<HubConnectionState> StateChanged;

      event EventHandler Disconnected;

      Task Dispose_VM(string vmId);

      void Init(string hubPath, string serverUrl);

      Task Request_VM(string vmId, Dictionary<string, object> options);

      Task Request_VM(string vmId, object options);

      Task StartAsync();

      Task Update_VM(string vmId, Dictionary<string, object> propertyValues);

      Task Update_VM(string vmId, object propertyValues);
   }

   /// <summary>
   /// Emulates dotNetify hub.
   /// </summary>
   public class HubEmulator
   {
      private readonly ServiceProvider _serviceProvider;
      private readonly Dictionary<string, ClientContext> _clientContexts = new Dictionary<string, ClientContext>();
      private readonly object _sync = new object();
      private HubCallerContext _hubCallerContext;

      internal class DotNetifyHubEmulatorProxy : IDotNetifyHubEmulatorProxy
      {
         public DotNetifyHub Hub { get; }

         public HubConnectionState ConnectionState { get; }

         public event EventHandler<ResponseVMEventArgs> Response_VM { add { } remove { } }

         public event EventHandler<HubConnectionState> StateChanged { add { } remove { } }

         public event EventHandler Disconnected { add { } remove { } }

         public DotNetifyHubEmulatorProxy(DotNetifyHub dotNetifyHub)
         {
            Hub = dotNetifyHub;
         }

         public void Dispose()
         {
         }

         public async Task Dispose_VM(string vmId)
         {
            var disposeMethod = Hub.GetType().GetMethod("DisposeVMAsync") ?? Hub.GetType().GetMethod("DisposeVMAsyc");
            var task = (Task) disposeMethod.Invoke(Hub, new object[] { vmId });
            await task;
         }

         public void Init(string hubPath, string serverUrl) => throw new NotImplementedException();

         public Task Request_VM(string vmId, Dictionary<string, object> options) => Hub.RequestVMAsync(vmId, options);

         public Task Request_VM(string vmId, object options) => Hub.RequestVMAsync(vmId, options);

         public Task StartAsync() => throw new NotImplementedException();

         public Task Update_VM(string vmId, Dictionary<string, object> propertyValues) => Hub.UpdateVMAsync(vmId, propertyValues);

         public Task Update_VM(string vmId, object propertyValues)
         {
            if (propertyValues is Dictionary<string, object> == false)
            {
               var json = System.Text.Json.JsonSerializer.Serialize(propertyValues);
               var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
               return Update_VM(vmId, data);
            }
            else
               return Update_VM(vmId, propertyValues as Dictionary<string, object>);
         }

         public void TerminateHubConnection() => Hub.OnDisconnectedAsync(null);
      }

      /// <summary>
      /// List of view model instances created by the hub emulator.
      /// </summary>
      public List<object> CreatedVMs { get; } = new List<object>();

      /// <summary>
      /// Delegate for the stub in the service provider to access the client proxy given a connection ID.
      /// </summary>
      internal Func<string, IClientProxy> GetClientProxy => connectionId => _clientContexts[connectionId].ClientProxy;

      /// <summary>
      /// Delegate for the stub in the service provider to access the active connection context.
      /// </summary>
      internal Func<IConnectionContext> GetConnectionContext => () => (_hubCallerContext as HubCallerContextStub).GetConnectionContext();

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="serviceProvider">Service provider from the builder object.</param>
      internal HubEmulator(ServiceProvider serviceProvider)
      {
         _serviceProvider = serviceProvider;
      }

      /// <summary>
      /// Create a client emulator.
      /// </summary>
      /// <param name="connectionId">Mock SignalR connection ID; auto-generated id null.</param>
      /// <param name="user">User principal.</param>
      /// <returns></returns>
      public IClientEmulator CreateClient(string connectionId = null, ClaimsPrincipal user = null)
      {
         lock (_sync)
         {
            var clientContext = new ClientContext(connectionId);

            var dotNetifyHub = _serviceProvider.GetRequiredService<DotNetifyHub>();

            user = user ?? new ClaimsPrincipal(new ClaimsIdentity());
            dotNetifyHub.Context = new HubCallerContextStub(clientContext.ConnectionContext, user);
            _hubCallerContext = dotNetifyHub.Context;

            _clientContexts[clientContext.ConnectionId] = clientContext;
            return new ClientEmulator(new DotNetifyHubEmulatorProxy(dotNetifyHub), clientContext, () => _clientContexts.Remove(connectionId));
         }
      }
   }
}