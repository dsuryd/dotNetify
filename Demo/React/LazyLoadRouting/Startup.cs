using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DotNetify;
using Jering.Javascript.NodeJS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.DependencyInjection;

namespace LazyLoadRouting
{
   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddMemoryCache();
         services.AddSignalR();
         services.AddDotNetify();

         StaticNodeJSService.Configure<OutOfProcessNodeJSServiceOptions>(options => options.TimeoutMS = 2000);
      }

      public void Configure(IApplicationBuilder app)
      {
         app.UseWebSockets();
         app.UseDotNetify(c => c.UseDeveloperLogging());

#pragma warning disable 618
         app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
         {
            HotModuleReplacement = true,
            HotModuleReplacementClientOptions = new Dictionary<string, string> { { "reload", "true" } },
         });
#pragma warning restore 618

         app.UseStaticFiles();
         app.UseRouting();
         app.UseEndpoints(endpoints => endpoints.MapHub<DotNetifyHub>("/dotnetify"));

         app.UseSsr(typeof(App), (string[] args) => StaticNodeJSService.InvokeFromFileAsync<string>("wwwroot/ssr", null, args), DefaultRequestHandler);

         // Client-side rendering.
         app.Run(DefaultRequestHandler);
      }

      private static async Task DefaultRequestHandler(HttpContext context)
      {
         using (var reader = new StreamReader(File.OpenRead("wwwroot/index.html")))
            await context.Response.WriteAsync(reader.ReadToEnd());
      }
   }
}