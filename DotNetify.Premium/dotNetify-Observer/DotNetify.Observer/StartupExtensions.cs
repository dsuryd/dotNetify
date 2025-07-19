using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetify.Observer
{
   public static class StartupExtensions
   {
      public static IServiceCollection AddDotNetifyObserver(this IServiceCollection services)
      {
         services.AddSingleton<IConnectionTracker, ConnectionTracker>();
         services.AddSingleton<INodeInfoBuilder, NodeInfoBuilder>();
         services.AddScoped<IAppState, AppState>();
         return services;
      }

      public static IDotNetifyConfiguration ConfigureObserver(this IDotNetifyConfiguration config)
      {
         config.RegisterAssembly(typeof(ConnectionGraphVM).Assembly);
         config.UseMiddleware<ObserveMiddleware>();
         return config;
      }

      public static IDotNetifyConfiguration ConfigureObserverHost(this IDotNetifyConfiguration config)
      {
         config.RegisterAssembly(typeof(ConnectionGraphVM).Assembly);
         config.UseMiddleware<ObserveForwardingMiddleware>();
         return config;
      }

      public static void MapObserver(this IEndpointRouteBuilder endpoints, string endpoint = "/observer")
      {
         ObserverAppVM.RouteRoot = endpoint.Trim('/');

         endpoints.MapGet(endpoint, CreateRequestDelegate(endpoints));

         endpoints.MapGet($"{endpoint.TrimEnd('/')}/reset", context =>
         {
            var connectionTracker = endpoints.CreateApplicationBuilder().ApplicationServices.GetService<IConnectionTracker>();
            if (connectionTracker != null)
            {
               connectionTracker.Reset();
               context.Response.Redirect(endpoint);
            }
            return Task.CompletedTask;
         });
      }

      private static RequestDelegate CreateRequestDelegate(IEndpointRouteBuilder endpoints)
      {
         var app = endpoints.CreateApplicationBuilder();

         app.Use(next => async context =>
         {
            try
            {
               License.Check();
            }
            catch (Exception ex)
            {
               await context.Response.WriteAsync(ex.Message);
               return;
            }

            context.Request.Path = "/observer-ui/index.html";

            // Set endpoint to null so the static files middleware will handle the request.
            context.SetEndpoint(null);

            await next(context);
         });

         if (app.ApplicationServices.GetService<StaticFileMiddleware>() == null)
            app.UseStaticFiles();

         return app.Build();
      }
   }
}