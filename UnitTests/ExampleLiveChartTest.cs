using System.Linq;
using DotNetify.DevApp;
using DotNetify.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
   [TestClass]
   public class ExampleLiveChartTest
   {
      private HubEmulator _hubEmulator;

      private struct ClientState
      {
         public string[,] Waveform { get; set; }
         public int[] Bar { get; set; }
         public double[] Pie { get; set; }
      }

      private struct ServerUpdate
      {
         public string[] Waveform_add { get; set; }
         public int[] Bar { get; set; }
         public double[] Pie { get; set; }
      }

      public ExampleLiveChartTest()
      {
         _hubEmulator = new HubEmulatorBuilder()
            .Register<LiveChartVM>()
            .Build();
      }

      [TestMethod]
      public void ExampleLiveChart_Connect_ReturnsInitialState()
      {
         var response = _hubEmulator
            .CreateClient()
            .Connect(nameof(LiveChartVM))
            .As<ClientState>();

         Assert.AreEqual(60, response.Waveform.Length);
         Assert.AreEqual(8, response.Bar.Length);
         Assert.AreEqual(3, response.Pie.Length);
      }

      [TestMethod]
      public void ExampleLiveChart_FiveSecondsDuration_ReturnsDataEverySecond()
      {
         var client = _hubEmulator.CreateClient();

         client.Connect(nameof(LiveChartVM));
         var response = client.Listen(5500);

         Assert.AreEqual(5, response.Count);
         Assert.AreEqual(2, response.First().As<ServerUpdate>().Waveform_add.Length);
         Assert.AreEqual(12, response.First().As<ServerUpdate>().Bar.Length);
         Assert.AreEqual(3, response.First().As<ServerUpdate>().Pie.Length);
         Assert.AreEqual(2, response.Last().As<ServerUpdate>().Waveform_add.Length);
         Assert.AreEqual(12, response.Last().As<ServerUpdate>().Bar.Length);
         Assert.AreEqual(3, response.Last().As<ServerUpdate>().Pie.Length);

         Assert.AreEqual(70, client.GetState<ClientState>().Waveform.Length);
      }
   }
}