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
using System.Diagnostics;
using System.Linq;

namespace DotNetify.Routing
{
   /// <summary>
   /// Information to pass from the controller to the Razor view.
   /// </summary>
   public class RoutingViewData
   {
      public string UrlPath { get; set; }
      public string Root { get; set; }
      public string Active { get; set; }
      public string Origin { get; set; }
      public string OriginRoot { get; set; }
      public RouteTemplate ActiveTemplate { get; set; }

      public RoutingViewData(string iUrlPath, string iViewUrl, Type iVMType)
      {
         UrlPath = iUrlPath;
         ActiveTemplate = new RouteTemplate { ViewUrl = iViewUrl, VMType = iVMType };
         Root = Origin = "";
      }
   }

   public static class RoutableExtension
   {
      /// <summary>
      /// Call this method from the controller to perform routing.
      /// </summary>
      /// <param name="iViewData">Routing view data.</param>
      /// <param name="oModel">Model to be passed to the view.</param>
      /// <returns>View URL.</returns>
      public static string Route(ref RoutingViewData iViewData, out IRoutable oModel)
      {
         var template = iViewData.ActiveTemplate;
         if (template != null)
         {
            try
            {
               oModel = template.VMType != null ? Activator.CreateInstance(template.VMType) as IRoutable : null;
               if (oModel != null)
                  oModel.RouteUrl(ref iViewData);
               return template.ViewUrl;
            }
            catch (Exception ex)
            {
               Trace.Fail(ex.ToString());
            }
         }

         oModel = null;
         return null;
      }

      /// <summary>
      /// Registers route templates.
      /// </summary>
      /// <param name="iRoutable">Routable view model.</param>
      /// <param name="iRoot">Root path to which all other paths will be evaluated.</param>
      /// <param name="iRouteTemplates">Route templates that belong to the view model.</param>
      public static void RegisterRoutes(this IRoutable iRoutable, string iRoot, List<RouteTemplate> iRouteTemplates)
      {
         iRoutable.RoutingState = new RoutingState { Root = iRoot, Templates = iRouteTemplates };
      }

      /// <summary>
      /// Defines a route from a route template that belongs to the view model.
      /// </summary>
      /// <param name="iRoutable">Routable view model.</param>
      /// <param name="iTemplateId">Identifies a template that belongs to this view model.</param>
      /// <param name="iPath">Optional path, to be used to replace parameterized template's URL pattern.</param>
      /// <returns>Route object to be bound to vmRoute on the view.</returns>
      public static Route GetRoute(this IRoutable iRoutable, string iTemplateId, string iPath = null)
      {
         if (iRoutable.RoutingState == null)
            iRoutable.RoutingState = new RoutingState();

         RouteTemplate template = null;
         if (iRoutable.RoutingState.Templates != null)
            template = iRoutable.RoutingState.Templates.FirstOrDefault(i => i.Id == iTemplateId);

         if (template == null)
            throw new InvalidOperationException(String.Format("ERROR: Route template '{0}' was not found.", iTemplateId));

         return new Route { TemplateId = template.Id, Path = iPath ?? template.UrlPattern };
      }

      /// <summary>
      /// Defines a route that belongs to another view model.
      /// </summary>
      /// <param name="iRoutable">Routable view model.</param>
      /// <param name="iRedirectRoot">Root path of the route. If the path partially matches the view model's root path, they will be combined.</param>
      /// <param name="iPath">Route path.</param>
      /// <returns>Route object to be bound to vmRoute on the view.</returns>
      public static Route Redirect(this IRoutable iRoutable, string iRedirectRoot, string iPath)
      {
         if (iRoutable.RoutingState == null)
            iRoutable.RoutingState = new RoutingState();

         return new Route { RedirectRoot = iRedirectRoot, Path = iPath };
      }

      /// <summary>
      /// Handles the activate event, which occurs when a route is being activated.
      /// </summary>
      /// <param name="iRoutable">Routable view model.</param>
      /// <param name="iEventHandler">Activate event handler.</param>
      public static void OnActivated(this IRoutable iRoutable, EventHandler<ActivatedEventArgs> iEventHandler)
      {
         if (iRoutable.RoutingState == null)
            iRoutable.RoutingState = new RoutingState();

         iRoutable.RoutingState.Activated += iEventHandler;
      }

      /// <summary>
      /// Handles the routed event, which occurs when this view model is being routed to.
      /// </summary>
      /// <param name="iRoutable">Routable view model.</param>
      /// <param name="iEventHandler">Routed event handler.</param>
      public static void OnRouted(this IRoutable iRoutable, EventHandler<RoutedEventArgs> iEventHandler)
      {
         if (iRoutable.RoutingState == null)
            iRoutable.RoutingState = new RoutingState();

         iRoutable.RoutingState.Routed += iEventHandler;
      }

      /// <summary>
      /// Performs routing. The URL path is given inside the view data, along with the initial route template to start from.
      /// This is a recursive method that will be called again by the nested views until the route is resolved.
      /// </summary>
      /// <param name="iRoutable">Routable view model.</param>
      /// <param name="iViewData">Routing view data.</param>
      public static void RouteUrl(this IRoutable iRoutable, ref RoutingViewData iViewData)
      {
         var routingState = iRoutable.RoutingState;

         iViewData.ActiveTemplate = null;
         routingState.Origin = iViewData.Origin;
         if (routingState.Templates != null)
         {
            iViewData.Root = iViewData.Root + "/" + routingState.Root;
            var bestMatch = MatchTemplate(routingState.Templates, iViewData.UrlPath, iViewData.Root);
            if (bestMatch != null)
            {
               iViewData.ActiveTemplate = bestMatch.Value.Key;
               if (bestMatch.Value.Value != null && typeof(IRoutable).IsAssignableFrom(bestMatch.Value.Key.VMType))
                  routingState.Active = bestMatch.Value.Value;
               else
                  routingState.Active = null;
            }
            // If there's no match, but the Active path has a default value, resolve its route.
            else if (routingState.Active != null)
            {
               bestMatch = MatchTemplate(routingState.Templates, routingState.Active, null);
               if (bestMatch != null)
                  iViewData.ActiveTemplate = bestMatch.Value.Key;
            }
         }

         // Pass along information from this view to the next nested view.
         iViewData.Origin = routingState.Active;
      }

      /// <summary>
      /// Returns HTML data attribute markup that contains routing initialization arguments.
      /// This needs to be placed in the same DOM element that has the "data-vm" attribute.
      /// </summary>
      /// <param name="iRoutable">Routable view model.</param>
      /// <param name="iViewData">Routing view data.</param>
      /// <returns>HTML data attribute markup.</returns>
      public static string InitArgs(this IRoutable iRoutable, object iViewData)
      {
         var routingState = iRoutable.RoutingState;

         string originRoot = "";
         if (iViewData is RoutingViewData)
         {
            var viewData = iViewData as RoutingViewData;
            originRoot = viewData.OriginRoot;
            viewData.OriginRoot += "/" + routingState.Root;
         }
         return string.Format("data-vm-root=\"{0}\" data-vm-arg = \"{{'RoutingState.Active': '{1}', 'RoutingState.Origin': '{2}'}}\"", originRoot, routingState.Active, routingState.Origin);
      }

      /// <summary>
      /// Matches a URL path to any of the route templates.
      /// </summary>
      /// <param name="iTemplates">Route templates.</param>
      /// <param name="iUrlPath">Url path to match.</param>
      /// <param name="iRoot">Root path.</param>
      /// <returns>The matching route template and local path.</returns>
      private static KeyValuePair<RouteTemplate, string>? MatchTemplate(List<RouteTemplate> iTemplates, string iUrlPath, string iRoot)
      {
         KeyValuePair<RouteTemplate, string>? bestMatch = null;

         var match = new Dictionary<RouteTemplate, string>();
         foreach (var template in iTemplates)
         {
            // Attempt to find a route template that at least partially matches the URL path.
            var root = template.Root != null ? template.Root : iRoot;
            string path;
            if (Match(iUrlPath, root, template.UrlPattern, out path))
               match.Add(template, path);
         }

         if (match.Count > 0)
         {
            // If there are more than one matches, select the one with the longest path.
            var maxLength = match.Max(i => i.Value.Length);
            bestMatch = match.First(i => i.Value.Length == maxLength);
         }
         return bestMatch;
      }

      /// <summary>
      /// Matches the URL path with the given URL pattern.
      /// </summary>
      /// <param name="iUrlPath">URL path.</param>
      /// <param name="iUrlPattern">URL pattern from a route template.</param>
      /// <param name="oMatchedPatch">Matched path.</param>
      /// <returns>True if the URL path matches the pattern.</returns>
      private static bool Match(string iUrlPath, string iRoot, string iUrlPattern, out string oPath)
      {
         iUrlPath = iUrlPath.ToLower();
         oPath = iUrlPattern;

         var route = iRoot != null ? iRoot.TrimEnd('/').ToLower() + "/" + iUrlPattern.ToLower() : iUrlPattern.ToLower();
         route = route.TrimEnd('/');

         if (!route.Contains(":"))
            return iUrlPath.StartsWith(route);

         var paths = iUrlPath.Split('/').ToList();
         var routes = route.Split('/').ToList().ConvertAll(i => i.Trim('('));

         var actionArgs = new Dictionary<string, string>();
         bool match = false;
         if (paths.Count <= routes.Count)
         {
            match = true;
            for (int i = 0; i < routes.Count && match; i++)
            {
               if (i >= paths.Count)
               {
                  if (!routes[i].EndsWith(")"))
                     match = false;
               }
               else
               {
                  if (routes[i].StartsWith(":"))
                     actionArgs.Add(routes[i].Trim(':', ')'), paths[i]);
                  else if (routes[i] != paths[i])
                     match = false;
               }
            }
         }

         if (match)
         {
            foreach (var arg in actionArgs)
               oPath = oPath.Replace(":" + arg.Key, arg.Value);
            while (oPath.Contains("(/:"))
            {
               var idxStart = oPath.IndexOf("(/:");
               var idxEnd = oPath.IndexOf(')', idxStart);
               oPath = oPath.Remove(idxStart, idxEnd - idxStart + 1);
            }
            oPath = oPath.Replace("(", "").Replace(")", "");
         }

         return match;
      }
   }
}
