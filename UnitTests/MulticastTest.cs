using DotNetify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTests
{
   [TestClass]
   public class MulticastTest
   {
      private class MulticastVM : BaseVM, IMulticast
      {
         public List<string> Messages { get; } = new List<string>();
      }

      [TestMethod]
      public void MulticastVM_Test()
      {
         var vmController = new MockVMController<MulticastVM>();
         var response = vmController.RequestVM();

         //Assert.AreEqual("Hello", response.GetVMProperty<string>("FirstName"));
      }
   }
}