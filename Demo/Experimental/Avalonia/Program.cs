using Avalonia;
using Avalonia.Logging.Serilog;
using HelloWorld.WebServer;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace HelloWorld
{
   internal class Program
   {
      private static void Main(string[] args)
      {
         Task.Run(() => BuildAvaloniaApp().Start<HelloWorld>());
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