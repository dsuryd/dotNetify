using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetify;
using System.Collections.Generic;

namespace UnitTests
{
   [TestClass]
   public class VMControllerFactoryTest
   {
      private class MemoryCache : IMemoryCache
      {
         private Dictionary<string, object> _cache = new Dictionary<string, object>();

         public object Get(string key) => _cache.ContainsKey(key) ? _cache[key] : null;

         public void Remove(string key) => _cache.Remove(key);

         public void Set<T>(string key, T cachedValue, MemoryCacheEntryOptions options = null) where T : class => _cache[key] = cachedValue;

         public bool TryGetValue<T>(string key, out T cachedValue) where T : class
         {
            cachedValue = (T)Get(key);
            return cachedValue != null;
         }
      }

      private class ServiceScopeFactory : IVMServiceScopeFactory
      {
         public IVMServiceScope CreateScope() => null;
      }

      [TestMethod]
      public void VMControllerFactory()
      {
         var id1 = "1";
         var id2 = "2";
         var memoryCache = new MemoryCache();
         var factory = new VMControllerFactory(memoryCache, new VMFactory(memoryCache, new VMTypesAccessor()), new ServiceScopeFactory());
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