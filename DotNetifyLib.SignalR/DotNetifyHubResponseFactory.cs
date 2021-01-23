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
using DotNetify.Forwarding;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;

namespace DotNetify
{
   /// <summary>
   /// Provides objects to send responses back to hub clients.
   /// </summary>
   public interface IDotNetifyHubResponseFactory
   {
      void CreateInstance(HubCallerContext context);

      HubCallerContext GetCallerContext(string connectionId);

      IDotNetifyHubResponse GetInstance();

      IDotNetifyHubResponse GetInstance(string connectionId);

      void InvokeGroupInstances(Action<IDotNetifyHubResponse> invokeDelegate);

      void RemoveInstance(string connectionId);
   }

   /// <summary>
   /// This class produces objects to send responses back to hub clients.
   /// </summary>
   public class DotNetifyHubResponseFactory : IDotNetifyHubResponseFactory
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
      public DotNetifyHubResponseFactory(IDotNetifyHubResponse hubResponse, IDotNetifyHubForwardResponseFactory hubForwardResponseFactory)
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
      /// Returns the hub response object of this hub.
      /// </summary>
      public IDotNetifyHubResponse GetInstance()
      {
         return _hubResponse;
      }

      /// <summary>
      /// Returns the hub response object of a connection, which could be associated with a forwarding hub.
      /// </summary>
      /// <param name="connectionId">Identifies the connection.</param>
      public IDotNetifyHubResponse GetInstance(string connectionId)
      {
         if (_responseHubCallerContexts.TryGetValue(connectionId, out HubCallerContext context))
            return _hubForwardResponseFactory.GetInstance(context.ConnectionId);

         return _hubResponse;
      }

      /// <summary>
      /// Runs action on the hub response objects of every forwarding hub connected this hub.
      /// </summary>
      /// <param name="invokeDelegate">Delegate that will be passed the hub response objects.</param>
      public void InvokeGroupInstances(Action<IDotNetifyHubResponse> invokeDelegate)
      {
         // Find a hub response object associated with a connection from every connected forwarding hub.
         var responses = _responseHubCallerContexts
            .GroupBy(x => x.Value.GetOriginConnectionContext().HubId)
            .Select(x => GetInstance(x.First().Value.ConnectionId))
            .Concat(new[] { _hubResponse })  // Add this hub's own response object.
            .ToList();

         responses.ForEach(x => invokeDelegate(x));
      }

      /// <summary>
      /// Remove the hub response object of a connection.
      /// </summary>
      /// <param name="connectionId">Identifies the connection.</param>
      public void RemoveInstance(string connectionId)
      {
         _responseHubCallerContexts.TryRemove(connectionId, out HubCallerContext _);
      }
   }
}