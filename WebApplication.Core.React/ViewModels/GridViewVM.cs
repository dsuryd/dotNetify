using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DotNetify;
using ViewModels.Components.MaterialUI;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates how to build rich web applications with less efforts and less code complexity.
   /// </summary>
   public class GridViewVM : BaseVM
   {
      private readonly EmployeeService _employeeService;

      /// <summary>
      /// The class that holds employee info for the master list to send to the browser. 
      /// </summary>
      public class EmployeeMaster
      {
         public int Id { get; set; }
         public string FirstName { get; set; }
         public string LastName { get; set; }
      }

      /// <summary>
      /// The class that holds employee details to send to the browser. 
      /// </summary>
      public class EmployeeDetails
      {
         public int Id { get; set; }
         public string FirstName { get; set; }
         public string LastName { get; set; }
         public string FullName { get; set; }
         public string Phone { get; set; }
         public int ReportTo { get; set; }
         public string ReportToName { get; set; }
      }

      /// <summary>
      /// List of employees.
      /// </summary>
      public IEnumerable<EmployeeMaster> Employees => _employeeService.GetAll().Select(i => new EmployeeMaster
      {
         Id = i.Id,
         FirstName = i.FirstName,
         LastName = i.LastName
      });

      /// <summary>
      /// This property receives the ID of the selected list item.
      /// </summary>
      public int SelectedId
      {
         get { return Get<int>(); }
         set
         {
            Set(value);
            Changed(nameof(Details));
         }
      }

      /// <summary>
      /// Details of the selected list item.
      /// </summary>
      public EmployeeDetails Details
      {
         get
         {
            var record = _employeeService.GetById(SelectedId);
            ReportToName = _employeeService.GetById(record.ReportTo)?.FullName;
            return new EmployeeDetails
            {
               FirstName = record.FirstName,
               LastName = record.LastName,
               FullName = record.FullName,
               Phone = record.Phone,
               ReportTo = record.ReportTo,
               ReportToName = ReportToName
            };
         }
      }

      public string ReportToName
      {
         get { return Get<string>() ?? ""; }
         set
         {
            Set(value);
            Changed(nameof(ReportToAutoComplete));
            ReportToErrorText = ReportToAutoComplete.Count() == 0 ? "No one by that name works here" : "";
         }
      }

      public IEnumerable<object> ReportToAutoComplete => _employeeService.GetAll()
         .Where(i => i.FullName.StartsWith(ReportToName, StringComparison.OrdinalIgnoreCase))
         .Select(i => new { Id = i.Id, Name = i.FullName });

      public AutoComplete ReportToAutoCompleteProps => new AutoComplete
      {
         floatingLabelText = "Report To",
         hintText = "Type the full name of the direct report here"
      };

      public string ReportToErrorText
      {
         get { return Get<string>(); }
         set { Set(value); }
      }


      /// <summary>
      /// When a list item is edited, this property will receive the edited item.
      /// </summary>
      public ICommand Update => new Command<EmployeeDetails>(changes =>
      {
         /// Real world app would do database update operation here.
         var record = _employeeService.GetById(changes.Id);
         if (record != null)
         {
            record.FirstName = changes.FirstName ?? record.FirstName;
            record.LastName = changes.LastName ?? record.LastName;
            record.ReportTo = changes.ReportTo;
            _employeeService.Update(record);
         }
      });

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="model">Employee model.</param>
      public GridViewVM()
      {
         // Normally this will be constructor-injected.
         _employeeService = new EmployeeService();

         SelectedId = _employeeService.GetAll().First().Id;
      }
   }
}