using Autofac;
using DotNetify.Client;
using System;
using System.Threading.Tasks;

namespace HelloWorld
{
   public static class Bootstrap
   {
      private static IContainer _container;

      static Bootstrap()
      {
         var builder = new ContainerBuilder();

         builder.RegisterType<DotNetifyHubProxy>().As<IDotNetifyHubProxy>().SingleInstance();
         builder.RegisterType<DotNetifyClient>().As<IDotNetifyClient>().InstancePerDependency();
         builder.RegisterType<AvaloniaUIThreadDispatcher>().As<IUIThreadDispatcher>().SingleInstance();
         builder.RegisterType<HelloWorldVMProxy>();

         _container = builder.Build();
      }

      public static T Resolve<T>()
      {
         try
         {
            return _container.Resolve<T>();
         }
         catch (Exception ex)
         {
            System.Diagnostics.Trace.TraceError(ex.ToString());
            throw ex;
         }
      }
   }

   public class AvaloniaUIThreadDispatcher : IUIThreadDispatcher
   {
      public Task InvokeAsync(Action action) => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(action);
   }
}