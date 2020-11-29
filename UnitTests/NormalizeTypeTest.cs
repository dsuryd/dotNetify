using DotNetify;
using DotNetify.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace UnitTests
{
   [TestClass]
   public class NormalizeTypeTest
   {
      [TestMethod]
      public void NormalizeType_ValueKindObject()
      {
         var jElement = JsonSerializer.Deserialize<object>("{\"firstName\": \"John\", \"lastName\": \"Doe\", \"age\": 24 }");
         var jObject = (JObject) jElement.NormalizeType();

         Assert.AreEqual("John", jObject["firstName"]);
         Assert.AreEqual("Doe", jObject["lastName"]);
         Assert.AreEqual(24, jObject["age"]);
      }

      [TestMethod]
      public void NormalizeType_ValueKindArray()
      {
         var jElement = JsonSerializer.Deserialize<object>("[{\"firstName\": \"John\", \"lastName\": \"Doe\", \"age\": 24 }]");
         var jArray = (JArray) jElement.NormalizeType();

         Assert.AreEqual("John", jArray[0]["firstName"]);
         Assert.AreEqual("Doe", jArray[0]["lastName"]);
         Assert.AreEqual(24, jArray[0]["age"]);
      }

      [TestMethod]
      public void NormalizeType_ValueKindString()
      {
         var jElement = JsonSerializer.Deserialize<string>("\"abc1234xyz\"");
         var value = jElement.NormalizeType();

         Assert.AreEqual("abc1234xyz", value);
      }

      [TestMethod]
      public void NormalizeType_ValueKindNumberInt()
      {
         var jElement = JsonSerializer.Deserialize<int>("1991");
         var value = jElement.NormalizeType();

         Assert.AreEqual(1991, value);
      }

      [TestMethod]
      public void NormalizeType_ValueKindNumberDouble()
      {
         var jElement = JsonSerializer.Deserialize<double>("3.14159265359");
         var value = jElement.NormalizeType();

         Assert.AreEqual(3.14159265359, value);
      }

      [TestMethod]
      public void NormalizeType_ValueKindTrue()
      {
         var jElement = JsonSerializer.Deserialize<bool>("true");
         var value = (bool) jElement.NormalizeType();

         Assert.IsTrue(value);
      }

      [TestMethod]
      public void NormalizeType_ValueKindFalse()
      {
         var jElement = JsonSerializer.Deserialize<bool>("false");
         var value = (bool) jElement.NormalizeType();

         Assert.IsFalse(value);
      }

      [TestMethod]
      public void NormalizeType_ValueKindNull()
      {
         var jElement = JsonSerializer.Deserialize<object>("null");
         var value = jElement.NormalizeType();

         Assert.IsNull(value);
      }

      [TestMethod]
      public void NormalizeType_Dictionary()
      {
         var vmData = JsonSerializer.Deserialize<Dictionary<string, object>>("{\"Key1\": \"\", \"Key2\": {\"Name\": \"John\"}, \"Key3\": 99}");
         var data = vmData?.ToDictionary(x => x.Key, x => x.Value.NormalizeType());

         Assert.AreEqual("", data["Key1"]);
         Assert.AreEqual("John", (data["Key2"] as JObject)["Name"]);
         Assert.AreEqual((long) 99, data["Key3"]);
      }
   }
}