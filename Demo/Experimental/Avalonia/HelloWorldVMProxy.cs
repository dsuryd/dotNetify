using DotNetify.Client;
using HelloWorld.WebServer;
using System;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace HelloWorld
{
   public class HelloWorldVMProxy : VMProxy<HelloWorldVM>
   {
      #region Server Bindings

      public string Greetings { get; set; }
      public string ServerTime { get; set; }
      public ObservableCollection<EmployeeInfo> Employees { get; set; }

      #endregion Server Bindings

      #region Local Bindings

      public string AddFirstName { get; set; }
      public string AddLastName { get; set; }

      public string EditFirstName { get; set; }
      public string EditLastName { get; set; }
      public bool CanEdit => SelectedEmployee.Count > 0;

      public ObservableCollection<EmployeeInfo> SelectedEmployee { get; } = new ObservableCollection<EmployeeInfo>();

      public Action AddCommand => async () =>
      {
         await DispatchAsync(nameof(HelloWorldVM.Add), $"{AddFirstName} {AddLastName}");

         AddFirstName = AddLastName = string.Empty;
         Changed(nameof(AddFirstName));
         Changed(nameof(AddLastName));
      };

      public Action UpdateCommand => async () =>
      {
         var employee = SelectedEmployee[0];
         employee.FirstName = EditFirstName;
         employee.LastName = EditLastName;

         await DispatchAsync(nameof(HelloWorldVM.Update), employee);
      };

      public Action RemoveCommand => async () =>
      {
         var employee = SelectedEmployee[0];
         await DispatchAsync(nameof(HelloWorldVM.Remove), employee.Id);

         SelectedEmployee.RemoveAt(0);
         EditFirstName = EditLastName = string.Empty;
         Changed(nameof(EditFirstName));
         Changed(nameof(EditLastName));
         Changed(nameof(CanEdit));
      };

      #endregion Local Bindings

      public HelloWorldVMProxy(IDotNetifyClient dotnetify) : base(dotnetify)
      {
         SelectedEmployee.CollectionChanged += OnSelectedEmployee;
      }

      private void OnSelectedEmployee(object sender, NotifyCollectionChangedEventArgs e)
      {
         if (e.Action == NotifyCollectionChangedAction.Add)
         {
            var selectedItem = e.NewItems[0] as EmployeeInfo;
            EditFirstName = selectedItem.FirstName;
            EditLastName = selectedItem.LastName;
            Changed(nameof(EditFirstName));
            Changed(nameof(EditLastName));
            Changed(nameof(CanEdit));
         }
      }
   }
}