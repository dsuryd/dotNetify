using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reactive.Linq;
using Rx = System.Reactive.Linq;
using DotNetify;

namespace UnitTests
{
   [TestClass]
   public class LiveChartVMTest
   {
      private class LiveChartVM : BaseVM
      {
         public long Data
         {
            get => Get<long>();
            set => Set(value);
         }

         public LiveChartVM()
         {
            Rx.Observable.Interval(TimeSpan.FromMilliseconds(200)).Subscribe(value =>
            {
               Data = value;
               PushUpdates();
            });
         }
      }

      [TestMethod]
      public void LiveChartVM_PushUpdates()
      {
         int updateCounter = 0;

         VMController.Register<LiveChartVM>();
         VMController.VMResponseDelegate handler = (string connectionId, string vmId, string vmData) =>
         {
            updateCounter++;
         };

         var vmController = new VMController(handler);
         vmController.OnRequestVM("conn1", typeof(LiveChartVM).Name);

         System.Threading.Thread.Sleep(1000);
         Assert.IsTrue(updateCounter >= 5);
      }
   }
}
