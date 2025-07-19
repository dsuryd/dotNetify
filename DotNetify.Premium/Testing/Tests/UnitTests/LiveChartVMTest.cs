using DotNetify.Testing;
using System.Linq;
using DevApp.ViewModels;
using Xunit;

namespace DotNetify.Testing.UnitTests
{
   public class LiveChartVMTest
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

      public LiveChartVMTest()
      {
         _hubEmulator = new HubEmulatorBuilder()
            .Register<LiveChartVM>(nameof(LiveChartVM))
            .Build();
      }

      [Fact]
      public void Connect_ReturnsInitialState()
      {
         var response = _hubEmulator
            .CreateClient()
            .Connect(nameof(LiveChartVM))
            .As<ClientState>();

         Assert.Equal(60, response.Waveform.Length);
         Assert.Equal(8, response.Bar.Length);
         Assert.Equal(3, response.Pie.Length);
      }

      [Fact]
      public void FiveSecondsDuration_ReturnsDataEverySecond()
      {
         var client = _hubEmulator.CreateClient();

         client.Connect(nameof(LiveChartVM));
         var response = client.Listen(5500);

         Assert.Equal(5, response.Count);
         Assert.Equal(2, response.First().As<ServerUpdate>().Waveform_add.Length);
         Assert.Equal(12, response.First().As<ServerUpdate>().Bar.Length);
         Assert.Equal(3, response.First().As<ServerUpdate>().Pie.Length);
         Assert.Equal(2, response.Last().As<ServerUpdate>().Waveform_add.Length);
         Assert.Equal(12, response.Last().As<ServerUpdate>().Bar.Length);
         Assert.Equal(3, response.Last().As<ServerUpdate>().Pie.Length);

         Assert.Equal(70, client.GetState<ClientState>().Waveform.Length);
      }
   }
}