using HelloWorld.WebServer;
using System.Collections.Generic;
using DotNetify.Client;
using System;

namespace HelloWorld
{
   public class HelloWorldVMProxy : VMProxy<HelloWorldVM>
   {
      public class EmployeeInfo
      {
         public int Id { get; set; }
         public string FirstName { get; set; }
         public string LastName { get; set; }
      }

      public string Greetings { get; set; }
      public string ServerTime { get; set; }
      public List<EmployeeInfo> Employees { get; set; }

      public string InputFirstName { get; set; }
      public string InputLastName { get; set; }

      public Action InputAdd => () =>
      {
         var task = DispatchAsync(nameof(HelloWorldVM.Add), $"{InputFirstName} {InputLastName}");

         InputFirstName = string.Empty;
         InputLastName = string.Empty;
      };

      public HelloWorldVMProxy(IDotNetifyClient dotnetify) : base(dotnetify)
      {
      }
   }
}