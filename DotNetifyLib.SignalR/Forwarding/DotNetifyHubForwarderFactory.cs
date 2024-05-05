/*
Copyright 2020 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

using AsyncKeyedLock;
using DotNetify.Client;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetify.Forwarding
{
    /// <summary>
    /// Provides objects for forwarding hub messages to another server.
    /// </summary>
    public interface IDotNetifyHubForwarderFactory
    {
        Task InvokeInstanceAsync(string serverUrl, ForwardingOptions config, Func<DotNetifyHubForwarder, Task> invoke);
    }

    /// <summary>
    /// This class produces hub forwarder objects for forwarding hub messages to another server.
    /// </summary>
    public class DotNetifyHubForwarderFactory : IDotNetifyHubForwarderFactory
    {
        private readonly AsyncKeyedLocker<DotNetifyHubForwarder> _semaphores = new AsyncKeyedLocker<DotNetifyHubForwarder>(o =>
        {
            o.PoolSize = 20;
            o.PoolInitialFill = 1;
        });
        private readonly ConcurrentDictionary<string, AsyncCollection<DotNetifyHubForwarder>> _hubForwarders = new ConcurrentDictionary<string, AsyncCollection<DotNetifyHubForwarder>>();
        private readonly IHubContext<DotNetifyHub> _globalHubContext;
        private readonly IDotNetifyHubProxyFactory _hubProxyFactory;

        public DotNetifyHubForwarderFactory(IHubContext<DotNetifyHub> globalHubContext, IDotNetifyHubProxyFactory hubProxyFactory)
        {
            _globalHubContext = globalHubContext;
            _hubProxyFactory = hubProxyFactory;
        }

        public async Task InvokeInstanceAsync(string serverUrl, ForwardingOptions config, Func<DotNetifyHubForwarder, Task> invoke)
        {
            var pool = GetConnectionPool(serverUrl, config);
            var hubForwarder = await pool.TakeAsync();

            using (await _semaphores.LockAsync(hubForwarder).ConfigureAwait(false))
            {
                await hubForwarder.StartAsync();

                try
                {
                    await invoke(hubForwarder);
                }
                catch (Exception ex)
                {
                    throw new ForwardingException(ex.Message, ex);
                }
                finally
                {
                    _ = pool.AddAsync(hubForwarder);
                }
            }
        }

        private AsyncCollection<DotNetifyHubForwarder> GetConnectionPool(string serverUrl, ForwardingOptions config)
        {
            return _hubForwarders.GetOrAdd(serverUrl, url =>
            {
                var newForwarders = Enumerable.Range(1, config.ConnectionPoolSize).Select(x => CreateInstance(serverUrl, config));
                return new AsyncCollection<DotNetifyHubForwarder>(new ConcurrentQueue<DotNetifyHubForwarder>(newForwarders));
            });
        }

        private DotNetifyHubForwarder CreateInstance(string serverUrl, ForwardingOptions config)
        {
            var hubProxy = _hubProxyFactory.GetInstance();

            if (config.UseMessagePack)
                (hubProxy as DotNetifyHubProxy).ConnectionBuilder = builder => builder.AddMessagePackProtocol();
            else
                (hubProxy as DotNetifyHubProxy).ConnectionBuilder = builder => builder.AddJsonProtocol();

            hubProxy.Init(null, serverUrl);
            return new DotNetifyHubForwarder(hubProxy, new DotNetifyHubResponse(_globalHubContext));
        }
    }
}