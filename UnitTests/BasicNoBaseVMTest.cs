using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using DotNetify;
using DotNetify.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Rx = System.Reactive.Linq;

namespace UnitTests
{
   [TestClass]
   public class BasicNoBaseVMTest
   {
      private class BasicNoBaseVM : INotifyPropertyChanged, IPushUpdates, IDisposable
      {
         private string _firstName;
         private string _lastName;

         public event PropertyChangedEventHandler PropertyChanged = delegate { };

         public event EventHandler RequestPushUpdates = delegate { };

         public event EventHandler Disposed;

         public string FirstName
         {
            get => _firstName ?? "Hello";
            set
            {
               _firstName = value;
               this.Changed(nameof(FullName));
            }
         }

         public string LastName
         {
            get => _lastName ?? "World";
            set
            {
               _lastName = value;
               this.Changed(nameof(FullName));
            }
         }

         public long Data { get; set; }

         public string FullName => $"{FirstName} {LastName}";

         public BasicNoBaseVM()
         { }

         public void Dispose() => Disposed?.Invoke(this, EventArgs.Empty);
      }

      private class BasicNoBaseVMLive : BasicNoBaseVM
      {
         public BasicNoBaseVMLive() : base()
         {
            Rx.Observable.Interval(TimeSpan.FromMilliseconds(200)).Subscribe(value =>
            {
               Data = value;
               this.Changed(nameof(Data));
               this.PushUpdates();
            });
         }
      }

      private HubEmulator _hubEmulator;

      [TestInitialize]
      public void Initialize()
      {
         _hubEmulator = new HubEmulatorBuilder()
            .Register<BasicNoBaseVM>()
            .Register<BasicNoBaseVMLive>()
            .Build();
      }

      [TestMethod]
      public void BasicNoBaseVM_Request()
      {
         var client = _hubEmulator.CreateClient();
         var response = client.Connect(nameof(BasicNoBaseVM)).As<dynamic>();

         Assert.AreEqual("Hello", (string) response.FirstName);
         Assert.AreEqual("World", (string) response.LastName);
         Assert.AreEqual("Hello World", (string) response.FullName);
      }

      [TestMethod]
      public void BasicNoBaseVM_Update()
      {
         var client = _hubEmulator.CreateClient();
         var response = client.Connect(nameof(BasicNoBaseVM)).As<dynamic>();

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var response1 = client.Dispatch(update).As<dynamic>();

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         var response2 = client.Dispatch(update).As<dynamic>();

         Assert.AreEqual("John World", (string) response1.FullName);
         Assert.AreEqual("John Doe", (string) response2.FullName);
      }

      [TestMethod]
      public void BasicNoBaseVM_Dispose()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(BasicNoBaseVM));

         bool dispose = false;
         var vm = _hubEmulator.CreatedVMs.Find(x => x is BasicNoBaseVM) as BasicNoBaseVM;
         vm.Disposed += (sender, e) => dispose = true;

         client.Destroy();
         Assert.IsTrue(dispose);
      }

      [TestMethod]
      public void BasicNoBaseVM_PushUpdates()
      {
         var client = _hubEmulator.CreateClient();
         var response = client.Connect(nameof(BasicNoBaseVMLive)).As<dynamic>();

         var responses = client.Listen(1000);
         Assert.IsTrue(responses.Count >= 3, $"{response.Count}");
      }
   }
}