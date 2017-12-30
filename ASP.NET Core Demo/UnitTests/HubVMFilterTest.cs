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

      private abstract class FilterBase<T> : IVMFilter<T> where T : Attribute
      {
         public event EventHandler<Tuple<T, VMContext>> Invoked;

         public Task Invoke(T attribute, VMContext context, NextFilterDelegate next)
         {
            Invoked?.Invoke(this, Tuple.Create(attribute, context));
            return next.Invoke(context);
         }
      }

      private class CustomFilter1 : FilterBase<CustomFilter1Attribute> { }
      private class CustomFilter2 : FilterBase<CustomFilter2Attribute> { }

      private FilterTestVM _vm;
      private CustomFilter1 _filter1;
      private CustomFilter2 _filter2;
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
            { typeof(FilterTestVM), (_vm = new FilterTestVM()) },
            { typeof(CustomFilter1), (_filter1 = new CustomFilter1()) },
            { typeof(CustomFilter2), (_filter2 = new CustomFilter2()) },
            { typeof(ExtractHeadersMiddleware), new ExtractHeadersMiddleware(_headersCache) },
            { typeof(JwtBearerAuthenticationMiddleware), new JwtBearerAuthenticationMiddleware(tokenValidationParameters) },
            { typeof(AuthorizeFilter), new AuthorizeFilter() }
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
      public void Filter_RequestIntercepted()
      {
         VMController.Register<FilterTestVM>();
         var hub = new MockDotNetifyHub()
            .UseMiddleware<ExtractHeadersMiddleware>()
            .UseMiddleware<JwtBearerAuthenticationMiddleware>()
            .UseFilter<AuthorizeFilter>()
            .UseFilter<CustomFilter1>()
            .UseFilter<CustomFilter2>()
            .Create();

         string vmName = null;
         dynamic vmData = null;
         hub.Response += (sender, e) =>
         {
            vmName = e.Item1;
            vmData = JsonConvert.DeserializeObject<dynamic>(e.Item2);
         };

         Action filter1Assertions = null;
         _filter1.Invoked += (sender, tuple) =>
         {
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
               Assert.AreEqual(hub.ConnectionId, connectionId);
               Assert.AreEqual("World", testVMPropValue);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
               Assert.AreEqual(_vm, vmContext.Instance);
               Assert.AreEqual("Welcome", attr.Property);
            };
         };

         Action filter2Assertions = null;
         _filter2.Invoked += (sender, tuple) =>
         {
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
               Assert.AreEqual(hub.ConnectionId, connectionId);
               Assert.AreEqual("World", testVMPropValue);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
               Assert.AreEqual(_vm, vmContext.Instance);
               Assert.AreEqual("Bienvenu", attr.Property);
            };
         };

         hub.RequestVM(nameof(FilterTestVM), _vmArg);

         Assert.AreEqual(nameof(FilterTestVM), vmName);
         Assert.AreEqual("World", vmData.Property.ToString());
         filter1Assertions();
         filter2Assertions();
      }

      [TestMethod]
      public void Filter_UpdateIntercepted()
      {
         VMController.Register<FilterTestVM>();
         var hub = new MockDotNetifyHub()
            .UseMiddleware<ExtractHeadersMiddleware>()
            .UseMiddleware<JwtBearerAuthenticationMiddleware>()
            .UseFilter<AuthorizeFilter>()
            .UseFilter<CustomFilter1>()
            .UseFilter<CustomFilter2>()
            .Create();

         Action filter1Assertions = null;
         _filter1.Invoked += (sender, tuple) =>
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
               Assert.AreEqual(hub.ConnectionId, connectionId);
               Assert.AreEqual("Goodbye", testVMPropValue);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
               Assert.AreEqual(_vm, vmContext.Instance);
               Assert.AreEqual("Welcome", attr.Property);
            };
         };

         Action filter2Assertions = null;
         _filter2.Invoked += (sender, tuple) =>
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
               Assert.AreEqual(hub.ConnectionId, connectionId);
               Assert.AreEqual("Goodbye", testVMPropValue);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
               Assert.AreEqual(_vm, vmContext.Instance);
               Assert.AreEqual("Bienvenu", attr.Property);
            };
         };

         hub.RequestVM(nameof(FilterTestVM), _vmArg);
         hub.UpdateVM(nameof(FilterTestVM), new Dictionary<string, object> { { "Property", "Goodbye" } });

         filter1Assertions();
         filter2Assertions();
      }

      [TestMethod]
      public void Filter_ResponseIntercepted()
      {
         VMController.Register<FilterTestVM>();
         var hub = new MockDotNetifyHub()
            .UseMiddleware<ExtractHeadersMiddleware>()
            .UseMiddleware<JwtBearerAuthenticationMiddleware>()
            .UseFilter<AuthorizeFilter>()
            .UseFilter<CustomFilter1>()
            .UseFilter<CustomFilter2>()
            .Create();

         Action filter1Assertions = null;
         _filter1.Invoked += (sender, tuple) =>
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
               Assert.AreEqual(hub.ConnectionId, connectionId);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
               Assert.AreEqual(_vm, vmContext.Instance);
               Assert.AreEqual("Welcome", attr.Property);
            };
         };

         Action filter2Assertions = null;
         _filter2.Invoked += (sender, tuple) =>
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
               Assert.AreEqual(hub.ConnectionId, connectionId);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
               Assert.AreEqual(_vm, vmContext.Instance);
               Assert.AreEqual("Bienvenu", attr.Property);
            };
         };

         hub.RequestVM(nameof(FilterTestVM), _vmArg);
         hub.UpdateVM(nameof(FilterTestVM), new Dictionary<string, object> { { "TriggerProperty", true } });

         filter1Assertions();
         filter2Assertions();
      }



      [TestMethod]
      public void Filter_PushUpdateIntercepted()
      {
         VMController.Register<FilterTestVM>();
         var hub = new MockDotNetifyHub()
            .UseMiddleware<ExtractHeadersMiddleware>()
            .UseMiddleware<JwtBearerAuthenticationMiddleware>()
            .UseFilter<AuthorizeFilter>()
            .UseFilter<CustomFilter1>()
            .UseFilter<CustomFilter2>()
            .Create();

         Action filter1Assertions = null;
         _filter1.Invoked += (sender, tuple) =>
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
               Assert.AreEqual(hub.ConnectionId, connectionId);
               Assert.IsTrue(authToken.StartsWith("Bearer "));
               Assert.AreEqual("admin", principalName);
               Assert.AreEqual(0, pipelineData.Count);
               Assert.AreEqual(_vm, vmContext.Instance);
               Assert.AreEqual("Welcome", attr.Property);
            };
         };

         hub.RequestVM(nameof(FilterTestVM), _vmArg);
         _vm.TriggerProperty = true;
         _vm.PushUpdates();

         filter1Assertions();
      }
   }
}
