using DotNetify.Client.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Blazor.Client
{
   public class CounterVMProxy : BlazorComponent, IDisposable
   {
      private readonly IDotNetifyClient _dotnetify = Startup.ServiceProvider.GetService<IDotNetifyClient>();

      public int CurrentCount { get; set; }

      public async Task IncrementCount() => await _dotnetify.DispatchAsync("IncrementCount", null);

      protected override async Task OnInitAsync() => await _dotnetify.ConnectAsync("CounterVM", this);

      public void Dispose() => _dotnetify.DisposeAsync();
   }
}