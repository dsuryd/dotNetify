using DotNetify.Client.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Threading.Tasks;

namespace Blazor.Client
{
   public class CounterVMProxy : BlazorComponent, IDisposable
   {
      [Inject]
      protected IDotNetifyClient DotNetify { get; set; }

      public int CurrentCount { get; set; }

      public async Task IncrementCount() => await DotNetify.DispatchAsync("IncrementCount", null);

      protected override async Task OnInitAsync() => await DotNetify.ConnectAsync("CounterVM", this);

      public void Dispose() => DotNetify.DisposeAsync();
   }
}