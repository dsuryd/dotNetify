using System;
using System.Threading;
using DotNetify;

namespace Blazor.Server
{
   public class RealTimePush : BaseVM
   {
      private Timer _timer;
      public string Greetings => "Hello World!";
      public DateTime ServerTime => DateTime.Now;

      public RealTimePush()
      {
         _timer = new Timer(state =>
         {
            Changed(nameof(ServerTime));
            PushUpdates();
         }, null, 0, 1000); // every 1000 ms.
      }

      public override void Dispose() => _timer.Dispose();
   }
}