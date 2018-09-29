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
using System.Linq;
using System.Threading;

namespace DotNetify
{
   // Implements multicast view models.  A multicast view model can be connected to multiple views,
   // basically serving as a single data source for those views.
   public class MulticastVM : BaseVM
   {
      /// <summary>
      /// Reference count of VMController instances.
      /// </summary>
      private int _reference = 1;

      /// <summary>
      /// Group name generated when this instance was created.
      /// </summary>
      private string _groupName;

      /// <summary>
      /// Occurs when the view model wants to push updates to all associated clients.
      /// This event is handled by the VMController.
      /// </summary>
      public event EventHandler<MulticastPushUpdatesEventArgs> RequestMulticastPushUpdates;

      /// <summary>
      /// Occurs when the view model wants to push data to specific clients.
      /// This event is handled by the VMController.
      /// </summary>
      public event EventHandler<SendEventArgs> RequestSend;

      /// <summary>
      /// Multicast group name.
      /// </summary>
      [Ignore]
      public virtual string GroupName { get; }

      /// <summary>
      /// Determine whether the view model can be shared with the calling VMController.
      /// </summary>
      internal virtual bool IsMember => _groupName == GroupName;

      /// <summary>
      /// Increment reference count.
      /// </summary>
      internal void AddRef()
      {
         Interlocked.Increment(ref _reference);
      }

      /// <summary>
      /// Overrides the base method to dispose only when the reference count is zero.
      /// </summary>
      public override void Dispose()
      {
         if (Interlocked.Decrement(ref _reference) == 0)
            base.Dispose();
      }

      /// <summary>
      /// Initializes the group name asssociated with this view model.
      /// the
      /// </summary>
      internal void Initialize()
      {
         _groupName = GroupName;
      }

      /// <summary>
      /// Overrides the base method to call the specialized multicast push updates method.
      /// </summary>
      public override void PushUpdates()
      {
         PushUpdatesExcept(null);
      }

      /// <summary>
      /// Pushes changed properties to all associated clients, excluding the given connection.
      /// </summary>
      /// <param name="excludedConnectionId">Connection to exclude.</param>
      internal void PushUpdatesExcept(string excludedConnectionId)
      {
         var delegates = RequestMulticastPushUpdates?.GetInvocationList().ToList();
         if (delegates != null && delegates.Count > 0)
         {
            var eventArgs = new MulticastPushUpdatesEventArgs { ExcludedConnectionId = excludedConnectionId };

            // First invocation cycle is to get the participating connection Ids from the VMControllers.
            RequestMulticastPushUpdates?.Invoke(this, eventArgs);

            // Invoke the first available event handler to do the actual data push.
            // If successful, the handler will set the PushData back to false.
            eventArgs.PushData = true;
            foreach (var d in delegates)
            {
               d.DynamicInvoke(this, eventArgs);
               if (!eventArgs.PushData)
                  break;
            }
         }
      }

      /// <summary>
      /// Sends data to specific connecting clients.
      /// </summary>
      /// <param name="connectionIds">Identifies the clients.</param>
      /// <param name="propertyName">Property name to send.</param>
      /// <param name="propertyValue">Property value to send.</param>
      protected void Send<T>(IList<string> connectionIds, string propertyName, T propertyValue = default(T))
      {
         var eventArgs = new SendEventArgs { Properties = new Dictionary<string, object> { { propertyName, propertyValue } } };
         foreach (string connectionId in connectionIds)
            eventArgs.ConnectionIds.Add(connectionId);
         Send(eventArgs);
      }

      /// <summary>
      /// Sends data to the clients.
      /// </summary>
      /// <param name="args">Contains information on who to send to.</param>
      protected void Send(SendEventArgs args)
      {
         var delegates = RequestSend?.GetInvocationList().ToList();
         if (delegates != null && delegates.Count > 0)
         {
            args.SendData = true;
            foreach (var d in delegates)
            {
               d.DynamicInvoke(this, args);
               if (!args.SendData)
                  break;
            }
         }
      }
   }
}