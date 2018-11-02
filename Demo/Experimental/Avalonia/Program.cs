using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Logging.Serilog;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using HelloWorld.WebServer;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() => BuildAvaloniaApp().Start<HelloWorld>(() => new HelloWorldVM()));
            CreateWebHostBuilder(args).Build().Run();
        }

        public static AppBuilder BuildAvaloniaApp() => 
            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();                
    }
}
