using Blazor.Shared;
using DotNetify.Client.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Blazor.Client
{
   public class FetchDataVMProxy : BlazorComponent, IDisposable
   {
      private readonly IDotNetifyClient _dotnetify = Startup.ServiceProvider.GetService<IDotNetifyClient>();

      public WeatherForecast[] Forecasts { get; set; }

      protected override async Task OnInitAsync() => await _dotnetify.ConnectAsync("FetchDataVM", this);

      public void Dispose() => _dotnetify.DisposeAsync();
   }
}