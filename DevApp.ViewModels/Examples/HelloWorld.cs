using System;
using System.Linq;
using System.Reactive.Linq;
using DotNetify;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class HelloWorldExample : BaseVM
   {
      public HelloWorldExample()
      {
         var markdown = new Markdown("DotNetify.DevApp.Docs.Examples.HelloWorld.md");

         AddProperty("ViewSource", markdown.GetSection(null, "HelloWorldVM.cs"));
         AddProperty("ViewModelSource", markdown.GetSection("HelloWorldVM.cs"));
      }
   }

   public class HelloWorldVM : BaseVM
   {
      private string _firstName = "Hello";
      private string _lastName = "World";

      public string FirstName
      {
         get => _firstName;
         set
         {
           _firstName = value;
            Changed(nameof(FullName));
         }
      }
      public string LastName
      {
         get => _lastName;
         set
         {
            _lastName = value;
            Changed(nameof(FullName));
         }
      }
      public string FullName => $"{FirstName} {LastName}";
   }
}