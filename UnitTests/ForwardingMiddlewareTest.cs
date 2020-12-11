using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using DotNetify;
using DotNetify.Client;
using DotNetify.DevApp;
using DotNetify.Forwarding;
using DotNetify.Testing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Microsoft.AspNetCore.SignalR;
using System;

namespace UnitTests
{
   [TestClass]
   public class ForwardingMiddlewareTest
   {
      private struct HelloWorldState
      {
         public string FirstName { get; set; }
         public string LastName { get; set; }
         public string FullName { get; set; }
      }

      static ForwardingMiddlewareTest()
      {
         VMController.Register<HelloWorldVM>();
      }

      [TestMethod]
      public void ForwardingMiddleware_ConnectForwarded()
      {
         var hubForwarderFactory = Substitute.For<IDotNetifyHubForwarderFactory>();

         var hubEmulator = new HubEmulatorBuilder()
                  .Register<HelloWorldVM>()
                  .UseMiddleware<ForwardingMiddleware>(hubForwarderFactory, "serverUrl", true)
                  .Build();

         var client = hubEmulator.CreateClient();
         Setup(hubForwarderFactory, client as IClientProxy);

         var response = client.Connect(nameof(HelloWorldVM)).As<HelloWorldState>();

         Assert.AreEqual("Hello", response.FirstName);
         Assert.AreEqual("World", response.LastName);
         Assert.AreEqual("Hello World", response.FullName);
      }

      [TestMethod]
      public void ForwardingMiddleware_DispatchForwarded()
      {
         var hubForwarderFactory = Substitute.For<IDotNetifyHubForwarderFactory>();

         var hubEmulator = new HubEmulatorBuilder()
                  .Register<HelloWorldVM>()
                  .UseMiddleware<ForwardingMiddleware>(hubForwarderFactory, "serverUrl", true)
                  .Build();

         var client = hubEmulator.CreateClient();
         Setup(hubForwarderFactory, client as IClientProxy);

         client.Connect(nameof(HelloWorldVM));

         var response = client.Dispatch(new { FirstName = "John" }).As<dynamic>();
         Assert.AreEqual("John World", (string) response.FullName);

         client.Dispatch(new { LastName = "Doe" });
         Assert.AreEqual("John Doe", client.GetState<HelloWorldState>().FullName);
      }

      [TestMethod]
      public void ForwardingMiddleware_DestroyForwarded()
      {
         bool dispose = false;
         var hubForwarderFactory = Substitute.For<IDotNetifyHubForwarderFactory>();

         var hubEmulator = new HubEmulatorBuilder()
                  .Register<HelloWorldVM>()
                  .UseMiddleware<ForwardingMiddleware>(hubForwarderFactory, "serverUrl", true)
                  .Build();

         var client = hubEmulator.CreateClient();
         Setup(hubForwarderFactory, client as IClientProxy, (callType, vmId) =>
         {
            dispose = callType == nameof(IDotNetifyHubMethod.Dispose_VM) && vmId == nameof(HelloWorldVM);
         });

         client.Connect(nameof(HelloWorldVM));
         client.Destroy();

         Assert.IsTrue(dispose);
      }

      #region Test Setup

      private VMController MockVMController(VMController.VMResponseDelegate response)
      {
         var options = Substitute.For<IOptions<MemoryCacheOptions>>();
         options.Value.Returns(new MemoryCacheOptions());

         var memoryCache = new MemoryCacheAdapter(new MemoryCache(options));
         var vmFactory = new VMFactory(memoryCache, new VMTypesAccessor());
         return new VMController(response, vmFactory);
      }

      private void Setup(IDotNetifyHubForwarderFactory hubForwarderFactory, IClientProxy client, Action<string, string> callback = null)
      {
         var hubProxy = Substitute.For<IDotNetifyHubProxy>();

         Task TestResponse(string connectionId, string vmId, string vmData)
         {
            hubProxy.Response_VM += Raise.EventWith(this, (ResponseVMEventArgs) new InvokeResponseEventArgs
            {
               MethodArgs = new string[] { vmId, vmData },
               Metadata = DotNetifyHubForwardResponse.BuildMetadata(connectionId).ToDictionary(x => x.Key, x => x.Value.ToString())
            });
            return Task.CompletedTask;
         }

         using (var vmController = MockVMController(TestResponse))
         {
            hubProxy.StartAsync().Returns(Task.CompletedTask);
            hubProxy.Invoke(Arg.Any<string>(), Arg.Any<object[]>(), Arg.Any<IDictionary<string, object>>())
               .Returns(arg =>
               {
                  var methodArgs = (object[]) arg[1];
                  var metadata = (IDictionary<string, object>) arg[2];

                  if (arg[0].ToString() == nameof(IDotNetifyHubMethod.Request_VM))
                  {
                     vmController.OnRequestVMAsync(
                        metadata[DotNetifyHubForwarder.CONNECTION_ID_TOKEN].ToString(),
                        methodArgs[0].ToString(),
                        methodArgs[1]
                     ).GetAwaiter().GetResult();
                  }
                  else if (arg[0].ToString() == nameof(IDotNetifyHubMethod.Update_VM))
                  {
                     vmController.OnUpdateVMAsync(
                        metadata[DotNetifyHubForwarder.CONNECTION_ID_TOKEN].ToString(),
                        methodArgs[0].ToString(),
                        methodArgs[1] as Dictionary<string, object>
                     ).GetAwaiter().GetResult();
                  }
                  else if (arg[0].ToString() == nameof(IDotNetifyHubMethod.Dispose_VM))
                  {
                     vmController.OnDisposeVM(metadata[DotNetifyHubForwarder.CONNECTION_ID_TOKEN].ToString(), methodArgs[0].ToString());
                  }

                  callback?.Invoke(arg[0].ToString(), methodArgs[0].ToString());

                  return Task.CompletedTask;
               });

            var hubResponse = Substitute.For<IDotNetifyHubResponse>();
            hubResponse.SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
               .Returns(arg =>
               {
                  client.SendCoreAsync(nameof(IDotNetifyHubMethod.Response_VM), new object[] { new object[] { arg[1], arg[2] } });
                  return Task.CompletedTask;
               });

            var hubForwarder = new DotNetifyHubForwarder(hubProxy, hubResponse);
            hubForwarderFactory.GetInstance(Arg.Any<string>()).Returns(hubForwarder);
         }
      }

      #endregion Test Setup
   }
}