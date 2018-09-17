using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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
      /// Constructor.
      /// </summary>
      public SimpleListVM()
      {
         // Normally this will be constructor-injected.
         _employeeService = new EmployeeService(7);

         // List of employees.
         AddProperty<IEnumerable<EmployeeInfo>>("Employees").SubscribeTo(
            Observable.Return(_employeeService
               .GetAll()
               .Select(i => new EmployeeInfo
               {
                  Id = i.Id,
                  FirstName = i.FirstName,
                  LastName = i.LastName
               }))
         );

         // If you use CRUD methods on a list, you must set the item key prop name of that list
         // by defining a string property that starts with that list's prop name, followed by "_itemKey".
         AddProperty<string>("Employees_itemKey").SubscribeTo(Observable.Return(nameof(EmployeeInfo.Id)));

         // When the Add button is clicked, this property will receive the new employee full name input.
         this.AddProperty<string>("Add").Subscribe(fullName =>
         {
            var names = fullName.Split(new char[] { ' ' }, 2);
            var newRecord = new EmployeeModel
            {
               FirstName = names.First(),
               LastName = names.Length > 1 ? names.Last() : ""
            };

            this.AddList("Employees", new EmployeeInfo
            {
               Id = _employeeService.Add(newRecord),
               FirstName = newRecord.FirstName,
               LastName = newRecord.LastName
            });
         });

         // Property to show notification when changes have been saved.
         var showNotification = AddProperty<bool>("ShowNotification");

         // When a list item is edited, this property will receive the edited item.
         AddProperty<EmployeeInfo>("Update").Subscribe(changes =>
         {
            /// Real world app would do database update operation here.
            var record = _employeeService.GetById(changes.Id);
            if (record != null)
            {
               record.FirstName = changes.FirstName ?? record.FirstName;
               record.LastName = changes.LastName ?? record.LastName;
               _employeeService.Update(record);

               showNotification.OnNext(true);
            }
         });

         // When the Remove button is clicked, this property will receive the employee Id to remove.
         AddProperty<int>("Remove").Subscribe(id =>
         {
            _employeeService.Delete(id);

            // Call special base method to remove an item from the list on the client-side.
            // This will be handled by dotNetify client-side library; no custom JSX needed.
            this.RemoveList("Employees", id);
         });
      }
   }
}