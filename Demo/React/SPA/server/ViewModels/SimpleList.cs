using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DotNetify;

namespace SPA
{
   public class SimpleListVM : BaseVM
   {
      private readonly IEmployeeRepository _repository;

      public class EmployeeInfo
      {
         public int Id { get; set; }
         public string FirstName { get; set; }
         public string LastName { get; set; }
      }

      // If you use CRUD methods on a list, you must set the item key prop name of that list.
      [ItemKey(nameof(Employee.Id))]
      public IEnumerable<EmployeeInfo> Employees => _repository
           .GetAll(10)
           .Select(i => new EmployeeInfo { Id = i.Id, FirstName = i.FirstName, LastName = i.LastName });

      public SimpleListVM(IEmployeeRepository repository)
      {
         _repository = repository;
      }

      public void Add(string fullName)
      {
         var names = fullName.Split(new char[] { ' ' }, 2);
         var employee = new Employee
         {
            FirstName = names.First(),
            LastName = names.Length > 1 ? names.Last() : ""
         };

         // Use CRUD base method to add the list item on the client.
         this.AddList("Employees", new EmployeeInfo
         {
            Id = _repository.Add(employee),
            FirstName = employee.FirstName,
            LastName = employee.LastName
         });
      }

      public void Update(EmployeeInfo employeeInfo)
      {
         var employee = _repository.Get(employeeInfo.Id);
         if (employee != null)
         {
            employee.FirstName = employeeInfo.FirstName ?? employee.FirstName;
            employee.LastName = employeeInfo.LastName ?? employee.LastName;
            _repository.Update(employee);
         }
      }

      public void Remove(int id)
      {
         _repository.Remove(id);

         // Use CRUD base method to remove the list item on the client.
         this.RemoveList(nameof(Employees), id);
      }
   }
}