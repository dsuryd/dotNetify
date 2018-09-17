using System;
using System.Timers;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This examples demonstrates real time push notification that updates a chart on the browser every second.
   /// </summary>
   public class LiveChartVM : BaseVM
   {
      private Timer _Timer;
      private int _Label;
      private Random _Random = new Random();

      public double[,] Data
      {
         get { return Get<double[,]>(); }
         set { Set(value); }
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      public LiveChartVM()
      {
         // Create initial data for the chart.
         Data = new double[20, 2];
         for (_Label = 0; _Label < 20; _Label++)
         {
            Data[_Label, 0] = _Label;
            Data[_Label, 1] = _Random.Next(1, 50);
         }

         // Run a timer every second to update the chart.
         _Timer = new Timer(1000);
         _Timer.Elapsed += Timer_Elapsed;
         _Timer.Start();
      }

      public override void Dispose()
      {
         _Timer.Stop();
         _Timer.Elapsed -= Timer_Elapsed;

         // Call base.Dispose to raise Disposed event.
         base.Dispose();
      }

      private void Timer_Elapsed(object sender, ElapsedEventArgs e)
      {
         Data = new double[,] { { _Label++, _Random.Next(1, 50) } };

         // This is a base method to cause changed properties from all active view models to be pushed to the browser.
         PushUpdates();
      }
   }
}
