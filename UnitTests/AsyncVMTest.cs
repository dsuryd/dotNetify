using DotNetify;
using DotNetify.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace UnitTests
{
   [TestClass]
   public class AsyncVMTest
   {
      private class AsyncVM : BaseVM
      {
         public double Value { get; set; }

         public Func<int, Task> Power => (async (int exponent) => await PowerAsync(exponent));

         public override async Task OnCreatedAsync()
         {
            await Task.Delay(100);
            Value = 2;
         }

         public async Task SetPowerAsync(int exponent)
         {
            await PowerAsync(exponent);
         }

         private async Task PowerAsync(int exponent)
         {
            await Task.Delay(100);
            Value = Math.Pow(Value, exponent);
            Changed(nameof(Value));
         }
      }

      private HubEmulator _hubEmulator;

      [TestInitialize]
      public void Initialize()
      {
         _hubEmulator = new HubEmulatorBuilder()
            .Register<AsyncVM>()
            .Build();
      }

      [TestMethod]
      public void AsyncVM_Request()
      {
         var client = _hubEmulator.CreateClient();
         var response = client.Connect(nameof(AsyncVM)).As<dynamic>();

         Assert.AreEqual(2, (double) response.Value);
      }

      [TestMethod]
      public void AsyncVM_Update()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(AsyncVM)).As<dynamic>();

         var update = new Dictionary<string, object>() { { "Power", 2 } };
         var response = client.Dispatch(update).As<dynamic>();

         Assert.AreEqual(4, (double) response.Value);
      }

      [TestMethod]
      public void AsyncVM_InvokeMethod()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(AsyncVM)).As<dynamic>();

         var response = client.Dispatch(new { SetPowerAsync = 3 }).As<dynamic>();
         Assert.AreEqual(8, (double) response.Value);
      }
   }
}