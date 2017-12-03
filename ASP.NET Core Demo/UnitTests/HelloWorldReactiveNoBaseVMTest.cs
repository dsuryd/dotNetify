using DotNetify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;

namespace UnitTests
{
   [TestClass]
   public class HelloWorldReactiveNoBaseVMTest
   {
      private static DateTime now = DateTime.Now;

      private class HelloWorldReactiveNoBaseVM : INotifyPropertyChanged, IPushUpdates, IReactiveProperties
      {
         public event EventHandler RequestPushUpdates = delegate { };
         public event PropertyChangedEventHandler PropertyChanged = delegate { };

         public IList<IReactiveProperty> RuntimeProperties { get; } = new List<IReactiveProperty>();

         public HelloWorldReactiveNoBaseVM()
         {
            var firstName = this.AddProperty("FirstName", "Hello");
            var lastName = this.AddProperty("LastName", "World");

            this.AddProperty<string>("FullName")
               .SubscribeTo(Observable.CombineLatest(firstName, lastName, FullNameDelegate));
         }

         public HelloWorldReactiveNoBaseVM(bool live) : this()
         {
            this.AddProperty("ServerTime", DateTime.MinValue)
               .SubscribeTo(Observable.Interval(TimeSpan.FromMilliseconds(200)).Select(_ => DateTime.Now).StartWith(now))
               .Subscribe(_ => this.PushUpdates());
         }

         private string FullNameDelegate(string firstName, string lastName) => $"{firstName} {lastName}";
      }

      [TestMethod]
      public void HelloWorldReactiveNoBaseVM_Request()
      {
         var vmController = new MockVMController<HelloWorldReactiveNoBaseVM>();
         var response = vmController.RequestVM();

         Assert.AreEqual("Hello", response.GetVMProperty<string>("FirstName"));
         Assert.AreEqual("World", response.GetVMProperty<string>("LastName"));
         Assert.AreEqual("Hello World", response.GetVMProperty<string>("FullName"));
      }

      [TestMethod]
      public void HelloWorldReactiveNoBaseVM_Update()
      {
         var vmController = new MockVMController<HelloWorldReactiveNoBaseVM>();
         vmController.RequestVM();

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var response1 = vmController.UpdateVM(update);

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         var response2 = vmController.UpdateVM(update);

         Assert.AreEqual("John World", response1["FullName"]);
         Assert.AreEqual("John Doe", response2["FullName"]);
      }

      [TestMethod]
      public void HelloWorldReactiveNoBaseVM_PushUpdates()
      {
         int updateCounter = 0;

         var vmController = new MockVMController<HelloWorldReactiveNoBaseVM>(new HelloWorldReactiveNoBaseVM(true));
         vmController.OnResponse += (sender, e) => updateCounter++;
         vmController.RequestVM();

         System.Threading.Thread.Sleep(1000);
         Assert.IsTrue(updateCounter >= 4);
      }
   }
}
