using DotNetify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UnitTests
{
   [TestClass]
   public class MasterDetailsNoBaseVMTest
   {
      private MasterNoBaseVM _masterVM = new MasterNoBaseVM();
      private string _detailsVMId = $"{nameof(MasterNoBaseVM)}.{nameof(DetailsNoBaseVM)}";

      private class MasterNoBaseVM : INotifyPropertyChanged, IMasterVM
      {
         private DetailsNoBaseVM _DetailsVM = new DetailsNoBaseVM() { Value = int.MaxValue };

         public event EventHandler SubVMCreated;
         public event EventHandler SubVMDisposing;
         public event PropertyChangedEventHandler PropertyChanged = delegate { };

         public INotifyPropertyChanged GetSubVM(string vmTypeName, string vmInstanceId = null) => vmTypeName == nameof(DetailsNoBaseVM) ? _DetailsVM : null;
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
      }

      [TestMethod]
      public void MasterDetailsNoBaseVM_Request()
      {
         var vmController = new MockVMController<MasterNoBaseVM>(_masterVM);
         var response = vmController.RequestVM(_detailsVMId);

         Assert.IsNotNull(response);
         Assert.AreEqual(int.MaxValue, response.GetVMProperty<int>("Value"));
      }

      [TestMethod]
      public void MasterDetailsNoBaseVM_Update()
      {
         var vmController = new MockVMController<MasterNoBaseVM>(_masterVM);
         vmController.RequestVM(_detailsVMId);

         var update = new Dictionary<string, object>() { { "Value", "99" } };
         vmController.UpdateVM(update, _detailsVMId);

         Assert.AreEqual(99, (_masterVM.GetSubVM(nameof(DetailsNoBaseVM)) as DetailsNoBaseVM).Value);
      }

      [TestMethod]
      public void MasterDetailsNoBaseVM_SubVMCreated()
      {
         object subVM = null;
         bool subVMCreated = false;
         _masterVM.SubVMCreated += (sender, e) => { subVM = sender; subVMCreated = true; };

         var vmController = new MockVMController<MasterNoBaseVM>(_masterVM);
         vmController.RequestVM(_detailsVMId);

         Assert.IsTrue(subVMCreated);
         Assert.IsTrue(subVM is DetailsNoBaseVM);
      }

      [TestMethod]
      public void MasterDetailsNoBaseVM_SubVMDisposing()
      {
         object subVM = null;
         bool subVMDisposing = false;
         _masterVM.SubVMDisposing += (sender, e) => { subVM = sender; subVMDisposing = true; };

         var vmController = new MockVMController<MasterNoBaseVM>(_masterVM);
         vmController.RequestVM(_detailsVMId);

         vmController.DisposeVM(_detailsVMId);
         Assert.IsTrue(subVMDisposing);
         Assert.IsTrue(subVM is DetailsNoBaseVM);
      }
   }
}
