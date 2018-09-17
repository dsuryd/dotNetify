using DotNetify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UnitTests
{
   [TestClass]
   public class MasterDetailsVMTest
   {
      private MasterVM _masterVM = new MasterVM();
      private string _detailsVMId = $"{nameof(MasterVM)}.{nameof(DetailsVM)}";

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
      }

      [TestMethod]
      public void MasterDetailsVM_Request()
      {
         var vmController = new MockVMController<MasterVM>(_masterVM);
         var response = vmController.RequestVM(_detailsVMId);

         Assert.AreEqual(int.MaxValue, response.GetVMProperty<int>("Value"));
      }

      [TestMethod]
      public void MasterDetailsVM_Update()
      {
         var vmController = new MockVMController<MasterVM>(_masterVM);
         vmController.RequestVM(_detailsVMId);

         var update = new Dictionary<string, object>() { { "Value", "99" } };
         vmController.UpdateVM(update, _detailsVMId);

         Assert.AreEqual(99, (_masterVM.GetSubVM(nameof(DetailsVM)) as DetailsVM).Value);
      }

      [TestMethod]
      public void MasterDetailsVM_SubVMCreated()
      {
         object subVM = null;
         bool subVMCreated = false;
         _masterVM.SubVMCreated += (sender, e) => { subVM = sender; subVMCreated = true; };

         var vmController = new MockVMController<MasterVM>(_masterVM);
         vmController.RequestVM(_detailsVMId);

         Assert.IsTrue(subVMCreated);
         Assert.IsTrue(subVM is DetailsVM);
      }

      [TestMethod]
      public void MasterDetailsVM_SubVMDisposing()
      {
         object subVM = null;
         bool subVMDisposing = false;
         _masterVM.SubVMDisposing += (sender, e) => { subVM = sender; subVMDisposing = true; };

         var vmController = new MockVMController<MasterVM>(_masterVM);
         vmController.RequestVM(_detailsVMId);

         vmController.DisposeVM(_detailsVMId);
         Assert.IsTrue(subVMDisposing);
         Assert.IsTrue(subVM is DetailsVM);
      }
   }
}
