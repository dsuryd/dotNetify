using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetify;

namespace UnitTest
{
   [TestClass]
   public class ObservableTest
   {
      public class TestObject
      {
         public string String { get; set; }
         public int Int { get; set; }
      }

      private class TestObservable : ObservableObject
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

         public TestObject Object
         {
            get { return Get<TestObject>(); }
            set { Set(value); }
         }
      }

      [TestMethod]
      public void Observable_PropertyChanged()
      {
         var vm = new TestObservable();

         string propertyChanged = null;
         vm.PropertyChanged += (sender, e) => propertyChanged = e.PropertyName;

         vm.String = "MyString";
         Assert.AreEqual("MyString", vm.String);
         Assert.AreEqual("String", propertyChanged);

         vm.Int = 42;
         Assert.AreEqual(42, vm.Int);
         Assert.AreEqual("Int", propertyChanged);

         vm.Double = 3.1456;
         Assert.AreEqual(3.1456, vm.Double);
         Assert.AreEqual("Double", propertyChanged);

         vm.Bool = true;
         Assert.IsTrue(vm.Bool);
         Assert.AreEqual("Bool", propertyChanged);

         vm.Object = new TestObject { String = "TestString", Int = 13 };
         Assert.IsNotNull(vm.Object);
         Assert.AreEqual("TestString", vm.Object.String);
         Assert.AreEqual(13, vm.Object.Int);
         Assert.AreEqual("Object", propertyChanged);
      }

      [TestMethod]
      public void Observable_Dispose()
      {
         var vm = new TestObservable();

         bool disposed = false;
         vm.Disposed += (sender, e) => disposed = true;

         vm.Dispose();
         Assert.IsTrue(disposed);
      }
   }
}
