using DotNetify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

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

         public override string GroupName => GroupNameTest();

         public void PushMessage(string message)
         {
            Message = message;
            PushUpdates();
         }

         internal static Func<string> GroupNameTest { get; set; }
      }

      [TestInitialize]
      public void Initialize()
      {
         MulticastTestVM.GroupNameTest = () => null;
      }

      [TestMethod]
      public void MulticastVM_ViewModelShared()
      {
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
         var random = new Random();
         MulticastTestVM.GroupNameTest = () => random.Next().ToString();

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
         dynamic responseData1 = null;
         dynamic responseData2 = null;
         Action<string, string> responseDelegate = (connId, data) =>
         {
            if (connId.EndsWith("GroupSend"))
            {
               var msg = JsonConvert.DeserializeObject<VMController.GroupSend>(data);
               if (msg.ConnectionIds.Contains("conn1"))
                  responseData1 = JObject.Parse(msg.Data);
               if (msg.ConnectionIds.Contains("conn2"))
                  responseData2 = JObject.Parse(msg.Data);
            }
         };

         var vmFactory = MockVMController<MulticastTestVM>.GetVMFactory();
         var vmController1 = new MockVMController<MulticastTestVM>(vmFactory, "conn1", responseDelegate);
         var vmController2 = new MockVMController<MulticastTestVM>(vmFactory, "conn2", responseDelegate);

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
         dynamic responseData1 = null;
         dynamic responseData2 = null;
         Action<string, string> responseDelegate = (connId, data) =>
         {
            if (connId.EndsWith("GroupSend"))
            {
               var msg = JsonConvert.DeserializeObject<VMController.GroupSend>(data);
               if (msg.ConnectionIds.Contains("conn1"))
                  responseData1 = JObject.Parse(msg.Data);
               if (msg.ConnectionIds.Contains("conn2"))
                  responseData2 = JObject.Parse(msg.Data);
            }
         };

         var vmFactory = MockVMController<MulticastTestVM>.GetVMFactory();
         var vmController1 = new MockVMController<MulticastTestVM>(vmFactory, "conn1", responseDelegate);
         var vmController2 = new MockVMController<MulticastTestVM>(vmFactory, "conn2", responseDelegate);

         vmController1.RequestVM();
         vmController2.RequestVM();

         var update = new Dictionary<string, object>() { { nameof(MulticastTestVM.Message), "Goodbye" } };
         vmController1.UpdateVM(update);

         Assert.AreEqual("Goodbye", responseData2.Message.Value);

         update = new Dictionary<string, object>() { { nameof(MulticastTestVM.Message), "Adios" } };
         vmController2.UpdateVM(update);

         Assert.AreEqual("Adios", responseData1.Message.Value);
      }

      [TestMethod]
      public void MulticastVM_Group()
      {
         MulticastTestVM.GroupNameTest = () => "group1";

         dynamic responseData1 = null;
         dynamic responseData2 = null;
         string removeGroup1 = null;
         string removeGroup2 = null;
         Action<string, string> responseDelegate = (connId, data) =>
         {
            if (connId.EndsWith(nameof(VMController.GroupSend)))
            {
               var message = JsonConvert.DeserializeObject<VMController.GroupSend>(data);
               responseData1 = JObject.Parse(message.Data);
               responseData2 = JObject.Parse(message.Data);
            }
            else if (connId.EndsWith(nameof(VMController.GroupRemove)))
            {
               var message = JsonConvert.DeserializeObject<VMController.GroupRemove>(data);
               if (message.ConnectionId == "conn1")
                  removeGroup1 = message.GroupName;
               else if (message.ConnectionId == "conn2")
                  removeGroup2 = message.GroupName;
            }
         };

         var vmFactory = MockVMController<MulticastTestVM>.GetVMFactory();
         var vmController1 = new MockVMController<MulticastTestVM>(vmFactory, "conn1", responseDelegate);
         var vmController2 = new MockVMController<MulticastTestVM>(vmFactory, "conn2", responseDelegate);

         vmController1.RequestVM(out string groupName1);
         vmController2.RequestVM(out string groupName2);

         Assert.AreEqual("group1", groupName1);
         Assert.AreEqual("group1", groupName2);

         var update = new Dictionary<string, object>() { { nameof(MulticastTestVM.Message), "Goodbye" } };
         vmController1.UpdateVM(update);

         Assert.AreEqual("Goodbye", responseData2.Message.Value);

         update = new Dictionary<string, object>() { { nameof(MulticastTestVM.Message), "Adios" } };
         vmController2.UpdateVM(update);

         Assert.AreEqual("Adios", responseData1.Message.Value);

         vmController1.DisposeVM();
         vmController2.DisposeVM();

         Assert.AreEqual("group1", removeGroup1);
         Assert.AreEqual("group1", removeGroup2);
      }
   }
}