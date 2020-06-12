using System.Threading.Tasks;
using DotNetify.Blazor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace DevApp.Blazor.Client
{
   public class Program
   {
      public static async Task Main(string[] args)
      {
         var builder = WebAssemblyHostBuilder.CreateDefault(args);
         builder.Services.UseDotNetifyBlazor();
         builder.RootComponents.Add<App>("app");
         await builder.Build().RunAsync();
      }
   }
}