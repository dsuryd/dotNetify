using DotNetify.Pulse;
using DotNetify.Security;
using Jering.Javascript.NodeJS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotNetify.DevApp
{
   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {
         // Add OpenID Connect server to produce JWT access tokens.
         services.AddAuthenticationServer();

         services.AddSignalR()
            //.AddMessagePackProtocol()
            ;
         services.AddDotNetify();
         services.AddDotNetifyPulse();
         services.AddMvc();

         services.AddScoped<IEmployeeRepository, EmployeeRepository>();
         services.AddSingleton<IMovieService, MovieService>();
         services.AddSingleton<IWebStoreService, WebStoreService>();

         StaticNodeJSService.Configure<OutOfProcessNodeJSServiceOptions>(options => options.TimeoutMS = 2000);
      }

      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         app.UseAuthentication();
         app.UseWebSockets();
         app.UseDotNetify(config =>
         {
            config.RegisterAssembly("DotNetify.DevApp.ViewModels");

            var tokenValidationParameters = new TokenValidationParameters
            {
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthServer.SecretKey)),
               ValidateIssuerSigningKey = true,
               ValidateAudience = false,
               ValidateIssuer = false,
               ValidateLifetime = true,
               ClockSkew = TimeSpan.FromSeconds(0)
            };

            // Middleware to do authenticate token in incoming request headers.
            config.UseJwtBearerAuthentication(tokenValidationParameters);

            // Filter to check whether user has permission to access view models with [Authorize] attribute.
            config.UseFilter<AuthorizeFilter>();

            // Middleware to log incoming/outgoing message; default to Sytem.Diagnostic.Trace.
            var logger = app.ApplicationServices.GetService<ILogger<VMController>>();
            config.UseDeveloperLogging(log =>
            {
               if (!log.Contains("vmId=PulseVM"))
                  logger.LogInformation(log);
               System.Diagnostics.Trace.WriteLine(log);
            });

            // Demonstration middleware that extracts auth token from incoming request headers.
            config.UseMiddleware<ExtractAccessTokenMiddleware>(tokenValidationParameters);

            // Demonstration filter that passes access token from the middleware to the ViewModels.SecurePageVM class instance.
            config.UseFilter<SetAccessTokenFilter>();
         });
         app.UseDotNetifyPulse();

         if (env.IsDevelopment())
         {
#pragma warning disable 618
            app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
            {
               HotModuleReplacement = true,
               HotModuleReplacementClientOptions = new Dictionary<string, string> { { "reload", "true" } },
            });
#pragma warning restore 618
         }

         app.UseStaticFiles();
         app.UseRouting();
         app.UseEndpoints(endpoints => endpoints.MapHub<DotNetifyHub>("/dotnetify"));

         //app.UseSsr(typeof(App), (string[] args) => StaticNodeJSService.InvokeFromFileAsync<string>("wwwroot/ssr", null, args), DefaultRequestHandler);
         app.Run(DefaultRequestHandler);
      }

      private static async Task DefaultRequestHandler(HttpContext context)
      {
         var uri = context.Request.Path.ToUriComponent();
         if (uri.EndsWith(".map"))
            return;
         else if (uri.EndsWith("_hmr") || uri.Contains("hot-update"))  // Fix HMR for deep links.
            context.Response.Redirect(Regex.Replace(uri, ".+/dist", "/dist"));

         using var reader = new StreamReader(File.OpenRead("wwwroot/index.html"));
         await context.Response.WriteAsync(reader.ReadToEnd());
      }
   }
}