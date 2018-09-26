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
   /// Indicates the view model that supports multicast to multiple views.
   /// </summary>
   public interface IMulticast
   {
      /// <summary>
      /// Determine whether the view model can be shared with the calling VMController.
      /// </summary>
      bool IsMember { get; }

      /// <summary>
      /// Occurs when the view model wants to push updates to all associated clients.
      /// This event is handled by the VMController.
      /// </summary>
      event EventHandler<MulticastPushUpdatesEventArgs> RequestMulticastPushUpdates;

      /// <summary>
      /// Pushes updates to all connected views.
      /// </summary>
      /// <param name="excludedConnectionId">Connection to exclude.</param>
      void PushUpdates(string excludedConnectionId);
   }

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
      /// Whether to push the data.
      /// </summary>
      public bool PushData { get; set; }
   }
}