using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using Newtonsoft.Json;

namespace ViewModels
{
   public class EmployeeRecord
   {
      public int Id { get; set; }

      public string FirstName { get; set; }

      public string LastName { get; set; }

      public int ReportTo { get; set; }

      public string Phone
      {
         get { return "716-555-34" + Id.ToString().PadLeft(2, '0'); }
      }

      public string FullName
      {
         get { return FirstName + " " + LastName; }
      }

      public static List<EmployeeRecord> GetMockupData()
      {
         var path = HttpContext.Current.Server.MapPath(@"/Content/employees.json");
         return JsonConvert.DeserializeObject<List<EmployeeRecord>>(File.ReadAllText(path));
      }
   }

   public class EmployeeModel
   {
      protected List<EmployeeRecord> _employeeRecords;
      protected int _newId = 100;
      protected int? _numRecords;

      public EmployeeModel()
      {
      }

      public EmployeeModel(int numRecords)
      {
         _numRecords = numRecords;
      }

      public virtual List<EmployeeRecord> GetAllRecords()
      {
         if (_employeeRecords == null)
            _employeeRecords = _numRecords != null ? EmployeeRecord.GetMockupData().Take(_numRecords.Value).ToList() : EmployeeRecord.GetMockupData();

         return _employeeRecords;
      }

      public void AddRecord(ref EmployeeRecord record)
      {
         record.Id = _newId++;
         _employeeRecords.Add(record);
      }

      public EmployeeRecord GetRecord(int id)
      {
         if (_employeeRecords == null)
            GetAllRecords();
         return _employeeRecords.FirstOrDefault(i => i.Id == id);
      }

      public void UpdateRecord(EmployeeRecord record)
      {
         var idx = _employeeRecords.FindIndex(i => i.Id == record.Id);
         if (idx >= 0)
            _employeeRecords[idx] = record;
      }

      public void RemoveRecord(int id)
      {
         _employeeRecords.Remove(_employeeRecords.FirstOrDefault(i => i.Id == id));
      }
   }
}
