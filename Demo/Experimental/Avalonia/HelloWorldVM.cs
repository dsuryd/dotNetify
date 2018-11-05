using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HelloWorld
{
   public class HelloWorldVM
   {
      private Task _initialize;

      public string Greetings => "Hello World";

      public HelloWorldVM()
      {
         _initialize = InitializeAsync();
      }

      private async Task InitializeAsync()
      {
         var vm = new DotNetifyClient(new DotNetifyHubProxy());
         await vm.ConnectAsync("HelloWorld");
      }
   }
}