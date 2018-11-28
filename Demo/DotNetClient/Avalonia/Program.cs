using Avalonia;
using Avalonia.Logging.Serilog;

namespace HelloWorld
{
   internal class Program
   {
      private static void Main(string[] args)
      {
         BuildAvaloniaApp().Start<HelloWorld>(() => Bootstrap.Resolve<HelloWorldVMProxy>());
      }

      public static AppBuilder BuildAvaloniaApp() =>
          AppBuilder.Configure<App>()
              .UsePlatformDetect()
              .LogToDebug();
   }
}