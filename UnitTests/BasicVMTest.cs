using DotNetify;
using DotNetify.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace UnitTests
{
   [TestClass]
   public class BasicVMTest
   {
      private class BasicVM : BaseVM
      {
         public string FirstName
         {
            get => Get<string>() ?? "Hello";
            set
            {
               Set(value);
               Changed(nameof(FullName));
            }
         }

         public string LastName
         {
            get => Get<string>() ?? "World";
            set
            {
               Set(value);
               Changed(nameof(FullName));
            }
         }

         public long Data
         {
            get => Get<long>();
            set => Set(value);
         }

         public string FullName => $"{FirstName} {LastName}";

         public BasicVM()
         { }
      }

      private class BasicVMLive : BasicVM
      {
         public BasicVMLive() : base()
         {
            Observable.Interval(TimeSpan.FromMilliseconds(200)).Subscribe(value =>
            {
               Data = value;
               PushUpdates();
            });
         }
      }

      private HubEmulator _hubEmulator;

      [TestInitialize]
      public void Initialize()
      {
         _hubEmulator = new HubEmulatorBuilder()
            .Register<BasicVM>()
            .Register<BasicVMLive>()
            .Build();
      }

      [TestMethod]
      public void BasicVM_Request()
      {
         var client = _hubEmulator.CreateClient();
         var response = client.Connect(nameof(BasicVM)).As<dynamic>();

         Assert.AreEqual("Hello", (string) response.FirstName);
         Assert.AreEqual("World", (string) response.LastName);
         Assert.AreEqual("Hello World", (string) response.FullName);
      }

      [TestMethod]
      public void BasicVM_Update()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(BasicVM)).As<dynamic>();

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var response1 = client.Dispatch(update).As<dynamic>();

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         var response2 = client.Dispatch(update).As<dynamic>();

         Assert.AreEqual("John World", (string) response1.FullName);
         Assert.AreEqual("John Doe", (string) response2.FullName);

         Assert.IsFalse(response1.ContainsKey("FirstName"));
         Assert.IsFalse(response2.ContainsKey("LastName"));
      }

      [TestMethod]
      public void BasicVM_Dispose()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(BasicVM)).As<dynamic>();

         bool dispose = false;

         var vm = _hubEmulator.CreatedVMs.Find(x => x is BasicVM) as BasicVM;
         vm.Disposed += (sender, e) => dispose = true;

         client.Destroy();
         Assert.IsTrue(dispose);
      }

      [TestMethod]
      public void BasicVM_PushUpdates()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(BasicVMLive)).As<dynamic>();

         var responses = client.Listen(1000);
         Assert.IsTrue(responses.Count >= 3, $"{responses.Count}");
      }
   }
}