using System.Collections.Generic;
using System.ComponentModel;
using DotNetify;
using DotNetify.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

         public object CreatedSubVM { get; set; }
         public object DisposedSubVM { get; set; }

         public event PropertyChangedEventHandler PropertyChanged = delegate { };

         public INotifyPropertyChanged GetSubVM(string vmTypeName, string vmInstanceId = null) => vmTypeName == nameof(DetailsNoBaseVM) ? _DetailsVM : null;

         public void OnSubVMCreated(object subVM) => CreatedSubVM = subVM;

         public void OnSubVMDisposing(object subVM) => DisposedSubVM = subVM;
      }

      private class DetailsNoBaseVM : INotifyPropertyChanged
      {
         public event PropertyChangedEventHandler PropertyChanged = delegate { };

         public int Value { get; set; }
      }

      private HubEmulator _hubEmulator;

      [TestInitialize]
      public void Initialize()
      {
         _hubEmulator = new HubEmulatorBuilder()
            .Register<MasterNoBaseVM>()
            .Register<DetailsNoBaseVM>()
            .Build();
      }

      [TestMethod]
      public void MasterDetailsNoBaseVM_Request()
      {
         var client = _hubEmulator.CreateClient();
         var response = client.Connect(_detailsVMId).As<dynamic>();

         Assert.AreEqual(int.MaxValue, (int) response.Value);
      }

      [TestMethod]
      public void MasterDetailsNoBaseVM_Update()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(_detailsVMId);

         var update = new Dictionary<string, object>() { { "Value", "99" } };
         client.Dispatch(update);

         var masterVM = _hubEmulator.CreatedVMs.Find(x => x is MasterNoBaseVM) as MasterNoBaseVM;
         Assert.AreEqual(99, (masterVM.GetSubVM(nameof(DetailsNoBaseVM)) as DetailsNoBaseVM).Value);
      }

      [TestMethod]
      public void MasterDetailsNoBaseVM_SubVMCreated()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(_detailsVMId).As<dynamic>();

         var masterVM = _hubEmulator.CreatedVMs.Find(x => x is MasterNoBaseVM) as MasterNoBaseVM;

         Assert.IsNotNull(masterVM.CreatedSubVM);
         Assert.IsTrue(masterVM.CreatedSubVM is DetailsNoBaseVM);
      }

      [TestMethod]
      public void MasterDetailsNoBaseVM_SubVMDisposing()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(_detailsVMId).As<dynamic>();
         client.Destroy();

         var masterVM = _hubEmulator.CreatedVMs.Find(x => x is MasterNoBaseVM) as MasterNoBaseVM;

         Assert.IsNotNull(masterVM.DisposedSubVM);
         Assert.IsTrue(masterVM.DisposedSubVM is DetailsNoBaseVM);
      }
   }
}