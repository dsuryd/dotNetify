using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetify;
using DotNetify.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Mvc = Microsoft.AspNetCore.Http;

namespace UnitTests
{
   [TestClass]
   public class WebApiBasicVMTest
   {
      private class BasicVM : BaseVM
      {
         public string FirstName
         {
            get => Get<string>() ?? "Hello";
            set
            {
               Set(value);
               Changed(nameof(FullName));
            }
         }

         public string LastName
         {
            get => Get<string>() ?? "World";
            set
            {
               Set(value);
               Changed(nameof(FullName));
            }
         }

         public string FullName => $"{FirstName} {LastName}";
      }

      private class ServiceScopeFactory : IVMServiceScopeFactory
      {
         public IVMServiceScope CreateScope() => null;
      }

      private class CustomMiddleware : IMiddleware
      {
         public Task Invoke(DotNetifyHubContext context, NextDelegate next)
         {
            if (context.CallType == nameof(DotNetifyHub.Request_VM))
            {
               var data = new { FirstName = "John" };
               context.Data = JObject.Parse(JsonConvert.SerializeObject(data));
            }
            else if (context.CallType == nameof(DotNetifyHub.Update_VM))
            {
               var data = context.Data as Dictionary<string, object>;
               data["FirstName"] = data["FirstName"].ToString().ToUpper();
               context.Data = data;
            }
            return next(context);
         }
      }

      [TestMethod]
      public async Task WebApiBasicVM_Request()
      {
         VMController.Register<BasicVM>();

         var webApi = new DotNetifyWebApi();
         var vmFactory = new VMFactory(new MockDotNetifyHub.MemoryCache(), new VMTypesAccessor());
         var hubPipeline = new MockDotNetifyHub().CreateHubPipeline();
         var serviceScopeFactory = new ServiceScopeFactory();

         var mockHttpContext = Substitute.For<Mvc.HttpContext>();
         webApi.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = mockHttpContext };

         var result = await webApi.Request_VM("BasicVM", null, vmFactory, null, serviceScopeFactory, hubPipeline, new HubPrincipalAccessor());

         dynamic response = JsonConvert.DeserializeObject(result);
         Assert.AreEqual("Hello", (string) response.FirstName);
         Assert.AreEqual("World", (string) response.LastName);
         Assert.AreEqual("Hello World", (string) response.FullName);
      }

      [TestMethod]
      public async Task WebApiBasicVM_Update()
      {
         VMController.Register<BasicVM>();

         var webApi = new DotNetifyWebApi();
         var vmFactory = new VMFactory(new MockDotNetifyHub.MemoryCache(), new VMTypesAccessor());
         var hubPipeline = new MockDotNetifyHub().CreateHubPipeline();
         var serviceScopeFactory = new ServiceScopeFactory();

         var mockHttpContext = Substitute.For<Mvc.HttpContext>();
         webApi.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = mockHttpContext };

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var result = await webApi.Update_VM("BasicVM", null, update, vmFactory, null, serviceScopeFactory, hubPipeline, new HubPrincipalAccessor());
         dynamic response1 = JsonConvert.DeserializeObject(result);

         update = new Dictionary<string, object>() { { "FirstName", "John" }, { "LastName", "Doe" } };
         result = await webApi.Update_VM("BasicVM", null, update, vmFactory, null, serviceScopeFactory, hubPipeline, new HubPrincipalAccessor());
         dynamic response2 = JsonConvert.DeserializeObject(result);

         Assert.AreEqual("John World", (string) response1.FullName);
         Assert.AreEqual("John Doe", (string) response2.FullName);
      }

      [TestMethod]
      public async Task WebApiBasicVM_RequestMiddleware()
      {
         VMController.Register<BasicVM>();

         var webApi = new DotNetifyWebApi();
         var vmFactory = new VMFactory(new MockDotNetifyHub.MemoryCache(), new VMTypesAccessor());
         var serviceScopeFactory = new ServiceScopeFactory();
         var hubPipeline = new MockDotNetifyHub()
            .UseMiddleware<CustomMiddleware>()
            .CreateHubPipeline();

         var mockHttpContext = Substitute.For<Mvc.HttpContext>();
         webApi.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = mockHttpContext };

         var result = await webApi.Request_VM("BasicVM", null, vmFactory, null, serviceScopeFactory, hubPipeline, new HubPrincipalAccessor());

         dynamic response = JsonConvert.DeserializeObject(result);
         Assert.AreEqual("John", (string) response.FirstName);
         Assert.AreEqual("World", (string) response.LastName);
         Assert.AreEqual("John World", (string) response.FullName);
      }

      [TestMethod]
      public async Task WebApiBasicVM_UpdateMiddleware()
      {
         VMController.Register<BasicVM>();

         var webApi = new DotNetifyWebApi();
         var vmFactory = new VMFactory(new MockDotNetifyHub.MemoryCache(), new VMTypesAccessor());
         var serviceScopeFactory = new ServiceScopeFactory();
         var hubPipeline = new MockDotNetifyHub()
            .UseMiddleware<CustomMiddleware>()
            .CreateHubPipeline();

         var mockHttpContext = Substitute.For<Mvc.HttpContext>();
         webApi.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = mockHttpContext };

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var result = await webApi.Update_VM("BasicVM", null, update, vmFactory, null, serviceScopeFactory, hubPipeline, new HubPrincipalAccessor());
         dynamic response = JsonConvert.DeserializeObject(result);

         Assert.AreEqual("JOHN World", (string) response.FullName);
      }
   }
}