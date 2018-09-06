using DotNetify.Elements;
using System.Linq;
using System.Reactive.Linq;

namespace DotNetify.DevApp
{
   public class HelloWorldExample : BaseVM
   {
      public HelloWorldExample()
      {
         var markdown = new Markdown("DotNetify.DevApp.Docs.Examples.HelloWorld.md");

         AddProperty("ViewSource", markdown.GetSection(null, "HelloWorldVM.cs"))
            .SubscribeTo(AddProperty<string>("Framework").Select(GetViewSource));

         AddProperty("ViewModelSource", markdown.GetSection("HelloWorldVM.cs"));
      }

      private string GetViewSource(string framework)
      {
         return framework == "Knockout" ?
            new Markdown("DotNetify.DevApp.Docs.Examples.Knockout.HelloWorld.md") :
            new Markdown("DotNetify.DevApp.Docs.Examples.HelloWorld.md").GetSection(null, "HelloWorldVM.cs");
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