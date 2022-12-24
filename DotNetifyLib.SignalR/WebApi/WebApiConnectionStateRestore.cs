using DotNetify.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetify.WebApi
{
   public static class WebApiConnectionStateRestore
   {
      public static bool IsRestoring { get; set; }

      public static IApplicationBuilder RestoreWebApiConnectionState(this IApplicationBuilder app)
      {
         var cache = app.ApplicationServices.GetService<IWebApiConnectionCache>();
         if (cache == null) return app;

         var vmControllerFactory = app.ApplicationServices.GetRequiredService<IWebApiVMControllerFactory>();
         var hubServiceProvider = app.ApplicationServices.GetRequiredService<IHubServiceProvider>();
         var principalAccessor = app.ApplicationServices.GetRequiredService<IPrincipalAccessor>();
         var hubPipeline = app.ApplicationServices.GetRequiredService<IHubPipeline>();
         var responseManager = app.ApplicationServices.GetRequiredService<IWebApiResponseManager>() as IDotNetifyHubResponseManager;

         IsRestoring = true;
         foreach (var connection in cache.GetConnections().GetAwaiter().GetResult())
         {
            foreach (var vmInfo in connection.VMInfo)
            {
               var httpCallerContext = new DotNetifyWebApi.HttpCallerContext(connection.Id);

               new DotNetifyHubHandler(vmControllerFactory, hubServiceProvider, principalAccessor, hubPipeline, responseManager) { CallerContext = httpCallerContext }
                  .RequestVMAsync(vmInfo.VMId, System.Text.Json.JsonSerializer.Deserialize<object>(vmInfo.VMArgs)).GetAwaiter().GetResult();
            }
         }

         IsRestoring = false;
         return app;
      }
   }
}