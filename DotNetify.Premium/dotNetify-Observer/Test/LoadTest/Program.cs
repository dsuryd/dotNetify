using System.IO;
using System.Threading.Tasks;
using DotNetify.LoadTester;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LoadTest
{
   internal class Program
   {
      private async static Task Main(string[] args)
      {
         var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

         IConfigurationRoot configuration = builder.Build();

         var loggerFactory = LoggerFactory.Create(configure => configure.AddConfiguration(configuration.GetSection("Logging")).AddConsole());
         await LoadTestRunner.RunAsync(args, loggerFactory);
      }
   }
}