using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetify;
using ViewModels;

namespace UnitTest.ViewModelsTest
{
   [TestClass]
   public class HelloWorldVMTest
   {
      [TestMethod]
      public void HelloWorldVM()
      {
         var vm = VMController.CreateInstance(typeof(HelloWorldVM), null) as HelloWorldVM;
         Assert.IsNotNull(vm);

         Assert.AreEqual("Hello", vm.FirstName);
         Assert.AreEqual("World", vm.LastName);
         Assert.AreEqual("Hello World", vm.FullName);

         vm.FirstName = "John";
         Assert.AreEqual("John World", vm.FullName);

         vm.LastName = "Doe";
         Assert.AreEqual("John Doe", vm.FullName);
      }
   }
}
