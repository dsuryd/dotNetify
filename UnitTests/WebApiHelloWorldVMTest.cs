using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DotNetify;
using DotNetify.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnitTests
{
   [TestClass]
   public class WebApiHelloWorldVMTest
   {
      private class HelloWorldVM : BaseVM
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
      public async Task HelloWorldVM_Request()
      {
         VMController.Register<HelloWorldVM>();

         var webApi = new DotNetifyWebApi();
         var vmFactory = new VMFactory(new MockDotNetifyHub.MemoryCache(), new VMTypesAccessor());
         var hubPipeline = new MockDotNetifyHub().CreateHubPipeline();

         var result = await webApi.Request_VM("HelloWorldVM", null, vmFactory, hubPipeline);

         dynamic response = JsonConvert.DeserializeObject(result);
         Assert.AreEqual("Hello", (string) response.FirstName);
         Assert.AreEqual("World", (string) response.LastName);
         Assert.AreEqual("Hello World", (string) response.FullName);
      }

      [TestMethod]
      public async Task HelloWorldVM_Update()
      {
         VMController.Register<HelloWorldVM>();

         var webApi = new DotNetifyWebApi();
         var vmFactory = new VMFactory(new MockDotNetifyHub.MemoryCache(), new VMTypesAccessor());
         var hubPipeline = new MockDotNetifyHub().CreateHubPipeline();

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var result = await webApi.Update_VM("HelloWorldVM", null, update, vmFactory, hubPipeline);
         dynamic response1 = JsonConvert.DeserializeObject(result);

         update = new Dictionary<string, object>() { { "FirstName", "John" }, { "LastName", "Doe" } };
         result = await webApi.Update_VM("HelloWorldVM", null, update, vmFactory, hubPipeline);
         dynamic response2 = JsonConvert.DeserializeObject(result);

         Assert.AreEqual("John World", (string) response1.FullName);
         Assert.AreEqual("John Doe", (string) response2.FullName);
      }

      [TestMethod]
      public async Task HelloWorldVM_RequestMiddleware()
      {
         VMController.Register<HelloWorldVM>();

         var webApi = new DotNetifyWebApi();
         var vmFactory = new VMFactory(new MockDotNetifyHub.MemoryCache(), new VMTypesAccessor());
         var hubPipeline = new MockDotNetifyHub()
            .UseMiddleware<CustomMiddleware>()
            .CreateHubPipeline();

         var result = await webApi.Request_VM("HelloWorldVM", null, vmFactory, hubPipeline);

         dynamic response = JsonConvert.DeserializeObject(result);
         Assert.AreEqual("John", (string) response.FirstName);
         Assert.AreEqual("World", (string) response.LastName);
         Assert.AreEqual("John World", (string) response.FullName);
      }

      [TestMethod]
      public async Task HelloWorldVM_UpdateMiddleware()
      {
         VMController.Register<HelloWorldVM>();

         var webApi = new DotNetifyWebApi();
         var vmFactory = new VMFactory(new MockDotNetifyHub.MemoryCache(), new VMTypesAccessor());
         var hubPipeline = new MockDotNetifyHub()
            .UseMiddleware<CustomMiddleware>()
            .CreateHubPipeline();

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var result = await webApi.Update_VM("HelloWorldVM", null, update, vmFactory, hubPipeline);
         dynamic response = JsonConvert.DeserializeObject(result);

         Assert.AreEqual("JOHN World", (string) response.FullName);
      }
   }
}