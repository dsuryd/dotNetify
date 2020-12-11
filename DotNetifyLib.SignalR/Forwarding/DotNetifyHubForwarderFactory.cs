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

using System.Collections.Concurrent;
using DotNetify.Client;
using Microsoft.AspNetCore.SignalR;

namespace DotNetify.Forwarding
{
   /// <summary>
   /// Provides objects for forwarding hub messages to another server.
   /// </summary>
   public interface IDotNetifyHubForwarderFactory
   {
      DotNetifyHubForwarder GetInstance(string key);
   }

   /// <summary>
   /// This class produces hub forwarder objects for forwarding hub messages to another server.
   /// </summary>
   public class DotNetifyHubForwarderFactory : IDotNetifyHubForwarderFactory
   {
      private static readonly ConcurrentDictionary<string, DotNetifyHubForwarder> _hubForwarders = new ConcurrentDictionary<string, DotNetifyHubForwarder>();
      private readonly IHubContext<DotNetifyHub> _globalHubContext;
      private readonly IDotNetifyHubProxyFactory _hubProxyFactory;

      public DotNetifyHubForwarderFactory(IHubContext<DotNetifyHub> globalHubContext, IDotNetifyHubProxyFactory hubProxyFactory)
      {
         _globalHubContext = globalHubContext;
         _hubProxyFactory = hubProxyFactory;
      }

      public DotNetifyHubForwarder GetInstance(string key)
      {
         return _hubForwarders.GetOrAdd(key, serverUrl =>
         {
            var hubProxy = _hubProxyFactory.GetInstance();
            hubProxy.Init(null, serverUrl);
            return new DotNetifyHubForwarder(hubProxy, new DotNetifyHubResponse(_globalHubContext));
         });
      }
   }
}