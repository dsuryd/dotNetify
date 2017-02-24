using System;
using System.Linq;
using System.Threading;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates real-time push of data from the server to the client.
   /// </summary>
   public class LiveChartVM : BaseVM
   {
      private Timer _timer;
      private int _count = 20;
      private const int _timeInterval = 1000;
      private Random _random = new Random();

      public int[][] InitialData => Enumerable
         .Range(0, _count)
         .Select(x => Enumerable.Range(0, 2).Select(y => y == 0 ? x : _random.Next(1, 50)).ToArray()).ToArray();

      public int[] NextData
      {
         get { return Get<int[]>(); }
         set { Set(value); }
      }

      public LiveChartVM()
      {
         _timer = new Timer(state =>
         {
            NextData = new int[] { _count++, _random.Next(1, 50) };
            PushUpdates(); // Base method to push changed properties from all active view models to the browser.
         }, null, _timeInterval, _timeInterval);
      }

      public override void Dispose()
      {
         _timer.Dispose();
         base.Dispose();   // Call base.Dispose to raise Disposed event.
      }
   }
}
