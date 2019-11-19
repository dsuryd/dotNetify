using DotNetify;
using DotNetify.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace UnitTests
{
   [TestClass]
   public class RuntimeVMTest
   {
      private HubEmulator _hubEmulator;

      [TestInitialize]
      public void Initialize()
      {
         VMController.Register("HelloWorldRuntimeVM", _ =>
         {
            var vm = new BaseVM();
            var firstName = vm.AddProperty("FirstName", "Hello");
            var lastName = vm.AddProperty("LastName", "World");
            vm.AddProperty<string>("FullName").SubscribeTo(Observable.CombineLatest(firstName, lastName, (fn, ln) => $"{fn} {ln}"));
            return vm;
         });

         VMController.Register("MyNamespace.HelloWorldRuntimeVM", _ =>
         {
            var vm = new BaseVM();
            var firstName = vm.AddProperty("FirstName", "John");
            var lastName = vm.AddProperty("LastName", "Hancock");
            vm.AddProperty<string>("FullName").SubscribeTo(Observable.CombineLatest(firstName, lastName, (fn, ln) => $"{fn} {ln}"));
            return vm;
         });

         var hubEmulatorBuilder = new HubEmulatorBuilder();

         foreach (var typeHelper in VMController.VMTypes)
            hubEmulatorBuilder.Register(typeHelper.FullName, typeHelper.CreateInstance());

         _hubEmulator = hubEmulatorBuilder.Build();
      }

      [TestMethod]
      public void RuntimeVM_Request()
      {
         var client = _hubEmulator.CreateClient();
         var response = client.Connect("HelloWorldRuntimeVM").As<dynamic>();

         Assert.AreEqual("Hello", (string) response.FirstName);
         Assert.AreEqual("World", (string) response.LastName);
         Assert.AreEqual("Hello World", (string) response.FullName);
      }

      [TestMethod]
      public void RuntimeVM_Update()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect("HelloWorldRuntimeVM");

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var response1 = client.Dispatch(update).As<dynamic>();

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         var response2 = client.Dispatch(update).As<dynamic>();

         Assert.AreEqual("John World", (string) response1.FullName);
         Assert.AreEqual("John Doe", (string) response2.FullName);
      }

      [TestMethod]
      public void RuntimeVM_RequestWithNamespace()
      {
         var options = new VMConnectOptions();
         options.VMArg.Set("namespace", "MyNamespace");

         var client = _hubEmulator.CreateClient();
         var response = client.Connect("HelloWorldRuntimeVM", options).As<dynamic>();

         Assert.AreEqual("John", (string) response.FirstName);
         Assert.AreEqual("Hancock", (string) response.LastName);
         Assert.AreEqual("John Hancock", (string) response.FullName);
      }
   }
}