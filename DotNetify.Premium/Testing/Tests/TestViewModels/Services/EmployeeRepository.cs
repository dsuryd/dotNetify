using Bogus;
using System.Collections.Generic;
using System.Linq;

namespace DevApp.ViewModels
{
   public class Employee
   {
      public int Id { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public string ReportTo { get; set; }
      public string Phone { get; set; }
   }

   public interface IEmployeeRepository
   {
      IEnumerable<Employee> GetAll(int count);

      Employee Get(int id);

      int Add(Employee model);

      void Update(Employee model);

      void Remove(int id);
   }

   public class EmployeeRepository : IEmployeeRepository
   {
      private IList<Employee> _mockData;

      public IEnumerable<Employee> GetAll(int count) => _mockData ?? GenerateMockData(count);

      public Employee Get(int id) => _mockData.FirstOrDefault(x => x.Id == id);

      public int Add(Employee model)
      {
         var employee = new Employee
         {
            Id = _mockData.Max(x => x.Id) + 1,
            FirstName = model.FirstName,
            LastName = model.LastName,
            ReportTo = model.ReportTo,
            Phone = model.Phone
         };

         _mockData.Add(employee);
         return employee.Id;
      }

      public void Update(Employee model)
      {
         var employee = Get(model.Id);

         employee.FirstName = model.FirstName;
         employee.LastName = model.LastName;
         employee.ReportTo = model.ReportTo;
         employee.Phone = model.Phone;
      }

      public void Remove(int id)
      {
         var employee = Get(id);
         _mockData.Remove(employee);
      }

      private IList<Employee> GenerateMockData(int count)
      {
         int id = 0;
         var list = new Faker<Employee>()
            .CustomInstantiator(f => new Employee { Id = ++id })
            .RuleFor(o => o.FirstName, f => f.Person.FirstName)
            .RuleFor(o => o.LastName, f => f.Person.LastName)
            .RuleFor(o => o.ReportTo, f => f.Person.FullName)
            .RuleFor(o => o.Phone, f => f.Phone.PhoneNumber("(###) ###-####"))
            .Generate(count);

         _mockData = list;
         return list;
      }
   }
}