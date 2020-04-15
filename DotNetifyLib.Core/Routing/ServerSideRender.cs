/*
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace DotNetify.Routing
{
   public static class ServerSideRender
   {
      public static string GetInitialStates(IVMFactory vmFactory, ref string path, Type entryVMType)
      {
         string result = null;

         try
         {
            using (var vmController = new VMController((arg1, arg2, arg3) => { }, vmFactory))
            {
               // Traverse the routing path to get initial states of all the view models involved.
               var vmStates = new List<string>();
               if (!Path.HasExtension(path))
               {
                  var viewData = new RoutingViewData(path, null, entryVMType);
                  RoutableExtension.Route(ref viewData, out object vm);
                  while (vm != null)
                  {
                     object vmArgs = null;
                     if (vm is IRoutable routable)
                     {
                        // If at the end of the path and the view model has a default route template (blank url pattern),
                        // append a slash to the path to ensure it's correctly routed.
                        if (path.Trim('/').Length > 0 && string.Compare(viewData.UrlPath, viewData.Root, true) == 0 && routable.RoutingState.Templates.Any(i => i.UrlPattern == ""))
                           path += "/";

                        // Determine the "RoutingState.Origin" property value and pass it as argument to the view model
                        // associated with the current path to set its initial state correctly.
                        string args = routable.InitArgs(viewData);
                        Match match = Regex.Match(args, "'RoutingState.Origin':\\s*'(.*?)'");
                        if (match.Success)
                           vmArgs = JsonConvert.DeserializeObject($"{{{match.Value}}}");
                     }

                     string vmName = vm.GetType().Name;
                     vmStates.Add($"\"{vmName}\":{vmController.GetInitialState(vmName, vmArgs)}");

                     // Traverse the next path.
                     RoutableExtension.Route(ref viewData, out vm);
                  }
                  result = $"{{{string.Join(",", vmStates)}}}";
               }
               return result;
            }
         }
         catch (Exception ex)
         {
            System.Diagnostics.Trace.Fail(ex.ToString());
            return null;
         }
      }
   }
}