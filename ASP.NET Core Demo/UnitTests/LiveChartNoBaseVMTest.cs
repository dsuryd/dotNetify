using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reactive.Linq;
using Rx = System.Reactive.Linq;
using System.ComponentModel;
using DotNetify;

namespace UnitTests
{
   [TestClass]
   public class LiveChartNoBaseVMTest
   {
      private class LiveChartNoBaseVM : INotifyPropertyChanged, IPushUpdates
      {
         public long Data { get; set; }

         public event PropertyChangedEventHandler PropertyChanged;
         public event EventHandler RequestPushUpdates;

         public LiveChartNoBaseVM()
         {
            Rx.Observable.Interval(TimeSpan.FromMilliseconds(200)).Subscribe(value =>
            {
               Data = value;
               this.Changed(nameof(Data));
               this.PushUpdates();
            });
         }
      }

      [TestMethod]
      public void LiveChartNoBaseVM_PushUpdates()
      {
         int updateCounter = 0;

         VMController.Register<LiveChartNoBaseVM>();
         VMController.VMResponseDelegate handler = (string connectionId, string vmId, string vmData) =>
         {
            updateCounter++;
         };

         var vmController = new VMController(handler);
         vmController.OnRequestVM("conn1", typeof(LiveChartNoBaseVM
            ).Name);

         System.Threading.Thread.Sleep(1000);
         Assert.IsTrue(updateCounter >= 5);
      }
   }
}
