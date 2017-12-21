using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using DotNetify;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UnitTests
{
   public class MockDotNetifyHub
   {
      private DotNetifyHub _hub;
      private MemoryCache _memoryCache = new MemoryCache();
      private List<Tuple<Type, Func<IMiddlewarePipeline>>> _middlewareFactories = new List<Tuple<Type, Func<IMiddlewarePipeline>>>();
      private Dictionary<Type, Func<IVMFilter>> _vmFilterFactories = new Dictionary<Type, Func<IVMFilter>>();
      private Func<Type, object[], object> _factoryMethod = (type, args) => Activator.CreateInstance(type, args ?? new object[] { });

      private class MemoryCache : IMemoryCache
      {
         private Dictionary<string, object> _cache = new Dictionary<string, object>();

         public object Get(string key) => _cache.ContainsKey(key) ? _cache[key] : null;

         public void Remove(string key) => _cache.Remove(key);

         public void Set<T>(string key, T cachedValue, MemoryCacheEntryOptions options = null) where T : class => _cache[key] = cachedValue;

         public bool TryGetValue<T>(string key, out T cachedValue) where T : class
         {
            cachedValue = (T)Get(key);
            return cachedValue != null;
         }
      }

      private class MockHubContext : IHubContext<DotNetifyHub>
      {
         public IHubClients MockHubClients { get; set; }
         public IHubClients Clients => MockHubClients;
         public IGroupManager Groups => throw new NotImplementedException();
      }

      private class MockHubClients : IHubClients
      {
         public IClientProxy MockClientProxy { get; set; }
         public IClientProxy All => null;
         public IClientProxy AllExcept(IReadOnlyList<string> excludedIds) => null;
         public IClientProxy Client(string connectionId) => MockClientProxy;
         public IClientProxy Group(string groupName) => null;
         public IClientProxy User(string userId) => null;
      }

      private class MockClientProxy : IClientProxy
      {
         public MockDotNetifyHub Hub { get; set; }
         public Task InvokeAsync(string method, object[] args)
         {
            Hub.GetType().GetMethod(method).Invoke(Hub, args);
            return Task.CompletedTask;
         }
      }

      private class MockHubConnectionContext : HubConnectionContext
      {
         private string _mockConnectionId = "AFF87FC6-5006-412B-947B-FDDA757330C2";
         private ClaimsPrincipal _mockPrincipal = new ClaimsPrincipal();

         public MockHubConnectionContext() : base(null, null) { }

         public override ClaimsPrincipal User => _mockPrincipal;
         public override string ConnectionId => _mockConnectionId;
      }

      public MockDotNetifyHub Create()
      {
         _hub = new DotNetifyHub(
            new VMControllerFactory(_memoryCache) { ResponseDelegate = ResponseVM },
            new HubPrincipalAccessor(),
            new HubPipeline(_middlewareFactories, _vmFilterFactories),
            new MockHubContext() { MockHubClients = new MockHubClients { MockClientProxy = new MockClientProxy { Hub = this } } })
         {
            Context = new HubCallerContext(new MockHubConnectionContext())
         };

         VMController.CreateInstance = (type, args) => Activator.CreateInstance(type, args);
         return this;
      }

      public event EventHandler<Tuple<string, string>> ClientResponse;

      public MockDotNetifyHub UseMiddleware<T>(params object[] args) where T : IMiddlewarePipeline
      {
         _middlewareFactories.Add(Tuple.Create<Type, Func<IMiddlewarePipeline>>(typeof(T),
            () => (IMiddlewarePipeline)_factoryMethod(typeof(T), args)));
         return this;
      }

      public MockDotNetifyHub UseFilter<T>(params object[] args) where T : IVMFilter
      {
         _vmFilterFactories.Add(typeof(T), () => (IVMFilter)_factoryMethod(typeof(T), args));
         return this;
      }

      public void RequestVM(string vmId, object vmArg = null) => _hub.Request_VM(vmId, vmArg);

      public void UpdateVM(string vmId, Dictionary<string, object> vmData) => _hub.Update_VM(vmId, vmData);

      public void ResponseVM(string connectionId, string vmId, string vmData) => _hub.Response_VM(connectionId, vmId, vmData);

      public void Response_VM(string vmId, string vmData) => ClientResponse?.Invoke(this, Tuple.Create(vmId, vmData));

      public void DisposeVM(string vmId) => _hub.Dispose_VM(vmId);
   }
}
