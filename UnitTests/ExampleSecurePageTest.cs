using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DotNetify;
using DotNetify.DevApp;
using DotNetify.Security;
using DotNetify.Testing;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Linq;

namespace UnitTests
{
   [TestClass]
   public class ExampleSecurePageTest
   {
      private HubEmulator _hubEmulator;

      private struct ClientState
      {
         public string SecureCaption { get; set; }
         public string SecureData { get; set; }
      }

      private class ExtractAccessTokenMiddleware : JwtBearerAuthenticationMiddleware
      {
         public ExtractAccessTokenMiddleware(TokenValidationParameters tokenValidationParameters) : base(tokenValidationParameters)
         {
         }

         public override Task Invoke(DotNetifyHubContext hubContext, NextDelegate next)
         {
            if (hubContext.Headers != null)
            {
               try
               {
                  ValidateBearerToken(ParseHeaders<HeaderData>(hubContext.Headers), out SecurityToken validatedToken);
                  if (validatedToken != null)
                     hubContext.PipelineData.Add("AccessToken", validatedToken);
               }
               catch (Exception)
               {
               }
            }

            return next(hubContext);
         }
      }

      private class SetAccessTokenFilter : IVMFilter<SetAccessTokenAttribute>
      {
         public Task Invoke(SetAccessTokenAttribute attr, VMContext vmContext, NextFilterDelegate next)
         {
            var methodInfo = vmContext.Instance.GetType().GetTypeInfo().GetMethod("SetAccessToken");
            var accessToken = vmContext.HubContext.PipelineData.ContainsKey("AccessToken") ? vmContext.HubContext.PipelineData["AccessToken"] : null;

            if (methodInfo != null && accessToken != null)
               methodInfo.Invoke(vmContext.Instance, new object[] { accessToken as SecurityToken });

            return next(vmContext);
         }
      }

      public const string SecretKey = "my_secretkey_123!";
      public static readonly SecurityKey SigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

      public ExampleSecurePageTest()
      {
         var tokenValidationParams = new TokenValidationParameters
         {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKeys = new List<SecurityKey> { SigningKey }
         };

         _hubEmulator = new HubEmulatorBuilder()
            .Register<SecurePageVM>()
            .UseMiddleware<JwtBearerAuthenticationMiddleware>(tokenValidationParams)
            .UseMiddleware<ExtractAccessTokenMiddleware>(tokenValidationParams)
            .UseFilter<SetAccessTokenFilter>()
            .Build();
      }

      [TestMethod]
      public void ExampleSecurePage_Connect_NoAuthToken_ReturnsNoSecureData()
      {
         var client = _hubEmulator.CreateClient();

         client.Connect(nameof(SecurePageVM));
         var state = client.GetState<ClientState>();

         Assert.IsNull(state.SecureCaption);
         Assert.IsNull(state.SecureData);
      }

      [TestMethod]
      public void ExampleSecurePage_Connect_GuestAuthToken_ReturnsSecureData()
      {
         var expireSeconds = 3;

         var client = _hubEmulator.CreateClient();

         var options = new VMConnectOptions();
         options.Headers.Set("Authorization", "Bearer " + CreateBearerToken("john", "guest", expireSeconds));

         client.Connect(nameof(SecurePageVM), options);

         var responses = client.Listen(expireSeconds * 1000);

         var state = client.GetState<ClientState>();
         Assert.AreEqual("Authenticated user: \"john\"", state.SecureCaption);
         Assert.IsTrue(state.SecureData.StartsWith("Access token will expire in"));
      }

      [TestMethod]
      public void ExampleSecurePage_Connect_ReplaceToken_ReturnsCorrectUser()
      {
         var expireSeconds = 5;

         var client = _hubEmulator.CreateClient();

         var options = new VMConnectOptions();
         options.Headers.Set("Authorization", "Bearer " + CreateBearerToken("john", "guest", expireSeconds));

         client.Connect(nameof(SecurePageVM), options);

         client.Listen(1000);

         var state = client.GetState<ClientState>();
         Assert.AreEqual("Authenticated user: \"john\"", state.SecureCaption);

         client.Dispatch(new Dictionary<string, object> {
            { "$headers", new { Authorization = "Bearer " + CreateBearerToken("bob", "guest", expireSeconds) } },
            { "Refresh", "true" }
         });

         client.Listen(1000);
         state = client.GetState<ClientState>();
         Assert.AreEqual("Authenticated user: \"bob\"", state.SecureCaption);
      }

      [TestMethod]
      public void ExampleSecurePage_Connect_SystemTextJsonProtocol_ReturnsSecureData()
      {
         var expireSeconds = 3;

         var client = _hubEmulator.CreateClient();

         var vmConnectOptions = new VMConnectOptions();
         vmConnectOptions.Headers.Set("Authorization", "Bearer " + CreateBearerToken("john", "guest", expireSeconds));

         var serializedString = ((JObject) vmConnectOptions).ToString();
         object options = JsonSerializer.Deserialize<object>(serializedString);

         client.Connect(nameof(SecurePageVM), options);

         var responses = client.Listen(expireSeconds * 1000);

         var state = client.GetState<ClientState>();
         Assert.AreEqual("Authenticated user: \"john\"", state.SecureCaption);
         Assert.IsTrue(state.SecureData.StartsWith("Access token will expire in"));
      }

      [TestMethod]
      public void ExampleSecurePage_Connect_MessagePackProtocol_ReturnsSecureData()
      {
         var expireSeconds = 3;

         var client = _hubEmulator.CreateClient();

         var vmConnectOptions = new VMConnectOptions();
         vmConnectOptions.Headers.Set("Authorization", "Bearer " + CreateBearerToken("john", "guest", expireSeconds));

         var options = ((JObject) vmConnectOptions).ToObject<Dictionary<object, object>>();

         client.Connect(nameof(SecurePageVM), options);

         var responses = client.Listen(expireSeconds * 1000);

         var state = client.GetState<ClientState>();
         Assert.AreEqual("Authenticated user: \"john\"", state.SecureCaption);
         Assert.IsTrue(state.SecureData.StartsWith("Access token will expire in"));
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