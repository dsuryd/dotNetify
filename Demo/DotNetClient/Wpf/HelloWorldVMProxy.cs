using DotNetify;
using DotNetify.Client;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ViewModels;

namespace HelloWorld
{
   /// <summary>
   /// This is WPF view model that serves as a proxy to the server-side view model.
   /// Properties in the "Server bindings" region will be initialized with data coming from the server on successful connection.
   /// </summary>
   public class HelloWorldVMProxy : INotifyPropertyChanged, IDisposable
   {
      private readonly IDotNetifyClient _dotnetify;
      private readonly IDotNetifyHubProxy _hubProxy;

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

      public HelloWorldVMProxy(IDotNetifyClient dotnetify, IDotNetifyHubProxy hubProxy)
      {
         _dotnetify = dotnetify;
         _hubProxy = hubProxy;

         AddCommand = new Command(Add);
         UpdateCommand = new Command(Update);
         RemoveCommand = new Command(Remove);

         _ = ConnectAsync();
      }

      public async Task ConnectAsync()
      {
         // Connect to the server-side view model.
         await _dotnetify.ConnectAsync(
            nameof(HelloWorldVM),
            this,
            new VMConnectOptions { VMArg = new { Greetings = "Hello World!" } }
         );

         _hubProxy.StateChanged += async (sender, state) =>
         {
            if (state == HubConnectionState.Connected)
            {
               Dispose();
               await ConnectAsync();
            }
         };
      }

      public void Dispose() => _dotnetify.DisposeAsync();
   }
}