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
   }
}
