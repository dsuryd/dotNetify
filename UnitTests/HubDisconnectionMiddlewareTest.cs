using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using DotNetify;

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
         public event EventHandler<HubCallerContext> Invoked;

         public Task OnDisconnected(HubCallerContext context)
         {
            Invoked?.Invoke(this, context);
            return Task.CompletedTask;
         }
      }

      private CustomMiddleware _middleware;

      [TestInitialize]
      public void Initialize()
      {
         _middleware = new CustomMiddleware();
         VMController.CreateInstance = (type, args) =>  type == typeof(CustomMiddleware) ? _middleware : Activator.CreateInstance(type, args);
      }

      [TestCleanup]
      public void Cleanup()
      {
         VMController.CreateInstance = (type, args) => Activator.CreateInstance(type, args);
      }

      [TestMethod]
      public void DisconnectionMiddleware_DisconnectionIntercepted()
      {
         VMController.Register<TestVM>();
         var hub = new MockDotNetifyHub()
            .UseMiddleware<CustomMiddleware>()
            .Create();

         bool middlewareInvoked = false;
         _middleware.Invoked += (sender, e) => middlewareInvoked = true;

         hub.RequestVM(nameof(TestVM));
         hub.OnDisconnected();
         Assert.IsTrue(middlewareInvoked);
      }
   }
}
