using Bogus;
using DotNetify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ViewModels
{
   /// <summary>
   /// This view model inherits from DotNetify.MulticastVM so that multiple clients can use the same instance.
   /// </summary>
   public class HelloWorldVM : MulticastVM
   {
      private Timer _timer;
      private int _idCounter;
      private List<EmployeeInfo> _employees;

      public class EmployeeInfo : ObservableObject
      {
         public int Id
         {
            get => Get<int>();
            set => Set(value);
         }

         public string FirstName
         {
            get => Get<string>();
            set => Set(value);
         }

         public string LastName
         {
            get => Get<string>();
            set => Set(value);
         }
      }

      /// <summary>
      /// This property will be initialized by the Avalonia client.
      /// </summary>
      public string Greetings
      {
         get => Get<string>();
         set
         {
            Set(value);
            PushUpdates(); // Must have this to push updates to other clients.
         }
      }

      public DateTime ServerTime => DateTime.Now;
      public IEnumerable<EmployeeInfo> Employees => _employees;

      /// <summary>
      /// Default constructor.
      /// </summary>
      public HelloWorldVM()
      {
         // Generate fake employee names.
         _employees = new Faker<EmployeeInfo>()
            .CustomInstantiator(f => new EmployeeInfo { Id = ++_idCounter })
            .RuleFor(o => o.FirstName, f => f.Person.FirstName)
            .RuleFor(o => o.LastName, f => f.Person.LastName)
            .Generate(10);

         // Send server time update every second.
         _timer = new Timer(state =>
         {
            Changed(nameof(ServerTime));
            PushUpdates();
         }, null, 0, 1000);
      }

      /// <summary>
      /// This method is called every time a client disconnects. When the last client disconnects, the disposing arg will be true.
      /// </summary>
      /// <param name="disposing">True if this instance will finally be disposed.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing)
            _timer.Dispose();
      }

      /// <summary>
      /// To use dotNetify CRUD methods, must set this to the property name that uniquely identifies the items in the Employees list.
      /// </summary>
      public string Employees_itemKey => nameof(EmployeeInfo.Id);

      /// <summary>
      /// Handles Add command from a client.
      /// </summary>
      public Action<string> Add => fullName =>
      {
         var names = fullName.Split(new char[] { ' ' }, 2);
         var employee = new EmployeeInfo
         {
            Id = ++_idCounter,
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

      /// <summary>
      /// Handles Update command from a client.
      /// </summary>
      public Action<EmployeeInfo> Update => data =>
      {
         var employee = _employees.Find(x => x.Id == data.Id);
         if (employee != null)
         {
            employee.FirstName = data.FirstName ?? employee.FirstName;
            employee.LastName = data.LastName ?? employee.LastName;

            // Use CRUD base method to update the list item on the client.
            this.UpdateList(nameof(Employees), employee);
         }
      };

      /// <summary>
      /// Handles Remove command from a client.
      /// </summary>
      public Action<int> Remove => id =>
      {
         var employee = _employees.Find(x => x.Id == id);
         _employees.Remove(employee);

         // Use CRUD base method to remove the list item on the client.
         this.RemoveList(nameof(Employees), id);
      };
   }
}