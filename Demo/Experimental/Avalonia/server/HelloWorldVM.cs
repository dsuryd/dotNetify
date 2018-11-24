using System;
using DotNetify;
using System.Threading;
using System.Collections.Generic;
using Bogus;
using System.Linq;

namespace HelloWorld.Server
{
   public class HelloWorldVM : MulticastVM
   {
      private Timer _timer;
      private int _id;
      private List<EmployeeInfo> _employees;

      public string Greetings
      {
         get => Get<string>();
         set
         {
            Set(value);
            PushUpdates();
         }
      }

      public DateTime ServerTime => DateTime.Now;

      public IEnumerable<EmployeeInfo> Employees => _employees;

      public HelloWorldVM()
      {
         _employees = new Faker<EmployeeInfo>()
            .CustomInstantiator(f => new EmployeeInfo { Id = ++_id })
            .RuleFor(o => o.FirstName, f => f.Person.FirstName)
            .RuleFor(o => o.LastName, f => f.Person.LastName)
            .Generate(10);

         _timer = new Timer(state =>
         {
            Changed(nameof(ServerTime));
            PushUpdates();
         }, null, 0, 1000);
      }

      protected override void Dispose(bool disposing)
      {
         if (disposing)
            _timer.Dispose();
      }

      public string Employees_itemKey => nameof(EmployeeInfo.Id);

      public Action<string> Add => fullName =>
      {
         var names = fullName.Split(new char[] { ' ' }, 2);
         var employee = new EmployeeInfo
         {
            Id = ++_id,
            FirstName = names.First(),
            LastName = names.Length > 1 ? names.Last() : ""
         };
         _employees.Add(employee);

         // Use CRUD base method to add the list item on the client.
         this.AddList(nameof(Employees), new EmployeeInfo
         {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName
         });
      };

      public Action<EmployeeInfo> Update => employeeInfo =>
      {
         var employee = _employees.Find(x => x.Id == employeeInfo.Id);
         if (employee != null)
         {
            employee.FirstName = employeeInfo.FirstName ?? employee.FirstName;
            employee.LastName = employeeInfo.LastName ?? employee.LastName;

            // Use CRUD base method to update the list item on the client.
            this.UpdateList(nameof(Employees), employee);
         }
      };

      public Action<int> Remove => id =>
      {
         var employee = _employees.Find(x => x.Id == id);
         _employees.Remove(employee);

         // Use CRUD base method to remove the list item on the client.
         this.RemoveList(nameof(Employees), id);
      };
   }
}