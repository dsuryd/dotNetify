using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.DependencyInjection;
using DotNetify;

namespace HelloWorld.WebPack
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSignalR();
            services.AddDotNetify();          
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.UseDotNetify();

#pragma warning disable 618
            app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
            {
                HotModuleReplacement = true,
                HotModuleReplacementClientOptions = new Dictionary<string, string> { { "reload", "true" } },
            });            
#pragma warning restore 618            

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints => 
            {
              endpoints.MapHub<DotNetifyHub>("/dotnetify");
              endpoints.MapFallbackToFile("index.html");
            }); 
        }
    }
}
