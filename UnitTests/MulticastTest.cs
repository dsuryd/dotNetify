using DotNetify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UnitTests
{
   [TestClass]
   public class MulticastTest
   {
      private class MulticastTestVM : MulticastVM
      {
         public string Message { get; set; } = "Hello";

         public override bool IsMember => MemberTest();

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
   }
}