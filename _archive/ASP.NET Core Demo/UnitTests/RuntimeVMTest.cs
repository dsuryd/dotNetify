using DotNetify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace UnitTests
{
   [TestClass]
   public class RuntimeVMTest
   {
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
      }

      [TestMethod]
      public void RuntimeVM_Request()
      {
         var vmController = new MockVMController<BaseVM>();
         var response = vmController.RequestVM("HelloWorldRuntimeVM");

         Assert.AreEqual("Hello", response.GetVMProperty<string>("FirstName"));
         Assert.AreEqual("World", response.GetVMProperty<string>("LastName"));
         Assert.AreEqual("Hello World", response.GetVMProperty<string>("FullName"));
      }

      [TestMethod]
      public void RuntimeVM_Update()
      {
         var vmController = new MockVMController<BaseVM>();
         vmController.RequestVM("HelloWorldRuntimeVM");

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var response1 = vmController.UpdateVM(update, "HelloWorldRuntimeVM");

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         var response2 = vmController.UpdateVM(update, "HelloWorldRuntimeVM");

         Assert.AreEqual("John World", response1["FullName"]);
         Assert.AreEqual("John Doe", response2["FullName"]);
      }

      [TestMethod]
      public void RuntimeVM_RequestWithNamespace()
      {
         var vmController = new MockVMController<BaseVM>();
         var response = vmController.RequestVM("HelloWorldRuntimeVM", JObject.Parse("{ namespace: 'MyNamespace' }"));

         Assert.AreEqual("John", response.GetVMProperty<string>("FirstName"));
         Assert.AreEqual("Hancock", response.GetVMProperty<string>("LastName"));
         Assert.AreEqual("John Hancock", response.GetVMProperty<string>("FullName"));
      }
   }
}
