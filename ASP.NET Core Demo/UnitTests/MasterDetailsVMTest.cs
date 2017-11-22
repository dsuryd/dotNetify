using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DotNetify;

namespace UnitTests
{
   [TestClass]
   public class MasterDetailsVMTest
   {
      private ResponseStub _response = new ResponseStub();
      private MasterVM _masterVM = new MasterVM();

      private class MasterVM : BaseVM
      {
         private DetailsVM _detailsVM = new DetailsVM() { Value = int.MaxValue };

         public event EventHandler SubVMCreated;
         public event EventHandler SubVMDisposing;

         public override BaseVM GetSubVM(string vmTypeName) => vmTypeName == nameof(DetailsVM) ? _detailsVM : base.GetSubVM(vmTypeName);
         public override void OnSubVMCreated(BaseVM subVM) => SubVMCreated?.Invoke(subVM, EventArgs.Empty);
         public override void OnSubVMDisposing(BaseVM subVM) => SubVMDisposing?.Invoke(subVM, EventArgs.Empty);
      }

      private class DetailsVM : BaseVM
      {
         public int Value { get; set; }
      }

      [TestInitialize]
      public void Initialize()
      {
         VMController.Register<MasterVM>();
         VMController.Register<DetailsVM>();

         var baseDelegate = VMController.CreateInstance;
         VMController.CreateInstance = (type, args) => type == typeof(MasterVM) ? _masterVM : baseDelegate(type, args);
      }

      [TestMethod]
      public void MasterDetailsVM_Request()
      {
         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", $"{nameof(MasterVM)}.{nameof(DetailsVM)}");

         Assert.AreEqual($"{nameof(MasterVM)}.{nameof(DetailsVM)}", _response.VMId);

         var vm = _response.GetVM<DetailsVM>();
         Assert.IsNotNull(vm);
         Assert.AreEqual(int.MaxValue, vm.Value);
      }

      [TestMethod]
      public void MasterDetailsVM_Update()
      {
         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", $"{nameof(MasterVM)}.{nameof(DetailsVM)}");
         vmController.OnUpdateVM("conn1", $"{nameof(MasterVM)}.{nameof(DetailsVM)}", _response.MockAction("Value", "99"));

         Assert.AreEqual(99, (_masterVM.GetSubVM(nameof(DetailsVM)) as DetailsVM).Value);
      }

      [TestMethod]
      public void MasterDetailsVM_SubVMCreated()
      {
         object subVM = null;
         bool subVMCreated = false;
         _masterVM.SubVMCreated += (sender, e) => { subVM = sender; subVMCreated = true; };

         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", $"{nameof(MasterVM)}.{nameof(DetailsVM)}");

         Assert.IsTrue(subVMCreated);
         Assert.IsTrue(subVM is DetailsVM);
      }

      [TestMethod]
      public void MasterDetailsVM_SubVMDisposing()
      {
         object subVM = null;
         bool subVMDisposing = false;
         _masterVM.SubVMDisposing += (sender, e) => { subVM = sender; subVMDisposing = true; };

         var vmController = new VMController(_response.Handler);
         vmController.OnRequestVM("conn1", $"{nameof(MasterVM)}.{nameof(DetailsVM)}");

         vmController.OnDisposeVM("conn1", $"{nameof(MasterVM)}.{nameof(DetailsVM)}");
         Assert.IsTrue(subVMDisposing);
         Assert.IsTrue(subVM is DetailsVM);
      }
   }
}
