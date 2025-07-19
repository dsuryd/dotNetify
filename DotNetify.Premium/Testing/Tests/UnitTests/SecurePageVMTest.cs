using DotNetify.Security;
using DotNetify.Testing;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevApp.ViewModels;
using Xunit;

namespace DotNetify.Testing.UnitTests
{
   public class SecurePageVMTest
   {
      private HubEmulator _hubEmulator;

      private struct ClientState
      {
         public string SecureCaption { get; set; }
         public string SecureData { get; set; }
      }

      public const string SecretKey = "my_secretkey_123!";
      public static readonly SecurityKey SigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

      public SecurePageVMTest()
      {
         var tokenValidationParams = new TokenValidationParameters
         {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKeys = new List<SecurityKey> { SigningKey }
         };

         _hubEmulator = new HubEmulatorBuilder()
            .Register<SecurePageVM>(nameof(SecurePageVM))
            .UseMiddleware<JwtBearerAuthenticationMiddleware>(tokenValidationParams)
            .UseMiddleware<ExtractAccessTokenMiddleware>(tokenValidationParams)
            .UseFilter<SetAccessTokenFilter>()
            .Build();
      }

      [Fact]
      public void Connect_NoAuthToken_ReturnsNoSecureData()
      {
         var client = _hubEmulator.CreateClient();

         client.Connect(nameof(SecurePageVM));
         var state = client.GetState<ClientState>();

         Assert.Null(state.SecureCaption);
         Assert.Null(state.SecureData);
      }

      [Fact]
      public void Connect_GuestAuthToken_ReturnsSecureData()
      {
         var expireSeconds = 3;

         var client = _hubEmulator.CreateClient();

         var options = new VMConnectOptions();
         options.Headers.Set("Authorization", $"Bearer {CreateBearerToken("john", "guest", expireSeconds)}");

         client.Connect(nameof(SecurePageVM), options);

         var responses = client.Listen(expireSeconds * 1000);

         var state = client.GetState<ClientState>();
         Assert.Equal("Authenticated user: \"john\"", state.SecureCaption);
         Assert.StartsWith("Access token will expire in", state.SecureData);
      }

      private string CreateBearerToken(string name, string role, int expirationSeconds)
      {
         var token = new JwtSecurityToken(null, "*", new List<Claim>
         {
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Role, role)
         }, null, DateTime.Now.AddSeconds(expirationSeconds), new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256));
         return new JwtSecurityTokenHandler().WriteToken(token);
      }
   }
}