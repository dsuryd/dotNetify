using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using DotNetify;
using DotNetify.Testing;

namespace UnitTests
{
   [TestClass]
   public class HubDisonnectionMiddlewareTest
   {
      private class TestVM : BaseVM
      {
         public string Property { get; set; }
      }

      private class CustomMiddleware : IDisconnectionMiddleware
      {
         public static event EventHandler<HubCallerContext> Invoked;

         public Task OnDisconnected(HubCallerContext context)
         {
            Invoked?.Invoke(this, context);
            return Task.CompletedTask;
         }
      }

      [TestMethod]
      public void DisconnectionMiddleware_DisconnectionIntercepted()
      {
         var hubEmulator = new HubEmulatorBuilder()
            .Register<TestVM>()
            .UseMiddleware<CustomMiddleware>()
            .Build();

         bool middlewareInvoked = false;
         CustomMiddleware.Invoked += (sender, e) => middlewareInvoked = true;

         var client = hubEmulator.CreateClient();
         client.Connect(nameof(TestVM));

         client.TerminateHubConnection();
         Assert.IsTrue(middlewareInvoked);
      }
   }
}