using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetify;
using DotNetify.Security;
using DotNetify.Testing;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnitTests
{
   [TestClass]
   public class HubMiddlewareTest
   {
      private class MiddlewareTestVM : BaseVM
      {
         public string Property { get; set; } = "Hello";

         public bool TriggerProperty
         {
            get { return false; }
            set { Changed(nameof(ResponseProperty)); }
         }

         public string ResponseProperty => "Triggered";
      }

      private class CustomMiddleware1 : IMiddleware
      {
         public static event EventHandler<DotNetifyHubContext> Invoked;

         public static void Cleanup()
         {
            if (Invoked != null)
               foreach (Delegate d in Invoked.GetInvocationList())
                  Invoked -= (EventHandler<DotNetifyHubContext>) d;
         }

         public virtual Task Invoke(DotNetifyHubContext context, NextDelegate next)
         {
            Invoked?.Invoke(this, context);
            context.PipelineData.Add(GetType().Name, null);
            return next(context);
         }
      }

      private class CustomMiddleware2 : IMiddleware
      {
         public static event EventHandler<DotNetifyHubContext> Invoked;

         public static void Cleanup()
         {
            if (Invoked != null)
               foreach (Delegate d in Invoked.GetInvocationList())
                  Invoked -= (EventHandler<DotNetifyHubContext>) d;
         }

         public virtual Task Invoke(DotNetifyHubContext context, NextDelegate next)
         {
            Invoked?.Invoke(this, context);
            context.PipelineData.Add(GetType().Name, null);
            return next(context);
         }
      }

      private class CustomMiddleware3 : IMiddleware
      {
         public Task Invoke(DotNetifyHubContext context, NextDelegate next)
         {
            if (context.CallType == "Response_VM")
            {
               var data = JsonConvert.DeserializeObject<IDictionary<string, object>>(context.Data.ToString());
               foreach (var key in data.Keys.ToList())
                  data[key] = data[key].ToString().ToUpper();
               context.Data = JsonConvert.SerializeObject(data);
            }
            return next(context);
         }
      }

      private class RequestStoppedMiddleware : IMiddleware
      {
         public Task Invoke(DotNetifyHubContext context, NextDelegate next)
         {
            return Task.CompletedTask;
         }
      }

      private TokenValidationParameters _tokenValidationParameters;
      private VMConnectOptions _vmConnectOptions;

      [TestInitialize]
      public void Initialize()
      {
         string secretKey = "dotnetifydemo_secretkey_123!";
         _tokenValidationParameters = new TokenValidationParameters
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

         _vmConnectOptions = new VMConnectOptions();
         _vmConnectOptions.VMArg.Set("Property", "World");
         _vmConnectOptions.Headers.Set("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsImp0aSI6IjI0YTUwOGJlLWJlMTktNDFhZS1iZmI1LTc5OGU4YmNjNDI3ZCIsImlhdCI6MTUxNDUyODgxNiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6WyJhZG1pbiIsImFkbWluIl0sImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6ImFkbWluIiwibmJmIjoxNTE0NTI4ODE2LCJleHAiOjE4Mjk4ODg4MTYsImlzcyI6IkRvdE5ldGlmeURlbW9TZXJ2ZXIiLCJhdWQiOiJEb3ROZXRpZnlEZW1vQXBwIn0.q2wZyS13FskQ094O9xbz4FLlRPPHf1GfKOUOTHJyLbk");
      }

      [TestCleanup]
      public void Cleanup()
      {
         CustomMiddleware1.Cleanup();
         CustomMiddleware2.Cleanup();
      }

      [TestMethod]
      public void Middleware_RequestIntercepted()
      {
         var hubEmulator = new HubEmulatorBuilder()
            .Register<MiddlewareTestVM>()
            .UseMiddleware<JwtBearerAuthenticationMiddleware>(_tokenValidationParameters)
            .UseMiddleware<CustomMiddleware1>()
            .UseMiddleware<CustomMiddleware2>()
            .Build();

         var client = hubEmulator.CreateClient();

         Action middleware1Assertions = null;
         CustomMiddleware1.Invoked += (sender, context) =>
         {
            if (middleware1Assertions != null)
               return;

            var vmId = context.VMId;
            var callType = context.CallType;
            var connectionId = context.CallerContext.ConnectionId;
            var testVMPropValue = (context.Data as JObject)[nameof(MiddlewareTestVM.Property)];
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            middleware1Assertions = () =>
            {
               Assert.AreEqual(nameof(MiddlewareTestVM), vmId);
               Assert.AreEqual("Request_VM", callType);
               Assert.AreEqual(client.ConnectionId, connectionId);
               Assert.AreEqual("World", testVMPropValue);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
            };
         };

         Action middleware2Assertions = null;
         CustomMiddleware2.Invoked += (sender, context) =>
         {
            if (middleware2Assertions != null)
               return;

            var vmId = context.VMId;
            var callType = context.CallType;
            var connectionId = context.CallerContext.ConnectionId;
            var testVMPropValue = (context.Data as JObject)[nameof(MiddlewareTestVM.Property)];
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            middleware2Assertions = () =>
            {
               Assert.AreEqual(nameof(MiddlewareTestVM), vmId);
               Assert.AreEqual("Request_VM", callType);
               Assert.AreEqual(client.ConnectionId, connectionId);
               Assert.AreEqual("World", testVMPropValue);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(1, pipelineData.Count);
            };
         };

         var request = client.Connect(nameof(MiddlewareTestVM), _vmConnectOptions).As<dynamic>();

         Assert.AreEqual("World", (string) request.Property);
         middleware1Assertions();
         middleware2Assertions();
      }

      [TestMethod]
      public void Middleware_UpdateIntercepted()
      {
         var hubEmulator = new HubEmulatorBuilder()
            .Register<MiddlewareTestVM>()
            .UseMiddleware<JwtBearerAuthenticationMiddleware>(_tokenValidationParameters)
            .UseMiddleware<CustomMiddleware1>()
            .UseMiddleware<CustomMiddleware2>()
            .Build();

         var client = hubEmulator.CreateClient();

         Action middleware1Assertions = null;
         CustomMiddleware1.Invoked += (sender, context) =>
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
               Assert.AreEqual(client.ConnectionId, connectionId);
               Assert.AreEqual("Goodbye", testVMPropValue);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
            };
         };

         Action middleware2Assertions = null;
         CustomMiddleware2.Invoked += (sender, context) =>
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
               Assert.AreEqual(client.ConnectionId, connectionId);
               Assert.AreEqual("Goodbye", testVMPropValue);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(1, pipelineData.Count);
            };
         };

         client.Connect(nameof(MiddlewareTestVM), _vmConnectOptions);
         client.Dispatch(new Dictionary<string, object> { { "Property", "Goodbye" } });

         middleware1Assertions();
         middleware2Assertions();
      }

      [TestMethod]
      public void Middleware_DisposeIntercepted()
      {
         var hubEmulator = new HubEmulatorBuilder()
            .Register<MiddlewareTestVM>()
            .UseMiddleware<JwtBearerAuthenticationMiddleware>(_tokenValidationParameters)
            .UseMiddleware<CustomMiddleware1>()
            .UseMiddleware<CustomMiddleware2>()
            .Build();

         var client = hubEmulator.CreateClient();

         Action middleware1Assertions = null;
         CustomMiddleware1.Invoked += (sender, context) =>
         {
            if (context.CallType != "Dispose_VM")
               return;

            var connectionId = context.CallerContext.ConnectionId;
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            middleware1Assertions = () =>
            {
               Assert.AreEqual(client.ConnectionId, connectionId);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
            };
         };

         Action middleware2Assertions = null;
         CustomMiddleware2.Invoked += (sender, context) =>
         {
            if (context.CallType != "Dispose_VM")
               return;

            var connectionId = context.CallerContext.ConnectionId;
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            middleware2Assertions = () =>
            {
               Assert.AreEqual(client.ConnectionId, connectionId);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(1, pipelineData.Count);
            };
         };

         client.Connect(nameof(MiddlewareTestVM), _vmConnectOptions);
         client.Destroy();

         middleware1Assertions();
         middleware2Assertions();
      }

      [TestMethod]
      public void Middleware_ResponseIntercepted()
      {
         var hubEmulator = new HubEmulatorBuilder()
            .Register<MiddlewareTestVM>()
            .UseMiddleware<JwtBearerAuthenticationMiddleware>(_tokenValidationParameters)
            .UseMiddleware<CustomMiddleware3>()
            .Build();

         var client = hubEmulator.CreateClient();
         client.Connect(nameof(MiddlewareTestVM), _vmConnectOptions);

         var response = client.Dispatch(new Dictionary<string, object> { { "TriggerProperty", true } }).As<dynamic>();
         Assert.AreEqual("TRIGGERED", (string) response.ResponseProperty);
      }

      [TestMethod]
      public void Middleware_RequestStopped()
      {
         var hubEmulator = new HubEmulatorBuilder()
            .Register<MiddlewareTestVM>()
            .UseMiddleware<RequestStoppedMiddleware>()
            .Build();

         var client = hubEmulator.CreateClient();

         var request = client.Connect(nameof(MiddlewareTestVM), _vmConnectOptions);
         Assert.IsTrue(request.Count == 0);
      }
   }
}