using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using DotNetify;

namespace UnitTests
{
   [TestClass]
   public class HelloWorldNoBaseVMTest
   {
      private ResponseStub _response = new ResponseStub();

      private class HelloWorldNoBaseVM  : INotifyPropertyChanged, IDisposable
      {
         private string _firstName;
         private string _lastName;

         public event PropertyChangedEventHandler PropertyChanged = delegate { };
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

         public string FullName => $"{FirstName} {LastName}";

         public void Dispose() => Disposed?.Invoke(this, EventArgs.Empty);
      }

      [TestMethod]
      public void HelloWorldNoBaseVM_Request()
      {
         VMController.Register<HelloWorldNoBaseVM>();

         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", typeof(HelloWorldNoBaseVM).Name);

         Assert.AreEqual(typeof(HelloWorldNoBaseVM).Name, _response.VMId);
         var vm = _response.GetVM<HelloWorldNoBaseVM>();
         Assert.IsNotNull(vm);
         Assert.AreEqual("Hello", vm.FirstName);
         Assert.AreEqual("World", vm.LastName);
         Assert.AreEqual("Hello World", vm.FullName);
      }

      [TestMethod]
      public void HelloWorldNoBaseVM_Update()
      {
         VMController.Register<HelloWorldNoBaseVM>();

         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", typeof(HelloWorldNoBaseVM).Name);

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         vmController.OnUpdateVM("conn1", typeof(HelloWorldNoBaseVM).Name, update);

         Assert.IsNotNull(_response.VMData);
         Assert.AreEqual("John World", _response.VMData["FullName"]);

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         vmController.OnUpdateVM("conn1", typeof(HelloWorldNoBaseVM).Name, update);

         Assert.IsNotNull(_response.VMData);
         Assert.AreEqual("John Doe", _response.VMData["FullName"]);
      }

      [TestMethod]
      public void HelloWorldNoBaseVM_Dispose()
      {
         bool dispose = false;
         var vm = new HelloWorldNoBaseVM();
         vm.Disposed += (sender, e) => dispose = true;

         var baseDelegate = VMController.CreateInstance;
         VMController.CreateInstance = (type, args) => type == typeof(HelloWorldNoBaseVM) ? vm : baseDelegate(type, args);
         VMController.Register<HelloWorldNoBaseVM>();

         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", typeof(HelloWorldNoBaseVM).Name);

         vmController.OnDisposeVM("conn1", typeof(HelloWorldNoBaseVM).Name);
         Assert.IsTrue(dispose);
      }
   }
}
