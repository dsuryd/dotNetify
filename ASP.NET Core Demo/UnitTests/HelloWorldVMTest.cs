using DotNetify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Rx = System.Reactive.Linq;

namespace UnitTests
{
   [TestClass]
   public class HelloWorldVMTest
   {
      private class HelloWorldVM : BaseVM
      {
         public string FirstName
         {
            get => Get<string>() ?? "Hello";
            set
            {
               Set(value);
               Changed(nameof(FullName));
            }
         }

         public string LastName
         {
            get => Get<string>() ?? "World";
            set
            {
               Set(value);
               Changed(nameof(FullName));
            }
         }

         public long Data
         {
            get => Get<long>();
            set => Set(value);
         }

         public string FullName => $"{FirstName} {LastName}";

         public HelloWorldVM()
         { }

         public HelloWorldVM(bool live) : this()
         {
            Rx.Observable.Interval(TimeSpan.FromMilliseconds(200)).Subscribe(value =>
            {
               Data = value;
               PushUpdates();
            });
         }
      }

      [TestMethod]
      public void HelloWorldVM_Request()
      {
         var vmController = new MockVMController<HelloWorldVM>();
         var response = vmController.RequestVM();

         Assert.AreEqual("Hello", response.GetVMProperty<string>("FirstName"));
         Assert.AreEqual("World", response.GetVMProperty<string>("LastName"));
         Assert.AreEqual("Hello World", response.GetVMProperty<string>("FullName"));
      }

      [TestMethod]
      public void HelloWorldVM_Update()
      {
         var vmController = new MockVMController<HelloWorldVM>();
         vmController.RequestVM();

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var response1 = vmController.UpdateVM(update);

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         var response2 = vmController.UpdateVM(update);

         Assert.AreEqual("John World", response1["FullName"]);
         Assert.AreEqual("John Doe", response2["FullName"]);
      }

      [TestMethod]
      public void HelloWorldVM_Dispose()
      {
         bool dispose = false;
         var vm = new HelloWorldVM();
         vm.Disposed += (sender, e) => dispose = true;

         var vmController = new MockVMController<HelloWorldVM>(vm);
         vmController.RequestVM();

         vmController.DisposeVM();
         Assert.IsTrue(dispose);
      }

      [TestMethod]
      public void HelloWorldVM_PushUpdates()
      {
         int updateCounter = 0;

         var vmController = new MockVMController<HelloWorldVM>(new HelloWorldVM(true));
         vmController.OnResponse += (sender, e) => updateCounter++;
         vmController.RequestVM();

         System.Threading.Thread.Sleep(1000);
         Assert.IsTrue(updateCounter >= 5);
      }
   }
}
