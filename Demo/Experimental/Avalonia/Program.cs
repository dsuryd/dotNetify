using Avalonia;
using Avalonia.Logging.Serilog;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace HelloWorld
{
   internal class Program
   {
      private static void Main(string[] args)
      {
         // Run Avalonia window on a separate thread.
         Task.Run(() => BuildAvaloniaApp().Start<HelloWorld>(() => Bootstrap.Resolve<HelloWorldVMProxy>()));

         // Run ASP.NET Core web server.
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