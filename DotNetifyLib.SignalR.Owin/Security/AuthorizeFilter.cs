/* 
Copyright 2017 Dicky Suryadi

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
using System.Security.Claims;
using System.Threading.Tasks;

namespace DotNetify.Security
{
   /// <summary>
   /// View model filter to perform authorization check.
   /// </summary>
   public class AuthorizeFilter : IVMFilter<AuthorizeAttribute>
   {
      public Task Invoke(AuthorizeAttribute auth, VMContext context, NextFilterDelegate next)
      {
         var principal = context.HubContext.Principal;

         bool authd = principal?.Identity?.IsAuthenticated == true;
         if (authd)
         {
            if (!string.IsNullOrEmpty(auth.Role))
               authd &= principal.IsInRole(auth.Role);

            if (!string.IsNullOrEmpty(auth.ClaimType))
               authd &= principal is ClaimsPrincipal ? (principal as ClaimsPrincipal).HasClaim(auth.ClaimType, auth.ClaimValue) : false;
         }

         if (!authd)
            throw new UnauthorizedAccessException();

         return next(context);
      }
   }
}
