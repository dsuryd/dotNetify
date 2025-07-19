using DotNetify.Testing;
using DevApp.ViewModels;
using Xunit;

namespace DotNetify.Testing.UnitTests
{
   public class HelloWorldVMTest
   {
      private HubEmulator _hubEmulator;

      private struct ClientState
      {
         public string FirstName { get; set; }
         public string LastName { get; set; }
         public string FullName { get; set; }
      }

      public HelloWorldVMTest()
      {
         _hubEmulator = new HubEmulatorBuilder()
            .Register<HelloWorldVM>(nameof(HelloWorldVM))
            .Build();
      }

      [Fact]
      public void Connect_ReturnsInitialState()
      {
         var client = _hubEmulator.CreateClient();

         var response = client.Connect(nameof(HelloWorldVM)).As<ClientState>();

         Assert.Equal("Hello", response.FirstName);
         Assert.Equal("World", response.LastName);
         Assert.Equal("Hello World", response.FullName);
      }

      [Fact]
      public void ChangeNames_FullNameChanged()
      {
         var client = _hubEmulator.CreateClient();

         client.Connect(nameof(HelloWorldVM));

         var response = client.Dispatch(new { FirstName = "John" }).As<dynamic>();
         Assert.Equal("John World", (string) response.FullName);

         client.Dispatch(new { LastName = "Doe" });
         Assert.Equal("John Doe", client.GetState<ClientState>().FullName);
      }
   }
}