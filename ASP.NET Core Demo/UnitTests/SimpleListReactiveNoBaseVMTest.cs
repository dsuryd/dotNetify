using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DotNetify;
using System.ComponentModel;

namespace UnitTests
{
   [TestClass]
   public class SimpleListReactiveNoBaseVMTest
   {
      private Response _response = new Response();
      private EmployeeService _employeeService = new EmployeeService();
      private SimpleListNoBaseVM _simpleListVM;

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

      private class SimpleListNoBaseVM : INotifyPropertyChanged, IReactiveProperties, IBaseVMAccessor
      {
         private readonly EmployeeService _employeeService;

         public class EmployeeInfo
         {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
         }

         public Action<BaseVM> OnInitialized { get; }
         public IList<IReactiveProperty> RuntimeProperties { get; } = new List<IReactiveProperty>();
         public event PropertyChangedEventHandler PropertyChanged = delegate { };

         private Action<string, object> AddList { get; set; }
         private Action<string, object> UpdateList { get; set; }
         private Action<string, object> RemoveList { get; set; }

         public SimpleListNoBaseVM(EmployeeService employeeService)
         {
            _employeeService = employeeService;

            OnInitialized = baseVM =>
            {
               AddList = (propName, value) => baseVM.AddList(propName, value);
               UpdateList = (propName, value) => baseVM.UpdateList(propName, value);
               RemoveList = (propName, value) => baseVM.RemoveList(propName, value);
            };

            this.AddProperty<IEnumerable<EmployeeInfo>>("Employees").SubscribeTo(
               Observable.Return(_employeeService
                  .GetAll()
                  .Select(i => new EmployeeInfo
                  {
                     Id = i.Id,
                     FirstName = i.FirstName,
                     LastName = i.LastName
                  }))
            );

            this.AddProperty<string>("Employees_itemKey").SubscribeTo(Observable.Return(nameof(EmployeeInfo.Id)));

            this.AddProperty<string>("Add").Subscribe(fullName =>
            {
               var names = fullName.Split(new char[] { ' ' }, 2);
               var newRecord = new EmployeeRecord
               {
                  FirstName = names.First(),
                  LastName = names.Length > 1 ? names.Last() : ""
               };

               AddList("Employees", new EmployeeInfo
               {
                  Id = _employeeService.Add(newRecord),
                  FirstName = newRecord.FirstName,
                  LastName = newRecord.LastName
               });
            });

            this.AddProperty<EmployeeInfo>("Update").Subscribe(changes =>
            {
               var record = _employeeService.GetById(changes.Id);
               if (record != null)
               {
                  record.FirstName = changes.FirstName ?? record.FirstName;
                  record.LastName = changes.LastName ?? record.LastName;
                  _employeeService.Update(record);
               }
            });

            var removeCommand = this.AddProperty<int>("Remove");
            removeCommand.Subscribe(id =>
            {
               _employeeService.Delete(id);
               RemoveList("Employees", id);
            });

            this.AddProperty<bool>("ShowNotification").SubscribeTo(removeCommand.Select(_ => true));
         }
      }

      [TestInitialize]
      public void Initialize()
      {
         _simpleListVM = new SimpleListNoBaseVM(_employeeService);
      }

      [TestMethod]
      public void SimpleListNoBaseVM_Create()
      {
         var vmController = new MockVMController<SimpleListNoBaseVM>(_simpleListVM);
         vmController.RequestVM();

         vmController.UpdateVM(new Dictionary<string, object>() { { "Add", "Peter Chen" } });

         var employee = _employeeService.GetAll().Last();
         Assert.AreEqual("Peter", employee.FirstName);
         Assert.AreEqual("Chen", employee.LastName);
      }

      [TestMethod]
      public void SimpleListNoBaseVM_Read()
      {
         var vmController = new MockVMController<SimpleListNoBaseVM>(_simpleListVM);
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
      public void SimpleListNoBaseVM_Update()
      {
         var vmController = new MockVMController<SimpleListNoBaseVM>(_simpleListVM);
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
      public void SimpleListNoBaseVM_Delete()
      {
         var vmController = new MockVMController<SimpleListNoBaseVM>(_simpleListVM);
         var vmEmployees = vmController.RequestVM();

         var employees = _employeeService.GetAll();
         Assert.AreEqual(3, employees.Count);
         Assert.IsTrue(employees.Exists(i => i.Id == 2));

         var response = vmController.UpdateVM(new Dictionary<string, object>() { { "Remove", "2" } });

         employees = _employeeService.GetAll();
         Assert.AreEqual(2, employees.Count);
         Assert.IsFalse(employees.Exists(i => i.Id == 2));
      }

      [TestMethod]
      public void SimpleListNoBaseVM_ShowNotification()
      {
         var vmController = new MockVMController<SimpleListNoBaseVM>(_simpleListVM);
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
