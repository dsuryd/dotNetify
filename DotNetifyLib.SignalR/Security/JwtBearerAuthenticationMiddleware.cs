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
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;

namespace DotNetify.Security
{
   /// <summary>
   /// Middleware to handle JWT bearer token authentication.
   /// </summary>
   public class JwtBearerAuthenticationMiddleware : IMiddleware
   {
      private class HeaderData
      {
         public string Authorization { get; set; }
      }

      private readonly TokenValidationParameters _tokenValidationParameters;

      public JwtBearerAuthenticationMiddleware(TokenValidationParameters tokenValidationParameters)
      {
         _tokenValidationParameters = tokenValidationParameters;
      }

      public Task Invoke(DotNetifyHubContext hubContext, NextDelegate next)
      {
         try
         {
            var headers = ParseHeaders<HeaderData>(hubContext);
            if (headers?.Authorization?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
            {
               var token = headers.Authorization.Substring("Bearer ".Length).Trim();
               hubContext.Principal = new JwtSecurityTokenHandler().ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken);
            }
         }
         catch (Exception ex)
         {
            Trace.WriteLine(ex.Message);
         }

         return next(hubContext);
      }

      private T ParseHeaders<T>(DotNetifyHubContext context) => context.Headers is JObject ? (context.Headers as JObject).ToObject<T>() : default(T);
   }

   /// <summary>
   /// Method extension to specify parameter type for the middleware.
   /// </summary>
   public static class JwtBearerAuthenticationMiddlewareExtensions
   {
      public static void UseJwtBearerAuthentication(this IDotNetifyConfiguration config, TokenValidationParameters tokenValidationParameters)
      {
         config.UseMiddleware<JwtBearerAuthenticationMiddleware>(tokenValidationParameters);
      }
   }
}
