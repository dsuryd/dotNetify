using DotNetify;
using DotNetify.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows;

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
            .AddSingleton<IUIThreadDispatcher, WpfUIThreadDispatcher>()
            .AddTransient<HelloWorldVMProxy>()
            .BuildServiceProvider();
      }

      public static T Resolve<T>() => _serviceProvider.GetRequiredService<T>();
   }

   public class WpfUIThreadDispatcher : IUIThreadDispatcher
   {
      public async Task InvokeAsync(Action action) => await Application.Current.Dispatcher.InvokeAsync(action);
   }
}