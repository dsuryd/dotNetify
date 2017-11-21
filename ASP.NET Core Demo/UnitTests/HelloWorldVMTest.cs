using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DotNetify;

namespace UnitTests
{
   [TestClass]
   public class HelloWorldVMTest
   {
      private ResponseStub _response = new ResponseStub();

      private class HelloWorldVM : BaseVM
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

         public string FullName => $"{FirstName} {LastName}";
      }

      [TestMethod]
      public void HelloWorldVM_Request()
      {
         VMController.Register<HelloWorldVM>();

         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", typeof(HelloWorldVM).Name);

         Assert.AreEqual(typeof(HelloWorldVM).Name, _response.VMId);
         var vm = _response.GetVM<HelloWorldVM>();
         Assert.IsNotNull(vm);
         Assert.AreEqual("Hello", vm.FirstName);
         Assert.AreEqual("World", vm.LastName);
         Assert.AreEqual("Hello World", vm.FullName);
      }

      [TestMethod]
      public void HelloWorldVM_Update()
      {
         VMController.Register<HelloWorldVM>();

         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", typeof(HelloWorldVM).Name);

         var update = new Dictionary<string, object>() { { "FirstName", "John" } };
         vmController.OnUpdateVM("conn1", typeof(HelloWorldVM).Name, update);

         Assert.IsNotNull(_response.VMData);
         Assert.AreEqual("John World", _response.VMData["FullName"]);

         update = new Dictionary<string, object>() { { "LastName", "Doe" } };
         vmController.OnUpdateVM("conn1", typeof(HelloWorldVM).Name, update);

         Assert.IsNotNull(_response.VMData);
         Assert.AreEqual("John Doe", _response.VMData["FullName"]);
      }

      [TestMethod]
      public void HelloWorldVM_Dispose()
      {
         bool dispose = false;
         var vm = new HelloWorldVM();
         vm.Disposed += (sender, e) => dispose = true;

         var baseDelegate = VMController.CreateInstance;
         VMController.CreateInstance = (type, args) => type == typeof(HelloWorldVM) ? vm : baseDelegate(type, args);
         VMController.Register<HelloWorldVM>();

         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", typeof(HelloWorldVM).Name);

         vmController.OnDisposeVM("conn1", typeof(HelloWorldVM).Name);
         Assert.IsTrue(dispose);
      }
   }
}
