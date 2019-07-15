using DotNetify.Blazor;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DevApp.Blazor.Client
{
   public class Startup
   {
      public void ConfigureServices( IServiceCollection services )
      {
         services.UseDotNetifyBlazor();
      }

      public void Configure( IComponentsApplicationBuilder app )
      {
         app.AddComponent<App>( "app" );
      }
   }
}