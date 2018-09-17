using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates simple CRUD operation on a list.  This example is not bandwidth-optimized
   /// to show that we can implement this without writing any javascript at all.  See BetterListVM for the example 
   /// where we optimize the bandwidth usage by writing a little bit of javascript.
   /// </summary>
   public class SimpleListVM : BaseVM
   {
      // In real world app we wouldn't store big data in a private variable (can be taxing for web server resource),
      // but just do a pass-through from the database to the client.   The usage of private variable here is just
      // for DEMO purpose, to allow users to edit the data and see the updates reflected on the server without
      // doing actual permanent editing.
      private readonly EmployeeModel _model;

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

         public bool EditFirstName { get; set; }

         public bool EditLastName { get; set; }
      }

      /// <summary>
      /// First name input.
      /// </summary>
      public string FirstName
      {
         get { return Get<string>(); }
         set { Set(value); }
      }

      /// <summary>
      /// Last name input.
      /// </summary>
      public string LastName
      {
         get { return Get<string>(); }
         set { Set(value); }
      }

      /// <summary>
      /// Adds a new employee.
      /// </summary>
      public ICommand Add => new Command(() =>
      {
         var record = new EmployeeRecord { FirstName = FirstName, LastName = LastName };
         _model.AddRecord(ref record);

         // Call this base method to send the new employee info back to the client.
         this.AddList(() => Employees, new EmployeeInfo { Id = record.Id, FirstName = record.FirstName, LastName = record.LastName });

         // Clear the inputs.
         FirstName = LastName = "";
      });

      /// <summary>
      /// When the Remove button is clicked, this property will receive the employee Id to remove.
      /// </summary>
      public ICommand Remove => new Command<string>(arg =>
      {
         var id = int.Parse(arg);
         _model.RemoveRecord(id);

         // Call this base method to tell the client to remove the employee from the list it holds.
         this.RemoveList(() => Employees, id);
      });

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
      public SimpleListVM(EmployeeModel model)
      {
         _model = model;
      }

      /// <summary>
      /// By convention, when VMController receives a property path that specifies an item in a list, it will look for
      /// a method that starts with the name in the path, in this case 'Employees' with a '_get' suffix, in order to
      /// access an item in that list, and then to use the returned object to set the updated value.
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