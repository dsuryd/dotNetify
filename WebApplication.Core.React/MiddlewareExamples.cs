using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DotNetify;
using DotNetify.Security;

namespace WebApplication.Core.React
{
   public class LogRequestMiddleware : IMiddleware
    {
      public void Invoke(DotNetifyHubContext hubContext)
      {
         Trace.WriteLine($"{hubContext.CallType} {hubContext.VMId}, {JsonConvert.SerializeObject(hubContext.Data)}");
      }
    }

   public class JwtBearerAuthenticationMiddleware : IMiddleware
   {
      private class HeaderData
      {
         public string Authorization { get; set; }
      }

      public void Invoke(DotNetifyHubContext hubContext)
      {
         var headers = ParseHeaders<HeaderData>(hubContext);
         if (headers?.Authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
         {
            var token = headers.Authorization.Substring("Bearer ".Length).Trim();
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            Trace.WriteLine("JWT: " + JsonConvert.SerializeObject(jwt));
         }
      }

      private T ParseHeaders<T>(DotNetifyHubContext context)
      {
         try
         {
            return context.Headers is JObject ? (context.Headers as JObject).ToObject<T>() : default(T);
         }
         catch (Exception) { }
         return default(T);
      }
   }

   public class AuthorizeFilter : IVMFilter<AuthorizeAttribute>
   {
      public void Invoke(AuthorizeAttribute auth, VMContext context)
      {
         var principal = context.HubContext.CallerContext.User;

         bool authd = principal.Identity.IsAuthenticated;
         if (authd)
         {
            if (!string.IsNullOrEmpty(auth.Role))
               authd &= principal.IsInRole(auth.Role);

            if (!string.IsNullOrEmpty(auth.ClaimType))
               authd &= principal is ClaimsPrincipal ? (principal as ClaimsPrincipal).HasClaim(auth.ClaimType, auth.ClaimValue) : false;
         }

         if (!authd)
            throw new UnauthorizedAccessException();
      }
   }
}
