using DotNetify;
using DotNetify.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace HelloWorld
{
   public static class Bootstrap
   {
      private static IServiceProvider _serviceProvider;

      static Bootstrap()
      {
         DotNetifyHubProxy.ServerUrl = "http://localhost:5000";

         _serviceProvider = new ServiceCollection()
            .AddDotNetifyClient()
            .AddSingleton<IUIThreadDispatcher, AvaloniaUIThreadDispatcher>()
            .AddTransient<HelloWorldVMProxy>()
            .BuildServiceProvider();
      }

      public static T Resolve<T>() => _serviceProvider.GetRequiredService<T>();
   }

   public class AvaloniaUIThreadDispatcher : IUIThreadDispatcher
   {
      public Task InvokeAsync(Action action) => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(action);
   }
}