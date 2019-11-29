using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

      [CustomFilter]
      private class MasterVM : BaseVM
      {
         private DetailsVM _detailsVM = new DetailsVM() { Value = int.MaxValue };

         public BaseVM CreatedSubVM { get; set; }
         public BaseVM DisposedSubVM { get; set; }

         public override BaseVM GetSubVM(string vmTypeName) => vmTypeName == nameof(DetailsVM) ? _detailsVM : base.GetSubVM(vmTypeName);

         public override void OnSubVMCreated(BaseVM subVM) => CreatedSubVM = subVM;

         public override void OnSubVMDisposing(BaseVM subVM) => DisposedSubVM = subVM;
      }

      [CustomFilter]
      private class DetailsVM : BaseVM
      {
         public int Value { get; set; }
      }

      [CustomFilter]
      private class RootVM : BaseVM
      {
      }

      private class CustomFilterAttribute : Attribute
      {
      }

      private class CustomFilter : IVMFilter<CustomFilterAttribute>
      {
         public static event EventHandler<Tuple<CustomFilterAttribute, VMContext>> Invoked;

         public static void Cleanup()
         {
            if (Invoked != null)
               foreach (Delegate d in Invoked.GetInvocationList())
                  Invoked -= (EventHandler<Tuple<CustomFilterAttribute, VMContext>>) d;
         }

         public Task Invoke(CustomFilterAttribute attribute, VMContext context, NextFilterDelegate next)
         {
            Invoked?.Invoke(this, Tuple.Create(attribute, context));
            return next.Invoke(context);
         }
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

      [TestCleanup]
      public void Cleanup()
      {
         CustomFilter.Cleanup();
      }

      [TestMethod]
      public void MasterDetailsVM_Request()
      {
         var client = _hubEmulator.CreateClient();
         var response = client.Connect(_detailsVMId).As<dynamic>();

         Assert.AreEqual(int.MaxValue, (int) response.Value);
      }

      [TestMethod]
      public void MasterDetailsVM_RequestWithCustomFilter_FiltersInvoked()
      {
         var contexts = new List<VMContext>();
         CustomFilter.Invoked += customFilter_Invoked;
         void customFilter_Invoked(object sender, Tuple<CustomFilterAttribute, VMContext> e) => contexts?.Add(e.Item2);

         var hubEmulator = new HubEmulatorBuilder()
            .Register<RootVM>()
            .Register<MasterVM>()
            .Register<DetailsVM>()
            .UseFilter<CustomFilter>()
            .Build();

         var client = hubEmulator.CreateClient();
         client.Connect(_detailsVMId).As<dynamic>();
         client.Destroy();

         Assert.IsTrue(contexts.Count >= 2);
         Assert.IsTrue(contexts[0].Instance is MasterVM);
         Assert.IsTrue(contexts[1].Instance is DetailsVM);

         contexts.Clear();

         client.Connect("RootVM.MasterVM.DetailsVM").As<dynamic>();

         Assert.IsTrue(contexts.Count >= 3);
         Assert.IsTrue(contexts[0].Instance is RootVM);
         Assert.IsTrue(contexts[1].Instance is MasterVM);
         Assert.IsTrue(contexts[2].Instance is DetailsVM);
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