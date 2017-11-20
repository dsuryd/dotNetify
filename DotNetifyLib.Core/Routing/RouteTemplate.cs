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

namespace DotNetify.Routing
{
   /// <summary>
   /// Defines a template for one or more routes that loads a common view.
   /// </summary>
   public class RouteTemplate
   {
      /// <summary>
      /// Identifies this template.
      /// </summary>
      public string Id { get; set; }

      /// <summary>
      /// Optional; only set it if you want to route this to a different root.
      /// </summary>
      public string Root { get; set; }

      /// <summary>
      /// URL pattern unique to this template.
      /// </summary>
      public string UrlPattern { get; set; }

      /// <summary>
      /// URL for the view to route to.
      /// </summary>
      public string ViewUrl { get; set; }

      /// <summary>
      /// URL for the javascript module for the view. Optional.
      /// </summary>
      public string JSModuleUrl { get; set; }

      /// <summary>
      /// Identifies the target DOM element where the view will be loaded.
      /// </summary>
      public string Target { get; set; }

      /// <summary>
      /// View model type associated with the view. Set this only for server-side routing.
      /// </summary>
      [Ignore]
      public Type VMType { get; set; }

      /// <summary>
      /// Default constructor.
      /// </summary>
      public RouteTemplate()
      {
      }

      /// <summary>
      /// Contructor that accepts the identity key and JS module to load.
      /// </summary>
      /// <param name="id">Identifies this template; also used for the View name and the URL pattern by default.</param>
      /// <param name="jsModuleUrl">URL of Javascript module.</param>
      public RouteTemplate(string id, string jsModuleUrl = null) : this()
      {
         Id = id;
         JSModuleUrl = jsModuleUrl;
      }
   }
}
