using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.DependencyInjection;
using DotNetify;
using DotNetify.Routing;

namespace LazyLoadRouting
{
   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddMemoryCache();
         services.AddSignalR();
         services.AddDotNetify();
         services.AddNodeServices();
      }

      public void Configure(IApplicationBuilder app, INodeServices nodeServices)
      {
         app.UseWebSockets();
         app.UseDotNetify(c => c.UseDeveloperLogging());
         ;

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

         var vmFactory = app.ApplicationServices.GetService<IVMFactory>();
         app.UseWhen(context => context.Request.Query["ssr"] != "false" && !Path.HasExtension(context.Request.Path.Value), appBuilder =>
         {
            appBuilder.Run(async context =>
            {
               // Server-side rendering.
               var path = context.Request.Path.Value;
               var ssrStates = ServerSideRender.GetInitialStates(vmFactory, ref path, typeof(App));

               var result = await nodeServices.InvokeAsync<string>("wwwroot/dist/ssr", path, ssrStates);
               await context.Response.WriteAsync(result);
            });
         });

         app.Run(async (context) =>
         {
            // Client-side rendering.
            using (var reader = new StreamReader(File.OpenRead("wwwroot/index.html")))
               await context.Response.WriteAsync(reader.ReadToEnd());
         });
      }
   }
}