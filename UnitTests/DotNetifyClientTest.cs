using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetify;
using DotNetify.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reactive.Linq;
using NSubstitute;

namespace UnitTests
{
   [TestClass]
   public class DotNetifyClientTest
   {
      private class HelloWorldVM : BaseVM
      {
         public class ListItem
         {
            public int Id { get; set; }
            public string Name { get; set; }
         }

         public string FirstName { get; set; }

         public string LastName { get; set; }

         public List<ListItem> List { get; set; } = new List<ListItem>();

         public HelloWorldVM()
         { }
      }

      [TestMethod]
      public async Task DotNetifyClient_ConnectAsyncWithoutOptions()
      {
         var mockHubProxy = Substitute.For<IDotNetifyHubProxy>();
         var sut = new DotNetifyClient(mockHubProxy, new DefaultUIThreadDispatcher());

         var helloWorldVM = new HelloWorldVM();
         await sut.ConnectAsync(nameof(HelloWorldVM), helloWorldVM);

         await mockHubProxy.Received().StartAsync();
         await mockHubProxy.Received().Request_VM(nameof(HelloWorldVM), null);
      }

      [TestMethod]
      public async Task DotNetifyClient_ConnectAsyncWithOptions()
      {
         var mockHubProxy = Substitute.For<IDotNetifyHubProxy>();
         var sut = new DotNetifyClient(mockHubProxy, new DefaultUIThreadDispatcher());
         var options = new VMConnectOptions
         {
            VMArg = new Dictionary<string, object> { { "FirstName", "Hello" } },
            Headers = new { OrganizationId = "MyOrgId" }
         };

         var helloWorldVM = new HelloWorldVM();
         await sut.ConnectAsync(nameof(HelloWorldVM), helloWorldVM, options);

         await mockHubProxy.Received().StartAsync();
         await mockHubProxy.Received().Request_VM(nameof(HelloWorldVM), Arg.Is<Dictionary<string, object>>(x =>
                  x.ContainsKey("$vmArg") &&
                  x["$vmArg"] == options.VMArg &&
                  x.ContainsKey("$headers") &&
                  x["$headers"] == options.Headers
                  ));
      }

      [TestMethod]
      public async Task DotNetifyClient_DispatchAsync()
      {
         var mockHubProxy = Substitute.For<IDotNetifyHubProxy>();
         var sut = new DotNetifyClient(mockHubProxy, new DefaultUIThreadDispatcher());

         var helloWorldVM = new HelloWorldVM();
         await sut.ConnectAsync(nameof(HelloWorldVM), helloWorldVM);
         await sut.DispatchAsync("FirstName", "Hello");

         await mockHubProxy.Received().Update_VM(nameof(HelloWorldVM),
            Arg.Is<Dictionary<string, object>>(x => x["FirstName"].ToString() == "Hello"));
      }

      [TestMethod]
      public async Task DotNetifyClient_DispatchAsyncMultiProps()
      {
         var mockHubProxy = Substitute.For<IDotNetifyHubProxy>();
         var sut = new DotNetifyClient(mockHubProxy, new DefaultUIThreadDispatcher());

         var expectedProp = new Dictionary<string, object>
         {
            { "FirstName", "Hello" }
         };
         var helloWorldVM = new HelloWorldVM();
         await sut.ConnectAsync(nameof(HelloWorldVM), helloWorldVM);
         await sut.DispatchAsync(expectedProp);

         await mockHubProxy.Received().Update_VM(nameof(HelloWorldVM), expectedProp);
      }

      [TestMethod]
      public async Task DotNetifyClient_Dispose()
      {
         var mockHubProxy = Substitute.For<IDotNetifyHubProxy>();
         mockHubProxy.ConnectionState.Returns(HubConnectionState.Connected);

         var sut = new DotNetifyClient(mockHubProxy, new DefaultUIThreadDispatcher());

         var helloWorldVM = new HelloWorldVM();
         await sut.ConnectAsync(nameof(HelloWorldVM), helloWorldVM);
         sut.Dispose();

         await mockHubProxy.Received().Dispose_VM(nameof(HelloWorldVM));
      }

      [TestMethod]
      public async Task DotNetifyClient_DisposeAsync()
      {
         var mockHubProxy = Substitute.For<IDotNetifyHubProxy>();
         mockHubProxy.ConnectionState.Returns(HubConnectionState.Connected);

         var sut = new DotNetifyClient(mockHubProxy, new DefaultUIThreadDispatcher());

         var helloWorldVM = new HelloWorldVM();
         await sut.ConnectAsync(nameof(HelloWorldVM), helloWorldVM);
         await sut.DisposeAsync();

         await mockHubProxy.Received().Dispose_VM(nameof(HelloWorldVM));
      }

      [TestMethod]
      public async Task DotNetifyClient_Response()
      {
         var mockHubProxy = Substitute.For<IDotNetifyHubProxy>();
         var sut = new DotNetifyClient(mockHubProxy, new DefaultUIThreadDispatcher());

         var helloWorldVM = new HelloWorldVM();
         await sut.ConnectAsync(nameof(HelloWorldVM), helloWorldVM);

         var eventArgs = new ResponseVMEventArgs()
         {
            VMId = nameof(HelloWorldVM),
            Data = new Dictionary<string, object>
            {
               { "FirstName", "Hello" },
               { "LastName", "World" }
            }
         };

         mockHubProxy.Response_VM += Raise.EventWith(new object(), eventArgs);

         Assert.AreEqual(helloWorldVM.FirstName, "Hello");
         Assert.AreEqual(helloWorldVM.LastName, "World");
         Assert.IsTrue(eventArgs.Handled);
      }

      [TestMethod]
      public async Task DotNetifyClient_ResponseCRUD()
      {
         var mockHubProxy = Substitute.For<IDotNetifyHubProxy>();
         var sut = new DotNetifyClient(mockHubProxy, new DefaultUIThreadDispatcher());

         var helloWorldVM = new HelloWorldVM();
         await sut.ConnectAsync(nameof(HelloWorldVM), helloWorldVM);

         var eventArgs = new ResponseVMEventArgs()
         {
            VMId = nameof(HelloWorldVM),
            Data = new Dictionary<string, object>
            {
               { "List_itemKey", "Id" }
            }
         };

         mockHubProxy.Response_VM += Raise.EventWith(new object(), eventArgs);

         eventArgs.Handled = false;
         eventArgs.Data = new Dictionary<string, object>
         {
            { "List_add", "{ Id: 1, Name: 'Item_A'}" }
         };

         mockHubProxy.Response_VM += Raise.EventWith(new object(), eventArgs);

         Assert.AreEqual(1, helloWorldVM.List.Count);
         Assert.AreEqual(1, helloWorldVM.List[0].Id);
         Assert.AreEqual("Item_A", helloWorldVM.List[0].Name);

         eventArgs.Handled = false;
         eventArgs.Data = new Dictionary<string, object>
         {
            { "List_update", "{ Id: 1, Name: 'Item_ABC'}" }
         };

         mockHubProxy.Response_VM += Raise.EventWith(new object(), eventArgs);

         Assert.AreEqual(1, helloWorldVM.List.Count);
         Assert.AreEqual(1, helloWorldVM.List[0].Id);
         Assert.AreEqual("Item_ABC", helloWorldVM.List[0].Name);

         eventArgs.Handled = false;
         eventArgs.Data = new Dictionary<string, object>
         {
            { "List_remove", "1" }
         };

         mockHubProxy.Response_VM += Raise.EventWith(new object(), eventArgs);

         Assert.AreEqual(0, helloWorldVM.List.Count);
      }
   }
}