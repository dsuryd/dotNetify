using DotNetify.Client;
using HelloWorld.WebServer;
using System;
using System.Collections.ObjectModel;

namespace HelloWorld
{
   public class HelloWorldVMProxy : VMProxy<HelloWorldVM>
   {
      public string Greetings { get; set; }
      public string ServerTime { get; set; }
      public ObservableCollection<EmployeeInfo> Employees { get; set; }

      public string InputFirstName { get; set; }
      public string InputLastName { get; set; }

      public Action InputAdd => async () =>
      {
         await DispatchAsync(nameof(HelloWorldVM.Add), $"{InputFirstName} {InputLastName}");

         Changed(nameof(InputFirstName), string.Empty);
         Changed(nameof(InputLastName), string.Empty);
      };

      public HelloWorldVMProxy(IDotNetifyClient dotnetify) : base(dotnetify)
      {
      }
   }
}