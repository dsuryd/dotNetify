using System;
using DotNetify;
using DotNetify.Security;
using System.Threading;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates the security feature.
   /// </summary>
   [Authorize]
   [SetAccessToken]
   public class SecurePageVM : BaseVM
   {
      private Timer _timer;
      private string _userName;
      private SecurityToken _accessToken;

      public string SecureCaption
      {
         get { return Get<string>(); }
         set { Set(value); }
      }

      public string SecureData => $"Access token will expire in {TokenElapsedTime} seconds";
      public double TokenElapsedTime => _accessToken != null ? (_accessToken.ValidTo - DateTime.UtcNow).TotalSeconds : 0;

      public SecurePageVM(IPrincipalAccessor principalAccessor)
      {
         _userName = principalAccessor.Principal.Identity.Name;

         _timer = new Timer(state =>
         {
            Changed(nameof(SecureData));
            PushUpdates();
         }, null, 0, 1000); 
      }

      public override void Dispose() => _timer.Dispose();

      public void SetAccessToken(SecurityToken accessToken)
      {
         _accessToken = accessToken;
         SecureCaption = $"User \"{_userName}\" was authenticated by {_accessToken?.Issuer}";
      }
   }

   [Authorize("admin")]
   [Authorize(ClaimsIdentity.DefaultRoleClaimType, "admin")]
   public class AdminVM : BaseVM
   {
      public string AdminCaption => "Only Admin role can see this";
   }
}
