using System;
using System.Security.Principal;
using System.Security.Claims;

namespace DotNetify.Security
{
   [AttributeUsage(AttributeTargets.Class)]
   public class AuthorizeAttribute : Attribute
   {
      public string ClaimType { get; set; }
      public string ClaimValue { get; set; }
      public string Role { get; set; }

      /// <summary>
      /// Returns whether the current security context is authorized to access the adorned class.
      /// </summary>
      /// <param name="principal">Security context.</param>
      /// <returns>True if authorized.</returns>
      public virtual bool IsAuthorized( IPrincipal principal )
      {
         if ( principal == null || principal.Identity == null )
            return false;

         bool authd = principal.Identity.IsAuthenticated;

         if ( !string.IsNullOrEmpty(Role) )
            authd &= principal.IsInRole(Role);

         if ( !string.IsNullOrEmpty(ClaimType) )
            authd &= principal is ClaimsPrincipal ? ( principal as ClaimsPrincipal ).HasClaim(ClaimType, ClaimValue) : false;

         return authd;
      }
   }
}
