using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BrunoLau.SpaServices.Webpack;
using DotNetify;
using DotNetify.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace RealtimeDb
{
   public class Startup
   {
      public IConfiguration Configuration { get; }

      public Startup(IConfiguration configuration)
      {
         Configuration = configuration;
      }

      public void ConfigureServices(IServiceCollection services)
      {
         services.AddSignalR();
         services.AddDotNetify();
         services.AddDotNetifyPostgres(new PostgresConfiguration
         {
            ConnectionString = Configuration.GetConnectionString("Postgres"),
            PublicationName = "dotnetify_pub",
            ReplicationSlotName = "dotnetify_slot"
         });

         services.AddDbContextFactory<UserAccountDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("Postgres")));
      }

      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         app.UseWebSockets();
         app.UseDotNetify();

         if (env.IsDevelopment())
            app.UseWebpackDevMiddlewareEx(new WebpackDevMiddlewareOptions
            {
               HotModuleReplacement = true,
               HotModuleReplacementClientOptions = new Dictionary<string, string> { { "reload", "true" } }
            });

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