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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DotNetify.Forwarding;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;

namespace DotNetify
{
   /// <summary>
   /// Provides objects to send responses back to hub clients.
   /// </summary>
   public interface IDotNetifyHubResponseManager : IDotNetifyHubResponse
   {
      void CreateInstance(HubCallerContext context);

      void RemoveInstance(string connectionId);

      HubCallerContext GetCallerContext(string connectionId);
   }

   /// <summary>
   /// This class produces objects to send responses back to hub clients.
   /// </summary>
   public class DotNetifyHubResponseManager : IDotNetifyHubResponseManager
   {
      private readonly IDotNetifyHubResponse _hubResponse;
      private readonly IDotNetifyHubForwardResponseFactory _hubForwardResponseFactory;
      private readonly ConcurrentDictionary<string, HubCallerContext> _responseHubCallerContexts = new ConcurrentDictionary<string, HubCallerContext>();

      /// <summary>
      /// Wraps a HubCallerContext and clone its items dictionary to avoid collision when the same caller context
      /// is used in a multiplex manner when forwarding messages.
      /// </summary>
      public class ResponseHubCallerContext : HubCallerContext
      {
         private readonly HubCallerContext _context;
         private readonly Dictionary<object, object> _items;

         public override string ConnectionId => _context.ConnectionId;
         public override string UserIdentifier => _context.UserIdentifier;
         public override ClaimsPrincipal User => _context.User;
         public override IDictionary<object, object> Items => _items;
         public override IFeatureCollection Features => _context.Features;
         public override CancellationToken ConnectionAborted => _context.ConnectionAborted;

         public override void Abort() => _context.Abort();

         public ResponseHubCallerContext(HubCallerContext context)
         {
            _context = context;
            _items = new Dictionary<object, object>(context.Items);
         }
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="hubResponse">Hub response object.</param>
      /// <param name="hubForwardResponseFactory">Factory to create response objects of forwarded connections.</param>
      public DotNetifyHubResponseManager(IDotNetifyHubResponse hubResponse, IDotNetifyHubForwardResponseFactory hubForwardResponseFactory)
      {
         _hubResponse = hubResponse;
         _hubForwardResponseFactory = hubForwardResponseFactory;
      }

      /// <summary>
      /// Creates a new hub response object for a hub caller context, if the connection is made by a forwarding hub.
      /// </summary>
      /// <param name="context"></param>
      public void CreateInstance(HubCallerContext context)
      {
         var originContext = context.GetOriginConnectionContext();
         if (originContext != null)
            _responseHubCallerContexts.GetOrAdd(originContext.ConnectionId, new ResponseHubCallerContext(context));
      }

      /// <summary>
      /// Returns the hub caller context associated with a connection.
      /// </summary>
      /// <param name="connectionId">Identifies the connection.</param>
      public HubCallerContext GetCallerContext(string connectionId)
      {
         if (_responseHubCallerContexts.TryGetValue(connectionId, out HubCallerContext context))
            return context;
         return null;
      }

      /// <summary>
      /// Remove the hub response object of a connection.
      /// </summary>
      /// <param name="connectionId">Identifies the connection.</param>
      public void RemoveInstance(string connectionId) => _responseHubCallerContexts.TryRemove(connectionId, out HubCallerContext _);

      /// <summary>
      /// Add a connection to a group.
      /// </summary>
      /// <param name="connectionId">SignalR connection.</param>
      /// <param name="groupName">SignalR group name.</param>
      public Task AddToGroupAsync(string connectionId, string groupName) => GetInstance(connectionId).AddToGroupAsync(connectionId, groupName);

      /// <summary>
      /// Removes a connection from a group.
      /// </summary>
      /// <param name="connectionId">SignalR connection.</param>
      /// <param name="groupName">SignalR group name.</param>
      public Task RemoveFromGroupAsync(string connectionId, string groupName) => GetInstance(connectionId).RemoveFromGroupAsync(connectionId, groupName);

      /// <summary>
      /// Invokes Response_VM on a connection.
      /// </summary>
      /// <param name="connectionId">SignalR connection.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public Task SendAsync(string connectionId, string vmId, string vmData) => GetInstance(connectionId).SendAsync(connectionId, vmId, vmData);

      /// <summary>
      /// Invokes Response_VM on many connections.
      /// </summary>
      /// <param name="connectionIds">List of SignalR connections.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public Task SendToManyAsync(IReadOnlyList<string> connectionIds, string vmId, string vmData)
      {
         // Map connections to associated hubs.
         var hubs = new List<Tuple<string, string>>();
         foreach (var connectionId in connectionIds)
         {
            string hubId = null;
            if (_responseHubCallerContexts.TryGetValue(connectionId, out HubCallerContext context))
               hubId = context.GetOriginConnectionContext().HubId;
            hubs.Add(Tuple.Create(hubId, connectionId));
         }

         // Group connections by hub.
         foreach (var group in hubs.GroupBy(x => x.Item1))
         {
            string hubId = group.Key;
            List<string> hubConnectionIds = group.Select(x => x.Item2).ToList();

            // Use any connection of the hub to send message.
            var hubResponse = hubId == null ? _hubResponse : GetInstance(_responseHubCallerContexts.FirstOrDefault(x => x.Value.GetOriginConnectionContext().HubId == hubId).Key);
            hubResponse.SendToManyAsync(hubConnectionIds, vmId, vmData);
         }

         return Task.CompletedTask;
      }

      /// <summary>
      /// Invokes Response_VM on a group.
      /// </summary>
      /// <param name="groupName">SignalR group name.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public Task SendToGroupAsync(string groupName, string vmId, string vmData)
      {
         GetAllHubInstances().ForEach(x => x.SendToGroupAsync(groupName, vmId, vmData));
         return Task.CompletedTask;
      }

      /// <summary>
      /// Invokes Response_VM on a group but exclude some connections.
      /// </summary>
      /// <param name="groupName">SignalR group name.</param>
      /// <param name="excludedConnectionIds">Excluded SignalR connections.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public Task SendToGroupExceptAsync(string groupName, IReadOnlyList<string> excludedIds, string vmId, string vmData)
      {
         GetAllHubInstances().ForEach(x => x.SendToGroupExceptAsync(groupName, excludedIds, vmId, vmData));
         return Task.CompletedTask;
      }

      /// <summary>
      /// Invokes Response_VM on a set of users.
      /// </summary>
      /// <param name="userIds">Identifies the users.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public Task SendToUsersAsync(IReadOnlyList<string> userIds, string vmId, string vmData)
      {
         GetAllHubInstances().ForEach(x => x.SendToUsersAsync(userIds, vmId, vmData));
         return Task.CompletedTask;
      }

      /// <summary>
      /// Returns the hub response object of a connection, which could be associated with a forwarding hub.
      /// </summary>
      /// <param name="connectionId">Identifies the connection.</param>
      private IDotNetifyHubResponse GetInstance(string connectionId)
      {
         return _responseHubCallerContexts.TryGetValue(connectionId, out HubCallerContext context) ? _hubForwardResponseFactory.GetInstance(context.ConnectionId) : _hubResponse;
      }

      /// <summary>
      /// Returns a response object from every forwarding hubs.
      /// </summary>
      /// <returns></returns>
      private List<IDotNetifyHubResponse> GetAllHubInstances()
      {
         return _responseHubCallerContexts
            .GroupBy(x => x.Value.GetOriginConnectionContext().HubId)
            .Select(x => GetInstance(x.First().Key))
            .Concat(new[] { _hubResponse })  // Add this hub's own response object.
            .ToList();
      }
   }
}