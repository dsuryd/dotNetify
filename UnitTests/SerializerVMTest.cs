using DotNetify;
using DotNetify.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UnitTests
{
   [TestClass]
   public class SerializerVMTest
   {
      private class SerializerVM : INotifyPropertyChanged, ISerializer, IDeserializer
      {
         private string _firstName = "Hello";
         private string _lastName = "World";

         public string FullName => $"{_firstName} {_lastName}";

         [DotNetify.Ignore]
         public string IgnoreMe { get; set; }

         public event PropertyChangedEventHandler PropertyChanged = delegate { };

         public string Serialize(object instance, List<string> ignoredPropertyNames)
         {
            if (ignoredPropertyNames != null && ignoredPropertyNames.Contains("IgnoreMe") == false)
               throw new Exception();

            if (instance == this)
               return "{'FirstName': '" + _firstName + "', 'LastName': '" + _lastName + "', 'FullName': '" + FullName + "'}";
            else
               return JsonConvert.SerializeObject(instance);
         }

         public bool Deserialize(object instance, string propertyPath, string newValue)
         {
            if (instance == this)
            {
               if (propertyPath == "FirstName")
                  _firstName = newValue;
               else if (propertyPath == "LastName")
                  _lastName = newValue;

               this.Changed(nameof(FullName));
            }
            return true;
         }
      }

      private HubEmulator _hubEmulator;

      [TestInitialize]
      public void Initialize()
      {
         _hubEmulator = new HubEmulatorBuilder()
            .Register<SerializerVM>()
            .Build();
      }

      [TestMethod]
      public void SerializerVM_Serialize()
      {
         var client = _hubEmulator.CreateClient();
         var response = client.Connect(nameof(SerializerVM)).As<dynamic>();

         Assert.AreEqual("Hello", (string) response.FirstName);
         Assert.AreEqual("World", (string) response.LastName);
         Assert.AreEqual("Hello World", (string) response.FullName);
      }

      [TestMethod]
      public void SerializerVM_Deserialize()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(SerializerVM));

         var response = client.Dispatch(new Dictionary<string, object>() { { "FirstName", "John" } }).As<dynamic>();
         Assert.AreEqual("John World", (string) response.FullName);

         response = client.Dispatch(new Dictionary<string, object>() { { "LastName", "Hancock" } }).As<dynamic>();
         Assert.AreEqual("John Hancock", (string) response.FullName);
      }
   }
}