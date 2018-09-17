using System;
using DotNetify;
using DotNetify.Security;
using System.Threading;
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
      private int AccessExpireTime => (int)(_accessToken.ValidTo - DateTime.UtcNow).TotalSeconds;

      public string SecureCaption { get; set; }
      public string SecureData { get; set; }

      public SecurePageVM(IPrincipalAccessor principalAccessor)
      {
         _userName = principalAccessor.Principal?.Identity.Name;
      }

      public override void Dispose() => _timer?.Dispose();

      public void SetAccessToken(SecurityToken accessToken)
      {
         _accessToken = accessToken;
         SecureCaption = $"Authenticated user: \"{_userName}\"";
         Changed(nameof(SecureCaption));

         _timer = _timer ?? new Timer(state =>
         {
            SecureData = _accessToken != null ? $"Access token will expire in {AccessExpireTime} seconds" : null;
            Changed(nameof(SecureData));
            PushUpdates();
         }, null, 0, 1000);
      }
   }

   [Authorize(Role = "admin")]
   [SetAccessToken]
   public class AdminSecurePageVM : BaseVM
   {
      public string TokenIssuer { get; set; }
      public string TokenValidFrom { get; set; }
      public string TokenValidTo { get; set; }

      public void SetAccessToken(SecurityToken accessToken)
      {
         TokenIssuer = $"Token issuer: \"{accessToken.Issuer}\"";
         TokenValidFrom = $"Valid from: {accessToken.ValidFrom.ToString("R")}";
         TokenValidTo = $"Valid to: {accessToken.ValidTo.ToString("R")}";

         Changed(nameof(TokenIssuer));
         Changed(nameof(TokenValidFrom));
         Changed(nameof(TokenValidTo));
      }
   }
}
