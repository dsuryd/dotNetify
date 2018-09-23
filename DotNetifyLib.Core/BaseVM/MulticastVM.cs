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
   public abstract class MulticastVM : BaseVM, IMulticast
   {
      /// <summary>
      /// Reference count of VMController instances.
      /// </summary>
      private int _reference = 1;

      /// <summary>
      /// Keeps changed properties to be multicasted to connected clients.
      /// </summary>
      private IDictionary<string, object> _changedProperties;

      /// <summary>
      /// Determine whether the view model can be shared with the calling VMController.
      /// </summary>
      public abstract bool IsMember { get; }

      /// <summary>
      /// Increment reference count.
      /// </summary>
      public void AddRef()
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
      /// Overrides the base method to cache the changed properties before it gets cleared.
      /// </summary>
      public override void PushUpdates()
      {
         _changedProperties = new Dictionary<string, object>(ChangedProperties);
         base.PushUpdates();
         base.AcceptChangedProperties();
      }

      /// <summary>
      /// Overrides the base method to return the cached changed properties.
      /// </summary>
      /// <returns>Changed properties.</returns>
      internal override IDictionary<string, object> AcceptChangedProperties()
      {
         return _changedProperties;
      }
   }
}