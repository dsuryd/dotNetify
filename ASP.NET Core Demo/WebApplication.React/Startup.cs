using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using DotNetify;
using DotNetify.Security;

namespace WebApplication.Core.React
{
   public class Startup
   {
      // This method gets called by the runtime. Use this method to add services to the container.
      // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddMvc();
         services.AddLocalization();

         services.AddSignalR();  // Required by dotNetify.
         services.AddDotNetify();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app)
      {
         app.UseStaticFiles();
         app.UseAuthServer(); // Provide auth tokens for Secure Page demo.

         app.UseWebSockets();
         app.UseSignalR(routes => routes.MapDotNetifyHub());   // Required by dotNetify.
         app.UseDotNetify(config =>
         {
            string secretKey = "dotnetifydemo_secretkey_123!";
            var tokenValidationParameters = new TokenValidationParameters
            {
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
               ValidAudience = "DotNetifyDemoApp",
               ValidIssuer = "DotNetifyDemoServer",
               ValidateIssuerSigningKey = true,
               ValidateAudience = true,
               ValidateIssuer = true,
               ValidateLifetime = true,
               ClockSkew = TimeSpan.FromSeconds(0)
            };

            // Middleware to log incoming/outgoing message; default to Sytem.Diagnostic.Trace.
            config.UseDeveloperLogging();

            // Middleware to do authenticate token in incoming request headers.
            config.UseJwtBearerAuthentication(tokenValidationParameters);

            // Filter to check whether user has permission to access view models with [Authorize] attribute.
            config.UseFilter<AuthorizeFilter>();

            // Demonstration middleware that extracts auth token from incoming request headers.
            config.UseMiddleware<ExtractAccessTokenMiddleware>(tokenValidationParameters);

            // Demonstration filter that passes access token from the middleware to the ViewModels.SecurePageVM class instance.
            config.UseFilter<SetAccessTokenFilter>();
         });

         app.UseMvc(routes =>
         {
            routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");
         });
      }
   }
}
