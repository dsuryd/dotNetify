using DotNetify;

namespace HelloWorld
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
}