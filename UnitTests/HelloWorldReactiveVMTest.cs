using DotNetify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

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
            AddProperty<string>("FullName")
               .SubscribeTo(Observable.CombineLatest(FirstName, LastName, FullNameDelegate))
               .SubscribedBy(
                  AddProperty<int>("NameLength"), x => x.Select(name => name.Length)
               );

            AddInternalProperty<string>("Internal1");
            AddInternalProperty("Internal2", 0);
         }

         public HelloWorldReactiveVM(bool live) : this()
         {
            AddProperty("ServerTime", DateTime.MinValue)
               .SubscribeTo(Observable.Interval(TimeSpan.FromMilliseconds(200)).Select(_ => DateTime.Now).StartWith(now))
               .Subscribe(_ => PushUpdates());
         }

         private string FullNameDelegate(string firstName, string lastName) => $"{firstName} {lastName}";
      }

      [TestMethod]
      public void HelloWorldReactiveVM_Request()
      {
         var vmController = new MockVMController<HelloWorldReactiveVM>();
         var response = vmController.RequestVM();

         Assert.AreEqual("Hello", response.GetVMProperty<string>("FirstName"));
         Assert.AreEqual("World", response.GetVMProperty<string>("LastName"));
         Assert.AreEqual("Hello World", response.GetVMProperty<string>("FullName"));
      }

      [TestMethod]
      public void HelloWorldReactiveVM_Update()
      {
         var vmController = new MockVMController<HelloWorldReactiveVM>();
         vmController.RequestVM();

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var response1 = vmController.UpdateVM(update);

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         var response2 = vmController.UpdateVM(update);

         Assert.AreEqual("John World", response1["FullName"]);
         Assert.AreEqual("John Doe", response2["FullName"]);

         Assert.AreEqual(10, response1["NameLength"]);
         Assert.AreEqual(8, response2["NameLength"]);
      }

      [TestMethod]
      public void HelloWorldReactiveVM_Dispose()
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
         Assert.IsTrue(updateCounter >= 4);
      }

      [TestMethod]
      public void HelloWorldReactiveVM_Internal()
      {
         var vmController = new MockVMController<HelloWorldReactiveVM>();
         var response = vmController.RequestVM();

         // Internal properties should not be sent to the client.
         Assert.IsNull(response.VMData["Internal1"]);
         Assert.IsNull(response.VMData["Internal2"]);
      }
   }
}