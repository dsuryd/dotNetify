using DotNetify.Client;
using HelloWorld.Server;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace HelloWorld
{
   public class HelloWorldVMProxy : INotifyPropertyChanged, IDisposable
   {
      private readonly IDotNetifyClient _dotnetify;

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
         await _dotnetify.DispatchAsync(nameof(HelloWorldVM.Add), $"{AddFirstName} {AddLastName}");

         AddFirstName = AddLastName = string.Empty;
         Changed(nameof(AddFirstName));
         Changed(nameof(AddLastName));
      };

      public Action UpdateCommand => async () =>
      {
         var employee = SelectedEmployee[0];
         employee.FirstName = EditFirstName;
         employee.LastName = EditLastName;

         await _dotnetify.DispatchAsync(nameof(HelloWorldVM.Update), employee);
      };

      public Action RemoveCommand => async () =>
      {
         var employee = SelectedEmployee[0];
         await _dotnetify.DispatchAsync(nameof(HelloWorldVM.Remove), employee.Id);

         SelectedEmployee.RemoveAt(0);
         EditFirstName = EditLastName = string.Empty;
         Changed(nameof(EditFirstName));
         Changed(nameof(EditLastName));
         Changed(nameof(CanEdit));
      };

      #endregion Local Bindings

      public event PropertyChangedEventHandler PropertyChanged;

      public HelloWorldVMProxy(IDotNetifyClient dotnetify)
      {
         _dotnetify = dotnetify;

         var connectOptions = new VMConnectOptions { VMArg = new { Greetings = "Hello World!" } };
         _dotnetify.ConnectAsync(nameof(HelloWorldVM), this, connectOptions);

         SelectedEmployee.CollectionChanged += OnSelectedEmployee;
      }

      public void Dispose() => _dotnetify.Dispose();

      private void Changed(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

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