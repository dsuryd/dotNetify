using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Newtonsoft.Json;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates simple CRUD operation on a list.  In this example, we optimize the bandwidth
   /// usage by writing a little bit of javascript (see SimpleList.js) to avoid refreshing the entire list
   /// every time an item is edited or removed like what the SimpleListVM example is doing.
   /// </summary>
   public class BetterListVM : BaseVM
   {
      // In real world app we wouldn't store big data in a private variable (can be taxing for web server resource),
      // but just do a pass-through from the database to the client.   The usage of private variable here is just
      // for DEMO purpose, to allow users to edit the data and see the updates reflected on the server without
      // doing actual permanent editing.
      private readonly EmployeeModel _model;

      /// <summary>
      /// The class that holds employee info to send to the browser.  It inherits from Observable because the names
      /// properties can be edited on the browser, and we would like to be notified of the change when that happens.
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
      /// When the Add button is clicked, this property will receive the new employee name input.
      /// </summary>
      public ICommand Add => new Command<string>(arg =>
      {
         var newEmployee = (EmployeeInfo)JsonConvert.DeserializeObject(arg, typeof(EmployeeInfo));
         var record = new EmployeeRecord { FirstName = newEmployee.FirstName, LastName = newEmployee.LastName };
         _model.AddRecord(ref record);

         // Set only the Id back to optimize bandwidth usage.
         NewId = record.Id;
      });

      /// <summary>
      /// Use this property to send new employee Id right after receiving Add command.
      /// </summary>
      public int NewId
      {
         get { return Get<int>(); }
         set { Set(value); }
      }

      /// <summary>
      /// When the Remove button is clicked, this property will receive the employee Id to remove.
      /// </summary>
      public ICommand Remove => new Command<string>(id => _model.RemoveRecord(int.Parse(id)));

      /// <summary>
      /// List of employees.
      /// </summary>
      public IEnumerable<EmployeeInfo> Employees
      {
         get { return _model.GetAllRecords().Select(i => new EmployeeInfo { Id = i.Id, FirstName = i.FirstName, LastName = i.LastName }); }
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="model">Employee model.</param>
      public BetterListVM(EmployeeModel model)
      {
         _model = model;
      }

      /// <summary>
      /// By convention, when VMController receives a list item from the client, it will look for the function that starts
      /// with the list property name and ends with "_get" to access the list item for the purpose of updating its value.
      /// </summary>
      /// <param name="iKey">List item key.</param>
      /// <returns>List item.</returns>
      public EmployeeInfo Employees_get(string iKey)
      {
         EmployeeInfo employeeInfo = null;

         var record = _model.GetRecord(int.Parse(iKey));
         if (record != null)
         {
            employeeInfo = new EmployeeInfo { Id = record.Id, FirstName = record.FirstName, LastName = record.LastName };

            // Handle the event when the employee data is changed on the client.
            if (employeeInfo != null)
               employeeInfo.PropertyChanged += Employee_PropertyChanged;
         }

         return employeeInfo;
      }

      /// <summary>
      /// Event handler that gets called when an employee info's property value changed.
      /// </summary>
      private void Employee_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         var employeeInfo = sender as EmployeeInfo;

         /// Real world app would do database update operation here.
         var record = _model.GetRecord(employeeInfo.Id);
         if (record != null)
         {
            record.FirstName = employeeInfo.FirstName;
            record.LastName = employeeInfo.LastName;
            _model.UpdateRecord(record);
         }
      }
   }
}