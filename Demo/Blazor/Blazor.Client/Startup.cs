using DotNetify.Client.Blazor;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Blazor.Client
{
   public class Startup
   {
      public static IServiceProvider ServiceProvider { get; set; }

      public void ConfigureServices(IServiceCollection services)
      {
         services.AddDotNetifyClient("http://localhost:61624");
      }

      public void Configure(IBlazorApplicationBuilder app)
      {
         app.AddComponent<App>("app");
         ServiceProvider = app.Services;
      }
   }
}