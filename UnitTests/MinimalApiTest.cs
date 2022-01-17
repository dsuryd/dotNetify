using System;
using System.Linq;
using System.Reactive.Linq;
using DotNetify;
using DotNetify.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
   [TestClass]
   public class MinimalApiTest
   {
      public interface ILiveDataService
      {
         IObservable<string> Tick { get; }
         IObservable<int[]> ServerUsage { get; }

         void Reset();
      }

      public class LiveDataService : ILiveDataService
      {
         private readonly Random _random = new Random();
         private int _count = 0;

         public IObservable<string> Tick { get; }
         public IObservable<int[]> ServerUsage { get; }

         public LiveDataService()
         {
            Tick = Observable
               .Interval(TimeSpan.FromMilliseconds(100))
               .Select(_ => $"{_count++}");

            ServerUsage = Observable
               .Interval(TimeSpan.FromMilliseconds(400))
               .StartWith(0)
               .Select(_ => Enumerable.Range(1, 10).Select(i => _random.Next(1, 100)).ToArray());
         }

         public void Reset() => _count = 0;
      }

      private struct HelloWorldState
      {
         public string FirstName { get; set; }
         public string LastName { get; set; }
      }

      private struct LiveDataState
      {
         public string Tick { get; set; }
         public int[,] ServerUsage { get; set; }
      }

      [TestMethod]
      public void MinimalApiTest_WithStaticValue_ReturnsInitialState()
      {
         var vmName = "HelloWorld";
         var builder = WebApplication.CreateBuilder();
         var app = builder.Build();

         app.MapVM(vmName, () => new { FirstName = "Hello", LastName = "World" });

         var vm = VMController.VMTypes.Find(x => x.Name == vmName).CreateInstance();

         var hubEmulator = new HubEmulatorBuilder()
            .Register(vmName, vm)
            .Build();

         var client = hubEmulator.CreateClient();

         var response = client.Connect(vmName).As<HelloWorldState>();

         Assert.AreEqual("Hello", response.FirstName);
         Assert.AreEqual("World", response.LastName);
      }

      [TestMethod]
      public void MinimalApiTest_WithObservable_ReturnsStatePeriodically()
      {
         var vmName = "LiveData";
         var builder = WebApplication.CreateBuilder();
         builder.Services.AddScoped<ILiveDataService, LiveDataService>();
         var app = builder.Build();

         app.MapVM(vmName, (ILiveDataService live) => new { ServerUsage = live.ServerUsage });

         var vm = VMController.VMTypes.Find(x => x.Name == vmName).CreateInstance();

         var hubEmulator = new HubEmulatorBuilder()
            .Register(vmName, vm)
            .Build();

         var client = hubEmulator.CreateClient();

         client.Connect(vmName);
         var response = client.Listen(2100);

         Assert.IsTrue(response.Count >= 5);
      }

      [TestMethod]
      public void MinimalApiTest_WithCommand_CommandsExecuted()
      {
         var vmName = "LiveData";
         var builder = WebApplication.CreateBuilder();
         builder.Services.AddScoped<ILiveDataService, LiveDataService>();
         var app = builder.Build();

         app.MapVM(vmName, (ILiveDataService live) => new { Tick = live.Tick, Reset = new Action(() => live.Reset()) });

         var vm = VMController.VMTypes.Find(x => x.Name == vmName).CreateInstance();

         var hubEmulator = new HubEmulatorBuilder()
            .Register(vmName, vm)
            .Build();

         var client = hubEmulator.CreateClient();

         client.Connect(vmName);
         var response = client.Listen(1000).As<LiveDataState>();

         Assert.IsTrue(int.Parse(response.Tick) >= 5);

         client.Dispatch(new { Reset = "" });
         response = client.Listen(200).As<LiveDataState>();
         Assert.IsTrue(int.Parse(response.Tick) < 5);
      }
   }
}