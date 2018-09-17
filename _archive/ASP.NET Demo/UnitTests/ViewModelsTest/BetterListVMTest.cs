using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetify;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ViewModels;

namespace UnitTest.ViewModelsTest
{
   [TestClass]
   public class BetterListVMTest
   {
      private string _connectionId;
      private string _vmId;
      private string _vmData;
      private TestEmployeeModel _model = new TestEmployeeModel();

      private JObject VMData => (JObject)JsonConvert.DeserializeObject(_vmData);

      private T GetVMProperty<T>(string propName) where T : class
      {
         return VMData[propName].ToObject(typeof(T)) as T;
      }

      private class TestEmployeeModel : EmployeeModel
      {
         public override List<EmployeeRecord> GetAllRecords()
         {
            if (_employeeRecords == null)
               _employeeRecords = new List<EmployeeRecord>
               {
                  new EmployeeRecord { Id = 1, FirstName = "John", LastName = "Doe" },
                  new EmployeeRecord { Id = 2, FirstName = "Mary", LastName = "Sue" },
                  new EmployeeRecord { Id = 3, FirstName = "Bob", LastName = "Smith" }
               };
            return _employeeRecords;
         }
      }

      public void TestResponse(string connectionId, string vmId, string vmData)
      {
         _connectionId = connectionId;
         _vmId = vmId;
         _vmData = vmData;
      }

      [TestInitialize]
      public void Initialize()
      {
         VMController.Register<BetterListVM>();

         var baseDelegate = VMController.CreateInstance;
         VMController.CreateInstance = (type, args) =>
         {
            if (type == typeof(BetterListVM))
               return new BetterListVM(_model);
            return baseDelegate(type, args);
         };
      }

      [TestMethod]
      public void BetterListVM_Create()
      {
         var vmController = new VMController(TestResponse);
         vmController.OnRequestVM("conn1", typeof(BetterListVM).Name);

         var update = new Dictionary<string, object>() { { "Add", "{ \"FirstName\": \"Peter\", \"LastName\": \"Chen\"}" } };
         vmController.OnUpdateVM("conn1", typeof(BetterListVM).Name, update);

         var employee = _model.GetAllRecords().Last();
         Assert.AreEqual("Peter", employee.FirstName);
         Assert.AreEqual("Chen", employee.LastName);
      }

      [TestMethod]
      public void BetterListVM_Read()
      {
         var vmController = new VMController(TestResponse);
         vmController.OnRequestVM("conn1", typeof(BetterListVM).Name);

         var vmEmployees = GetVMProperty<List<EmployeeRecord>>("Employees");
         Assert.IsNotNull(vmEmployees);
         Assert.AreEqual(3, vmEmployees.Count);
         Assert.AreEqual("John", vmEmployees[0].FirstName);
         Assert.AreEqual("Doe", vmEmployees[0].LastName);
         Assert.AreEqual("Mary", vmEmployees[1].FirstName);
         Assert.AreEqual("Sue", vmEmployees[1].LastName);
         Assert.AreEqual("Bob", vmEmployees[2].FirstName);
         Assert.AreEqual("Smith", vmEmployees[2].LastName);
      }

      [TestMethod]
      public void BetterListVM_Update()
      {
         var vmController = new VMController(TestResponse);
         vmController.OnRequestVM("conn1", typeof(BetterListVM).Name);

         var update = new Dictionary<string, object>() { { "Employees.$1.FirstName", "Teddy" } };
         vmController.OnUpdateVM("conn1", typeof(BetterListVM).Name, update);

         update = new Dictionary<string, object>() { { "Employees.$1.LastName", "Lee" } };
         vmController.OnUpdateVM("conn1", typeof(BetterListVM).Name, update);

         update = new Dictionary<string, object>() { { "Employees.$3.FirstName", "Beth" } };
         vmController.OnUpdateVM("conn1", typeof(BetterListVM).Name, update);

         update = new Dictionary<string, object>() { { "Employees.$3.LastName", "Larson" } };
         vmController.OnUpdateVM("conn1", typeof(BetterListVM).Name, update);

         var employee = _model.GetAllRecords().First();
         Assert.AreEqual("Teddy", employee.FirstName);
         Assert.AreEqual("Lee", employee.LastName);

         employee = _model.GetAllRecords().Last();
         Assert.AreEqual("Beth", employee.FirstName);
         Assert.AreEqual("Larson", employee.LastName);
      }

      [TestMethod]
      public void BetterListVM_Delete()
      {
         var vmController = new VMController(TestResponse);
         vmController.OnRequestVM("conn1", typeof(BetterListVM).Name);

         var employees = _model.GetAllRecords();
         Assert.AreEqual(3, employees.Count);
         Assert.IsTrue(employees.Exists(i => i.Id == 2));

         var update = new Dictionary<string, object>() { { "Remove", "2" } };
         vmController.OnUpdateVM("conn1", typeof(BetterListVM).Name, update);

         Assert.AreEqual(2, employees.Count);
         Assert.IsFalse(employees.Exists(i => i.Id == 2));
      }
   }
}