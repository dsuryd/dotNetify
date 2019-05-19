using DotNetify.Client.Blazor;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Client
{
   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddDotNetifyClient("http://localhost:61624");
      }

      public void Configure(IBlazorApplicationBuilder app)
      {
         app.AddComponent<App>("app");
      }
   }
}