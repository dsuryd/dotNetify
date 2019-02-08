using Blazor.Shared;
using DotNetify.Client.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Blazor.Client
{
   public class FetchDataVMProxy : BlazorComponent, INotifyPropertyChanged, IDisposable
   {
      private readonly IDotNetifyClient _dotnetify;

      public event PropertyChangedEventHandler PropertyChanged { add { } remove { } }

      public WeatherForecast[] Forecasts { get; set; }

      public FetchDataVMProxy() : base()
      {
         _dotnetify = Startup.ServiceProvider.GetService<IDotNetifyClient>();
      }

      protected override async Task OnInitAsync() => await _dotnetify.ConnectAsync("FetchDataVM", this);

      public void Dispose() => _dotnetify.DisposeAsync();
   }
}