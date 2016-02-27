using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetify;
using ViewModels;

namespace UnitTest.ViewModelsTest
{
   [TestClass]
   public class ControlTypesVMTest
   {
      [TestMethod]
      public void ControlTypesVM_TextBox()
      {
         var vm = VMController.CreateInstance(typeof(ControlTypesVM), null) as ControlTypesVM;
         Assert.IsNotNull(vm);

         Assert.IsTrue(String.IsNullOrEmpty(vm.TextBoxValue));
         Assert.IsTrue(String.IsNullOrEmpty(vm.TextBoxResult));
         Assert.IsFalse(string.IsNullOrEmpty(vm.TextBoxPlaceHolder));

         vm.TextBoxValue = "text box test";
         Assert.IsTrue(vm.TextBoxResult.Contains("text box test"));
      }

      [TestMethod]
      public void ControlTypesVM_SearchBox()
      {
         var vm = VMController.CreateInstance(typeof(ControlTypesVM), null) as ControlTypesVM;
         Assert.IsNotNull(vm);

         Assert.IsTrue(String.IsNullOrEmpty(vm.SearchBox));
         Assert.IsNotNull(vm.SearchResults);
         Assert.AreEqual(0, vm.SearchResults.Count());
         Assert.IsFalse(string.IsNullOrEmpty(vm.SearchBoxPlaceHolder));

         vm.SearchBox = "m";
         Assert.AreEqual(2, vm.SearchResults.Count());

         vm.SearchBox = "ma";
         Assert.AreEqual(1, vm.SearchResults.Count());
         Assert.AreEqual("Mars", vm.SearchResults.First());
      }

      [TestMethod]
      public void ControlTypesVM_CheckBox()
      {
         var vm = VMController.CreateInstance(typeof(ControlTypesVM), null) as ControlTypesVM;
         Assert.IsNotNull(vm);

         Assert.IsTrue(vm.CheckBoxResult.Contains("enabled"));
         Assert.IsTrue(vm.EnableMeCheckBox);
         Assert.IsTrue(vm.ShowMeCheckBox);

         vm.EnableMeCheckBox = false;
         Assert.IsFalse(vm.EnableMeCheckBox);
         Assert.IsTrue(vm.CheckBoxResult.Contains("disabled"));

         vm.ShowMeCheckBox = false;
         Assert.IsFalse(vm.EnableMeCheckBox);
      }
   }
}
