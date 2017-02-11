using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates simple CRUD operation on a list. 
   /// </summary>
   public class SimpleListVM : BaseVM
   {
      private readonly EmployeeService _employeeService;

      /// <summary>
      /// The class that holds employee info to send to the browser.  It inherits from Observable because the names
      /// properties can be edited on the browser, and we would like to be notified of the change when that happens.
      /// </summary>
      public class EmployeeInfo : Observable
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
         var names = arg.Split(new char[] { ' ' }, 2);
         var newRecord = new EmployeeModel
         {
            FirstName = names.First(),
            LastName = names.Length > 1 ? names.Last() : ""
         };

         this.AddList(() => Employees, new EmployeeInfo
         {
            Id = _employeeService.Add(newRecord),
            FirstName = newRecord.FirstName,
            LastName = newRecord.LastName
         });
      });

      /// <summary>
      /// When the Remove button is clicked, this property will receive the employee Id to remove.
      /// </summary>
      public ICommand Remove => new Command<string>(arg =>
      {
         int id = int.Parse(arg);
         _employeeService.Delete(id);

         this.RemoveList(() => Employees, id);
      });

      /// <summary>
      /// List of employees.
      /// </summary>
      public IEnumerable<EmployeeInfo> Employees => _employeeService.GetAll().Select(i => new EmployeeInfo
      {
         Id = i.Id,
         FirstName = i.FirstName,
         LastName = i.LastName
      });

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="model">Employee model.</param>
      public SimpleListVM()
      {
         // Normally this will be constructor-injected.
         _employeeService = new EmployeeService(7);
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

         var record = _employeeService.GetById(int.Parse(iKey));
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
         var record = _employeeService.GetById(employeeInfo.Id);
         if (record != null)
         {
            record.FirstName = employeeInfo.FirstName;
            record.LastName = employeeInfo.LastName;
            _employeeService.Update(record);
         }
      }
   }
}