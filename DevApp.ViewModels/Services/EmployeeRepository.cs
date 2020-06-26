using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;

namespace DotNetify.DevApp
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
      Task<IEnumerable<Employee>> GetAllAsync(int count);

      Task<Employee> GetAsync(int id);

      Task<int> AddAsync(Employee model);

      Task UpdateAsync(Employee model);

      Task RemoveAsync(int id);
   }

   public class EmployeeRepository : IEmployeeRepository
   {
      private IList<Employee> _mockData;

      public async Task<IEnumerable<Employee>> GetAllAsync(int count)
      {
         await Task.Delay(10);
         return _mockData ?? GenerateMockData(count);
      }

      public async Task<Employee> GetAsync(int id)
      {
         await Task.Delay(10);
         return _mockData.FirstOrDefault(x => x.Id == id);
      }

      public async Task<int> AddAsync(Employee model)
      {
         await Task.Delay(10);

         var employee = new Employee
         {
            Id = _mockData.Count > 0 ? _mockData.Max(x => x.Id) + 1 : 1,
            FirstName = model.FirstName,
            LastName = model.LastName,
            ReportTo = model.ReportTo,
            Phone = model.Phone
         };

         _mockData.Add(employee);
         return employee.Id;
      }

      public async Task UpdateAsync(Employee model)
      {
         await Task.Delay(10);

         var employee = await GetAsync(model.Id);

         employee.FirstName = model.FirstName;
         employee.LastName = model.LastName;
         employee.ReportTo = model.ReportTo;
         employee.Phone = model.Phone;
      }

      public async Task RemoveAsync(int id)
      {
         await Task.Delay(10);

         var employee = await GetAsync(id);
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