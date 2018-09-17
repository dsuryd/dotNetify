using DotNetify;
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

      [TestMethod]
      public void SerializerVM_Serialize()
      {
         var vmController = new MockVMController<SerializerVM>();
         var response = vmController.RequestVM();

         Assert.AreEqual("Hello", response.GetVMProperty<string>("FirstName"));
         Assert.AreEqual("World", response.GetVMProperty<string>("LastName"));
         Assert.AreEqual("Hello World", response.GetVMProperty<string>("FullName"));
      }

      [TestMethod]
      public void SerializerVM_Deserialize()
      {
         var vmController = new MockVMController<SerializerVM>();
         vmController.RequestVM();

         var response = vmController.UpdateVM(new Dictionary<string, object>() { { "FirstName", "John" } });
         Assert.AreEqual("John World", response["FullName"]);

         response = vmController.UpdateVM(new Dictionary<string, object>() { { "LastName", "Hancock" } });
         Assert.AreEqual("John Hancock", response["FullName"]);
      }
   }
}
