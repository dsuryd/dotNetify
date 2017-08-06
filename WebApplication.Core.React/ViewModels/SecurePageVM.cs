using System;
using DotNetify;
using DotNetify.Security;
using System.Threading;
using System.Security.Claims;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates the security feature.
   /// </summary>
   [Authorize]
   public class SecurePageVM : BaseVM
   {
      private Timer _timer;

      public string SecureCaption { get; }
      public string SecureData => DateTime.Now.ToString();

      public SecurePageVM(IPrincipalAccessor principalAccessor)
      {
         SecureCaption = $"Only authenticated user \"{principalAccessor.Principal.Identity.Name}\" can see this live clock.";

         _timer = new Timer(state =>
         {
            Changed(nameof(SecureData));
            PushUpdates();
         }, null, 0, 1000); 
      }

      public override void Dispose() => _timer.Dispose();
   }

   [Authorize("admin")]
   [Authorize(ClaimsIdentity.DefaultRoleClaimType, "admin")]
   public class AdminSecurePageVM : BaseVM
   {
      public string AdminCaption => "Only Admin role can see this";
   }
}
