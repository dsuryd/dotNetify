using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DotNetify;
using ViewModels;

namespace UnitTest.ViewModelsTest
{
   [TestClass]
   public class ControlTypesVMTest
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

      [ClassInitialize]
      public static void Initialize(TestContext testContext)
      {
         VMController.RegisterAssembly<BaseVM>(typeof(ControlTypesVM).Assembly);
      }

      [TestMethod]
      public void ControlTypesVM_TextBox()
      {
         var vmController = new VMController(TestResponse);
         vmController.OnRequestVM("conn1", typeof(ControlTypesVM).Name);

         var vm = GetVM<ControlTypesVM>();
         Assert.IsTrue(String.IsNullOrEmpty(vm.TextBoxValue));
         Assert.IsTrue(String.IsNullOrEmpty(vm.TextBoxResult));
         Assert.IsFalse(string.IsNullOrEmpty(vm.TextBoxPlaceHolder));

         var update = new Dictionary<string, object>() { { "TextBoxValue", "text box test" } };
         vmController.OnUpdateVM("conn1", typeof(ControlTypesVM).Name, update);

         Assert.IsTrue(VMData["TextBoxResult"].ToString().Contains("text box test"));
      }

      [TestMethod]
      public void ControlTypesVM_SearchBox()
      {
         var vmController = new VMController(TestResponse);
         vmController.OnRequestVM("conn1", typeof(ControlTypesVM).Name);

         var vm = GetVM<ControlTypesVM>();
         Assert.IsTrue(String.IsNullOrEmpty(vm.SearchBox));
         Assert.IsNotNull(vm.SearchResults);
         Assert.AreEqual(0, vm.SearchResults.Count());
         Assert.IsFalse(string.IsNullOrEmpty(vm.SearchBoxPlaceHolder));

         var update = new Dictionary<string, object>() { { "SearchBox", "m" } };
         vmController.OnUpdateVM("conn1", typeof(ControlTypesVM).Name, update);
         Assert.AreEqual(2, VMData["SearchResults"].ToList().Count);

         update = new Dictionary<string, object>() { { "SearchBox", "ma" } };
         vmController.OnUpdateVM("conn1", typeof(ControlTypesVM).Name, update);
         Assert.AreEqual("Mars", VMData["SearchResults"].ToList().First());
      }

      [TestMethod]
      public void ControlTypesVM_CheckBox()
      {
         var vmController = new VMController(TestResponse);
         vmController.OnRequestVM("conn1", typeof(ControlTypesVM).Name);

         var vm = GetVM<ControlTypesVM>();
         Assert.IsTrue(vm.CheckBoxResult.Contains("enabled"));
         Assert.IsTrue(vm.EnableMeCheckBox);
         Assert.IsTrue(vm.ShowMeCheckBox);

         var update = new Dictionary<string, object>() { { "EnableMeCheckBox", "false" } };
         vmController.OnUpdateVM("conn1", typeof(ControlTypesVM).Name, update);
         Assert.IsTrue(VMData["CheckBoxResult"].ToString().Contains("disabled"));

         update = new Dictionary<string, object>() { { "ShowMeCheckBox", "false" } };
         vmController.OnUpdateVM("conn1", typeof(ControlTypesVM).Name, update);
         Assert.IsFalse(String.IsNullOrEmpty(VMData["CheckBoxResult"].ToString()));
      }

      [TestMethod]
      public void ControlTypesVM_SimpleDropDownList()
      {
         var vmController = new VMController(TestResponse);
         vmController.OnRequestVM("conn1", typeof(ControlTypesVM).Name);

         var vm = GetVM<ControlTypesVM>();
         Assert.AreEqual(4, vm.SimpleDropDownOptions.Count);
         Assert.AreEqual("One", vm.SimpleDropDownOptions[0]);
         Assert.AreEqual("Two", vm.SimpleDropDownOptions[1]);
         Assert.AreEqual("Three", vm.SimpleDropDownOptions[2]);
         Assert.AreEqual("Four", vm.SimpleDropDownOptions[3]);

         var update = new Dictionary<string, object>() { { "SimpleDropDownValue", "Two" } };
         vmController.OnUpdateVM("conn1", typeof(ControlTypesVM).Name, update);
         Assert.IsTrue(VMData["SimpleDropDownResult"].ToString().Contains("Two"));
      }

      [TestMethod]
      public void ControlTypesVM_DropDownList()
      {
         var vmController = new VMController(TestResponse);
         vmController.OnRequestVM("conn1", typeof(ControlTypesVM).Name);

         var vm = GetVM<ControlTypesVM>();
         Assert.AreEqual(4, vm.DropDownOptions.Count);
         Assert.AreEqual(1, vm.DropDownOptions[0].Id);
         Assert.AreEqual(2, vm.DropDownOptions[1].Id);
         Assert.AreEqual(3, vm.DropDownOptions[2].Id);
         Assert.AreEqual(4, vm.DropDownOptions[3].Id);

         var update = new Dictionary<string, object>() { { "DropDownValue", 2 } };
         vmController.OnUpdateVM("conn1", typeof(ControlTypesVM).Name, update);
         Assert.IsTrue(VMData["DropDownResult"].ToString().Contains("Object Two"));
      }

      [TestMethod]
      public void ControlTypesVM_RadioButton()
      {
         var vmController = new VMController(TestResponse);
         vmController.OnRequestVM("conn1", typeof(ControlTypesVM).Name);

         var vm = GetVM<ControlTypesVM>();
         Assert.AreEqual("label-success", vm.RadioButtonStyle);
         Assert.AreEqual("green", vm.RadioButtonValue);

         var update = new Dictionary<string, object>() { { "RadioButtonValue", "yellow" } };
         vmController.OnUpdateVM("conn1", typeof(ControlTypesVM).Name, update);
         Assert.IsTrue(VMData["RadioButtonStyle"].ToString().Contains("label-warning"));
      }

      [TestMethod]
      public void ControlTypesVM_Button()
      {
         var vmController = new VMController(TestResponse);
         vmController.OnRequestVM("conn1", typeof(ControlTypesVM).Name);

         var vm = GetVM<ControlTypesVM>();
         Assert.IsFalse(vm.ButtonClicked);
         Assert.AreEqual(0, vm.ClickCount);

         var update = new Dictionary<string, object>() { { "ButtonClicked", "true" } };
         vmController.OnUpdateVM("conn1", typeof(ControlTypesVM).Name, update);
         Assert.AreEqual(1, VMData["ClickCount"]);

         vmController.OnUpdateVM("conn1", typeof(ControlTypesVM).Name, update);
         Assert.AreEqual(2, VMData["ClickCount"]);
      }
   }
}