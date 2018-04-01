/*
Copyright 2016-2018 Dicky Suryadi

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

using System.Security.Principal;
using DotNetify.Security;
using System.Threading;

namespace DotNetify
{
   /// <summary>
   /// Implements ambient data store for the current principal of the hub.
   /// </summary>
   internal class HubPrincipalAccessor : IPrincipalAccessor
   {
      private readonly static AsyncLocal<IPrincipal> _asyncLocal = new AsyncLocal<IPrincipal>();

      public IPrincipal Principal
      {
         get { return _asyncLocal.Value; }
         set { _asyncLocal.Value = value; }
      }
   }
}