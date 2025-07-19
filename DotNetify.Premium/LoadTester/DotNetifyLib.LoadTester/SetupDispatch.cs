using System;
using System.Threading.Tasks;

namespace DotNetify.LoadTester
{
   public interface ISetupDispatch
   {
      ISetupConnect Repeat(uint count, uint millisecondsDelay);

      ISetupConnect RepeatContinuously(uint millisecondsDelay);

      ISetupConnect Once();
   }

   internal class SetupDispatch : ISetupDispatch
   {
      private readonly LoadTestClient _client;
      private readonly ISetupConnect _setupConnect;
      private readonly string _vmId;
      private readonly object _dispatchArgs;

      public SetupDispatch(ISetupConnect setupConnect, LoadTestClient client, string vmId, object dispatchArgs)
      {
         _setupConnect = setupConnect;
         _client = client;
         _vmId = vmId;
         _dispatchArgs = dispatchArgs;

         AddDispatchAction();
      }

      public ISetupConnect Once()
      {
         return _setupConnect;
      }

      public ISetupConnect Repeat(uint count, uint millisecondsDelay)
      {
         for (int i = 1; i < count; i++)
         {
            AddDelayAction(millisecondsDelay);
            AddDispatchAction();
         }
         return _setupConnect;
      }

      public ISetupConnect RepeatContinuously(uint millisecondsDelay)
      {
         Task action()
         {
            AddDelayAction(millisecondsDelay);
            AddDispatchAction();
            _client.AddAction(_ => action());
            return Task.CompletedTask;
         }

         _client.AddAction(_ => action());
         return _setupConnect;
      }

      private void AddDispatchAction()
      {
         _client.AddAction(x => x.DispatchAsync(_vmId, _dispatchArgs));
      }

      private void AddDelayAction(uint millisecondsDelay)
      {
         _client.AddAction(x => Task.Delay((int) millisecondsDelay, x.StopToken));
      }
   }
}