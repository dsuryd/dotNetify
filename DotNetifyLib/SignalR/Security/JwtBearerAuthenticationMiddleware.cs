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
using System.Security.Claims;
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
      protected class HeaderData
      {
         public string Authorization { get; set; }
      }

      private readonly TokenValidationParameters _tokenValidationParameters;

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="tokenValidationParameters">Parameters for validating a token.</param>
      public JwtBearerAuthenticationMiddleware(TokenValidationParameters tokenValidationParameters)
      {
         _tokenValidationParameters = tokenValidationParameters;
      }

      /// <summary>
      /// Invokes middleware.
      /// </summary>
      /// <param name="hubContext">DotNetify hub context.</param>
      /// <param name="next">Next middleware delegate.</param>
      public virtual Task Invoke(DotNetifyHubContext hubContext, NextDelegate next)
      {
         ClaimsPrincipal principal = null;
         try
         {
            SecurityToken validatedToken;
            principal = ValidateBearerToken(ParseHeaders<HeaderData>(hubContext.Headers), out validatedToken);
         }
         catch (Exception ex)
         {
            Trace.WriteLine(ex.Message);
         }

         hubContext.Principal = principal ?? hubContext.CallerContext.User;
         return next(hubContext);
      }

      /// <summary>
      /// Validates the bearer token inside the Authorization header.
      /// </summary>
      /// <param name="headers">Headers data.</param>
      /// <param name="validatedToken">Validated bearer token.</param>
      /// <returns>Claims principal from the token.</returns>
      protected ClaimsPrincipal ValidateBearerToken(HeaderData headers, out SecurityToken validatedToken)
      {
         validatedToken = null;
         if (headers?.Authorization?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
         {
            var token = headers.Authorization.Substring("Bearer ".Length).Trim();
            return new JwtSecurityTokenHandler().ValidateToken(token, _tokenValidationParameters, out validatedToken);
         }
         return null;
      }

      /// <summary>
      /// Parses headers JSON into object of a certain type.
      /// </summary>
      /// <param name="headers">Headers in JSON or null.</param>
      /// <returns>Headers object.</returns>
      protected T ParseHeaders<T>(object headers) => headers is JObject ? (headers as JObject).ToObject<T>() : default(T);
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
