/* 
Copyright 2015 Dicky Suryadi

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

namespace DotNetify.Routing
{
   /// <summary>
   /// Routings state event arguments.
   /// </summary>
   public class RoutedEventArgs : EventArgs { public string From { get; set; } }
   public class ActivatedEventArgs : EventArgs { public string Active { get; set; } }

   /// <summary>
   /// Routing state information of a view model that implements IRoutable. 
   /// </summary>
   public class RoutingState
   {
      private string _Active;
      private string _Origin;

      /// <summary>
      /// Route templates registered by the view model.
      /// </summary>
      public List<RouteTemplate> Templates { get; set; }

      /// <summary>
      /// Root path to which all other paths will be evaluated.
      /// </summary>
      public string Root { get; set; }

      /// <summary>
      /// Currently active route path.
      /// </summary>
      public string Active
      {
         get { return _Active; }
         set
         {
            _Active = value;
            if (Activated != null)
               Activated(this, new ActivatedEventArgs { Active = value });
         }
      }

      /// <summary>
      /// The origin path from which the view model was routed.
      /// </summary>
      public string Origin
      {
         get { return _Origin; }
         set
         {
            _Origin = value;
            if (Routed != null)
               Routed(this, new RoutedEventArgs { From = value });
         }
      }

      /// <summary>
      /// Occurs when a route is being activated.
      /// </summary>
      public event EventHandler<ActivatedEventArgs> Activated;

      /// <summary>
      /// Occurs when this view model is being routed to.
      /// </summary>
      public event EventHandler<RoutedEventArgs> Routed;
   }
}
