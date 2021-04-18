using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using DotNetify;

namespace HelloWorld
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddDotNetify();          
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.UseDotNetify();          

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