using DotNetify;
using DotNetify.Security;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates the security feature.
   /// </summary>
   [Authorize]
   public class SecurePageVM : BaseVM
   {
      public string Title => "Secure Page";
   }
}
