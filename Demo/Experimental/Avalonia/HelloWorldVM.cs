using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HelloWorld
{
   public class HelloWorldVM : INotifyPropertyChanged
   {
      private readonly Task _initializeTask;
      private readonly IViewState _viewState;

      public event PropertyChangedEventHandler PropertyChanged = delegate { };

      public string Greetings { get; set; } = "";
      public string ServerTime { get; set; } = "";

      public HelloWorldVM()
      {
         _viewState = new ViewState(this);
         _initializeTask = InitializeAsync();
      }

      private async Task InitializeAsync()
      {
         var vm = new DotNetifyClient(new DotNetifyHubProxy());
         await vm.ConnectAsync("HelloWorld", _viewState);
      }
   }
}