using Autofac;
using DotNetify.Client;
using System;

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
}