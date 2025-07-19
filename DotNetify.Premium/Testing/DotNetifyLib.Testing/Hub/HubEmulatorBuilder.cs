using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetify.Security;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetify.Testing
{
   /// <summary>
   /// Builds a DotNetifyHub emulator.
   /// </summary>
   public class HubEmulatorBuilder
   {
      private readonly ServiceCollection _services = new ServiceCollection();
      private readonly Dictionary<string, Type> _registeredVMTypes = new Dictionary<string, Type>();
      private readonly Dictionary<string, object> _registeredVMs = new Dictionary<string, object>();
      private readonly Dictionary<string, GroupProxy> _groupProxies = new Dictionary<string, GroupProxy>();
      private List<Tuple<Type, Func<IMiddlewarePipeline>>> _middlewareFactories = new List<Tuple<Type, Func<IMiddlewarePipeline>>>();
      private List<Tuple<Type, Func<IVMFilter>>> _vmFilterFactories = new List<Tuple<Type, Func<IVMFilter>>>();
      private Func<string, IClientProxy> _getClientProxy = arg => null;
      private Func<IConnectionContext> _getConnectionContext = () => null;
      private ServiceProvider _serviceProvider = null;

      internal class GroupProxy : IClientProxy
      {
         private readonly Func<string, IClientProxy> _getClientProxy;

         public string GroupName { get; }
         public List<string> ConnectionIds { get; } = new List<string>();
         public IReadOnlyList<string> Excludes { get; set; }

         public GroupProxy(string groupName, Func<string, IClientProxy> getClientProxy)
         {
            GroupName = groupName;
            _getClientProxy = getClientProxy;
         }

         public Task SendAsync(string method, object[] args, CancellationToken cancellationToken = default(CancellationToken))
         {
            return SendCoreAsync(method, args, cancellationToken);
         }

         public async Task SendCoreAsync(string method, object[] args, CancellationToken cancellationToken = default(CancellationToken))
         {
            var connectionIds = Excludes != null ? ConnectionIds.Except(Excludes) : ConnectionIds;
            foreach (var connectionId in connectionIds)
               await _getClientProxy(connectionId).SendCoreAsync(method, args, cancellationToken);

            Excludes = null;
         }
      }

      internal class ManyClientsProxy : IClientProxy
      {
         private readonly Func<string, IClientProxy> _getClientProxy;

         public List<string> ConnectionIds { get; } = new List<string>();

         public ManyClientsProxy(IReadOnlyList<string> connectionIds, Func<string, IClientProxy> getClientProxy)
         {
            ConnectionIds = connectionIds.ToList();
            _getClientProxy = getClientProxy;
         }

         public Task SendAsync(string method, object[] args, CancellationToken cancellationToken = default(CancellationToken))
         {
            return SendCoreAsync(method, args, cancellationToken);
         }

         public async Task SendCoreAsync(string method, object[] args, CancellationToken cancellationToken = default(CancellationToken))
         {
            foreach (var connectionId in ConnectionIds)
               await _getClientProxy(connectionId).SendCoreAsync(method, args, cancellationToken);
         }
      }

      /// <summary>
      /// Occurs when a view model instance is created.
      /// </summary>
      internal event EventHandler<object> VMCreated;

      /// <summary>
      /// Constructor.
      /// </summary>
      public HubEmulatorBuilder()
      {
         _services.AddLogging();
         _services.AddMemoryCache();
         _services.AddDotNetify();
         _services.AddTransient<ExtractHeadersMiddleware>();

         LogTraceDelegate logTraceDelegate = log => Trace.WriteLine(log);
         UseMiddleware<TestLogMiddleware>(logTraceDelegate);
      }

      /// <summary>
      /// Builds the hub emulator.
      /// </summary>
      public HubEmulator Build()
      {
         _serviceProvider = BuildServiceProvider(_services);

         // Test if all registered VMs can be instantiated.
         foreach (Type type in _registeredVMTypes.Values)
         {
            try
            {
               _serviceProvider.GetRequiredService(type);
            }
            catch (Exception ex)
            {
               throw ex;
            }
         }

         var hubEmulator = new HubEmulator(_serviceProvider);
         VMCreated += (sender, vm) => hubEmulator.CreatedVMs.Add(vm);
         _getClientProxy = hubEmulator.GetClientProxy;
         _getConnectionContext = hubEmulator.GetConnectionContext;
         return hubEmulator;
      }

      /// <summary>
      /// Adds all the required services to instantiate registered view models.
      /// </summary>
      /// <param name="configure">Delegate that provides the ServiceCollection object.</param>
      public HubEmulatorBuilder AddServices(Action<ServiceCollection> configure)
      {
         configure?.Invoke(_services);
         return this;
      }

      /// <summary>
      /// Registers a view model type.
      /// </summary>
      /// <typeparam name="T">View model type.</typeparam>
      /// <param name="vmId">Identifies the view model type.</param>
      public HubEmulatorBuilder Register<T>(string vmId = null) where T : class
      {
         vmId = vmId ?? typeof(T).Name;

         if (_registeredVMTypes.ContainsKey(vmId) || _registeredVMs.ContainsKey(vmId))
            throw new InvalidOperationException($"'{vmId}' was already registered.");

         _services.AddTransient<T>();
         _registeredVMTypes.Add(vmId, typeof(T));
         return this;
      }

      /// <summary>
      /// Registers a view model instance.
      /// </summary>
      /// <typeparam name="T">View model type.</typeparam>
      /// <param name="vmId">Identifies the view model type.</param>
      /// <param name="vm">View model instance.</param>
      /// <returns></returns>
      public HubEmulatorBuilder Register<T>(string vmId, T vm) where T : class
      {
         if (_registeredVMTypes.ContainsKey(vmId) || _registeredVMs.ContainsKey(vmId))
            throw new InvalidOperationException($"'{vmId}' was already registered.");

         _registeredVMs.Add(vmId, vm);
         return this;
      }

      /// <summary>
      /// Uses a middleware.
      /// </summary>
      public HubEmulatorBuilder UseMiddleware<T>(params object[] args) where T : class, IMiddlewarePipeline
      {
         _services.AddTransient<T>();
         _middlewareFactories.Add(BuildMiddleware<T>(args));
         return this;
      }

      /// <summary>
      /// Uses a view model filter.
      /// </summary>
      public HubEmulatorBuilder UseFilter<T>(params object[] args) where T : class, IVMFilter
      {
         _services.AddTransient<T>();
         _vmFilterFactories.Add(Tuple.Create<Type, Func<IVMFilter>>(typeof(T), () =>
            args?.Length > 0 ? (T) Activator.CreateInstance(typeof(T), args) : _serviceProvider.GetRequiredService<T>()
         ));
         return this;
      }

      /// <summary>
      /// Adds a DotNetifyHub type to a service collection.
      /// </summary>
      /// <param name="services">Service collection.</param>
      private ServiceCollection AddDotNetifyHub(ServiceCollection services)
      {
         var clientProxyStub = Stubber.Create<IClientProxy>()
            .Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask)
            .Object;

         var hubClientStub = Stubber.Create<IHubClients>()
            .Setup(x => x.Client(It.IsAny<string>())).Returns((string connectionId) => _getClientProxy(connectionId))
            .Setup(x => x.Clients(It.IsAny<IReadOnlyList<string>>())).Returns((IReadOnlyList<string> connectionIds) =>
            {
               return new ManyClientsProxy(connectionIds, _getClientProxy);
            })
            .Setup(x => x.Group(It.IsAny<string>())).Returns((string groupName) => _groupProxies[groupName])
            .Setup(x => x.GroupExcept(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>()))
               .Returns((string groupName, IReadOnlyList<string> excludes) =>
               {
                  var groupProxy = _groupProxies[groupName];
                  groupProxy.Excludes = excludes;
                  return groupProxy;
               })
            .Setup(x => x.Users(It.IsAny<IReadOnlyList<string>>())).Returns(clientProxyStub)
            .Object;

         var hubGroupStub = Stubber.Create<IGroupManager>()
            .Setup(x => x.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
               .Returns((string connectionId, string groupName, CancellationToken token) =>
               {
                  if (!_groupProxies.ContainsKey(groupName))
                     _groupProxies.Add(groupName, new GroupProxy(groupName, _getClientProxy));
                  _groupProxies[groupName].ConnectionIds.Add(connectionId);
                  return Task.CompletedTask;
               })
            .Setup(x => x.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask)
            .Object;

         var hubContextStub = Stubber.Create<IHubContext<DotNetifyHub>>()
            .Setup(x => x.Clients).Returns(hubClientStub)
            .Setup(x => x.Groups).Returns(hubGroupStub)
            .Object;

         services.AddTransient(provider => hubContextStub);
         services.AddTransient(provider => _getConnectionContext());
         services.AddTransient<DotNetifyHub>();
         return services;
      }

      /// <summary>
      /// Builds a middleware.
      /// </summary>
      private Tuple<Type, Func<IMiddlewarePipeline>> BuildMiddleware<T>(params object[] args) where T : class, IMiddlewarePipeline
      {
         return Tuple.Create<Type, Func<IMiddlewarePipeline>>(typeof(T), () =>
            args?.Length > 0 ? (T) ActivatorUtilities.CreateInstance(_serviceProvider, typeof(T), args) : _serviceProvider.GetRequiredService<T>()
         );
      }

      /// <summary>
      /// Builds a service provider from a service collection that can provide a DotNetifyHub instance.
      /// </summary>
      /// <param name="services">Service collection.</param>
      private ServiceProvider BuildServiceProvider(ServiceCollection services)
      {
         ServiceProvider serviceProvider = null;
         AddDotNetifyHub(services);

         var vmTypesAccessorStub = Stubber.Create<IVMTypesAccessor>()
            .Setup(x => x.Types)
               .Returns(_registeredVMTypes.Values.Select(type => new TypeHelper(type, args =>
               {
                  var instance = serviceProvider.GetRequiredService(type);
                  VMCreated?.Invoke(this, instance);
                  return instance;
               })
               )
               .Union(_registeredVMs.Select(kvp => new TypeHelper(kvp.Key, args => kvp.Value))))
            .Object;

         services.AddTransient(provider => vmTypesAccessorStub);

         serviceProvider = services.BuildServiceProvider();

         var middlewareFactories = serviceProvider.GetRequiredService<IList<Tuple<Type, Func<IMiddlewarePipeline>>>>();
         _middlewareFactories.ForEach(x => middlewareFactories.Add(x));

         var extractHeadersMiddleware = BuildMiddleware<ExtractHeadersMiddleware>();
         // Place the middleware after any forwarding middleware to ensure it forwards unprocessed data.
         int pos = middlewareFactories.ToList().FindLastIndex(x => x.Item1.Name == "ForwardingMiddleware") + 1;
         middlewareFactories.Insert(pos, extractHeadersMiddleware);

         var vmFilterFactories = serviceProvider.GetRequiredService<IDictionary<Type, Func<IVMFilter>>>();
         _vmFilterFactories.ForEach(x => vmFilterFactories.Add(x.Item1, x.Item2));

         return serviceProvider;
      }
   }
}