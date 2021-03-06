﻿/*
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
using Microsoft.AspNetCore.SignalR;

namespace DotNetify
{
   /// <summary>
   /// Provides invocation of a SignalR hub method for sending view model responses to clients.
   /// </summary>
   public interface IDotNetifyHubResponse
   {
      Task AddToGroupAsync(string connectionId, string groupName);

      Task RemoveFromGroupAsync(string connectionId, string groupName);

      Task SendAsync(string connectionId, string vmId, string vmData);

      Task SendToManyAsync(IReadOnlyList<string> connectionIds, string vmId, string vmData);

      Task SendToGroupAsync(string groupName, string vmId, string vmData);

      Task SendToGroupExceptAsync(string groupName, IReadOnlyList<string> excludedIds, string vmId, string vmData);

      Task SendToUsersAsync(IReadOnlyList<string> userIds, string vmId, string vmData);
   }

   /// <summary>
   /// Implements invocation of a SignalR hub method for sending view model responses to clients.
   /// </summary>
   public class DotNetifyHubResponse : IDotNetifyHubResponse
   {
      private readonly IHubContext<DotNetifyHub> _hubContext;
      private static readonly string RESPONSE_VM = nameof(IDotNetifyHubMethod.Response_VM);

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="hubContext">DotNetify hub context.</param>
      public DotNetifyHubResponse(IHubContext<DotNetifyHub> hubContext)
      {
         _hubContext = hubContext;
      }

      /// <summary>
      /// Add a connection to a group.
      /// </summary>
      /// <param name="connectionId">SignalR connection.</param>
      /// <param name="groupName">SignalR group name.</param>
      public Task AddToGroupAsync(string connectionId, string groupName) => _hubContext.Groups.AddToGroupAsync(connectionId, groupName);

      /// <summary>
      /// Removes a connection from a group.
      /// </summary>
      /// <param name="connectionId">SignalR connection.</param>
      /// <param name="groupName">SignalR group name.</param>
      public Task RemoveFromGroupAsync(string connectionId, string groupName) => _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName);

      /// <summary>
      /// Invokes Response_VM on a connection.
      /// </summary>
      /// <param name="connectionId">SignalR connection.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public Task SendAsync(string connectionId, string vmId, string vmData) => _hubContext.Clients.Client(connectionId).SendAsync(RESPONSE_VM, new object[] { vmId, vmData });

      /// <summary>
      /// Invokes Response_VM on many connections.
      /// </summary>
      /// <param name="connectionIds">List of SignalR connections.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public Task SendToManyAsync(IReadOnlyList<string> connectionIds, string vmId, string vmData)
      {
         return _hubContext.Clients.Clients(connectionIds).SendAsync(RESPONSE_VM, new object[] { vmId, vmData });
      }

      /// <summary>
      /// Invokes Response_VM on a group.
      /// </summary>
      /// <param name="groupName">SignalR group name.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public Task SendToGroupAsync(string groupName, string vmId, string vmData) => _hubContext.Clients.Group(groupName).SendAsync(RESPONSE_VM, new object[] { vmId, vmData });

      /// <summary>
      /// Invokes Response_VM on a group but exclude some connections.
      /// </summary>
      /// <param name="groupName">SignalR group name.</param>
      /// <param name="excludedConnectionIds">Excluded SignalR connections.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public Task SendToGroupExceptAsync(string groupName, IReadOnlyList<string> excludedConnectionIds, string vmId, string vmData) => _hubContext.Clients.GroupExcept(groupName, excludedConnectionIds).SendAsync(RESPONSE_VM, new object[] { vmId, vmData });

      /// <summary>
      /// Invokes Response_VM on a set of users.
      /// </summary>
      /// <param name="userIds">Identifies the users.</param>
      /// <param name="vmId">Identifies the view model.</param>
      /// <param name="vmData">View model data.</param>
      public Task SendToUsersAsync(IReadOnlyList<string> userIds, string vmId, string vmData) => _hubContext.Clients.Users(userIds).SendAsync(RESPONSE_VM, new object[] { vmId, vmData });
   }
}