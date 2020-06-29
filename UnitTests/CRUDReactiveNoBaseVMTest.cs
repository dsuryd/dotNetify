using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using DotNetify;
using DotNetify.Elements;
using DotNetify.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
   [TestClass]
   public class CrudReactiveNoBaseVMTest
   {
      private EmployeeService _employeeService = new EmployeeService();

      private class EmployeeRecord
      {
         public int Id { get; set; }
         public string FirstName { get; set; }
         public string LastName { get; set; }
      }

      private class ClientState
      {
         public List<CrudVMNoBaseVM.EmployeeInfo> Employees { get; set; }
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

      private class CrudVMNoBaseVM : INotifyPropertyChanged, IReactiveProperties, IBaseVMAccessor
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

         public CrudVMNoBaseVM(EmployeeService employeeService)
         {
            _employeeService = employeeService;

            OnInitialized = baseVM =>
            {
               AddList = (propName, value) => baseVM.AddList(propName, value);
               UpdateList = (propName, value) => baseVM.UpdateList(propName, value);
               RemoveList = (propName, value) => baseVM.RemoveList(propName, value);
            };

            this.AddProperty<IEnumerable<EmployeeInfo>>("Employees")
               .WithItemKey(nameof(EmployeeInfo.Id))
               .SubscribeTo(
                  Observable.Return(_employeeService
                     .GetAll()
                     .Select(i => new EmployeeInfo
                     {
                        Id = i.Id,
                        FirstName = i.FirstName,
                        LastName = i.LastName
                     }
                  )
               )
            );

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

      private HubEmulator _hubEmulator;

      [TestInitialize]
      public void Initialize()
      {
         _hubEmulator = new HubEmulatorBuilder()
            .AddServices(services => services.AddTransient(_ => _employeeService))
            .Register<CrudVMNoBaseVM>()
            .Build();
      }

      [TestMethod]
      public void CrudVMNoBaseVM_Create()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(CrudVMNoBaseVM));

         client.Dispatch(new Dictionary<string, object>() { { "Add", "Peter Chen" } });

         var employee = _employeeService.GetAll().Last();
         Assert.AreEqual("Peter", employee.FirstName);
         Assert.AreEqual("Chen", employee.LastName);
      }

      [TestMethod]
      public void CrudVMNoBaseVM_Read()
      {
         var client = _hubEmulator.CreateClient();
         var response = client.Connect(nameof(CrudVMNoBaseVM)).As<ClientState>();
         var vmEmployees = response.Employees;

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
      public void CrudVMNoBaseVM_Update()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(CrudVMNoBaseVM));

         client.Dispatch(new Dictionary<string, object>() { { "Update", "{ Id: 1, FirstName: 'Teddy' }" } });
         client.Dispatch(new Dictionary<string, object>() { { "Update", "{ Id: 1, LastName: 'Lee' }" } });

         client.Dispatch(new Dictionary<string, object>() { { "Update", "{ Id: 3, FirstName: 'Beth' }" } });
         client.Dispatch(new Dictionary<string, object>() { { "Update", "{ Id: 3, LastName: 'Larson' }" } });

         var employee = _employeeService.GetById(1);
         Assert.AreEqual("Teddy", employee.FirstName);
         Assert.AreEqual("Lee", employee.LastName);

         employee = _employeeService.GetById(3);
         Assert.AreEqual("Beth", employee.FirstName);
         Assert.AreEqual("Larson", employee.LastName);
      }

      [TestMethod]
      public void CrudVMNoBaseVM_Delete()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(CrudVMNoBaseVM));

         var employees = _employeeService.GetAll();
         Assert.AreEqual(3, employees.Count);
         Assert.IsTrue(employees.Exists(i => i.Id == 2));

         var response = client.Dispatch(new Dictionary<string, object>() { { "Remove", "2" } });

         employees = _employeeService.GetAll();
         Assert.AreEqual(2, employees.Count);
         Assert.IsFalse(employees.Exists(i => i.Id == 2));
      }

      [TestMethod]
      public void CrudVMNoBaseVM_ShowNotification()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(CrudVMNoBaseVM));

         var employees = _employeeService.GetAll();
         Assert.AreEqual(3, employees.Count);

         var response = client.Dispatch(new Dictionary<string, object>() { { "Remove", "2" } }).As<dynamic>();
         Assert.AreEqual(true, (bool) response.ShowNotification);

         var response2 = client.Dispatch(new Dictionary<string, object>() { { "Update", "{ Id: 1, LastName: 'Lee' }" } });
         Assert.AreEqual(0, response2.Count);

         var response3 = client.Dispatch(new Dictionary<string, object>() { { "Remove", "1" } }).As<dynamic>();
         Assert.AreEqual(true, (bool) response3.ShowNotification);
      }
   }
}