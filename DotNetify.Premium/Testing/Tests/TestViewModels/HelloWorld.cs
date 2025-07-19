using DotNetify;

namespace DevApp.ViewModels
{
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