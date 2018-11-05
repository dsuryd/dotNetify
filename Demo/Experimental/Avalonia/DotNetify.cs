using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
   public class DotNetifyClient
   {
      private readonly IDotNetifyHubProxy _hubProxy;

      public DotNetifyClient(IDotNetifyHubProxy hubProxy)
      {
         _hubProxy = hubProxy;
      }

      public async Task ConnectAsync(string vmId, RequestVMOptions options = null)
      {
         await _hubProxy.StartAsync();
         _hubProxy.Request_VM(vmId, options);
      }
   }
}