using DotNetify;
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

         public BasicReactiveVM(bool live) : this()
         {
            AddProperty("ServerTime", DateTime.MinValue)
               .SubscribeTo(Observable.Interval(TimeSpan.FromMilliseconds(200)).Select(_ => DateTime.Now).StartWith(now))
               .Subscribe(_ => PushUpdates());
         }

         private string FullNameDelegate(string firstName, string lastName) => $"{firstName} {lastName}";
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

      [TestMethod]
      public void BasicReactiveVM_Request()
      {
         var vmController = new MockVMController<BasicReactiveVM>();
         var response = vmController.RequestVM();

         Assert.AreEqual("Hello", response.GetVMProperty<string>("FirstName"));
         Assert.AreEqual("World", response.GetVMProperty<string>("LastName"));
         Assert.AreEqual("Hello World", response.GetVMProperty<string>("FullName"));
      }

      [TestMethod]
      public void BasicReactiveVM_Update()
      {
         var vmController = new MockVMController<BasicReactiveVM>();
         vmController.RequestVM();

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var response1 = vmController.UpdateVM(update);

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         var response2 = vmController.UpdateVM(update);

         Assert.AreEqual("John World", response1["FullName"]);
         Assert.AreEqual("John Doe", response2["FullName"]);

         Assert.AreEqual(10, response1["NameLength"]);
         Assert.AreEqual(8, response2["NameLength"]);
      }

      [TestMethod]
      public async Task BasicReactiveVM_UpdateAsync()
      {
         var vmController = new MockVMController<BasicReactiveVMAsync>();
         vmController.RequestVM();

         dynamic data1 = null;
         vmController.OnResponse += (sender, e) =>
         {
            dynamic data = JObject.Parse(e);
            if (data.NameLengthAsync != null)
            {
               if (data1 == null)
               {
                  data1 = data;
               }
            }
         };

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         var response1 = vmController.UpdateVM(update);

         Assert.AreEqual("John World", response1["FullName"]);

         await Task.Delay(2000);
         Assert.AreEqual(10, data1.NameLengthAsync.Value);
      }

      [TestMethod]
      public void BasicReactiveVM_Dispose()
      {
         bool dispose = false;
         var vm = new BasicReactiveVM();
         vm.Disposed += (sender, e) => dispose = true;

         var vmController = new MockVMController<BasicReactiveVM>(vm);
         vmController.RequestVM();

         vmController.DisposeVM();
         Assert.IsTrue(dispose);
      }

      [TestMethod]
      public void ReactiveVM_PushUpdates()
      {
         int updateCounter = 0;

         var vmController = new MockVMController<BasicReactiveVM>(new BasicReactiveVM(true));
         vmController.OnResponse += (sender, e) => updateCounter++;
         vmController.RequestVM();

         System.Threading.Thread.Sleep(1000);
         Assert.IsTrue(updateCounter >= 4);
      }

      [TestMethod]
      public void BasicReactiveVM_Internal()
      {
         var vmController = new MockVMController<BasicReactiveVM>();
         var response = vmController.RequestVM();

         // Internal properties should not be sent to the client.
         Assert.IsNull(response.VMData["Internal1"]);
         Assert.IsNull(response.VMData["Internal2"]);
      }
   }
}