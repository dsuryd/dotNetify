using DotNetify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using Rx = System.Reactive.Linq;

namespace UnitTests
{
   [TestClass]
   public class HelloWorldNoBaseVMTest
   {
      private class HelloWorldNoBaseVM : INotifyPropertyChanged, IPushUpdates, IDisposable
      {
         private string _firstName;
         private string _lastName;

         public event PropertyChangedEventHandler PropertyChanged = delegate { };
         public event EventHandler RequestPushUpdates = delegate { };
         public event EventHandler Disposed;

         public string FirstName
         {
            get => _firstName ?? "Hello";
            set
            {
               _firstName = value;
               this.Changed(nameof(FullName));
            }
         }

         public string LastName
         {
            get => _lastName ?? "World";
            set
            {
               _lastName = value;
               this.Changed(nameof(FullName));
            }
         }

         public long Data { get; set; }

         public string FullName => $"{FirstName} {LastName}";

         public HelloWorldNoBaseVM()
         { }

         public HelloWorldNoBaseVM(bool live) : this()
         {
            Rx.Observable.Interval(TimeSpan.FromMilliseconds(200)).Subscribe(value =>
            {
               Data = value;
               this.Changed(nameof(Data));
               this.PushUpdates();
            });
         }

         public void Dispose() => Disposed?.Invoke(this, EventArgs.Empty);
      }

      [TestMethod]
      public void HelloWorldNoBaseVM_Request()
      {
         var vmController = new MockVMController<HelloWorldNoBaseVM>();
         var response = vmController.RequestVM();

         Assert.AreEqual("Hello", response.GetVMProperty<string>("FirstName"));
         Assert.AreEqual("World", response.GetVMProperty<string>("LastName"));
         Assert.AreEqual("Hello World", response.GetVMProperty<string>("FullName"));
      }

      [TestMethod]
      public void HelloWorldNoBaseVM_Update()
      {
         var vmController = new MockVMController<HelloWorldNoBaseVM>();
         vmController.RequestVM();

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var response1 = vmController.UpdateVM(update);

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         var response2 = vmController.UpdateVM(update);

         Assert.AreEqual("John World", response1["FullName"]);
         Assert.AreEqual("John Doe", response2["FullName"]);
      }

      [TestMethod]
      public void HelloWorldNoBaseVM_Dispose()
      {
         bool dispose = false;
         var vm = new HelloWorldNoBaseVM();
         vm.Disposed += (sender, e) => dispose = true;

         var vmController = new MockVMController<HelloWorldNoBaseVM>(vm);
         vmController.RequestVM();

         vmController.DisposeVM();
         Assert.IsTrue(dispose);
      }

      [TestMethod]
      public void HelloWorldNoBaseVM_PushUpdates()
      {
         int updateCounter = 0;

         var vmController = new MockVMController<HelloWorldNoBaseVM>(new HelloWorldNoBaseVM(true));
         vmController.OnResponse += (sender, e) => updateCounter++;
         vmController.RequestVM();

         System.Threading.Thread.Sleep(1000);
         Assert.IsTrue(updateCounter >= 5);
      }
   }
}
