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
      private List<EmployeeRecord> _EmployeeRecords;
      private int _NewId = 100;
      private int _NumRecords = 0;

      public EmployeeModel(int NumRecords = 0)
      {
         _NumRecords = NumRecords;
      }

      public List<EmployeeRecord> GetAllRecords()
      {
         if (_EmployeeRecords == null)
            _EmployeeRecords = _NumRecords > 0 ? EmployeeRecord.GetMockupData().Take(_NumRecords).ToList() : EmployeeRecord.GetMockupData();

         return _EmployeeRecords;
      }

      public void AddRecord( ref EmployeeRecord iRecord )
      {
         iRecord.Id = _NewId++;
         _EmployeeRecords.Add(iRecord);
      }

      public EmployeeRecord GetRecord(int iId)
      {
         if (_EmployeeRecords == null)
            GetAllRecords();
         return _EmployeeRecords.FirstOrDefault(i => i.Id == iId);
      }

      public void UpdateRecord(EmployeeRecord iRecord)
      {
         var idx = _EmployeeRecords.FindIndex(i => i.Id == iRecord.Id);
         if (idx >= 0)
            _EmployeeRecords[idx] = iRecord;
      }

      public void RemoveRecord( int iId )
      {
         _EmployeeRecords.Remove(_EmployeeRecords.FirstOrDefault(i => i.Id == iId));
      }

      public static List<EmployeeRecord> AllRecords
      {
         get { return EmployeeRecord.GetMockupData(); }
      }
   }
}
