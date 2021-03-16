using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetify;
using DotNetify.Client;
using DotNetify.DevApp;
using DotNetify.Forwarding;
using DotNetify.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using NSubstitute;

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

      private struct UserInfo
      {
         public string Id { get; set; }
         public string CorrelationId { get; set; }
      }

      private struct MessageInfo
      {
         public string UserName { get; set; }
         public string Text { get; set; }
      }

      private struct ChatRoomState
      {
         public List<UserInfo> Users { get; set; }
         public List<MessageInfo> Messages { get; set; }
      }

      private IClientEmulator _forwardingClient;

      [TestInitialize]
      public void Initialize()
      {
         var hubEmulator = new HubEmulatorBuilder()
          .AddServices(x => x.AddLogging())
          .Register<HelloWorldVM>()
          .Register<ChatRoomVM>()
          .Build();

         _forwardingClient = hubEmulator.CreateClient("forwarder-conn-id");
      }

      [TestMethod]
      public void ForwardingMiddleware_ConnectForwarded()
      {
         var hubForwarderFactory = Substitute.For<IDotNetifyHubForwarderFactory>();

         var hubEmulator = new HubEmulatorBuilder()
            .AddServices(x => x.AddLogging())
            .UseMiddleware<ForwardingMiddleware>(hubForwarderFactory, "serverUrl", new ForwardingOptions())
            .Build();

         var client = hubEmulator.CreateClient();
         Setup(_forwardingClient, hubForwarderFactory, client);

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
            .AddServices(x => x.AddLogging())
            .UseMiddleware<ForwardingMiddleware>(hubForwarderFactory, "serverUrl", new ForwardingOptions())
            .Build();

         var client = hubEmulator.CreateClient();
         Setup(_forwardingClient, hubForwarderFactory, client);

         client.Connect(nameof(HelloWorldVM));

         var response = client.Dispatch(new { FirstName = "John" }).As<HelloWorldState>();
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
            .AddServices(x => x.AddLogging())
            .UseMiddleware<ForwardingMiddleware>(hubForwarderFactory, "serverUrl", new ForwardingOptions())
            .Build();

         var client = hubEmulator.CreateClient();
         Setup(_forwardingClient, hubForwarderFactory, client, (callType, vmId) =>
         {
            dispose = callType == nameof(IDotNetifyHubMethod.Dispose_VM) && vmId == nameof(HelloWorldVM);
         });

         client.Connect(nameof(HelloWorldVM));
         client.Destroy();

         Assert.IsTrue(dispose);
      }

      [TestMethod]
      public void ForwardingMiddleware_Multicast_UserConnects_OtherUserIsNotified()
      {
         var expectedClient1CorrelationId = "0.123";
         var expectedClient2CorrelationId = "0.456";

         var hubForwarderFactory = Substitute.For<IDotNetifyHubForwarderFactory>();
         var hubEmulator = new HubEmulatorBuilder()
                  .AddServices(x => x.AddLogging())
                  .UseMiddleware<ForwardingMiddleware>(hubForwarderFactory, "serverUrl", new ForwardingOptions())
                  .Build();

         var client1 = hubEmulator.CreateClient("client1-conn-id");
         var client2 = hubEmulator.CreateClient("client2-conn-id");

         Setup(_forwardingClient, hubForwarderFactory, new[] { client1, client2 });

         client1.Connect(nameof(ChatRoomVM));
         client1.Dispatch(new { AddUser = expectedClient1CorrelationId });

         var client2Response = client2.Connect(nameof(ChatRoomVM)).As<ChatRoomState>();

         Assert.AreEqual(1, client2Response.Users.Count);
         Assert.AreEqual(expectedClient1CorrelationId, client2Response.Users[0].CorrelationId);

         var client1Responses = client1.Listen(() =>
         {
            client2.Dispatch(new { AddUser = expectedClient2CorrelationId });
         });

         Assert.AreEqual(1, client1Responses.Count);
         Assert.AreEqual("client2-conn-id", (string) client1Responses.As<dynamic>().Users_add.Id);
         Assert.AreEqual(expectedClient2CorrelationId, (string) client1Responses.As<dynamic>().Users_add.CorrelationId);
         Assert.IsTrue(client1.GetState<ChatRoomState>().Users.Any(x => x.CorrelationId == expectedClient2CorrelationId));
      }

      [TestMethod]
      public void ForwardingMiddleware_Multicast_UserLeaves_OtherUserIsNotified()
      {
         var hubForwarderFactory = Substitute.For<IDotNetifyHubForwarderFactory>();
         var hubEmulator = new HubEmulatorBuilder()
            .AddServices(x => x.AddLogging())
            .UseMiddleware<ForwardingMiddleware>(hubForwarderFactory, "serverUrl", new ForwardingOptions())
            .Build();

         var client1 = hubEmulator.CreateClient("client1-conn-id");
         var client2 = hubEmulator.CreateClient("client2-conn-id");

         Setup(_forwardingClient, hubForwarderFactory, new[] { client1, client2 });

         client1.Connect(nameof(ChatRoomVM));
         var response = client1.Dispatch(new { AddUser = "0.123" }).As<dynamic>();
         string expectedClient1Id = response.Users_add.Id;

         client2.Connect(nameof(ChatRoomVM));
         client2.Dispatch(new { AddUser = "0.456" });

         var client2Responses = client2.Listen(() =>
         {
            client1.Destroy();
         });

         Assert.AreEqual(1, client2Responses.Count);
         Assert.AreEqual(expectedClient1Id, (string) client2Responses.As<dynamic>().Users_remove);
         Assert.IsFalse(client2.GetState<ChatRoomState>().Users.Any(x => x.Id == expectedClient1Id));
      }

      [TestMethod]
      public async Task ForwardingMiddleware_Multicast_UserSendsMessage_OtherUserReceivesMessage()
      {
         string expectedClient1Text = "Hi there!";
         string expectedClient1UserName = "Rick";

         string expectedClient2Text = "What's up, Rick?";
         string expectedClient2UserName = "Carol";

         var hubForwarderFactory = Substitute.For<IDotNetifyHubForwarderFactory>();
         var hubEmulator = new HubEmulatorBuilder()
            .AddServices(x => x.AddLogging())
            .UseMiddleware<ForwardingMiddleware>(hubForwarderFactory, "serverUrl", new ForwardingOptions())
            .Build();

         var client1 = hubEmulator.CreateClient("client1-conn-id");
         var client2 = hubEmulator.CreateClient("client2-conn-id");
         var client3 = hubEmulator.CreateClient("client3-conn-id");

         Setup(_forwardingClient, hubForwarderFactory, new[] { client1, client2, client3 });

         client1.Connect(nameof(ChatRoomVM));
         client1.Dispatch(new { AddUser = "0.123" });

         client2.Connect(nameof(ChatRoomVM));
         client2.Dispatch(new { AddUser = "0.456" });

         client3.Connect(nameof(ChatRoomVM));
         client3.Dispatch(new { AddUser = "0.789" });

         var client3AsyncResponses = client3.ListenAsync();
         var client2AsyncResponses = client2.ListenAsync();

         client1.Dispatch(new
         {
            SendMessage = new
            {
               Text = expectedClient1Text,
               Date = DateTime.Now,
               UserName = expectedClient1UserName
            }
         });

         var client2Responses = await client2AsyncResponses;
         var client3Responses = await client3AsyncResponses;

         Assert.AreEqual(1, client2Responses.Count);
         Assert.AreEqual(expectedClient1Text, (string) client2Responses.As<dynamic>().Messages_add.Text);
         Assert.AreEqual(expectedClient1UserName, (string) client2Responses.As<dynamic>().Messages_add.UserName);
         Assert.IsTrue(client2.GetState<ChatRoomState>().Messages.Any(x => x.Text == expectedClient1Text && x.UserName == expectedClient1UserName));

         Assert.AreEqual(1, client3Responses.Count);
         Assert.AreEqual(expectedClient1Text, (string) client3Responses.As<dynamic>().Messages_add.Text);
         Assert.AreEqual(expectedClient1UserName, (string) client3Responses.As<dynamic>().Messages_add.UserName);
         Assert.IsTrue(client3.GetState<ChatRoomState>().Messages.Any(x => x.Text == expectedClient1Text && x.UserName == expectedClient1UserName));

         var client1Responses = client1.Listen(() =>
         {
            client2.Dispatch(new
            {
               SendMessage = new
               {
                  Text = expectedClient2Text,
                  Date = DateTime.Now,
                  UserName = expectedClient2UserName
               }
            });
         });

         Assert.AreEqual(1, client1Responses.Count);
         Assert.AreEqual(expectedClient2Text, (string) client1Responses.As<dynamic>().Messages_add.Text);
         Assert.AreEqual(expectedClient2UserName, (string) client1Responses.As<dynamic>().Messages_add.UserName);
         Assert.IsTrue(client1.GetState<ChatRoomState>().Messages.Any(x => x.Text == expectedClient2Text && x.UserName == expectedClient2UserName));
      }

      [TestMethod]
      public void ForwardingMiddleware_FilterOption_ConnectForwarded()
      {
         var forwardingClient1 = new HubEmulatorBuilder()
          .AddServices(x => x.AddLogging())
          .Register<HelloWorldVM>()
          .Build()
          .CreateClient("forwarder-conn-id-1");

         var forwardingClient2 = new HubEmulatorBuilder()
          .AddServices(x => x.AddLogging())
          .Register<ChatRoomVM>()
          .Build()
          .CreateClient("forwarder-conn-id-2");

         var hubForwarderFactory1 = Substitute.For<IDotNetifyHubForwarderFactory>();
         var hubForwarderFactory2 = Substitute.For<IDotNetifyHubForwarderFactory>();

         var hubEmulator = new HubEmulatorBuilder()
            .AddServices(x => x.AddLogging())
            .UseMiddleware<ForwardingMiddleware>(hubForwarderFactory1, "server1Url",
               new ForwardingOptions
               {
                  Filter = context => context.VMId == nameof(HelloWorldVM)
               })
            .UseMiddleware<ForwardingMiddleware>(hubForwarderFactory2, "server2Url",
               new ForwardingOptions
               {
                  Filter = context => context.VMId == nameof(ChatRoomVM)
               })
            .Build();

         var client = hubEmulator.CreateClient();

         Setup(forwardingClient1, hubForwarderFactory1, client);
         Setup(forwardingClient2, hubForwarderFactory2, client);

         var response = client.Connect(nameof(HelloWorldVM)).As<HelloWorldState>();

         Assert.AreEqual("Hello", response.FirstName);
         Assert.AreEqual("World", response.LastName);
         Assert.AreEqual("Hello World", response.FullName);

         client.Destroy();

         var response2 = client.Connect(nameof(ChatRoomVM)).As<ChatRoomState>();

         Assert.AreEqual(0, response2.Users.Count);
      }

      #region Test Setup

      private void Setup(IClientEmulator forwardingClient, IDotNetifyHubForwarderFactory hubForwarderFactory, IClientEmulator client, Action<string, string> callback = null)
      {
         Setup(forwardingClient, hubForwarderFactory, new IClientEmulator[] { client }, callback);
      }

      private void Setup(IClientEmulator forwardingClient, IDotNetifyHubForwarderFactory hubForwarderFactory, IClientEmulator[] clients, Action<string, string> callback = null)
      {
         // Build a mock hub proxy that will be used to forward messages.  This proxy will be set to communicate to another hub emulator.
         var hubProxy = Substitute.For<IDotNetifyHubProxy>();
         hubProxy.StartAsync().Returns(Task.CompletedTask);
         hubProxy.ConnectionState.Returns(HubConnectionState.Connected);

         // When the hub proxy's Invoke method is called, call the other hub emulator's Invoke method,
         // then get the response through the client emulator connecting with that hub, and finally
         // raise the hub proxy's own Response_VM event with the response.
         hubProxy.Invoke(Arg.Any<string>(), Arg.Any<object[]>(), Arg.Any<IDictionary<string, object>>())
            .Returns(arg =>
            {
               var callType = arg[0].ToString();
               var methodArgs = (arg[1] as object[]).Select(x =>
               {
                  if (x is string)
                     return x;
                  if (x != null)
                     return JObject.FromObject(x);
                  return null;
               }).ToArray();

               var metadataString = JsonSerializer.Serialize((IDictionary<string, object>) arg[2]);
               var metadata = Newtonsoft.Json.JsonConvert.DeserializeObject<IDictionary<string, object>>(metadataString);
               foreach (var kvp in metadata.ToList())
                  metadata[kvp.Key] = kvp.Value is string ? kvp.Value : JObject.FromObject(kvp.Value);

               forwardingClient.Hub.InvokeAsync(callType, methodArgs, metadata).GetAwaiter().GetResult();

               var responses = forwardingClient.ResponseHistory.Select(x => x.Payload).ToArray();
               forwardingClient.ResponseHistory.Clear();
               foreach (var response in responses)
               {
                  hubProxy.Response_VM += Raise.EventWith(this,
                     (ResponseVMEventArgs) new InvokeResponseEventArgs
                     {
                        MethodName = response[0].ToString(),
                        MethodArgs = (response[1] as object[]).Select(x => x.ToString()).ToArray(),
                        Metadata = (response[2] as Dictionary<string, object>).ToDictionary(x => x.Key, x => x.Value.ToString())
                     });
               }
               callback?.Invoke(callType, methodArgs[0].ToString());

               return Task.CompletedTask;
            });

         // Build a mock hub response that will receive response back to the above hub proxy.
         var hubResponse = Stubber.Create<IDotNetifyHubResponse>()
            .Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
               .Returns((string connectionId, string vmId, string data) =>
               {
                  var client = clients.First(x => x.ConnectionId == connectionId);
                  return (client as IClientProxy).SendCoreAsync(nameof(IDotNetifyHubMethod.Response_VM), new object[] { new object[] { vmId, data } });
               })
            .Setup(x => x.SendToManyAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<string>(), It.IsAny<string>()))
               .Returns((IReadOnlyList<string> connectionIds, string vmId, string data) =>
               {
                  foreach (var connectionId in connectionIds)
                  {
                     var client = clients.First(x => x.ConnectionId == connectionId);
                     (client as IClientProxy).SendCoreAsync(nameof(IDotNetifyHubMethod.Response_VM), new object[] { new object[] { vmId, data } });
                  }
                  return Task.CompletedTask;
               })
            .Object;

         var hubForwarder = new DotNetifyHubForwarder(hubProxy, hubResponse);
         hubForwarderFactory.InvokeInstanceAsync(Arg.Any<string>(), Arg.Any<ForwardingOptions>(), Arg.Any<Func<DotNetifyHubForwarder, Task>>())
            .Returns((callInfo) => callInfo.Arg<Func<DotNetifyHubForwarder, Task>>().Invoke(hubForwarder));
      }

      #endregion Test Setup
   }
}