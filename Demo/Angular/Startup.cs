using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.DependencyInjection;
using DotNetify;

namespace helloworld
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();
        services.AddSignalR();
        services.AddDotNetify();          

        services.AddSpaStaticFiles(c => c.RootPath = "./dist");
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseCors(builder => builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin()
        .AllowCredentials());

      app.UseWebSockets();
      app.UseSignalR(routes => routes.MapDotNetifyHub());
      app.UseDotNetify();  

      app.UseStaticFiles();
      app.UseSpaStaticFiles();
      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = ".";
        if (env.IsDevelopment())
          spa.UseAngularCliServer(npmScript: "start");
      });

      app.Run(async (context) =>
      {
        await context.Response.WriteAsync("Hello World!");
      });
    }
  }
}