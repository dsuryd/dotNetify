using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Globalization;
using System.Threading;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates how to leverage the two-way data binding on both the server-side view model and the code-behind
   /// on the browser to build rich web applications with less efforts and less code complexity.
   /// </summary>
   public class GridViewVM : BaseVM
   {
      // In real world app we wouldn't store big data in a private variable (can be taxing for web server resource),
      // but just do a pass-through from the database to the client.   The usage of private variable here is just
      // for DEMO purpose, to allow users to edit the data and see the updates reflected on the server without
      // doing actual permanent editing.
      private readonly EmployeeModel _model;

      /// <summary>
      /// This class holds the list header info.
      /// </summary>
      public class HeaderInfo
      {
         public string Id { get; set; }
         public string Text { get; set; }
      }

      /// <summary>
      /// The class that holds employee info to send to the client.  It inherits from Observable
      /// because the client can edit the names properties and we would like to be notified of the 
      /// changes if that happens.
      /// </summary>
      public class EmployeeInfo : ObservableObject
      {
         public int Id { get; set; }

         public string FirstName
         {
            get { return Get<string>(); }
            set { Set(value); }
         }

         public string LastName
         {
            get { return Get<string>(); }
            set { Set(value); }
         }
      }

      /// <summary>
      /// Localizable list title.
      /// </summary>
      public string ListTitle
      {
         get { return Properties.Resources.GridViewVM_ListTitle; }
      }

      /// <summary>
      /// Localizable place holder text for the search box.
      /// </summary>
      public string SearchPlaceHolder
      {
         get { return Properties.Resources.GridViewVM_SearchPlaceHolder; }
      }

      /// <summary>
      /// This property receives search keyword as it's being typed.
      /// </summary>
      public string Search
      {
         get { return Get<string>(); }
         set
         {
            Set(value.ToLower());
            Changed(() => Employees);  // Employees accessor uses the search keyword to construct its return value.
         }
      }

      /// <summary>
      /// Localizable list header.  The IDs will be used to identify sort columns.
      /// </summary>
      public List<HeaderInfo> ListHeader
      {
         get
         {
            return new List<HeaderInfo>()
                {
                    new HeaderInfo { Id = "FirstName", Text = Properties.Resources.GridViewVM_FirstName },
                    new HeaderInfo { Id = "LastName", Text = Properties.Resources.GridViewVM_LastName }
                };
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
            Changed(() => SelectedDetails);  // Update the details.
         }
      }

      /// <summary>
      /// Details of the selected list item.
      /// </summary>
      public object SelectedDetails
      {
         get
         {
            if (SelectedId == 0)
               return new { FullName = "", Phone = "" };

            var record = _model.GetRecord(SelectedId);
            return new { FullName = record.FullName, Phone = record.Phone };
         }
      }

      /// <summary>
      /// This property receives the selected UI culture code.
      /// </summary>
      public string ResourceCulture
      {
         get { return Get<string>(); }
         set
         {
            Set(value);

            // Change the UI culture, then raise change events for all localizable string properties.
            CultureInfo.CurrentUICulture = new CultureInfo(value);
            Changed(() => ListTitle);
            Changed(() => SearchPlaceHolder);
            Changed(() => ListHeader);
         }
      }

      /// <summary>
      /// List of employees.
      /// </summary>
      public IEnumerable<EmployeeInfo> Employees
      {
         get
         {
            // Get the list content, filtered by the search text. 
            var records = _model.GetAllRecords().Where(i => String.IsNullOrEmpty(Search) || i.FirstName.ToLower().StartsWith(Search) || i.LastName.ToLower().StartsWith(Search));

            // If the current selection is not on the new list, move the selection to the first one on the list.
            if (records.Any(i => i.Id == SelectedId) == false)
               SelectedId = records.Count() > 0 ? records.First().Id : 0;

            // Convert records to EmployeeInfo so that only relevant info gets sent to the client.
            return records.Select(i => new EmployeeInfo { Id = i.Id, FirstName = i.FirstName, LastName = i.LastName });
         }
      }

      /// <summary>
      /// By convention, when VMController receives a list item from the client, it will look for the function that starts
      /// with the list property name and ends with "_get" to access the list item for the purpose of updating its value.
      /// </summary>
      /// <param name="key">List item key.</param>
      /// <returns>List item.</returns>
      public EmployeeInfo Employees_get(string key)
      {
         EmployeeInfo employeeInfo = null;

         var record = _model.GetRecord(int.Parse(key));
         if (record != null)
            employeeInfo = new EmployeeInfo { Id = record.Id, FirstName = record.FirstName, LastName = record.LastName };

         // Handle the event when the employee data is changed on the client.
         if (employeeInfo != null)
            employeeInfo.PropertyChanged += Employee_PropertyChanged;

         return employeeInfo;
      }

      /// <summary>
      /// If employee info changed and is currently selected, make sure the displayed name and phone are updated too.
      /// </summary>
      private void Employee_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         var employeeInfo = sender as EmployeeInfo;

         var record = _model.GetRecord(employeeInfo.Id);
         if (record != null)
         {
            record.FirstName = employeeInfo.FirstName;
            record.LastName = employeeInfo.LastName;
            _model.UpdateRecord(record);
         }

         // If the record is currently selected, also update the displayed info of the selection.
         if (employeeInfo.Id == SelectedId)
            Changed(() => SelectedDetails);
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      public GridViewVM(EmployeeModel model)
      {
         _model = model;
         SelectedId = 1;
      }
   }
}