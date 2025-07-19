using System;
using System.Threading.Tasks;

namespace DotNetify.LoadTester
{
   public interface ISetupConnect
   {
      ISetupConnect Wait(uint millisecondsDelay);

      ISetupDispatch Dispatch(object dispatchArgs);

      ISetupDispatch Dispatch(Func<IClientVM, object> getDispatchArgs);

      ISetupConnect Destroy();

      ISetupConnect OnServerResponse(ServerResponseDelegate callback);

      ISetupConnect OnDestroyed(Action<IClientVM> callback);
   }

   internal class SetupConnect : ISetupConnect
   {
      private readonly LoadTestClient _client;
      private readonly LoadTestClientBuilder _clientBuilder;
      private readonly string _vmId;

      public SetupConnect(LoadTestClientBuilder clientBuilder, LoadTestClient client, string vmId)
      {
         _client = client;
         _clientBuilder = clientBuilder;
         _vmId = vmId;
      }

      public ISetupDispatch Dispatch(object dispatchArgs)
      {
         return new SetupDispatch(this, _client, _vmId, dispatchArgs);
      }

      public ISetupDispatch Dispatch(Func<IClientVM, object> getDispatchArgs)
      {
         return new SetupDispatch(this, _client, _vmId, getDispatchArgs);
      }

      public ISetupConnect Destroy()
      {
         _client.AddAction(x =>
         {
            x.Destroy(_vmId);
            return Task.CompletedTask;
         });
         return this;
      }

      public ISetupConnect OnServerResponse(ServerResponseDelegate callback)
      {
         _client.HandleServerResponse(_vmId, callback);
         return this;
      }

      public ISetupConnect OnDestroyed(Action<IClientVM> callback)
      {
         _client.HandleDestroyed(_vmId, callback);
         return this;
      }

      public ISetupConnect Wait(uint millisecondsDelay)
      {
         _client.AddAction(x => Task.Delay((int) millisecondsDelay, x.StopToken));
         return this;
      }
   }
}