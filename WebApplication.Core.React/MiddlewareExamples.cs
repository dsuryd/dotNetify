using System;
using System.Diagnostics;
using DotNetify;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace WebApplication.Core.React
{
   public class LogRequestMiddleware : IMiddleware
    {
      public void Invoke(IDotNetifyHubContext hubContext)
      {
         Trace.WriteLine($"{hubContext.MethodName} {hubContext.VMId}, {JsonConvert.SerializeObject(hubContext.Data)}");
      }
    }

   public class JwtBearerAuthenticationMiddleware : IMiddleware
   {
      private class HeaderData
      {
         public string Authorization { get; set; }
      }

      public void Invoke(IDotNetifyHubContext hubContext)
      {
         var headers = hubContext.Headers<HeaderData>();
         if (headers != null && headers.Authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
         {
            var token = headers.Authorization.Substring("Bearer ".Length).Trim();
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            Trace.WriteLine("JWT: " + JsonConvert.SerializeObject(jwt));
         }
      }
   }
}
