using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace DotNetify.LoadTester
{
   public interface ILoadTestClientBuilder
   {
      string ClientId { get; }

      ISetupConnect Connect(string vmId);

      ISetupConnect Connect(string vmId, VMConnectOptions connectOptions);

      ILoadTestClientBuilder AddPrefix(string prefix, bool overwrite = false);
   }

   public class LoadTestClientBuilder : ILoadTestClientBuilder
   {
      private readonly LoadTestClient _client;

      public string ClientId { get; private set; }

      public LoadTestClientBuilder(string clientId, string serverUrl, CancellationTokenSource cancelTokenSource, ILogger logger)
      {
         ClientId = clientId;
         _client = new LoadTestClient(clientId, serverUrl, cancelTokenSource, logger);
      }

      internal LoadTestClient Build()
      {
         return _client;
      }

      public ISetupConnect Connect(string vmId) => Connect(vmId, null);

      public ISetupConnect Connect(string vmId, VMConnectOptions connectOptions)
      {
         try
         {
            License.CheckUsage();
         }
         catch (Exception ex)
         {
            throw ex;
         }

         _client.AddAction(x => x.ConnectAsync(vmId, connectOptions));
         return new SetupConnect(this, _client, vmId);
      }

      public ILoadTestClientBuilder AddPrefix(string prefix, bool overwrite = false)
      {
         _client.Id = prefix + "." + (!overwrite ? _client.Id : _client.Id.Split('.')[1]);
         ClientId = _client.Id;
         return this;
      }
   }
}