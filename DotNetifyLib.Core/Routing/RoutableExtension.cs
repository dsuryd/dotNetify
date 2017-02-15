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
using System.Reflection;

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

      public RoutingViewData(string urlPath, string viewUrl, Type vmType)
      {
         UrlPath = urlPath;
         ActiveTemplate = new RouteTemplate { ViewUrl = viewUrl, VMType = vmType };
         Root = Origin = "";
      }
   }

   public static class RoutableExtension
   {
      /// <summary>
      /// Call this method from the controller to perform routing.
      /// </summary>
      /// <param name="viewData">Routing view data.</param>
      /// <param name="oModel">Model to be passed to the view.</param>
      /// <returns>View URL.</returns>
      public static string Route(ref RoutingViewData viewData, out IRoutable oModel)
      {
         var template = viewData.ActiveTemplate;
         if (template != null)
         {
            try
            {
               oModel = template.VMType != null ? VMController.CreateInstance(template.VMType, null) as IRoutable : null;
               if (oModel != null)
                  oModel.RouteUrl(ref viewData);
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
      /// <param name="routable">Routable view model.</param>
      /// <param name="root">Root path to which all other paths will be evaluated.</param>
      /// <param name="routeTemplates">Route templates that belong to the view model.</param>
      public static void RegisterRoutes(this IRoutable routable, string root, List<RouteTemplate> routeTemplates)
      {
         routable.RoutingState = new RoutingState { Root = root, Templates = routeTemplates };
      }

      /// <summary>
      /// Defines a route from a route template that belongs to the view model.
      /// </summary>
      /// <param name="routable">Routable view model.</param>
      /// <param name="templateId">Identifies a template that belongs to this view model.</param>
      /// <param name="path">Optional path, to be used to replace parameterized template's URL pattern.</param>
      /// <returns>Route object to be bound to vmRoute on the view.</returns>
      public static Route GetRoute(this IRoutable routable, string templateId, string path = null)
      {
         if (routable.RoutingState == null)
            routable.RoutingState = new RoutingState();

         RouteTemplate template = null;
         if (routable.RoutingState.Templates != null)
            template = routable.RoutingState.Templates.FirstOrDefault(i => i.Id == templateId);

         if (template == null)
            throw new InvalidOperationException(String.Format("ERROR: Route template '{0}' was not found.", templateId));

         return new Route { TemplateId = template.Id, Path = path ?? template.UrlPattern };
      }

      /// <summary>
      /// Defines a route that belongs to another view model.
      /// </summary>
      /// <param name="routable">Routable view model.</param>
      /// <param name="redirectRoot">Root path of the route. If the path partially matches the view model's root path, they will be combined.</param>
      /// <param name="path">Route path.</param>
      /// <returns>Route object to be bound to vmRoute on the view.</returns>
      public static Route Redirect(this IRoutable routable, string redirectRoot, string path)
      {
         if (routable.RoutingState == null)
            routable.RoutingState = new RoutingState();

         return new Route { RedirectRoot = redirectRoot, Path = path };
      }

      /// <summary>
      /// Handles the activate event, which occurs when a route is being activated.
      /// </summary>
      /// <param name="iRoutable">Routable view model.</param>
      /// <param name="eventHandler">Activate event handler.</param>
      public static void OnActivated(this IRoutable iRoutable, EventHandler<ActivatedEventArgs> eventHandler)
      {
         if (iRoutable.RoutingState == null)
            iRoutable.RoutingState = new RoutingState();

         iRoutable.RoutingState.Activated += eventHandler;
      }

      /// <summary>
      /// Handles the routed event, which occurs when this view model is being routed to.
      /// </summary>
      /// <param name="routable">Routable view model.</param>
      /// <param name="eventHandler">Routed event handler.</param>
      public static void OnRouted(this IRoutable routable, EventHandler<RoutedEventArgs> eventHandler)
      {
         if (routable.RoutingState == null)
            routable.RoutingState = new RoutingState();

         routable.RoutingState.Routed += eventHandler;
      }

      /// <summary>
      /// Performs routing. The URL path is given inside the view data, along with the initial route template to start from.
      /// This is a recursive method that will be called again by the nested views until the route is resolved.
      /// </summary>
      /// <param name="routable">Routable view model.</param>
      /// <param name="viewData">Routing view data.</param>
      public static void RouteUrl(this IRoutable routable, ref RoutingViewData viewData)
      {
         var routingState = routable.RoutingState;
         if (routingState == null)
            return;

         viewData.ActiveTemplate = null;
         routingState.Origin = viewData.Origin;
         if (routingState.Templates != null)
         {
            viewData.Root = viewData.Root + "/" + routingState.Root;
            var bestMatch = MatchTemplate(routingState.Templates, viewData.UrlPath, viewData.Root);
            Trace.WriteLine($"[dotNetify] Matched route {viewData.UrlPath}: {bestMatch?.Value}");
            if (bestMatch != null)
            {
               viewData.ActiveTemplate = bestMatch.Value.Key;
               if (bestMatch.Value.Value != null && typeof(IRoutable).GetTypeInfo().IsAssignableFrom(bestMatch.Value.Key.VMType))
                  routingState.Active = bestMatch.Value.Value;
               else
                  routingState.Active = null;
            }
            // If there's no match, but the Active path has a default value, resolve its route.
            else if (routingState.Active != null)
            {
               bestMatch = MatchTemplate(routingState.Templates, routingState.Active, null);
               if (bestMatch != null)
                  viewData.ActiveTemplate = bestMatch.Value.Key;
            }
         }

         // Pass along information from this view to the next nested view.
         viewData.Origin = routingState.Active;
      }

      /// <summary>
      /// Returns HTML data attribute markup that contains routing initialization arguments.
      /// This needs to be placed in the same DOM element that has the "data-vm" attribute.
      /// </summary>
      /// <param name="routable">Routable view model.</param>
      /// <param name="viewData">Routing view data.</param>
      /// <returns>HTML data attribute markup.</returns>
      public static string InitArgs(this IRoutable routable, object viewData)
      {
         var routingState = routable.RoutingState;

         string originRoot = "";
         if (viewData is RoutingViewData)
         {
            var routingViewData = viewData as RoutingViewData;
            originRoot = routingViewData.OriginRoot;
            routingViewData.OriginRoot += "/" + routingState.Root;
         }
         return string.Format("data-vm-root=\"{0}\" data-vm-arg = \"{{'RoutingState.Active': '{1}', 'RoutingState.Origin': '{2}'}}\"", originRoot, routingState.Active, routingState.Origin);
      }

      /// <summary>
      /// Matches a URL path to any of the route templates.
      /// </summary>
      /// <param name="templates">Route templates.</param>
      /// <param name="urlPath">Url path to match.</param>
      /// <param name="root">Root path.</param>
      /// <returns>The matching route template and local path.</returns>
      private static KeyValuePair<RouteTemplate, string>? MatchTemplate(List<RouteTemplate> templates, string urlPath, string rootPath)
      {
         KeyValuePair<RouteTemplate, string>? bestMatch = null;

         var match = new Dictionary<RouteTemplate, string>();
         foreach (var template in templates)
         {
            // Attempt to find a route template that at least partially matches the URL path.
            var root = template.Root != null ? template.Root : rootPath;
            string path;
            if (Match(urlPath, root, template.UrlPattern, out path))
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
      /// <param name="urlPath">URL path.</param>
      /// <param name="urlPattern">URL pattern from a route template.</param>
      /// <param name="oMatchedPatch">Matched path.</param>
      /// <returns>True if the URL path matches the pattern.</returns>
      private static bool Match(string urlPath, string root, string urlPattern, out string oPath)
      {
         urlPath = urlPath.ToLower();
         oPath = urlPattern;

         var route = root != null ? root.TrimEnd('/').ToLower() + "/" + urlPattern.ToLower() : urlPattern.ToLower();
         route = route.TrimEnd('/');

         if (!route.Contains(":"))
            return urlPath.StartsWith(route);

         var paths = urlPath.Split('/').ToList();
         var routes = route.Split('/').Select(i => i.Trim('(')).ToList();

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
