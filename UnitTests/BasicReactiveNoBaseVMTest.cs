using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using DotNetify;
using DotNetify.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
   [TestClass]
   public class BasicReactiveNoBaseVMTest
   {
      private static DateTime now = DateTime.Now;

      private class BasicReactiveNoBaseVM : INotifyPropertyChanged, IPushUpdates, IReactiveProperties
      {
         public event EventHandler RequestPushUpdates = delegate { };

         public event PropertyChangedEventHandler PropertyChanged = delegate { };

         public IList<IReactiveProperty> RuntimeProperties { get; } = new List<IReactiveProperty>();

         public BasicReactiveNoBaseVM()
         {
            var firstName = this.AddProperty("FirstName", "Hello");
            var lastName = this.AddProperty("LastName", "World");

            this.AddProperty<string>("FullName")
               .SubscribeTo(Observable.CombineLatest(firstName, lastName, FullNameDelegate));
         }

         private string FullNameDelegate(string firstName, string lastName) => $"{firstName} {lastName}";
      }

      private class BasicReactiveNoBaseVMLive : BasicReactiveNoBaseVM
      {
         public BasicReactiveNoBaseVMLive() : base()
         {
            this.AddProperty("ServerTime", DateTime.MinValue)
               .SubscribeTo(Observable.Interval(TimeSpan.FromMilliseconds(200)).Select(_ => DateTime.Now).StartWith(now))
               .Subscribe(_ => this.PushUpdates());
         }
      }

      private HubEmulator _hubEmulator;

      [TestInitialize]
      public void Initialize()
      {
         _hubEmulator = new HubEmulatorBuilder()
            .Register<BasicReactiveNoBaseVM>()
            .Register<BasicReactiveNoBaseVMLive>()
            .Build();
      }

      [TestMethod]
      public void BasicReactiveNoBaseVM_Request()
      {
         var client = _hubEmulator.CreateClient();
         var response = client.Connect(nameof(BasicReactiveNoBaseVM)).As<dynamic>();

         Assert.AreEqual("Hello", (string) response.FirstName);
         Assert.AreEqual("World", (string) response.LastName);
         Assert.AreEqual("Hello World", (string) response.FullName);
      }

      [TestMethod]
      public void BasicReactiveNoBaseVM_Update()
      {
         var client = _hubEmulator.CreateClient();
         var response = client.Connect(nameof(BasicReactiveNoBaseVM)).As<dynamic>();

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var response1 = client.Dispatch(update).As<dynamic>();

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         var response2 = client.Dispatch(update).As<dynamic>();

         Assert.AreEqual("John World", (string) response1.FullName);
         Assert.AreEqual("John Doe", (string) response2.FullName);
      }

      [TestMethod]
      public void BasicReactiveNoBaseVM_PushUpdates()
      {
         var client = _hubEmulator.CreateClient();
         var response = client.Connect(nameof(BasicReactiveNoBaseVMLive)).As<dynamic>();

         var responses = client.Listen(1000);
         Assert.IsTrue(responses.Count >= 3, $"{response.Count}");
      }
   }
}