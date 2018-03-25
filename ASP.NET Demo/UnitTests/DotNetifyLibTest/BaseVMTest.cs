using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetify;

namespace UnitTest
{
   [TestClass]
   public class BaseVMTest
   {
      private class TestVM : BaseVM
      {
         public string String
         {
            get { return Get<string>(); }
            set { Set(value); }
         }

         public int Int
         {
            get { return Get<int>(); }
            set { Set(value); }
         }

         public double Double
         {
            get { return Get<double>(); }
            set { Set(value); }
         }

         public bool Bool
         {
            get { return Get<bool>(); }
            set { Set(value); }
         }

         [DotNetify.Ignore]
         public string IgnoreMe { get; set; }

         public override BaseVM GetSubVM(string vmTypeName)
         {
            return new TestVM { String = vmTypeName };
         }

         public override BaseVM GetSubVM(string vmTypeName, string vmInstanceId)
         {
            return new TestVM { String = vmTypeName + vmInstanceId };
         }
      }

      [TestMethod]
      public void BaseVM_AcceptChangedProperties()
      {
         var vm = new TestVM();

         vm.String = "MyString";
         Assert.IsNotNull(vm.ChangedProperties);
         Assert.AreEqual(1, vm.ChangedProperties.Count);
         Assert.IsTrue(vm.ChangedProperties.ContainsKey("String"));
         Assert.AreEqual("MyString", vm.ChangedProperties["String"]);

         vm.AcceptChangedProperties();

         vm.Int = 13;
         vm.Double = 3.1456;
         Assert.IsNotNull(vm.ChangedProperties);
         Assert.AreEqual(2, vm.ChangedProperties.Count);
         Assert.IsTrue(vm.ChangedProperties.ContainsKey("Int"));
         Assert.AreEqual(13, vm.ChangedProperties["Int"]);
         Assert.IsTrue(vm.ChangedProperties.ContainsKey("Double"));
         Assert.AreEqual(3.1456, vm.ChangedProperties["Double"]);

         vm.AcceptChangedProperties();

         Assert.IsNotNull(vm.ChangedProperties);
         Assert.AreEqual(0, vm.ChangedProperties.Count);
      }

      [TestMethod]
      public void BaseVM_IgnoredProperties()
      {
         var vm = new TestVM();

         Assert.IsNotNull(vm.IgnoredProperties);
         Assert.IsTrue(vm.IgnoredProperties.Contains("IgnoreMe"));

         Assert.IsFalse(vm.IgnoredProperties.Contains("Bool"));
         vm.Ignore(() => vm.Bool);
         Assert.IsTrue(vm.IgnoredProperties.Contains("Bool"));

         Assert.IsFalse(vm.IgnoredProperties.Contains(nameof(TestVM.Double)));
         vm.Ignore(nameof(TestVM.Double));
         Assert.IsTrue(vm.IgnoredProperties.Contains(nameof(TestVM.Double)));
      }

      [TestMethod]
      public void BaseVM_PushUpdates()
      {
         var vm = new TestVM();

         bool request = false;
         vm.String = "Hello";
         vm.RequestPushUpdates += (sender, e) => request = true;
         vm.PushUpdates();

         Assert.IsTrue(request);
      }

      [TestMethod]
      public void BaseVM_GetSubVM()
      {
         var vm = new TestVM();

         var test1 = vm.GetSubVM(typeof(TestVM).Name) as TestVM;
         Assert.IsNotNull(test1);
         Assert.AreEqual("TestVM", test1.String);

         var test2 = vm.GetSubVM(typeof(TestVM).Name, "1") as TestVM;
         Assert.IsNotNull(test2);
         Assert.AreEqual("TestVM1", test2.String);
      }
   }
}