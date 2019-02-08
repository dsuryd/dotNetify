using DotNetify.Client.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Blazor.Client
{
   public class CounterVMProxy : BlazorComponent, INotifyPropertyChanged, IDisposable
   {
      private readonly IDotNetifyClient _dotnetify;

      public event PropertyChangedEventHandler PropertyChanged { add { } remove { } }

      public int CurrentCount { get; set; }

      public CounterVMProxy() : base()
      {
         _dotnetify = Startup.ServiceProvider.GetService<IDotNetifyClient>();
      }

      protected override async Task OnInitAsync() => await _dotnetify.ConnectAsync("CounterVM", this);

      public void Dispose() => _dotnetify.DisposeAsync();

      public async Task IncrementCount() => await _dotnetify.DispatchAsync("IncrementCount", null);
   }
}