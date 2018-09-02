using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DotNetify.DevApp
{
   public static class AuthServer
   {
      public const string SecretKey = "my_secretkey_123!";

      // Source: https://github.com/aspnet-contrib/AspNet.Security.OpenIdConnect.Server
      public static void AddAuthenticationServer(this IServiceCollection services)
      {
         var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

         services.AddAuthentication().AddOpenIdConnectServer(options =>
         {
            options.AccessTokenHandler = new JwtSecurityTokenHandler();
            options.SigningCredentials.AddKey(signingKey);

            options.AllowInsecureHttp = true;
            options.TokenEndpointPath = "/token";

            options.Provider.OnValidateTokenRequest = context =>
            {
               context.Validate();
               return Task.CompletedTask;
            };

            options.Provider.OnHandleTokenRequest = context =>
            {
               if (context.Request.Password != "dotnetify")
               {
                  context.Reject(
                      error: OpenIdConnectConstants.Errors.InvalidGrant,
                      description: "Invalid user credentials.");
                  return Task.CompletedTask;
               }

               var identity = new ClaimsIdentity(context.Scheme.Name,
                  OpenIdConnectConstants.Claims.Name,
                  OpenIdConnectConstants.Claims.Role);

               identity.AddClaim(OpenIdConnectConstants.Claims.Name, context.Request.Username);
               identity.AddClaim(OpenIdConnectConstants.Claims.Subject, context.Request.Username);

               identity.AddClaim(ClaimTypes.Name, context.Request.Username,
                  OpenIdConnectConstants.Destinations.AccessToken,
                  OpenIdConnectConstants.Destinations.IdentityToken);

               if (context.Request.Username == "admin")
               {
                  identity.AddClaim(ClaimTypes.Role, "admin",
                     OpenIdConnectConstants.Destinations.AccessToken,
                     OpenIdConnectConstants.Destinations.IdentityToken);
               }

               var ticket = new AuthenticationTicket(
                  new ClaimsPrincipal(identity),
                  new AuthenticationProperties(),
                  context.Scheme.Name);

               ticket.SetAccessTokenLifetime(System.TimeSpan.FromSeconds(30));

               ticket.SetScopes(
                        OpenIdConnectConstants.Scopes.Profile,
                        OpenIdConnectConstants.Scopes.OfflineAccess);

               context.Validate(ticket);
               return Task.CompletedTask;
            };
         });
      }
   }
}