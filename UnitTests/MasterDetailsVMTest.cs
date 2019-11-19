using System.Collections.Generic;
using DotNetify;
using DotNetify.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

         public BaseVM CreatedSubVM { get; set; }
         public BaseVM DisposedSubVM { get; set; }

         public override BaseVM GetSubVM(string vmTypeName) => vmTypeName == nameof(DetailsVM) ? _detailsVM : base.GetSubVM(vmTypeName);

         public override void OnSubVMCreated(BaseVM subVM) => CreatedSubVM = subVM;

         public override void OnSubVMDisposing(BaseVM subVM) => DisposedSubVM = subVM;
      }

      private class DetailsVM : BaseVM
      {
         public int Value { get; set; }
      }

      private HubEmulator _hubEmulator;

      [TestInitialize]
      public void Initialize()
      {
         _hubEmulator = new HubEmulatorBuilder()
            .Register<MasterVM>()
            .Register<DetailsVM>()
            .Build();
      }

      [TestMethod]
      public void MasterDetailsVM_Request()
      {
         var client = _hubEmulator.CreateClient();
         var response = client.Connect(_detailsVMId).As<dynamic>();

         Assert.AreEqual(int.MaxValue, (int) response.Value);
      }

      [TestMethod]
      public void MasterDetailsVM_Update()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(_detailsVMId);

         var update = new Dictionary<string, object>() { { "Value", "99" } };
         client.Dispatch(update);

         var masterVM = _hubEmulator.CreatedVMs.Find(x => x is MasterVM) as MasterVM;
         Assert.AreEqual(99, (masterVM.GetSubVM(nameof(DetailsVM)) as DetailsVM).Value);
      }

      [TestMethod]
      public void MasterDetailsVM_SubVMCreated()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(_detailsVMId).As<dynamic>();

         var masterVM = _hubEmulator.CreatedVMs.Find(x => x is MasterVM) as MasterVM;

         Assert.IsNotNull(masterVM.CreatedSubVM);
         Assert.IsTrue(masterVM.CreatedSubVM is DetailsVM);
      }

      [TestMethod]
      public void MasterDetailsVM_SubVMDisposing()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(_detailsVMId).As<dynamic>();
         client.Destroy();

         var masterVM = _hubEmulator.CreatedVMs.Find(x => x is MasterVM) as MasterVM;

         Assert.IsNotNull(masterVM.DisposedSubVM);
         Assert.IsTrue(masterVM.DisposedSubVM is DetailsVM);
      }
   }
}