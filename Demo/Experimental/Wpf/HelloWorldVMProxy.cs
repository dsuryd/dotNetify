using DotNetify;
using DotNetify.Client;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace HelloWorld
{
   /// <summary>
   /// This is Avalonia view model that serves as a proxy to the server-side view model.
   /// Properties in the "Server bindings" region will be initialized with data coming from the server on successful connection.
   /// </summary>
   public class HelloWorldVMProxy : INotifyPropertyChanged, IDisposable
   {
      private readonly IDotNetifyClient _dotnetify;

      /// <summary>
      /// This class is a representation of the server-side view model, to provide this proxy with data types and strongly-typed names.
      /// </summary>
      public class HelloWorldVM
      {
         public class EmployeeInfo : ObservableObject
         {
            public int Id
            {
               get => Get<int>();
               set => Set(value);
            }

            public string FirstName
            {
               get => Get<string>();
               set => Set(value);
            }

            public string LastName
            {
               get => Get<string>();
               set => Set(value);
            }
         }

         public Action<string> Add { get; }
         public Action<EmployeeInfo> Update { get; }
         public Action<int> Remove { get; }
      }

      #region Server Bindings

      public string Greetings { get; set; }
      public string ServerTime { get; set; }
      public ObservableCollection<HelloWorldVM.EmployeeInfo> Employees { get; set; }

      #endregion Server Bindings

      #region Local Bindings

      public string AddFirstName { get; set; }
      public string AddLastName { get; set; }

      public string EditFirstName { get; set; }
      public string EditLastName { get; set; }
      public bool CanEdit => SelectedEmployee != null;

      private HelloWorldVM.EmployeeInfo _selectedEmployee;

      public HelloWorldVM.EmployeeInfo SelectedEmployee
      {
         get => _selectedEmployee;
         set
         {
            _selectedEmployee = value;
            OnSelectedEmployee();
         }
      }

      public ICommand AddCommand { get; }
      public ICommand UpdateCommand { get; }
      public ICommand RemoveCommand { get; }

      private Action Add => async () =>
      {
         await _dotnetify.DispatchAsync(nameof(HelloWorldVM.Add), $"{AddFirstName} {AddLastName}");

         AddFirstName = AddLastName = string.Empty;
         Changed(nameof(AddFirstName));
         Changed(nameof(AddLastName));
      };

      private Action Update => async () =>
      {
         var employee = SelectedEmployee;
         employee.FirstName = EditFirstName;
         employee.LastName = EditLastName;

         await _dotnetify.DispatchAsync(nameof(HelloWorldVM.Update), employee);
      };

      private Action Remove => async () =>
      {
         var employee = SelectedEmployee;
         await _dotnetify.DispatchAsync(nameof(HelloWorldVM.Remove), employee.Id);

         SelectedEmployee = null;
         EditFirstName = EditLastName = string.Empty;
         Changed(nameof(EditFirstName));
         Changed(nameof(EditLastName));
         Changed(nameof(CanEdit));
      };

      private void OnSelectedEmployee()
      {
         if (SelectedEmployee != null)
         {
            EditFirstName = SelectedEmployee.FirstName;
            EditLastName = SelectedEmployee.LastName;
            Changed(nameof(EditFirstName));
            Changed(nameof(EditLastName));
            Changed(nameof(CanEdit));
         }
      }

      #endregion Local Bindings

      #region INotifyPropertyChanged

      public event PropertyChangedEventHandler PropertyChanged;

      public void Changed(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

      #endregion INotifyPropertyChanged

      public HelloWorldVMProxy(IDotNetifyClient dotnetify)
      {
         _dotnetify = dotnetify;

         AddCommand = new Command(Add);
         UpdateCommand = new Command(Update);
         RemoveCommand = new Command(Remove);

         Connect();
      }

      public void Connect()
      {
         // Connect to the server-side view model.
         _dotnetify.ConnectAsync(
            nameof(HelloWorldVM),
            this,
            new VMConnectOptions { VMArg = new { Greetings = "Hello World!" } }
         );
      }

      public void Dispose() => _dotnetify.Dispose();
   }
}