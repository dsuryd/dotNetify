using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Rx = System.Reactive.Linq;
using DotNetify;

namespace UnitTests
{
   [TestClass]
   public class ReactiveVMTest
   {
      private ResponseStub _response = new ResponseStub();
      private string reactiveVMId = nameof(ReactiveVM);
      private string liveReactiveVMId = nameof(LiveReactiveVM);
      private static DateTime now = DateTime.Now;

      private class ReactiveVM : BaseVM
      {
         public ReactiveProperty<string> FirstName { get; set; } = "Hello";
         public ReactiveProperty<string> LastName { get; set; } = "World";
         public ReactiveProperty<string> FullName { get; set; } = "";

         public ReactiveVM()
         {
            FullName
               .SubscribeTo(Rx.Observable.CombineLatest(FirstName, LastName, FullNameDelegate))
               .OnChanged(() => Changed(nameof(FullName)));
         }

         private string FullNameDelegate(string firstName, string lastName) => $"{firstName} {lastName}";
      }

      private class LiveReactiveVM : BaseVM
      {
         public ReactiveProperty<DateTime> ServerTime { get; set; } = DateTime.MinValue;

         public LiveReactiveVM()
         {
            ServerTime
               .SubscribeTo(Rx.Observable.Interval(TimeSpan.FromMilliseconds(200)).Select(_ => DateTime.Now).StartWith(now))
               .OnChanged(() => Changed(nameof(ServerTime)))
               .OnChanged(() => PushUpdates());
         }
      }

      [TestMethod]
      public void ReactiveVM_Request()
      {
         VMController.Register<ReactiveVM>();

         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", typeof(ReactiveVM).Name);

         Assert.AreEqual(reactiveVMId, _response.VMId);
         var vm = _response.GetVM<ReactiveVM>();
         Assert.IsNotNull(vm);
         Assert.AreEqual("Hello", (string)vm.FirstName);
         Assert.AreEqual("World", (string)vm.LastName);
         Assert.AreEqual("Hello World", (string)vm.FullName);
      }

      [TestMethod]
      public void ReactiveVM_Update()
      {
         VMController.Register<ReactiveVM>();

         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", reactiveVMId);

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         vmController.OnUpdateVM("conn1", reactiveVMId, update);

         Assert.IsNotNull(_response.VMData);
         Assert.AreEqual("John World", _response.VMData["FullName"]);

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         vmController.OnUpdateVM("conn1", reactiveVMId, update);

         Assert.IsNotNull(_response.VMData);
         Assert.AreEqual("John Doe", _response.VMData["FullName"]);
      }

      [TestMethod]
      public void ReactiveVM_Dispose()
      {
         bool dispose = false;
         var vm = new ReactiveVM();
         vm.Disposed += (sender, e) => dispose = true;

         var baseDelegate = VMController.CreateInstance;
         VMController.CreateInstance = (type, args) => type == typeof(ReactiveVM) ? vm : baseDelegate(type, args);
         VMController.Register<ReactiveVM>();

         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", reactiveVMId);

         vmController.OnDisposeVM("conn1", reactiveVMId);
         Assert.IsTrue(dispose);
      }

      [TestMethod]
      public void LiveReactiveVM_PushUpdates()
      {
         int updateCounter = 0;

         VMController.Register<LiveReactiveVM>();
         VMController.VMResponseDelegate handler = (string connectionId, string vmId, string vmData) =>
         {
            updateCounter++;
         };

         var vmController = new VMController(handler);
         vmController.OnRequestVM("conn1", liveReactiveVMId);

         System.Threading.Thread.Sleep(1000);
         Assert.IsTrue(updateCounter >= 5);
      }
   }
}
