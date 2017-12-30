using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetify;
using DotNetify.Security;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace UnitTests
{
   [TestClass]
   public class HubMiddlewareTest
   {
      private class MiddlewareTestVM : BaseVM
      {
         public string Property { get; set; } = "Hello";
      }

      private abstract class MiddlewareBase : IMiddleware
      {
         public event EventHandler<DotNetifyHubContext> Invoked;

         public virtual Task Invoke(DotNetifyHubContext context, NextDelegate next)
         {
            Invoked?.Invoke(this, context);
            context.PipelineData.Add(GetType().Name, null);
            return next(context);
         }
      }

      private class CustomMiddleware1 : MiddlewareBase { }
      private class CustomMiddleware2 : MiddlewareBase { }

      private CustomMiddleware1 _middleware1;
      private CustomMiddleware2 _middleware2;
      private JObject _vmArg;
      private IMemoryCache _headersCache = MockDotNetifyHub.CreateMemoryCache();

      [TestInitialize]
      public void Initialize()
      {
         string secretKey = "dotnetifydemo_secretkey_123!";
         var tokenValidationParameters = new TokenValidationParameters
         {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidAudience = "DotNetifyDemoApp",
            ValidIssuer = "DotNetifyDemoServer",
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(0)
         };

         var types = new Dictionary<Type, object>
         {
            {typeof(CustomMiddleware1), (_middleware1 = new CustomMiddleware1()) },
            {typeof(CustomMiddleware2), (_middleware2 = new CustomMiddleware2()) },
            {typeof(ExtractHeadersMiddleware), new ExtractHeadersMiddleware(_headersCache) },
            {typeof(JwtBearerAuthenticationMiddleware), new JwtBearerAuthenticationMiddleware(tokenValidationParameters) },
         };

         VMController.CreateInstance = (type, args) => types.ContainsKey(type) ? types[type] : Activator.CreateInstance(type, args);

         _vmArg = JObject.Parse(@"{
            Property: 'World',
            $headers: {
               Authorization: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsImp0aSI6IjI0YTUwOGJlLWJlMTktNDFhZS1iZmI1LTc5OGU4YmNjNDI3ZCIsImlhdCI6MTUxNDUyODgxNiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6WyJhZG1pbiIsImFkbWluIl0sImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6ImFkbWluIiwibmJmIjoxNTE0NTI4ODE2LCJleHAiOjE4Mjk4ODg4MTYsImlzcyI6IkRvdE5ldGlmeURlbW9TZXJ2ZXIiLCJhdWQiOiJEb3ROZXRpZnlEZW1vQXBwIn0.q2wZyS13FskQ094O9xbz4FLlRPPHf1GfKOUOTHJyLbk'
            }
         }");
      }

      [TestCleanup]
      public void Cleanup()
      {
         VMController.CreateInstance = (type, args) => Activator.CreateInstance(type, args);
      }

      [TestMethod]
      public void Middleware_RequestIntercepted()
      {
         VMController.Register<MiddlewareTestVM>();
         var hub = new MockDotNetifyHub()
            .UseMiddleware<ExtractHeadersMiddleware>()
            .UseMiddleware<JwtBearerAuthenticationMiddleware>()
            .UseMiddleware<CustomMiddleware1>()
            .UseMiddleware<CustomMiddleware2>()
            .Create();

         string vmName = null;
         dynamic vmData = null;
         hub.Response += (sender, e) =>
         {
            vmName = e.Item1;
            vmData = JsonConvert.DeserializeObject<dynamic>(e.Item2);
         };

         Action middleware1Assertions = null;
         _middleware1.Invoked += (sender, context) =>
         {
            var callType = context.CallType;
            var connectionId = context.CallerContext.ConnectionId;
            var testVMPropValue = (context.Data as JObject)[nameof(MiddlewareTestVM.Property)];
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            middleware1Assertions = () =>
            {
               Assert.AreEqual("Request_VM", callType);
               Assert.AreEqual(hub.ConnectionId, connectionId);
               Assert.AreEqual("World", testVMPropValue);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
            };
         };

         Action middleware2Assertions = null;
         _middleware2.Invoked += (sender, context) =>
         {
            var callType = context.CallType;
            var connectionId = context.CallerContext.ConnectionId;
            var testVMPropValue = (context.Data as JObject)[nameof(MiddlewareTestVM.Property)];
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            middleware2Assertions = () =>
            {
               Assert.AreEqual("Request_VM", callType);
               Assert.AreEqual(hub.ConnectionId, connectionId);
               Assert.AreEqual("World", testVMPropValue);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(1, pipelineData.Count);
            };
         };

         hub.RequestVM(nameof(MiddlewareTestVM), _vmArg);

         Assert.AreEqual(nameof(MiddlewareTestVM), vmName);
         Assert.AreEqual("World", vmData.Property.ToString());
         middleware1Assertions();
         middleware2Assertions();
      }

      [TestMethod]
      public void Middleware_UpdateIntercepted()
      {
         VMController.Register<MiddlewareTestVM>();
         var hub = new MockDotNetifyHub()
            .UseMiddleware<ExtractHeadersMiddleware>()
            .UseMiddleware<JwtBearerAuthenticationMiddleware>()
            .UseMiddleware<CustomMiddleware1>()
            .UseMiddleware<CustomMiddleware2>()
            .Create();

         Action middleware1Assertions = null;
         _middleware1.Invoked += (sender, context) =>
         {
            if (context.CallType != "Update_VM")
               return;

            var connectionId = context.CallerContext.ConnectionId;
            var testVMPropValue = (context.Data as Dictionary<string, object>)[nameof(MiddlewareTestVM.Property)];
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            middleware1Assertions = () =>
            {
               Assert.AreEqual(hub.ConnectionId, connectionId);
               Assert.AreEqual("Goodbye", testVMPropValue);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
            };
         };

         Action middleware2Assertions = null;
         _middleware2.Invoked += (sender, context) =>
         {
            if (context.CallType != "Update_VM")
               return;

            var connectionId = context.CallerContext.ConnectionId;
            var testVMPropValue = (context.Data as Dictionary<string, object>)[nameof(MiddlewareTestVM.Property)];
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            middleware2Assertions = () =>
            {
               Assert.AreEqual(hub.ConnectionId, connectionId);
               Assert.AreEqual("Goodbye", testVMPropValue);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(1, pipelineData.Count);
            };
         };

         hub.RequestVM(nameof(MiddlewareTestVM), _vmArg);
         hub.UpdateVM(nameof(MiddlewareTestVM), new Dictionary<string, object> { { "Property", "Goodbye" } });

         middleware1Assertions();
         middleware2Assertions();
      }

      [TestMethod]
      public void Middleware_DisposeIntercepted()
      {
         VMController.Register<MiddlewareTestVM>();
         var hub = new MockDotNetifyHub()
            .UseMiddleware<ExtractHeadersMiddleware>()
            .UseMiddleware<JwtBearerAuthenticationMiddleware>()
            .UseMiddleware<CustomMiddleware1>()
            .UseMiddleware<CustomMiddleware2>()
            .Create();

         Action middleware1Assertions = null;
         _middleware1.Invoked += (sender, context) =>
         {
            if (context.CallType != "Dispose_VM")
               return;

            var connectionId = context.CallerContext.ConnectionId;
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            middleware1Assertions = () =>
            {
               Assert.AreEqual(hub.ConnectionId, connectionId);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
            };
         };

         Action middleware2Assertions = null;
         _middleware2.Invoked += (sender, context) =>
         {
            if (context.CallType != "Dispose_VM")
               return;

            var connectionId = context.CallerContext.ConnectionId;
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            middleware2Assertions = () =>
            {
               Assert.AreEqual(hub.ConnectionId, connectionId);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(1, pipelineData.Count);
            };
         };

         hub.RequestVM(nameof(MiddlewareTestVM), _vmArg);
         hub.DisposeVM(nameof(MiddlewareTestVM));

         middleware1Assertions();
         middleware2Assertions();
      }
   }
}
