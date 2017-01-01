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

namespace DotNetify.Routing
{
   /// <summary>
   /// Route that's used for deep linking URLs to views.
   /// </summary>
   public class Route
   {
      /// <summary>
      /// Identifies the route template.
      /// </summary>
      public string TemplateId { get; set; }

      /// <summary>
      /// Route path relative to the root path.
      /// </summary>
      public string Path { get; set; }

      /// <summary>
      /// Optional; only set it if you want to redirect to a different root.
      /// </summary>
      public string RedirectRoot { get; set; }
   }
}
