using DotNetify.Client;
using ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HelloWorld
{
   /// <summary>
   /// This is Avalonia view model that serves as a proxy to the server-side view model.
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
      public bool CanEdit => SelectedEmployee.Count > 0;

      public ObservableCollection<HelloWorldVM.EmployeeInfo> SelectedEmployee { get; } = new ObservableCollection<HelloWorldVM.EmployeeInfo>();

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

      private void OnSelectedEmployee(object sender, NotifyCollectionChangedEventArgs e)
      {
         if (e.Action == NotifyCollectionChangedAction.Add)
         {
            var selectedItem = e.NewItems[0] as HelloWorldVM.EmployeeInfo;
            EditFirstName = selectedItem.FirstName;
            EditLastName = selectedItem.LastName;
            Changed(nameof(EditFirstName));
            Changed(nameof(EditLastName));
            Changed(nameof(CanEdit));
         }
      }

      #endregion Local Bindings

      #region INotifyPropertyChanged

      public event PropertyChangedEventHandler PropertyChanged;

      private void Changed(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

      #endregion INotifyPropertyChanged

      public HelloWorldVMProxy(IDotNetifyClient dotnetify, IDotNetifyHubProxy hubProxy)
      {
         _dotnetify = dotnetify;
         _hubProxy = hubProxy;
         SelectedEmployee.CollectionChanged += OnSelectedEmployee;

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

      public void Dispose() => _dotnetify.Dispose();
   }
}