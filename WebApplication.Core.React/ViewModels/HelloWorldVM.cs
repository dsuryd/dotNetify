using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates the two-way data binding between this server-side view model and the browser view.
   /// </summary>
   public class HelloWorldVM : BaseVM
   {
      public string FirstName
      {
         get { return Get<string>() ?? "Hello"; }
         set
         {
            Set(value);
            Changed(nameof(FullName));   // Raise the FullName's change event to update the view.
         }
      }

      public string LastName
      {
         get { return Get<string>() ?? "World"; }
         set
         {
            Set(value);
            Changed(nameof(FullName));
         }
      }

      public string FullName => $"{FirstName} {LastName}";
   }
}
