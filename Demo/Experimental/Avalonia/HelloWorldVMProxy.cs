using HelloWorld.WebServer;
using System.Collections.Generic;
using DotNetify.Client;

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

      public HelloWorldVMProxy(IDotNetifyClient dotnetify) : base(dotnetify)
      {
      }
   }
}