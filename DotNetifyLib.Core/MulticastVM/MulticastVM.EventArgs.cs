/*
Copyright 2018 Dicky Suryadi

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
using System.Collections.Generic;

namespace DotNetify
{
   /// <summary>
   /// Event arguments used by a multicast view model to request push updates.
   /// </summary>
   public class MulticastPushUpdatesEventArgs : EventArgs
   {
      /// <summary>
      /// Identifies the connections to push the data to.
      /// </summary>
      public IList<string> ConnectionIds { get; } = new List<string>();

      /// <summary>
      /// Connection to exclude.
      /// </summary>
      public string ExcludedConnectionId { get; set; }

      /// <summary>
      /// Whether to push the data. After it's pushed, this will reset to signal success.
      /// </summary>
      public bool PushData { get; set; }
   }

   /// <summary>
   /// Event aguments used by a multicast view model to request sending data to specific clients.
   /// </summary>
   public class SendEventArgs : EventArgs
   {
      /// <summary>
      /// Identifies the connections to push the data to.
      /// </summary>
      public IList<string> ConnectionIds { get; } = new List<string>();

      /// <summary>
      /// Identifies the connections to exclude.
      /// </summary>
      public IList<string> ExcludedConnectionIds { get; } = new List<string>();

      /// <summary>
      /// Group name to push the data to.
      /// </summary>
      public string GroupName { get; set; }

      /// <summary>
      /// Identifies the users to push the data to.
      /// </summary>
      public IList<string> UserIds { get; } = new List<string>();

      /// <summary>
      /// Property values to push.
      /// </summary>
      public IDictionary<string, object> Properties { get; set; }

      /// <summary>
      /// Whether to send the data. After it's sent, this will reset to signal success.
      /// </summary>
      internal bool SendData { get; set; }
   }
}