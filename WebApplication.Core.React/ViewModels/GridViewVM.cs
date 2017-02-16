using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Microsoft.Extensions.Localization;
using ViewModels.Components.MaterialUI;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates how to build rich web applications with less efforts and less code complexity.
   /// </summary>
   public class GridViewVM : BaseVM
   {
      private readonly EmployeeService _employeeService;
      private readonly IEnumerable<object> _emptyList = new List<object>();
      private readonly EmployeeDetails _emptyDetails = new EmployeeDetails();

      /// <summary>
      /// The class that holds employee info for the master list to send to the browser. 
      /// </summary>
      public class EmployeeMaster
      {
         public int Id { get; set; }
         public string FirstName { get; set; }
         public string LastName { get; set; }

         public EmployeeMaster(EmployeeModel record)
         {
            Id = record.Id;
            FirstName = record.FirstName;
            LastName = record.LastName;
         }
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
      public IEnumerable<EmployeeMaster> Employees
      {
         get
         {
            var result = _employeeService.GetAll()
               .Where(i => string.IsNullOrEmpty(EmployeeSearch) || i.FullName.ToLower().Contains(EmployeeSearch))
               .Select(record => new EmployeeMaster(record));

            if (!result.Any(i => i.Id == SelectedId))
               SelectedId = result.Count() > 0 ? result.First().Id : -1;

            return result;
         }
      }

      /// <summary>
      /// If you use CRUD methods on a list, you must set the item key prop name of that list
      /// by defining a string property that starts with that list's prop name, followed by "_itemKey".
      /// </summary>
      public string Employees_itemKey => nameof(EmployeeMaster.Id);

      public string EmployeeSearch
      {
         get { return Get<string>(); }
         set
         {
            Set(value.ToLower());
            Changed(nameof(Employees));
         }
      }

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
            return record != null ? new EmployeeDetails
            {
               Id = record.Id,
               FirstName = record.FirstName,
               LastName = record.LastName,
               FullName = record.FullName,
               Phone = record.Phone,
               ReportTo = record.ReportTo,
               ReportToName = _employeeService.GetById(record.ReportTo)?.FullName
            } : _emptyDetails;
         }
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

            this.UpdateList(nameof(Employees), new EmployeeMaster(record));
            Changed(nameof(Details));
         }
      });

      public string ReportToSearch
      {
         get { return Get<string>(); }
         set
         {
            Set(value);
            Changed(nameof(ReportToSearchResult));
            Changed(nameof(ReportToErrorText));
         }
      }

      public IEnumerable<object> ReportToSearchResult => !string.IsNullOrEmpty(ReportToSearch) ?
         _employeeService.GetAll()
            .Where(i => i.Id != Details.Id && i.FullName.StartsWith(ReportToSearch, StringComparison.OrdinalIgnoreCase))
            .Select(i => new { Id = i.Id, Name = i.FullName }) : _emptyList;

      public AutoComplete ReportToAutoCompleteProps => new AutoComplete
      {
         floatingLabelText = "Report To",
         hintText = "Type the full name of the direct report here"
      };

      public string ReportToErrorText => !string.IsNullOrEmpty(ReportToSearch) && ReportToSearchResult.Count() == 0 ? "No one by that name works here" : "";

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="model">Employee model.</param>
      public GridViewVM( IStringLocalizer<GridViewVM> localizer)
      {
         // Normally this will be constructor-injected.
         _employeeService = new EmployeeService();

         var test = localizer.GetAllStrings();
      }
   }
}