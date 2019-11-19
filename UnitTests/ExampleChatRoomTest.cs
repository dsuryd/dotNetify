using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetify.DevApp;
using DotNetify.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
   [TestClass]
   public class ExampleChatRoomTest
   {
      private HubEmulator _hubEmulator;

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

      private struct ClientState
      {
         public List<UserInfo> Users { get; set; }
         public List<MessageInfo> Messages { get; set; }
      }

      public ExampleChatRoomTest()
      {
         _hubEmulator = new HubEmulatorBuilder()
            .Register<ChatRoomVM>()
            .Build();
      }

      [TestMethod]
      public void ExampleChatRoom_UserConnects_OtherUserIsNotified()
      {
         var expectedClient1CorrelationId = "0.123";
         var expectedClient2CorrelationId = "0.456";

         var client1 = _hubEmulator.CreateClient();
         var client2 = _hubEmulator.CreateClient();

         client1.Connect(nameof(ChatRoomVM));
         client1.Dispatch(new { AddUser = expectedClient1CorrelationId });

         var client2Response = client2.Connect(nameof(ChatRoomVM)).As<ClientState>();

         Assert.AreEqual(1, client2Response.Users.Count);
         Assert.AreEqual(expectedClient1CorrelationId, client2Response.Users[0].CorrelationId);

         var client1Responses = client1.Listen(() =>
         {
            client2.Dispatch(new { AddUser = expectedClient2CorrelationId });
         });

         Assert.AreEqual(1, client1Responses.Count);
         Assert.AreEqual(expectedClient2CorrelationId, (string) client1Responses.As<dynamic>().Users_add.CorrelationId);
         Assert.IsTrue(client1.GetState<ClientState>().Users.Any(x => x.CorrelationId == expectedClient2CorrelationId));
      }

      [TestMethod]
      public void ExampleChatRoom_UserLeaves_OtherUserIsNotified()
      {
         var client1 = _hubEmulator.CreateClient();
         var client2 = _hubEmulator.CreateClient();

         client1.Connect(nameof(ChatRoomVM));
         var response = client1.Dispatch(new { AddUser = "0.123" }).As<dynamic>();
         string expectedClient1Id = response.Users_add.Id;

         client2.Connect(nameof(ChatRoomVM));
         client2.Dispatch("{AddUser: '0.456'}");

         var client2Responses = client2.Listen(() =>
         {
            client1.Destroy();
         });

         Assert.AreEqual(1, client2Responses.Count);
         Assert.AreEqual(expectedClient1Id, (string) client2Responses.As<dynamic>().Users_remove);
         Assert.IsFalse(client2.GetState<ClientState>().Users.Any(x => x.Id == expectedClient1Id));
      }

      [TestMethod]
      public async Task ExampleChatRoom_UserSendsMessage_OtherUserReceivesMessage()
      {
         string expectedClient1Text = "Hi there!";
         string expectedClient1UserName = "Rick";

         string expectedClient2Text = "What's up, Rick?";
         string expectedClient2UserName = "Carol";

         var client1 = _hubEmulator.CreateClient();
         var client2 = _hubEmulator.CreateClient();
         var client3 = _hubEmulator.CreateClient();

         client1.Connect(nameof(ChatRoomVM));
         client1.Dispatch(new { AddUser = "0.123" });

         client2.Connect(nameof(ChatRoomVM));
         client2.Dispatch(new { AddUser = "0.456" });

         client3.Connect(nameof(ChatRoomVM));
         client3.Dispatch(new { AddUser = "0.789" });

         var client3AsyncResponses = client3.ListenAsync();

         var client2Responses = client2.Listen(() =>
         {
            client1.Dispatch(new
            {
               SendMessage = new
               {
                  Text = expectedClient1Text,
                  Date = DateTime.Now,
                  UserName = expectedClient1UserName
               }
            });
         });

         var client3Responses = await client3AsyncResponses;

         Assert.AreEqual(1, client2Responses.Count);
         Assert.AreEqual(expectedClient1Text, (string) client2Responses.As<dynamic>().Messages_add.Text);
         Assert.AreEqual(expectedClient1UserName, (string) client2Responses.As<dynamic>().Messages_add.UserName);
         Assert.IsTrue(client2.GetState<ClientState>().Messages.Any(x => x.Text == expectedClient1Text && x.UserName == expectedClient1UserName));

         Assert.AreEqual(1, client3Responses.Count);
         Assert.AreEqual(expectedClient1Text, (string) client3Responses.As<dynamic>().Messages_add.Text);
         Assert.AreEqual(expectedClient1UserName, (string) client3Responses.As<dynamic>().Messages_add.UserName);
         Assert.IsTrue(client3.GetState<ClientState>().Messages.Any(x => x.Text == expectedClient1Text && x.UserName == expectedClient1UserName));

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
         Assert.IsTrue(client1.GetState<ClientState>().Messages.Any(x => x.Text == expectedClient2Text && x.UserName == expectedClient2UserName));
      }
   }
}