using System.Threading.Tasks;
using DevApp.ViewModels;
using DotNetify.Testing;
using Xunit;

namespace DotNetify.Testing.UnitTests
{
   public class LiveHubTest
   {
      private static readonly string TEST_SERVER_URL = "https://dotnetify.net";

      private struct ClientState
      {
         public string FirstName { get; set; }
         public string LastName { get; set; }
         public string FullName { get; set; }
      }

      [Fact]
      public async Task Connect_ReturnsInitialState()
      {
         using (var client = await LiveHub.CreateClientAsync(TEST_SERVER_URL))
         {
            var response = client.Connect(nameof(HelloWorldVM)).As<ClientState>();

            Assert.Equal("Hello", response.FirstName);
            Assert.Equal("World", response.LastName);
            Assert.Equal("Hello World", response.FullName);
         }
      }

      [Fact]
      public async Task ChangeNames_FullNameChanged()
      {
         using (var client = await LiveHub.CreateClientAsync(TEST_SERVER_URL))
         {
            client.Connect(nameof(HelloWorldVM));

            var response = client.Dispatch(new { FirstName = "John" }).As<dynamic>();
            Assert.Equal("John World", (string) response.FullName);

            client.Dispatch(new { LastName = "Doe" });
            Assert.Equal("John Doe", client.GetState<ClientState>().FullName);
         }
      }
   }
}