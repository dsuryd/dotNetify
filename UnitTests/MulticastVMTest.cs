using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetify;
using DotNetify.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
   [TestClass]
   public class MulticastVMTest
   {
      private class MulticastTestVM : MulticastVM
      {
         internal List<bool> DisposeCalls { get; } = new List<bool>();

         public string Message
         {
            get => Get<string>() ?? "Hello";
            set => Set(value);
         }

         public string Group => GroupName;

         public override string GroupName => GroupNameTest();

         public void PushMessage(string message)
         {
            Message = message;
            PushUpdates();
         }

         internal static Func<string> GroupNameTest { get; set; }

         protected override void Dispose(bool disposing)
         {
            DisposeCalls.Add(disposing);
         }
      }

      private HubEmulator _hubEmulator;

      [TestInitialize]
      public void Initialize()
      {
         MulticastTestVM.GroupNameTest = () => null;

         _hubEmulator = new HubEmulatorBuilder()
            .Register<MulticastTestVM>()
            .Build();
      }

      [TestMethod]
      public void MulticastVM_ViewModelShared()
      {
         var client1 = _hubEmulator.CreateClient();
         var client2 = _hubEmulator.CreateClient();

         var response = client1.Connect(nameof(MulticastTestVM)).As<dynamic>();

         Assert.AreEqual("Hello", (string) response.Message);

         var update = new Dictionary<string, object>() { { nameof(MulticastTestVM.Message), "World" } };
         client1.Dispatch(update);

         response = client2.Connect(nameof(MulticastTestVM)).As<dynamic>();
         Assert.AreEqual("World", (string) response.Message);
      }

      [TestMethod]
      public void MulticastVM_ViewModelNotShared()
      {
         var random = new Random();
         MulticastTestVM.GroupNameTest = () => random.Next().ToString();

         var client1 = _hubEmulator.CreateClient();
         var client2 = _hubEmulator.CreateClient();

         var response = client1.Connect(nameof(MulticastTestVM)).As<dynamic>();
         Assert.AreEqual("Hello", (string) response.Message);

         var update = new Dictionary<string, object>() { { nameof(MulticastTestVM.Message), "World" } };
         client1.Dispatch(update);

         response = client2.Connect(nameof(MulticastTestVM)).As<dynamic>();
         Assert.AreEqual("Hello", (string) response.Message);
      }

      [TestMethod]
      public void MulticastVM_ViewModelDisposed()
      {
         var client1 = _hubEmulator.CreateClient();
         var client2 = _hubEmulator.CreateClient();
         var client3 = _hubEmulator.CreateClient();

         var response = client1.Connect(nameof(MulticastTestVM)).As<dynamic>();
         Assert.AreEqual("Hello", (string) response.Message);

         var update = new Dictionary<string, object>() { { nameof(MulticastTestVM.Message), "World" } };
         client1.Dispatch(update);

         response = client2.Connect(nameof(MulticastTestVM)).As<dynamic>();
         Assert.AreEqual("World", (string) response.Message);

         client1.Destroy();
         client2.Destroy();

         response = client3.Connect(nameof(MulticastTestVM)).As<dynamic>();
         Assert.AreEqual("Hello", (string) response.Message);
      }

      [TestMethod]
      public async Task MulticastVM_PushUpdates()
      {
         var client1 = _hubEmulator.CreateClient();
         var client2 = _hubEmulator.CreateClient();

         client1.Connect(nameof(MulticastTestVM));
         client2.Connect(nameof(MulticastTestVM));

         var vm = _hubEmulator.CreatedVMs.Find(x => x is MulticastTestVM) as MulticastTestVM;

         var client1ResponsesTask = client1.ListenAsync();
         var client2ResponsesTask = client2.ListenAsync();

         vm.PushMessage("Goodbye");

         var client1Response = (await client1ResponsesTask).As<dynamic>();
         var client2Response = (await client2ResponsesTask).As<dynamic>();

         client1ResponsesTask = client1.ListenAsync();
         client2ResponsesTask = client2.ListenAsync();

         Assert.AreEqual("Goodbye", (string) client1Response.Message);
         Assert.AreEqual("Goodbye", (string) client2Response.Message);

         vm.PushMessage("World");

         client1Response = (await client1ResponsesTask).As<dynamic>();
         client2Response = (await client2ResponsesTask).As<dynamic>();

         Assert.AreEqual("World", (string) client1Response.Message);
         Assert.AreEqual("World", (string) client2Response.Message);
      }

      [TestMethod]
      public void MulticastVM_ChangedDataMulticasted()
      {
         var client1 = _hubEmulator.CreateClient();
         var client2 = _hubEmulator.CreateClient();

         client1.Connect(nameof(MulticastTestVM));
         client2.Connect(nameof(MulticastTestVM));

         var update = new Dictionary<string, object>() { { nameof(MulticastTestVM.Message), "Goodbye" } };
         var client2Response = client2.Listen(() => client1.Dispatch(update)).As<dynamic>();

         Assert.AreEqual("Goodbye", (string) client2Response.Message);

         update = new Dictionary<string, object>() { { nameof(MulticastTestVM.Message), "Adios" } };
         var client1Response = client1.Listen(() => client2.Dispatch(update)).As<dynamic>();

         Assert.AreEqual("Adios", (string) client1Response.Message);
      }

      [TestMethod]
      public void MulticastVM_Group()
      {
         MulticastTestVM.GroupNameTest = () => "group1";

         var client1 = _hubEmulator.CreateClient();
         var client2 = _hubEmulator.CreateClient();

         var client1Response = client1.Connect(nameof(MulticastTestVM)).As<dynamic>();
         var client2Response = client2.Connect(nameof(MulticastTestVM)).As<dynamic>();

         Assert.AreEqual("group1", (string) client1Response.Group);
         Assert.AreEqual("group1", (string) client2Response.Group);

         var update = new Dictionary<string, object>() { { nameof(MulticastTestVM.Message), "Goodbye" } };
         client2Response = client2.Listen(() => client1.Dispatch(update)).As<dynamic>();

         Assert.AreEqual("Goodbye", (string) client2Response.Message);

         update = new Dictionary<string, object>() { { nameof(MulticastTestVM.Message), "Adios" } };
         client1Response = client1.Listen(() => client2.Dispatch(update)).As<dynamic>();

         Assert.AreEqual("Adios", (string) client1Response.Message);

         client1.Destroy();
         client2.Destroy();

         var vm = _hubEmulator.CreatedVMs.Find(x => x is MulticastTestVM) as MulticastTestVM;

         Assert.AreEqual(2, vm.DisposeCalls.Count);
         Assert.IsFalse(vm.DisposeCalls[0]);
         Assert.IsTrue(vm.DisposeCalls[1]);
      }
   }
}