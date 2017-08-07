using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace WebApplication.Core.React
{
   public static class AppBuilderExtension
   {
      /// <summary>
      /// Provides end point that generates authentication tokens.
      /// </summary>
      /// <param name="app"></param>
      public static void UseAuthServer(this IApplicationBuilder app)
      {
         string secretKey = "dotnetifydemo_secretkey_123!";
         var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

         app.UseMiddleware<AuthServerMiddleware>(Options.Create(new TokenProviderOptions
         {
            Audience = "DotNetifyDemoApp",
            Issuer = "DotNetifyDemoServer",
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
         }));
      }
   }

   public class TokenProviderOptions
   {
      public string Path { get; set; } = "/token";
      public string Issuer { get; set; }
      public string Audience { get; set; }
      public TimeSpan Expiration { get; set; } = TimeSpan.FromSeconds(30);
      public SigningCredentials SigningCredentials { get; set; }
   }

   /// <summary>
   /// Middleware for generating authentication tokens.
   /// Reference: https://stormpath.com/blog/token-authentication-asp-net-core
   /// </summary>
   public class AuthServerMiddleware
   {
      private readonly RequestDelegate _next;
      private readonly TokenProviderOptions _options;

      public AuthServerMiddleware(RequestDelegate next, IOptions<TokenProviderOptions> options)
      {
         _next = next;
         _options = options.Value;
      }

      public Task Invoke(HttpContext context)
      {
         // If the request path doesn't match, skip
         if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            return _next(context);

         // Request must be POST with Content-Type: application/x-www-form-urlencoded
         if (!context.Request.Method.Equals("POST") || !context.Request.HasFormContentType)
         {
            context.Response.StatusCode = 400;
            return context.Response.WriteAsync("Bad request.");
         }

         return GenerateToken(context);
      }

      private async Task GenerateToken(HttpContext context)
      {
         var username = context.Request.Form["username"];
         var password = context.Request.Form["password"];

         var identity = await GetIdentity(username, password);
         if (identity == null)
         {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid username or password.");
            return;
         }

         var now = DateTimeOffset.Now;

         // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
         // You can add other claims here, if you want:
         var claims = new List<Claim>
         {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
         };

         claims.AddRange(identity.Claims);

         // Create the JWT and write it to a string
         var jwt = new JwtSecurityToken(
             issuer: _options.Issuer,
             audience: _options.Audience,
             claims: claims,
             notBefore: now.DateTime,
             expires: now.DateTime.Add(_options.Expiration),
             signingCredentials: _options.SigningCredentials);
         var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

         var response = new
         {
            access_token = encodedJwt,
            expires_in = (int)_options.Expiration.TotalSeconds
         };

         // Serialize and return the response
         context.Response.ContentType = "application/json";
         await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
      }

      private Task<ClaimsIdentity> GetIdentity(string username, string password)
      {
         ClaimsIdentity identity = null;

         if (username == "guest" && password == "dotnetify")
         {
            identity = new ClaimsIdentity(new GenericIdentity(username, "Token"), new Claim[] {
               new Claim(ClaimsIdentity.DefaultNameClaimType, username)
            });
         }
         else if (username == "admin" && password == "dotnetify")
         {
            identity = new ClaimsIdentity(new GenericIdentity(username, "Token"), new Claim[] {
               new Claim(ClaimsIdentity.DefaultNameClaimType, username),
               new Claim(ClaimsIdentity.DefaultRoleClaimType, "admin")
            });
         }

         return Task.FromResult<ClaimsIdentity>(identity);
      }
   }
}
