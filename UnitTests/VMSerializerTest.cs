using System;
using System.Collections.Generic;
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
         Assert.IsNull(resolver.IgnoredPropertyNames);

         var serializer = new VMSerializer();
         var ignoredPropertyNames = new List<String> {"World"};
         serializer.Serialize(new {Hello = "Hello", World = "World"}, ignoredPropertyNames);

         Assert.AreSame(ignoredPropertyNames, resolver.IgnoredPropertyNames);
      }
   }
}
