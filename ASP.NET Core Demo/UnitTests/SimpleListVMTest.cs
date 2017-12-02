using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using DotNetify;

namespace UnitTests
{
   [TestClass]
   public class SimpleListVMTest
   {
      private Response _response = new Response();
      private EmployeeService _employeeService = new EmployeeService();
      private SimpleListVM _simpleListVM;

      private class EmployeeRecord
      {
         public int Id { get; set; }
         public string FirstName { get; set; }
         public string LastName { get; set; }
      }

      private class EmployeeService
      {
         private Dictionary<int, EmployeeRecord> _employeeRecords;
         private int _nextId = 100;

         public List<EmployeeRecord> GetAll()
         {
            if (_employeeRecords == null)
               _employeeRecords = new Dictionary<int, EmployeeRecord>
               {
                  { 1, new EmployeeRecord { Id = 1, FirstName = "John", LastName = "Doe" } },
                  { 2,  new EmployeeRecord { Id = 2, FirstName = "Mary", LastName = "Sue" } },
                  { 3,  new EmployeeRecord { Id = 3, FirstName = "Bob", LastName = "Smith" } }
               };
            return _employeeRecords.Values.ToList();
         }

         public EmployeeRecord GetById(int id) => _employeeRecords[id];

         public int Add(EmployeeRecord record)
         {
            record.Id = _nextId++;
            _employeeRecords.Add(record.Id, record);
            return record.Id;
         }

         public void Update(EmployeeRecord record) => _employeeRecords[record.Id] = record;
         public void Delete(int id) => _employeeRecords.Remove(id);
      }

      private class SimpleListVM : BaseVM
      {
         private readonly EmployeeService _employeeService;

         public class EmployeeInfo
         {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
         }

         public IEnumerable<EmployeeInfo> Employees => _employeeService.GetAll().Select(i => new EmployeeInfo
         {
            Id = i.Id,
            FirstName = i.FirstName,
            LastName = i.LastName
         });

         public string Employees_itemKey => nameof(EmployeeInfo.Id);

         public Action<string> Add => fullName =>
         {
            var names = fullName.Split(new char[] { ' ' }, 2);
            var newRecord = new EmployeeRecord
            {
               FirstName = names.First(),
               LastName = names.Length > 1 ? names.Last() : ""
            };

            this.AddList(nameof(Employees), new EmployeeInfo
            {
               Id = _employeeService.Add(newRecord),
               FirstName = newRecord.FirstName,
               LastName = newRecord.LastName
            });
         };

         public Action<EmployeeInfo> Update => changes =>
         {
            var record = _employeeService.GetById(changes.Id);
            if (record != null)
            {
               record.FirstName = changes.FirstName ?? record.FirstName;
               record.LastName = changes.LastName ?? record.LastName;
               _employeeService.Update(record);
}
         };

         public Action<int> Remove => id =>
         {
            _employeeService.Delete(id);
            this.RemoveList(nameof(Employees), id);
            ShowNotification = true;
         };

         private bool _showNotification;
         public bool ShowNotification
         {
            get
            {
               var value = _showNotification;
               _showNotification = false;
               return value;
            }
            set
            {
               _showNotification = value;
               Changed(nameof(ShowNotification));
            }
         }

         public SimpleListVM(EmployeeService employeeService)
         {
            _employeeService = employeeService;
         }
      }

      [TestInitialize]
      public void Initialize()
      {
         _simpleListVM = new SimpleListVM(_employeeService);
      }

      [TestMethod]
      public void SimpleListVM_Create()
      {
         var vmController = new MockVMController<SimpleListVM>(_simpleListVM);
         vmController.RequestVM();

         vmController.UpdateVM(new Dictionary<string, object>() { { "Add", "Peter Chen" } });

         var employee = _employeeService.GetAll().Last();
         Assert.AreEqual("Peter", employee.FirstName);
         Assert.AreEqual("Chen", employee.LastName);
      }

      [TestMethod]
      public void SimpleListVM_Read()
      {
         var vmController = new MockVMController<SimpleListVM>(_simpleListVM);
         var vmEmployees = vmController
            .RequestVM()
            .GetVMProperty<List<EmployeeRecord>>("Employees");

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
      public void SimpleListVM_Update()
      {
         var vmController = new MockVMController<SimpleListVM>(_simpleListVM);
         var vmEmployees = vmController.RequestVM();

         vmController.UpdateVM(new Dictionary<string, object>() { { "Update", "{ Id: 1, FirstName: 'Teddy' }" } });
         vmController.UpdateVM(new Dictionary<string, object>() { { "Update", "{ Id: 1, LastName: 'Lee' }" } });

         vmController.UpdateVM(new Dictionary<string, object>() { { "Update", "{ Id: 3, FirstName: 'Beth' }" } });
         vmController.UpdateVM(new Dictionary<string, object>() { { "Update", "{ Id: 3, LastName: 'Larson' }" } });

         var employee = _employeeService.GetById(1);
         Assert.AreEqual("Teddy", employee.FirstName);
         Assert.AreEqual("Lee", employee.LastName);

         employee = _employeeService.GetById(3);
         Assert.AreEqual("Beth", employee.FirstName);
         Assert.AreEqual("Larson", employee.LastName);
      }

      [TestMethod]
      public void SimpleListVM_Delete()
      {
         var vmController = new MockVMController<SimpleListVM>(_simpleListVM);
         var vmEmployees = vmController.RequestVM();

         var employees = _employeeService.GetAll();
         Assert.AreEqual(3, employees.Count);
         Assert.IsTrue(employees.Exists(i => i.Id == 2));

         vmController.UpdateVM(new Dictionary<string, object>() { { "Remove", "2" } });

         employees = _employeeService.GetAll();
         Assert.AreEqual(2, employees.Count);
         Assert.IsFalse(employees.Exists(i => i.Id == 2));
      }

      [TestMethod]
      public void SimpleListVM_ShowNotification()
      {
         var vmController = new MockVMController<SimpleListVM>(_simpleListVM);
         var vmEmployees = vmController.RequestVM();

         var employees = _employeeService.GetAll();
         Assert.AreEqual(3, employees.Count);

         var response = vmController.UpdateVM(new Dictionary<string, object>() { { "Remove", "2" } });
         Assert.AreEqual(true, response["ShowNotification"]);

         var response2 = vmController.UpdateVM(new Dictionary<string, object>() { { "Update", "{ Id: 1, LastName: 'Lee' }" } });
         Assert.AreEqual(null, response2);

         var response3 = vmController.UpdateVM(new Dictionary<string, object>() { { "Remove", "1" } });
         Assert.AreEqual(true, response3["ShowNotification"]);
      }
   }
}
