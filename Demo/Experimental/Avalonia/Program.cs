using System;
using Avalonia;
using Avalonia.Logging.Serilog;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start<HelloWorld>(() => new HelloWorldVM());
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug();
    }
}
