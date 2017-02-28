using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WebApplication.Core.React;

namespace ViewModels
{
   public class EmployeeModel
   {
      public int Id { get; set; }

      public string FirstName { get; set; }

      public string LastName { get; set; }

      public int ReportTo { get; set; }

      public string Phone => $"716-555-34{Id.ToString().PadLeft(2, '0')}";

      public string FullName => $"{FirstName} {LastName}";
   }

   public class EmployeeService
   {
      private List<EmployeeModel> _employees;
      private int _newId = 100;

      private List<EmployeeModel> MockupData => JsonConvert.DeserializeObject<List<EmployeeModel>>(this.GetEmbeddedResource("employees.json"));

      public EmployeeService(int? numRecords = null)
      {
         _employees = numRecords != null ? MockupData.Take(numRecords.Value).ToList() : MockupData;
      }

      public List<EmployeeModel> GetAll() => _employees;

      public EmployeeModel GetById(int id) => _employees.FirstOrDefault(i => i.Id == id);

      public int Add(EmployeeModel record)
      {
         record.Id = _newId++;
         _employees.Add(record);
         return record.Id;
      }

      public void Update(EmployeeModel record)
      {
         var idx = _employees.FindIndex(i => i.Id == record.Id);
         if (idx >= 0)
            _employees[idx] = record;
      }

      public void Delete(int id) => _employees.Remove(_employees.FirstOrDefault(i => i.Id == id));
   }
}
