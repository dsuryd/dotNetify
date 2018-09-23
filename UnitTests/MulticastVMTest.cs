using DotNetify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace UnitTests
{
   [TestClass]
   public class MulticastVMTest
   {
      private class MulticastTestVM : MulticastVM
      {
         public string Message
         {
            get => Get<string>() ?? "Hello";
            set => Set(value);
         }

         public override bool IsMember => MemberTest();

         public void PushMessage(string message)
         {
            Message = message;
            PushUpdates();
         }

         internal static Func<bool> MemberTest { get; set; } = () => true;
      }

      [TestMethod]
      public void MulticastVM_ViewModelShared()
      {
         MulticastTestVM.MemberTest = () => true;

         var vmFactory = MockVMController<MulticastTestVM>.GetVMFactory();
         var vmController1 = new MockVMController<MulticastTestVM>(vmFactory);
         var vmController2 = new MockVMController<MulticastTestVM>(vmFactory);

         var response = vmController1.RequestVM();
         Assert.AreEqual("Hello", response.GetVMProperty<string>(nameof(MulticastTestVM.Message)));

         var update = new Dictionary<string, object>() { { nameof(MulticastTestVM.Message), "World" } };
         vmController1.UpdateVM(update);

         response = vmController2.RequestVM();
         Assert.AreEqual("World", response.GetVMProperty<string>(nameof(MulticastTestVM.Message)));
      }

      [TestMethod]
      public void MulticastVM_ViewModelNotShared()
      {
         MulticastTestVM.MemberTest = () => false;

         var vmFactory = MockVMController<MulticastTestVM>.GetVMFactory();
         var vmController1 = new MockVMController<MulticastTestVM>(vmFactory);
         var vmController2 = new MockVMController<MulticastTestVM>(vmFactory);

         var response = vmController1.RequestVM();
         Assert.AreEqual("Hello", response.GetVMProperty<string>(nameof(MulticastTestVM.Message)));

         var update = new Dictionary<string, object>() { { nameof(MulticastTestVM.Message), "World" } };
         vmController1.UpdateVM(update);

         response = vmController2.RequestVM();
         Assert.AreEqual("Hello", response.GetVMProperty<string>(nameof(MulticastTestVM.Message)));
      }

      [TestMethod]
      public void MulticastVM_ViewModelDisposed()
      {
         MulticastTestVM.MemberTest = () => true;

         var vmFactory = MockVMController<MulticastTestVM>.GetVMFactory();
         var vmController1 = new MockVMController<MulticastTestVM>(vmFactory);
         var vmController2 = new MockVMController<MulticastTestVM>(vmFactory);
         var vmController3 = new MockVMController<MulticastTestVM>(vmFactory);

         var response = vmController1.RequestVM();
         Assert.AreEqual("Hello", response.GetVMProperty<string>(nameof(MulticastTestVM.Message)));

         var update = new Dictionary<string, object>() { { nameof(MulticastTestVM.Message), "World" } };
         vmController1.UpdateVM(update);

         response = vmController2.RequestVM();
         Assert.AreEqual("World", response.GetVMProperty<string>(nameof(MulticastTestVM.Message)));

         vmController1.DisposeVM();
         vmController2.DisposeVM();

         response = vmController3.RequestVM();
         Assert.AreEqual("Hello", response.GetVMProperty<string>(nameof(MulticastTestVM.Message)));
      }

      [TestMethod]
      public void MulticastVM_PushUpdates()
      {
         MulticastTestVM.MemberTest = () => true;

         var vmFactory = MockVMController<MulticastTestVM>.GetVMFactory();
         var vmController1 = new MockVMController<MulticastTestVM>(vmFactory);
         var vmController2 = new MockVMController<MulticastTestVM>(vmFactory);

         dynamic responseData1 = null;
         dynamic responseData2 = null;
         vmController1.OnResponse += (sender, data) => responseData1 = JObject.Parse(data);
         vmController2.OnResponse += (sender, data) => responseData2 = JObject.Parse(data);

         vmController1.RequestVM();
         vmController2.RequestVM();

         var vm = vmFactory.GetInstance(nameof(MulticastTestVM)) as MulticastTestVM;
         vm.PushMessage("Goodbye");

         Assert.AreEqual("Goodbye", responseData1.Message.Value);
         Assert.AreEqual("Goodbye", responseData2.Message.Value);

         vm.PushMessage("World");

         Assert.AreEqual("World", responseData1.Message.Value);
         Assert.AreEqual("World", responseData2.Message.Value);
      }

      [TestMethod]
      public void MulticastVM_ChangedDataMulticasted()
      {
         MulticastTestVM.MemberTest = () => true;

         var vmFactory = MockVMController<MulticastTestVM>.GetVMFactory();
         var vmController1 = new MockVMController<MulticastTestVM>(vmFactory);
         var vmController2 = new MockVMController<MulticastTestVM>(vmFactory);

         dynamic responseData1 = null;
         dynamic responseData2 = null;
         vmController1.OnResponse += (sender, data) => responseData1 = JObject.Parse(data);
         vmController2.OnResponse += (sender, data) => responseData2 = JObject.Parse(data);

         vmController1.RequestVM();
         vmController2.RequestVM();

         var update = new Dictionary<string, object>() { { nameof(MulticastTestVM.Message), "Goodbye" } };
         vmController1.UpdateVM(update);

         Assert.AreEqual("Goodbye", responseData2.Message.Value);

         update = new Dictionary<string, object>() { { nameof(MulticastTestVM.Message), "Adios" } };
         vmController2.UpdateVM(update);

         Assert.AreEqual("Adios", responseData1.Message.Value);
      }
   }
}