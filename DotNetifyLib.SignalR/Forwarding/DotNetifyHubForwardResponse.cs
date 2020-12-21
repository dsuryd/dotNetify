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

using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetify.Forwarding;
using Microsoft.AspNetCore.SignalR;

namespace DotNetify
{
   /// <summary>
   /// Implements invocation of a SignalR hub method for sending view model responses to clients.
   /// </summary>
   public class DotNetifyHubForwardResponse : IDotNetifyHubResponse
   {
      private static readonly string RESPONSE_VM = nameof(IDotNetifyHubMethod.Response_VM);

      private readonly IHubContext<DotNetifyHub> _hubContext;
      private readonly string _connectionId;

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="hubContext">DotNetify hub context for sending messages.</param>
      /// <param name="connectionId">Identifies the connection to send message to.</param>
      public DotNetifyHubForwardResponse(IHubContext<DotNetifyHub> hubContext, string connectionId)
      {
         _hubContext = hubContext;
         _connectionId = connectionId;
      }

      /// <summary>
      /// Add a connection to a group.
      /// </summary>
      /// <param name="connectionId">SignalR connection.</param>
      /// <param name="groupName">SignalR group name.</param>
      public Task AddToGroupAsync(string connectionId, string groupName)
      {
         return _hubContext.Clients.Client(_connectionId).SendAsync(RESPONSE_VM,
            new object[] { nameof(AddToGroupAsync), new object[] { connectionId, groupName }, DotNetifyHubForwarder.BuildResponseMetadata(connectionId) });
      }

      /// <summary>
      /// Removes a connection from a group.
      /// </summary>
      /// <param name="connectionId">SignalR connection.</param>
      /// <param name="groupName">SignalR group name.</param>
      public Task RemoveFromGroupAsync(string connectionId, string groupName)
      {
         return _hubContext.Clients.Client(_connectionId).SendAsync(RESPONSE_VM,
            new object[] { nameof(RemoveFromGroupAsync), new object[] { connectionId, groupName }, DotNetifyHubForwarder.BuildResponseMetadata(connectionId) });
      }

      /// <summary>
      /// Invokes Response_VM on a connection.
      /// </summary>
      /// <param name="connectionId">SignalR connection.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public Task SendAsync(string connectionId, string vmId, string vmData)
      {
         return _hubContext.Clients.Client(_connectionId).SendAsync(RESPONSE_VM,
            new object[] { nameof(SendAsync), new object[] { vmId, vmData }, DotNetifyHubForwarder.BuildResponseMetadata(connectionId) });
      }

      /// <summary>
      /// Invokes Response_VM on a group.
      /// </summary>
      /// <param name="groupName">SignalR group name.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public Task SendToGroupAsync(string groupName, string vmId, string vmData)
      {
         return _hubContext.Clients.Client(_connectionId).SendAsync(RESPONSE_VM,
            new object[] { nameof(SendToGroupAsync), new object[] { groupName, vmId, vmData } });
      }

      /// <summary>
      /// Invokes Response_VM on a group but exclude some connections.
      /// </summary>
      /// <param name="groupName">SignalR group name.</param>
      /// <param name="excludedConnectionIds">Excluded SignalR connections.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public Task SendToGroupExceptAsync(string groupName, IReadOnlyList<string> excludedConnectionIds, string vmId, string vmData)
      {
         return _hubContext.Clients.Client(_connectionId).SendAsync(RESPONSE_VM,
            new object[] { nameof(SendToGroupExceptAsync), new object[] { groupName, excludedConnectionIds, vmId, vmData } });
      }

      /// <summary>
      /// Invokes Response_VM on a set of users.
      /// </summary>
      /// <param name="userIds">Identifies the users.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public Task SendToUsersAsync(IReadOnlyList<string> userIds, string vmId, string vmData)
      {
         return _hubContext.Clients.Client(_connectionId).SendAsync(RESPONSE_VM,
            new object[] { nameof(SendToUsersAsync), new object[] { userIds, vmId, vmData } });
      }
   }
}