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
   public class HubVMFilterTest
   {
      [Authorize]
      [CustomFilter1(Property = "Welcome")]
      [CustomFilter2(Property = "Bienvenu")]
      private class FilterTestVM : BaseVM
      {
         public string Property { get; set; } = "Hello";

         public bool TriggerProperty
         {
            get { return false; }
            set { Changed(nameof(ResponseProperty)); }
         }

         public string ResponseProperty => "Triggered";
      }

      private class CustomFilter1Attribute : Attribute
      {
         public string Property { get; set; }
      }

      private class CustomFilter2Attribute : Attribute
      {
         public string Property { get; set; }
      }

      private class CustomFilter1 : IVMFilter<CustomFilter1Attribute>
      {
         public static event EventHandler<Tuple<CustomFilter1Attribute, VMContext>> Invoked;

         public Task Invoke(CustomFilter1Attribute attribute, VMContext context, NextFilterDelegate next)
         {
            Invoked?.Invoke(this, Tuple.Create(attribute, context));
            return next.Invoke(context);
         }
      }

      private class CustomFilter2 : IVMFilter<CustomFilter2Attribute>
      {
         public static event EventHandler<Tuple<CustomFilter2Attribute, VMContext>> Invoked;

         public Task Invoke(CustomFilter2Attribute attribute, VMContext context, NextFilterDelegate next)
         {
            if (context.HubContext.CallType == "Response_VM")
            {
               var data = JsonConvert.DeserializeObject<IDictionary<string, object>>(context.HubContext.Data.ToString());
               foreach (var key in data.Keys.ToList())
                  data[key] = data[key].ToString().ToUpper();
               context.HubContext.Data = JsonConvert.SerializeObject(data);
            }

            Invoked?.Invoke(this, Tuple.Create(attribute, context));
            return next.Invoke(context);
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

      [TestMethod]
      public void Filter_RequestIntercepted()
      {
         var vm = new FilterTestVM();

         var hubEmulator = new HubEmulatorBuilder()
            .Register(nameof(FilterTestVM), vm)
            .UseMiddleware<JwtBearerAuthenticationMiddleware>(_tokenValidationParameters)
            .UseFilter<AuthorizeFilter>()
            .UseFilter<CustomFilter1>()
            .UseFilter<CustomFilter2>()
            .Build();

         var client = hubEmulator.CreateClient();

         Action filter1Assertions = null;
         CustomFilter1.Invoked += (sender, tuple) =>
         {
            if (filter1Assertions != null)
               return;

            var attr = tuple.Item1;
            var vmContext = tuple.Item2;
            var context = vmContext.HubContext;
            var callType = context.CallType;
            var connectionId = context.CallerContext.ConnectionId;
            var testVMPropValue = (context.Data as JObject)[nameof(FilterTestVM.Property)];
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            filter1Assertions = () =>
            {
               Assert.AreEqual("Request_VM", callType);
               Assert.AreEqual(client.ConnectionId, connectionId);
               Assert.AreEqual("World", testVMPropValue);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
               Assert.AreEqual(vm, vmContext.Instance);
               Assert.AreEqual("Welcome", attr.Property);
            };
         };

         Action filter2Assertions = null;
         CustomFilter2.Invoked += (sender, tuple) =>
         {
            if (filter2Assertions != null)
               return;

            var attr = tuple.Item1;
            var vmContext = tuple.Item2;
            var context = vmContext.HubContext;
            var callType = context.CallType;
            var connectionId = context.CallerContext.ConnectionId;
            var testVMPropValue = (context.Data as JObject)[nameof(FilterTestVM.Property)];
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            filter2Assertions = () =>
            {
               Assert.AreEqual("Request_VM", callType);
               Assert.AreEqual(client.ConnectionId, connectionId);
               Assert.AreEqual("World", testVMPropValue);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
               Assert.AreEqual(vm, vmContext.Instance);
               Assert.AreEqual("Bienvenu", attr.Property);
            };
         };

         var response = client.Connect(nameof(FilterTestVM), _vmConnectOptions).As<dynamic>();

         Assert.AreEqual("WORLD", (string) response.Property);
         filter1Assertions();
         filter2Assertions();
      }

      [TestMethod]
      public void Filter_UpdateIntercepted()
      {
         var vm = new FilterTestVM();

         var hubEmulator = new HubEmulatorBuilder()
            .Register(nameof(FilterTestVM), vm)
            .UseMiddleware<JwtBearerAuthenticationMiddleware>(_tokenValidationParameters)
            .UseFilter<AuthorizeFilter>()
            .UseFilter<CustomFilter1>()
            .UseFilter<CustomFilter2>()
            .Build();

         var client = hubEmulator.CreateClient();

         Action filter1Assertions = null;
         CustomFilter1.Invoked += (sender, tuple) =>
         {
            if (tuple.Item2.HubContext.CallType != "Update_VM")
               return;

            var attr = tuple.Item1;
            var vmContext = tuple.Item2;
            var context = tuple.Item2.HubContext;
            var connectionId = context.CallerContext.ConnectionId;
            var testVMPropValue = (context.Data as Dictionary<string, object>)[nameof(FilterTestVM.Property)];
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            filter1Assertions = () =>
            {
               Assert.AreEqual(client.ConnectionId, connectionId);
               Assert.AreEqual("Goodbye", testVMPropValue);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
               Assert.AreEqual(vm, vmContext.Instance);
               Assert.AreEqual("Welcome", attr.Property);
            };
         };

         Action filter2Assertions = null;
         CustomFilter2.Invoked += (sender, tuple) =>
         {
            if (tuple.Item2.HubContext.CallType != "Update_VM")
               return;

            var attr = tuple.Item1;
            var vmContext = tuple.Item2;
            var context = tuple.Item2.HubContext;
            var connectionId = context.CallerContext.ConnectionId;
            var testVMPropValue = (context.Data as Dictionary<string, object>)[nameof(FilterTestVM.Property)];
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            filter2Assertions = () =>
            {
               Assert.AreEqual(client.ConnectionId, connectionId);
               Assert.AreEqual("Goodbye", testVMPropValue);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
               Assert.AreEqual(vm, vmContext.Instance);
               Assert.AreEqual("Bienvenu", attr.Property);
            };
         };

         client.Connect(nameof(FilterTestVM), _vmConnectOptions).As<dynamic>();
         client.Dispatch(new Dictionary<string, object> { { "Property", "Goodbye" } });

         filter1Assertions();
         filter2Assertions();
      }

      [TestMethod]
      public void Filter_ResponseIntercepted()
      {
         var vm = new FilterTestVM();

         var hubEmulator = new HubEmulatorBuilder()
            .Register(nameof(FilterTestVM), vm)
            .UseMiddleware<JwtBearerAuthenticationMiddleware>(_tokenValidationParameters)
            .UseFilter<AuthorizeFilter>()
            .UseFilter<CustomFilter1>()
            .UseFilter<CustomFilter2>()
            .Build();

         var client = hubEmulator.CreateClient();

         Action filter1Assertions = null;
         CustomFilter1.Invoked += (sender, tuple) =>
         {
            if (tuple.Item2.HubContext.CallType != "Response_VM")
               return;

            var attr = tuple.Item1;
            var vmContext = tuple.Item2;
            var context = vmContext.HubContext;
            var connectionId = context.CallerContext.ConnectionId;
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            filter1Assertions = () =>
            {
               Assert.AreEqual(client.ConnectionId, connectionId);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
               Assert.AreEqual(vm, vmContext.Instance);
               Assert.AreEqual("Welcome", attr.Property);
            };
         };

         Action filter2Assertions = null;
         CustomFilter2.Invoked += (sender, tuple) =>
         {
            if (tuple.Item2.HubContext.CallType != "Response_VM")
               return;

            var attr = tuple.Item1;
            var vmContext = tuple.Item2;
            var context = vmContext.HubContext;
            var connectionId = context.CallerContext.ConnectionId;
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            filter2Assertions = () =>
            {
               Assert.AreEqual(client.ConnectionId, connectionId);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
               Assert.AreEqual(vm, vmContext.Instance);
               Assert.AreEqual("Bienvenu", attr.Property);
            };
         };

         client.Connect(nameof(FilterTestVM), _vmConnectOptions);
         var response = client.Dispatch(new Dictionary<string, object> { { "TriggerProperty", true } }).As<dynamic>();

         filter1Assertions();
         filter2Assertions();
         Assert.AreEqual("TRIGGERED", (string) response.ResponseProperty);
      }

      [TestMethod]
      public void Filter_PushUpdateIntercepted()
      {
         var vm = new FilterTestVM();

         var hubEmulator = new HubEmulatorBuilder()
            .Register(nameof(FilterTestVM), vm)
            .UseMiddleware<JwtBearerAuthenticationMiddleware>(_tokenValidationParameters)
            .UseFilter<AuthorizeFilter>()
            .UseFilter<CustomFilter1>()
            .UseFilter<CustomFilter2>()
            .Build();

         var client = hubEmulator.CreateClient();

         Action filter1Assertions = null;
         CustomFilter1.Invoked += (sender, tuple) =>
         {
            if (tuple.Item2.HubContext.CallType != "Response_VM")
               return;

            var attr = tuple.Item1;
            var vmContext = tuple.Item2;
            var context = vmContext.HubContext;
            var connectionId = context.CallerContext.ConnectionId;
            var authToken = (context.Headers as JObject)["Authorization"].ToString();
            var principalName = context.Principal.Identity.Name;
            var pipelineData = new Dictionary<string, object>(context.PipelineData);

            filter1Assertions = () =>
            {
               Assert.AreEqual(client.ConnectionId, connectionId);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
               Assert.AreEqual(vm, vmContext.Instance);
               Assert.AreEqual("Welcome", attr.Property);
            };
         };

         client.Connect(nameof(FilterTestVM), _vmConnectOptions);
         vm.TriggerProperty = true;
         vm.PushUpdates();

         filter1Assertions();
      }
   }
}