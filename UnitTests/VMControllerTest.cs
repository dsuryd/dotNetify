using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DotNetify;

namespace UnitTest
{
   public class UnitTestVM : BaseVM
   {
      public string FirstName
      {
         get { return Get<string>(); }
         set
         {
            Set(value);
            Changed(() => FullName);
         }
      }

      public string LastName
      {
         get { return Get<string>(); }
         set
         {
            Set(value);
            Changed(() => FullName);
         }
      }

      public string FullName => $"{FirstName} {LastName}";

      public int Age
      {
         get { return Get<int>(); }
         set { Set(value); }
      }

      public UnitTestVM()
      {
         FirstName = "John";
         LastName = "Smith";
         Age = 25;
      }
   }

   [TestClass]
   public class VMControllerTest
   {
      private string _connectionId;
      private string _vmId;
      private string _vmData;

      static VMControllerTest()
      {
         VMController.RegisterAssembly<BaseVM>(typeof(UnitTestVM).Assembly);
      }

      public void TestResponse(string connectionId, string vmId, string vmData)
      {
         _connectionId = connectionId;
         _vmId = vmId;
         _vmData = vmData;
      }

      [TestMethod]
      public void VMController_OnRequestVM()
      {
         var vmController = new VMController(TestResponse);
         vmController.OnRequestVM("conn1", typeof(UnitTestVM).Name);

         Assert.AreEqual("conn1", _connectionId);
         Assert.AreEqual(typeof(UnitTestVM).Name, _vmId);
         var vmData = JsonConvert.DeserializeObject<UnitTestVM>(_vmData);
         Assert.IsNotNull(vmData);
         Assert.AreEqual("John", vmData.FirstName);
         Assert.AreEqual("Smith", vmData.LastName);
         Assert.AreEqual("John Smith", vmData.FullName);
         Assert.AreEqual(25, vmData.Age);

         vmController.Dispose();
      }

      [TestMethod]
      public void VMController_OnUpdateVM()
      {
         var vmController = new VMController(TestResponse);
         vmController.OnRequestVM("conn1", typeof(UnitTestVM).Name);

         vmController.OnUpdateVM("conn1", typeof(UnitTestVM).Name, new Dictionary<string, object>() { { "LastName", "Doe" }, { "Age", 42 } });

         Assert.AreEqual("conn1", _connectionId);
         Assert.AreEqual(typeof(UnitTestVM).Name, _vmId);
         var vmData = (JObject)JsonConvert.DeserializeObject(_vmData);
         Assert.AreEqual("John Doe", vmData["FullName"]);

         vmController.OnRequestVM("conn1", typeof(UnitTestVM).Name);
         Assert.AreEqual("conn1", _connectionId);
         Assert.AreEqual(typeof(UnitTestVM).Name, _vmId);
         vmData = (JObject)JsonConvert.DeserializeObject(_vmData);
         Assert.AreEqual("Doe", vmData["LastName"]);
         Assert.AreEqual(42, vmData["Age"]);

         vmController.Dispose();
      }
   }
}