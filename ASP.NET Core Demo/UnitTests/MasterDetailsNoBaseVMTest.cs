using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using DotNetify;

namespace UnitTests
{
   [TestClass]
   public class MasterDetailsNoBaseVMTest
   {
      private ResponseStub _response = new ResponseStub();
      private MasterNoBaseVM _masterVM = new MasterNoBaseVM();

      private class MasterNoBaseVM : INotifyPropertyChanged, IMasterVM
      {
         private DetailsNoBaseVM _detailsVM = new DetailsNoBaseVM() { Value = int.MaxValue };

         public event EventHandler SubVMCreated;
         public event EventHandler SubVMDisposing;
         public event PropertyChangedEventHandler PropertyChanged = delegate { };

         public INotifyPropertyChanged GetSubVM(string vmTypeName, string vmInstanceId = null) => vmTypeName == nameof(DetailsNoBaseVM) ? _detailsVM : null;
         public void OnSubVMCreated(object subVM) => SubVMCreated?.Invoke(subVM, EventArgs.Empty);
         public void OnSubVMDisposing(object subVM) => SubVMDisposing?.Invoke(subVM, EventArgs.Empty);
      }

      private class DetailsNoBaseVM : INotifyPropertyChanged
      {
         public event PropertyChangedEventHandler PropertyChanged = delegate { };

         public int Value { get; set; }
      }

      [TestInitialize]
      public void Initialize()
      {
         VMController.Register<MasterNoBaseVM>();
         VMController.Register<DetailsNoBaseVM>();

         var baseDelegate = VMController.CreateInstance;
         VMController.CreateInstance = (type, args) => type == typeof(MasterNoBaseVM) ? _masterVM : baseDelegate(type, args);
      }

      [TestMethod]
      public void MasterDetailsNoBaseVM_Request()
      {
         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", $"{nameof(MasterNoBaseVM)}.{nameof(DetailsNoBaseVM)}");

         Assert.AreEqual($"{nameof(MasterNoBaseVM)}.{nameof(DetailsNoBaseVM)}", _response.VMId);

         var vm = _response.GetVM<DetailsNoBaseVM>();
         Assert.IsNotNull(vm);
         Assert.AreEqual(int.MaxValue, vm.Value);
      }

      [TestMethod]
      public void MasterDetailsNoBaseVM_Update()
      {
         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", $"{nameof(MasterNoBaseVM)}.{nameof(DetailsNoBaseVM)}");
         vmController.OnUpdateVM("conn1", $"{nameof(MasterNoBaseVM)}.{nameof(DetailsNoBaseVM)}", _response.MockAction("Value", "99"));

         Assert.AreEqual(99, (_masterVM.GetSubVM(nameof(DetailsNoBaseVM)) as DetailsNoBaseVM).Value);
      }

      [TestMethod]
      public void MasterDetailsNoBaseVM_SubVMCreated()
      {
         object subVM = null;
         bool subVMCreated = false;
         _masterVM.SubVMCreated += (sender, e) => { subVM = sender; subVMCreated = true; };

         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", $"{nameof(MasterNoBaseVM)}.{nameof(DetailsNoBaseVM)}");

         Assert.IsTrue(subVMCreated);
         Assert.IsTrue(subVM is DetailsNoBaseVM);
      }

      [TestMethod]
      public void MasterDetailsNoBaseVM_SubVMDisposing()
      {
         object subVM = null;
         bool subVMDisposing = false;
         _masterVM.SubVMDisposing += (sender, e) => { subVM = sender; subVMDisposing = true; };

         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", $"{nameof(MasterNoBaseVM)}.{nameof(DetailsNoBaseVM)}");

         vmController.OnDisposeVM("conn1", $"{nameof(MasterNoBaseVM)}.{nameof(DetailsNoBaseVM)}");
         Assert.IsTrue(subVMDisposing);
         Assert.IsTrue(subVM is DetailsNoBaseVM);
      }
   }
}
