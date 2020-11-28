using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetify;
using DotNetify.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Mvc = Microsoft.AspNetCore.Http;
using ms = Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;

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

         public string Metadata { get; set; }

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

      private IMemoryCache _memoryCache;
      private List<Tuple<Type, Func<IMiddlewarePipeline>>> _middlewareFactories = new List<Tuple<Type, Func<IMiddlewarePipeline>>>();
      private Dictionary<Type, Func<IVMFilter>> _vmFilterFactories = new Dictionary<Type, Func<IVMFilter>>();

      [TestInitialize]
      public void Initialize()
      {
         var options = Substitute.For<IOptions<ms.MemoryCacheOptions>>();
         options.Value.Returns(new ms.MemoryCacheOptions());
         _memoryCache = new MemoryCacheAdapter(new ms.MemoryCache(options));
      }

      [TestMethod]
      public async Task WebApiBasicVM_Request()
      {
         var vmId = nameof(BasicVM);
         VMController.Register<BasicVM>();

         var vmControllerFactory = new WebApiVMControllerFactory(new VMFactory(_memoryCache, new VMTypesAccessor()), new ServiceScopeFactory());
         var serviceProvider = new HubServiceProvider();
         var principalAccessor = new HubPrincipalAccessor();
         var hubPipeline = new HubPipeline(_middlewareFactories, _vmFilterFactories);
         var webApi = new DotNetifyWebApi();

         var mockHttpContext = Substitute.For<Mvc.HttpContext>();
         mockHttpContext.Connection.Id = Guid.NewGuid().ToString();
         webApi.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = mockHttpContext };

         var result = await webApi.Request_VM(vmId, null, vmControllerFactory, serviceProvider, principalAccessor, hubPipeline);

         await webApi.Dispose_VM(vmId, vmControllerFactory, serviceProvider, principalAccessor, hubPipeline);

         dynamic response = JsonConvert.DeserializeObject(result);
         Assert.AreEqual("Hello", (string) response.FirstName);
         Assert.AreEqual("World", (string) response.LastName);
         Assert.AreEqual("Hello World", (string) response.FullName);
      }

      [TestMethod]
      public async Task WebApiBasicVM_Update()
      {
         var vmId = nameof(BasicVM);
         VMController.Register<BasicVM>();

         var vmControllerFactory = new WebApiVMControllerFactory(new VMFactory(_memoryCache, new VMTypesAccessor()), new ServiceScopeFactory());
         var serviceProvider = new HubServiceProvider();
         var principalAccessor = new HubPrincipalAccessor();
         var hubPipeline = new HubPipeline(_middlewareFactories, _vmFilterFactories);
         var webApi = new DotNetifyWebApi();

         var mockHttpContext = Substitute.For<Mvc.HttpContext>();
         mockHttpContext.Connection.Id = Guid.NewGuid().ToString();
         webApi.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = mockHttpContext };

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var result = await webApi.Update_VM(vmId, update, vmControllerFactory, serviceProvider, principalAccessor, hubPipeline);
         dynamic response1 = JsonConvert.DeserializeObject(result);

         webApi = new DotNetifyWebApi();
         webApi.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = mockHttpContext };

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         result = await webApi.Update_VM(vmId, update, vmControllerFactory, serviceProvider, principalAccessor, hubPipeline);
         dynamic response2 = JsonConvert.DeserializeObject(result);

         await webApi.Dispose_VM(vmId, vmControllerFactory, serviceProvider, principalAccessor, hubPipeline);

         Assert.AreEqual("John World", (string) response1.FullName);
         Assert.AreEqual("John Doe", (string) response2.FullName);
      }

      [TestMethod]
      public async Task WebApiBasicVM_UpdateNoResponse()
      {
         var vmId = nameof(BasicVM);
         VMController.Register<BasicVM>();

         var vmControllerFactory = new WebApiVMControllerFactory(new VMFactory(_memoryCache, new VMTypesAccessor()), new ServiceScopeFactory());
         var serviceProvider = new HubServiceProvider();
         var principalAccessor = new HubPrincipalAccessor();
         var hubPipeline = new HubPipeline(_middlewareFactories, _vmFilterFactories);
         var webApi = new DotNetifyWebApi();

         var mockHttpContext = Substitute.For<Mvc.HttpContext>();
         mockHttpContext.Connection.Id = Guid.NewGuid().ToString();
         webApi.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = mockHttpContext };

         var update = new Dictionary<string, object>() { { "Metadata", "Test" } };
         var result = await webApi.Update_VM(vmId, update, vmControllerFactory, serviceProvider, principalAccessor, hubPipeline);

         await webApi.Dispose_VM(vmId, vmControllerFactory, serviceProvider, principalAccessor, hubPipeline);

         Assert.IsNotNull(result);
      }

      [TestMethod]
      public async Task WebApiBasicVM_RequestMiddleware()
      {
         var vmId = nameof(BasicVM);
         VMController.Register<BasicVM>();

         _middlewareFactories.Add(Tuple.Create<Type, Func<IMiddlewarePipeline>>(typeof(CustomMiddleware), () => new CustomMiddleware()));

         var vmControllerFactory = new WebApiVMControllerFactory(new VMFactory(_memoryCache, new VMTypesAccessor()), new ServiceScopeFactory());
         var serviceProvider = new HubServiceProvider();
         var principalAccessor = new HubPrincipalAccessor();
         var hubPipeline = new HubPipeline(_middlewareFactories, _vmFilterFactories);
         var webApi = new DotNetifyWebApi();

         var mockHttpContext = Substitute.For<Mvc.HttpContext>();
         mockHttpContext.Connection.Id = Guid.NewGuid().ToString();
         webApi.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = mockHttpContext };

         var result = await webApi.Request_VM(vmId, null, vmControllerFactory, serviceProvider, principalAccessor, hubPipeline);

         await webApi.Dispose_VM(vmId, vmControllerFactory, serviceProvider, principalAccessor, hubPipeline);

         dynamic response = JsonConvert.DeserializeObject(result);
         Assert.AreEqual("John", (string) response.FirstName);
         Assert.AreEqual("World", (string) response.LastName);
         Assert.AreEqual("John World", (string) response.FullName);
      }

      [TestMethod]
      public async Task WebApiBasicVM_UpdateMiddleware()
      {
         var vmId = nameof(BasicVM);
         VMController.Register<BasicVM>();

         _middlewareFactories.Add(Tuple.Create<Type, Func<IMiddlewarePipeline>>(typeof(CustomMiddleware), () => new CustomMiddleware()));

         var vmControllerFactory = new WebApiVMControllerFactory(new VMFactory(_memoryCache, new VMTypesAccessor()), new ServiceScopeFactory());
         var serviceProvider = new HubServiceProvider();
         var principalAccessor = new HubPrincipalAccessor();
         var hubPipeline = new HubPipeline(_middlewareFactories, _vmFilterFactories);
         var webApi = new DotNetifyWebApi();

         var mockHttpContext = Substitute.For<Mvc.HttpContext>();
         mockHttpContext.Connection.Id = Guid.NewGuid().ToString();
         webApi.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = mockHttpContext };

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var result = await webApi.Update_VM(vmId, update, vmControllerFactory, serviceProvider, principalAccessor, hubPipeline);
         dynamic response = JsonConvert.DeserializeObject(result);

         await webApi.Dispose_VM(vmId, vmControllerFactory, serviceProvider, principalAccessor, hubPipeline);

         Assert.AreEqual("JOHN World", (string) response.FullName);
      }
   }
}