using System.Collections.Generic;
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
      /// The class that holds employee info to send to the browser.  
      /// </summary>
      public class EmployeeInfo
      {
         public int Id { get; set; }
         public string FirstName { get; set; }
         public string LastName { get; set; }
      }

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
      /// When the Add button is clicked, this property will receive the new employee full name input.
      /// </summary>
      public ICommand Add => new Command<string>(fullName =>
      {
         var names = fullName.Split(new char[] { ' ' }, 2);
         var newRecord = new EmployeeModel
         {
            FirstName = names.First(),
            LastName = names.Length > 1 ? names.Last() : ""
         };

         // Call special base method to add the new employee info back to the list on the client-side.
         // This will be handled by dotNetify client-side library; no custom JSX needed.
         this.AddList(() => Employees, new EmployeeInfo
         {
            Id = _employeeService.Add(newRecord),
            FirstName = newRecord.FirstName,
            LastName = newRecord.LastName
         });
      });

      /// <summary>
      /// When a list item is edited, this property will receive the edited item.
      /// </summary>
      public ICommand Update => new Command<EmployeeInfo>(changes =>
      {
         /// Real world app would do database update operation here.
         var record = _employeeService.GetById(changes.Id);
         if (record != null)
         {
            record.FirstName = changes.FirstName ?? record.FirstName;
            record.LastName = changes.LastName ?? record.LastName;
            _employeeService.Update(record);

            ShowNotification = true;
         }
      });

      /// <summary>
      /// When the Remove button is clicked, this property will receive the employee Id to remove.
      /// </summary>
      public ICommand Remove => new Command<int>(id =>
      {
         _employeeService.Delete(id);

         // Call special base method to remove an item from the list on the client-side.
         // This will be handled by dotNetify client-side library; no custom JSX needed.
         this.RemoveList(() => Employees, id);
      });

      /// <summary>
      /// Whether to show notification that changes have been saved.
      /// Once this property is accessed, it will revert itself back to false.
      /// </summary>
      private bool _showNotification;
      public bool ShowNotification
      {
         get
         {
            var value = _showNotification;
            _showNotification = false;
            return value;
         }
         set
         {
            _showNotification = value;
            Changed(nameof(ShowNotification));
         }
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="model">Employee model.</param>
      public SimpleListVM()
      {
         // Normally this will be constructor-injected.
         _employeeService = new EmployeeService(7);
      }
   }
}