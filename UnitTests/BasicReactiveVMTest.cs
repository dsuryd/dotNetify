using DotNetify;
using DotNetify.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests
{
   [TestClass]
   public class BasicReactiveVMTest
   {
      private static DateTime now = DateTime.Now;

      private class BasicReactiveVM : BaseVM
      {
         public ReactiveProperty<string> FirstName { get; set; } = "Hello";
         public ReactiveProperty<string> LastName { get; set; } = "World";

         public BasicReactiveVM()
         {
            AddProperty<string>("FullName")
               .SubscribeTo(Observable.CombineLatest(FirstName, LastName, FullNameDelegate))
               .SubscribedBy(
                  AddProperty<int>("NameLength"), x => x.Select(name => name.Length)
               );

            AddInternalProperty<string>("Internal1");
            AddInternalProperty("Internal2", 0);
         }

         private string FullNameDelegate(string firstName, string lastName) => $"{firstName} {lastName}";
      }

      private class BasicReactiveVMLive : BasicReactiveVM
      {
         public BasicReactiveVMLive() : base()
         {
            AddProperty("ServerTime", DateTime.MinValue)
               .SubscribeTo(Observable.Interval(TimeSpan.FromMilliseconds(200)).Select(_ => DateTime.Now).StartWith(now))
               .Subscribe(_ => PushUpdates());
         }
      }

      private class BasicReactiveVMAsync : BaseVM
      {
         public ReactiveProperty<string> FirstName { get; set; } = "Hello";
         public ReactiveProperty<string> LastName { get; set; } = "World";

         public BasicReactiveVMAsync()
         {
            AddProperty<string>("FullName")
               .SubscribeTo(Observable.CombineLatest(FirstName, LastName, FullNameDelegate))
               .SubscribedByAsync(
                  AddProperty<int>("NameLengthAsync").SubscribedBy(_ => PushUpdates(), out IDisposable subs),
                  async x => await GetNameLengthAsync(x));
         }

         private async Task<int> GetNameLengthAsync(string name)
         {
            await Task.Delay(1000);
            return name.Length;
         }

         private string FullNameDelegate(string firstName, string lastName) => $"{firstName} {lastName}";
      }

      private HubEmulator _hubEmulator;

      [TestInitialize]
      public void Initialize()
      {
         _hubEmulator = new HubEmulatorBuilder()
            .Register<BasicReactiveVM>()
            .Register<BasicReactiveVMLive>()
            .Register<BasicReactiveVMAsync>()
            .Build();
      }

      [TestMethod]
      public void BasicReactiveVM_Request()
      {
         var client = _hubEmulator.CreateClient();
         var response = client.Connect(nameof(BasicReactiveVM)).As<dynamic>();

         Assert.AreEqual("Hello", (string) response.FirstName);
         Assert.AreEqual("World", (string) response.LastName);
         Assert.AreEqual("Hello World", (string) response.FullName);
      }

      [TestMethod]
      public void BasicReactiveVM_Update()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(BasicReactiveVM));

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var response1 = client.Dispatch(update).As<dynamic>();

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         var response2 = client.Dispatch(update).As<dynamic>();

         Assert.AreEqual("John World", (string) response1.FullName);
         Assert.AreEqual("John Doe", (string) response2.FullName);

         Assert.AreEqual(10, (int) response1.NameLength);
         Assert.AreEqual(8, (int) response2.NameLength);
      }

      [TestMethod]
      public async Task BasicReactiveVM_UpdateAsync()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(BasicReactiveVMAsync));

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var response1 = client.Dispatch(update).As<dynamic>();

         Assert.AreEqual("John World", (string) response1.FullName);

         var response2 = (await client.ListenAsync(2000)).As<dynamic>();

         Assert.AreEqual(10, (int) response2.NameLengthAsync);
      }

      [TestMethod]
      public void BasicReactiveVM_Dispose()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(BasicReactiveVM));

         bool dispose = false;
         var vm = _hubEmulator.CreatedVMs.Find(x => x is BasicReactiveVM) as BasicReactiveVM;
         vm.Disposed += (sender, e) => dispose = true;

         client.Destroy();
         Assert.IsTrue(dispose);
      }

      [TestMethod]
      public void ReactiveVM_PushUpdates()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(BasicReactiveVMLive));

         var responses = client.Listen(1000);
         Assert.IsTrue(responses.Count >= 3, $"{responses.Count}");
      }

      [TestMethod]
      public void BasicReactiveVM_Internal()
      {
         var client = _hubEmulator.CreateClient();
         var response = client.Connect(nameof(BasicReactiveVM)).As<dynamic>();

         // Internal properties should not be sent to the client.
         Assert.IsNull(response.Internal1);
         Assert.IsNull(response.Internal2);
      }
   }
}