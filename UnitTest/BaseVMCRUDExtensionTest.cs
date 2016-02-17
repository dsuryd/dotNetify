using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetify;

namespace UnitTest
{
   [TestClass]
   public class BaseVMCRUDExtensionTest
   {
      public class TestObject
      {
         public int Id { get; set; }
         public string Name { get; set; }
      }

      private class TestVM : BaseVM
      {
         public List<TestObject> Sequence
         {
            get { return Get<List<TestObject>>(); }
            set { Set(value); }
         }

         public TestVM()
         {
            Sequence = new List<TestObject> {
                    new TestObject { Id = 1, Name = "One" },
                    new TestObject { Id = 2, Name = "" },
                    new TestObject { Id = 3, Name = "Three" }
                };
         }
      }

      [TestMethod]
      public void BaseVM_CRUD()
      {
         var vm = new TestVM();

         vm.AddList(() => vm.Sequence, new TestObject { Id = 4, Name = "Four" });
         Assert.IsNotNull(vm.ChangedProperties);
         Assert.IsTrue(vm.ChangedProperties.ContainsKey("Sequence_add"));
         Assert.IsNotNull(vm.ChangedProperties["Sequence_add"] as TestObject);
         Assert.AreEqual(4, (vm.ChangedProperties["Sequence_add"] as TestObject).Id);
         Assert.AreEqual("Four", (vm.ChangedProperties["Sequence_add"] as TestObject).Name);

         vm.UpdateList(() => vm.Sequence, new TestObject { Id = 2, Name = "Two" });
         Assert.IsNotNull(vm.ChangedProperties);
         Assert.IsTrue(vm.ChangedProperties.ContainsKey("Sequence_update"));
         Assert.IsNotNull(vm.ChangedProperties["Sequence_update"] as TestObject);
         Assert.AreEqual(2, (vm.ChangedProperties["Sequence_update"] as TestObject).Id);
         Assert.AreEqual("Two", (vm.ChangedProperties["Sequence_update"] as TestObject).Name);

         vm.RemoveList(() => vm.Sequence, 3);
         Assert.IsNotNull(vm.ChangedProperties);
         Assert.IsTrue(vm.ChangedProperties.ContainsKey("Sequence_remove"));
         Assert.AreEqual(3, vm.ChangedProperties["Sequence_remove"]);
      }
   }
}
