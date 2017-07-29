using System;
using DotNetify;
using DotNetify.Security;
using System.Threading;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates the security feature.
   /// </summary>
   [Authorize]
   public class SecurePageVM : BaseVM
   {
      private Timer _timer;

      public string SecureCaption => "Only authenticated user can see this live clock.";
      public string SecureData => DateTime.Now.ToString();

      public SecurePageVM()
      {
         _timer = new Timer(state =>
         {
            Changed(nameof(SecureData));
            PushUpdates();
         }, null, 0, 1000); 
      }

      public override void Dispose() => _timer.Dispose();
   }
}
