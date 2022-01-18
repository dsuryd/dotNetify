using System;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using DotNetify;
using DotNetify.Security;
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

         void Reset(int count);
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
               .Interval(TimeSpan.FromMilliseconds(200))
               .Select(_ => $"{_count++}");

            ServerUsage = Observable
               .Interval(TimeSpan.FromMilliseconds(400))
               .StartWith(0)
               .Select(_ => Enumerable.Range(1, 10).Select(i => _random.Next(1, 100)).ToArray());
         }

         public void Reset(int count) => _count = count;
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
         builder.Services.AddDotNetify().AddSignalR();
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
         builder.Services.AddDotNetify().AddSignalR();
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
      public async Task MinimalApiTest_WithCommand_CommandsExecuted()
      {
         var vmName = "LiveDataWithCommand";
         var builder = WebApplication.CreateBuilder();
         builder.Services.AddDotNetify().AddSignalR();
         builder.Services.AddScoped<ILiveDataService, LiveDataService>();
         var app = builder.Build();

         app.MapVM(vmName, (ILiveDataService live) => new { Tick = live.Tick, Reset = new Action<int>(x => live.Reset(x)) });

         var vm = VMController.VMTypes.Find(x => x.Name == vmName).CreateInstance();

         var hubEmulator = new HubEmulatorBuilder()
            .Register(vmName, vm)
            .Build();

         var client = hubEmulator.CreateClient();

         client.Connect(vmName);

         var task = client.ListenAsync(1000);

         client.Dispatch(new { Reset = 1000 });
         var response = (await task).As<LiveDataState>();

         Assert.IsTrue(int.Parse(response.Tick) >= 1000);
      }

      [TestMethod]
      public void MinimalApiTest_WithAuthorizeOnUnauthenticatedClient_AccessDenied()
      {
         var vmName = "HelloWorldAccessDenied";
         var builder = WebApplication.CreateBuilder();
         builder.Services.AddDotNetify().AddSignalR();
         var app = builder.Build();

         app.MapVM(vmName, [Authorize]() => new { FirstName = "Hello", LastName = "World" });

         var vm = VMController.VMTypes.Find(x => x.Name == vmName).CreateInstance();

         var hubEmulator = new HubEmulatorBuilder()
            .Register(vmName, vm)
            .UseFilter<AuthorizeFilter>()
            .Build();

         var identity = Stubber.Create<IIdentity>().Setup(x => x.AuthenticationType).Returns(string.Empty).Object;
         var client = hubEmulator.CreateClient(user: null);

         var response = client.Connect(vmName);
         Assert.IsTrue(response.First().ToString().Contains(nameof(UnauthorizedAccessException)));
      }

      [TestMethod]
      public void MinimalApiTest_WithAuthorizeOnAuthenticatedClient_AccessGranted()
      {
         var vmName = "HelloWorldAccessGranted";
         var builder = WebApplication.CreateBuilder();
         builder.Services.AddDotNetify().AddSignalR();
         var app = builder.Build();

         app.MapVM(vmName, [Authorize]() => new { FirstName = "Hello", LastName = "World" });

         var vm = VMController.VMTypes.Find(x => x.Name == vmName).CreateInstance();

         var hubEmulator = new HubEmulatorBuilder()
            .Register(vmName, vm)
            .UseFilter<AuthorizeFilter>()
            .Build();

         var identity = Stubber.Create<IIdentity>().Setup(x => x.AuthenticationType).Returns("CustomAuth").Object;
         var client = hubEmulator.CreateClient(user: new ClaimsPrincipal(identity));

         var response = client.Connect(vmName).As<HelloWorldState>();

         Assert.AreEqual("Hello", response.FirstName);
         Assert.AreEqual("World", response.LastName);
      }
   }
}