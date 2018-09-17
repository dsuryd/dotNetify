using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetify;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ViewModels;

namespace UnitTest.ViewModelsTest
{
   [TestClass]
   public class HelloWorldVMTest
   {
      private string _connectionId;
      private string _vmId;
      private string _vmData;

      private T GetVM<T>() where T : class
      {
         return JsonConvert.DeserializeObject<T>(_vmData);
      }

      private JObject VMData => (JObject)JsonConvert.DeserializeObject(_vmData);

      public void TestResponse(string connectionId, string vmId, string vmData)
      {
         _connectionId = connectionId;
         _vmId = vmId;
         _vmData = vmData;
      }

      [TestMethod]
      public void HelloWorldVM()
      {
         VMController.RegisterAssembly<BaseVM>(typeof(HelloWorldVM).Assembly);

         var vmController = new VMController(TestResponse);
         vmController.OnRequestVM("conn1", typeof(HelloWorldVM).Name);

         Assert.AreEqual(typeof(HelloWorldVM).Name, _vmId);
         var vm = GetVM<HelloWorldVM>();
         Assert.IsNotNull(vm);
         Assert.AreEqual("Hello", vm.FirstName);
         Assert.AreEqual("World", vm.LastName);
         Assert.AreEqual("Hello World", vm.FullName);

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         vmController.OnUpdateVM("conn1", typeof(HelloWorldVM).Name, update);

         Assert.IsNotNull(VMData);
         Assert.AreEqual("John World", VMData["FullName"]);

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         vmController.OnUpdateVM("conn1", typeof(HelloWorldVM).Name, update);

         Assert.IsNotNull(VMData);
         Assert.AreEqual("John Doe", VMData["FullName"]);
      }
   }
}