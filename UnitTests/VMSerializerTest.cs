using System.Collections.Generic;
using System.Linq;
using DotNetify;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
   [TestClass]
   public class VMSerializerTest
   {
      [TestMethod]
      public void SerializerSettings_UseVMContractResolver_ByDefault()
      {
         Assert.AreSame(VMSerializer.SerializerSettings.ContractResolver.GetType(), typeof(VMContractResolver));
      }

      [TestMethod]
      public void Serialize_SetsIgnoredPropertyNames()
      {
         var resolver = VMSerializer.SerializerSettings.ContractResolver as VMContractResolver;
         Assert.IsNotNull(resolver);

         var serializer = new VMSerializer();
         var ignoredPropertyNames = new List<string> { "World" };
         serializer.Serialize(new { Hello = "Hello", World = "World" }, ignoredPropertyNames);

         Assert.IsTrue(ignoredPropertyNames.All(name => resolver.IgnoredPropertyNames.Contains(name)));
      }
   }
}