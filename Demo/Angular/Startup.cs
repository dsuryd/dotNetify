using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DotNetify;

namespace HelloWorld
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();
        services.AddSignalR();
        services.AddDotNetify();          

        services.AddSpaStaticFiles(configuration =>
        {
            configuration.RootPath = "./dist";
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseWebSockets();
      app.UseDotNetify();  

      app.UseRouting();
      app.UseEndpoints(endpoints => endpoints.MapHub<DotNetifyHub>("/dotnetify"));
      

      app.UseStaticFiles();
      app.UseSpaStaticFiles();
      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = ".";
        if (env.IsDevelopment())
          spa.UseAngularCliServer(npmScript: "start");
      }); 
    }
  }
}