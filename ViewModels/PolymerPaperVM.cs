using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates binding to Polymer paper elements.
   /// </summary>
   public class PolymerPaperVM : BaseVM
   {
      public string FirstName
      {
         get { return Get<string>() ?? "Hello"; }
         set
         {
            Set(value);
            Changed(() => FullName);   // Raise the FullName's change event to update the view.
         }
      }

      public string FirstName_label => "First Name";
      public string FirstName_error_message => "First name is required";

      public string LastName
      {
         get { return Get<string>() ?? "World"; }
         set
         {
            Set(value);
            Changed(() => FullName);
         }
      }

      public string LastName_label => "Last Name";
      public string LastName_error_message => "Last name is required";

      public string FullName => $"{FirstName} {LastName}";
   }
}
