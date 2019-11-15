using DotNetify.DevApp;
using DotNetify.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
   [TestClass]
   public class ExampleHelloWorldTest
   {
      private HubEmulator _hubEmulator;

      private struct ClientState
      {
         public string FirstName { get; set; }
         public string LastName { get; set; }
         public string FullName { get; set; }
      }

      public ExampleHelloWorldTest()
      {
         _hubEmulator = new HubEmulatorBuilder()
            .Register<HelloWorldVM>(nameof(HelloWorldVM))
            .Build();
      }

      [TestMethod]
      public void ExampleHelloWorld_Connect_ReturnsInitialState()
      {
         var client = _hubEmulator.CreateClient();

         var response = client.Connect(nameof(HelloWorldVM)).As<ClientState>();

         Assert.AreEqual("Hello", response.FirstName);
         Assert.AreEqual("World", response.LastName);
         Assert.AreEqual("Hello World", response.FullName);
      }

      [TestMethod]
      public void ExampleHelloWorld_ChangeNames_FullNameChanged()
      {
         var client = _hubEmulator.CreateClient();

         client.Connect(nameof(HelloWorldVM));

         var response = client.Dispatch(new { FirstName = "John" }).As<dynamic>();
         Assert.AreEqual("John World", (string) response.FullName);

         client.Dispatch(new { LastName = "Doe" });
         Assert.AreEqual("John Doe", client.GetState<ClientState>().FullName);
      }
   }
}