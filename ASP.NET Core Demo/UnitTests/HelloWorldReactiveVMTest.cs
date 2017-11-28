using DotNetify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Rx = System.Reactive.Linq;

namespace UnitTests
{
   [TestClass]
   public class HelloWorldReactiveVMTest
   {
      private static DateTime now = DateTime.Now;

      private class HelloWorldReactiveVM : BaseVM
      {
         public ReactiveProperty<string> FirstName { get; set; } = "Hello";
         public ReactiveProperty<string> LastName { get; set; } = "World";

         public HelloWorldReactiveVM()
         {
            this.AddReactiveProperty<string>("FullName")
               .SubscribeTo(Rx.Observable.CombineLatest(FirstName, LastName, FullNameDelegate));
         }

         public HelloWorldReactiveVM(bool live) : this()
         {
            this.AddReactiveProperty("ServerTime", DateTime.MinValue)
               .SubscribeTo(Rx.Observable.Interval(TimeSpan.FromMilliseconds(200)).Select(_ => DateTime.Now).StartWith(now))
               .OnChanged(() => PushUpdates());
         }

         private string FullNameDelegate(string firstName, string lastName) => $"{firstName} {lastName}";
      }

      [TestMethod]
      public void ReactiveVM_Request()
      {
         var vmController = new MockVMController<HelloWorldReactiveVM>();
         var vm = vmController.RequestVM();

         Assert.IsNotNull(vm);
         Assert.AreEqual("Hello", (string) vm.FirstName);
         Assert.AreEqual("World", (string)vm.LastName);
         Assert.AreEqual("Hello World", vm.GetProperty<string>("FullName"));
      }

      [TestMethod]
      public void ReactiveVM_Update()
      {
         var vmController = new MockVMController<HelloWorldReactiveVM>();
         vmController.RequestVM();

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var response1 = vmController.UpdateVM(update);

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         var response2 = vmController.UpdateVM(update);

         Assert.AreEqual("John World", response1["FullName"]);
         Assert.AreEqual("John Doe", response2["FullName"]);
      }

      [TestMethod]
      public void ReactiveVM_Dispose()
      {
         bool dispose = false;
         var vm = new HelloWorldReactiveVM();
         vm.Disposed += (sender, e) => dispose = true;

         var vmController = new MockVMController<HelloWorldReactiveVM>(vm);
         vmController.RequestVM();

         vmController.DisposeVM();
         Assert.IsTrue(dispose);
      }

      [TestMethod]
      public void ReactiveVM_PushUpdates()
      {
         int updateCounter = 0;

         var vmController = new MockVMController<HelloWorldReactiveVM>(new HelloWorldReactiveVM(true));
         vmController.OnResponse += (sender, e) => updateCounter++;
         vmController.RequestVM();

         System.Threading.Thread.Sleep(1000);
         Assert.IsTrue(updateCounter >= 5);
      }
   }
}
