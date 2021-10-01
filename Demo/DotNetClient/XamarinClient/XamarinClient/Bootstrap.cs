using DotNetify;
using DotNetify.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace XamarinClient
{
   public static class Bootstrap
   {
      private static IServiceProvider _serviceProvider;

      static Bootstrap()
      {
         DotNetifyHubProxy.ServerUrl = "http://10.0.2.2:5000";

         _serviceProvider = new ServiceCollection()
            .AddDotNetifyClient()
            .AddSingleton<IUIThreadDispatcher, XamarinUIThreadDispatcher>()
            .AddTransient<HelloWorldVMProxy>()
            .BuildServiceProvider();
      }

      public static T Resolve<T>() => _serviceProvider.GetRequiredService<T>();
   }

   public class XamarinUIThreadDispatcher : IUIThreadDispatcher
   {
      public async Task InvokeAsync(Action action)
      {
         if (!MainThread.IsMainThread)
         {
            await MainThread.InvokeOnMainThreadAsync(action);
         }
         else
         {
            action?.Invoke();
         }

         await Task.CompletedTask;
      }
   }
}