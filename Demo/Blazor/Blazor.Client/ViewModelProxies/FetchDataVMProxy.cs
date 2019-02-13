using Blazor.Shared;
using DotNetify.Client.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Threading.Tasks;

namespace Blazor.Client
{
   public class FetchDataVMProxy : BlazorComponent, IDisposable
   {
      [Inject]
      protected IDotNetifyClient DotNetify { get; set; }

      public WeatherForecast[] Forecasts { get; set; }

      protected override async Task OnInitAsync() => await DotNetify.ConnectAsync("FetchDataVM", this);

      public void Dispose() => DotNetify.DisposeAsync();
   }
}