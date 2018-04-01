using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetify;

namespace UnitTest
{
   [TestClass]
   public class VMControllerFactoryTest
   {
      [TestMethod]
      public void VMControllerFactory()
      {
         var id1 = "1";
         var id2 = "2";
         var factory = new VMControllerFactory(new MemoryCacheAdapter(), new ServiceScopeFactoryAdapter(null));
         factory.ResponseDelegate = (string connectionId, string vmId, string vmData) => { };

         Assert.IsNotNull(factory as IVMControllerFactory);

         var instance1 = factory.GetInstance(id1);
         var instance2 = factory.GetInstance(id2);
         var instance1Again = factory.GetInstance(id1);

         Assert.IsNotNull(instance1);
         Assert.IsNotNull(instance2);
         Assert.IsNotNull(instance1Again);
         Assert.AreNotEqual(instance1, instance2);
         Assert.AreEqual(instance1, instance1Again);

         Assert.IsTrue(factory.Remove(id1));
         Assert.IsFalse(factory.Remove(id1));

         var newInstance1 = factory.GetInstance(id1);
         Assert.AreNotEqual(instance1, newInstance1);
      }
   }
}