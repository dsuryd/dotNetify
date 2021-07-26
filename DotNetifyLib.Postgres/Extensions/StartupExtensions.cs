using Microsoft.Extensions.DependencyInjection;

namespace DotNetify.Postgres
{
   public static class StartupExtensions
   {
      public static IServiceCollection AddDotNetifyPostgres(this IServiceCollection services, PostgresConfiguration config)
      {
         services.AddSingleton(config);
         services.AddSingleton<IPostgresReplicationSubscriber, PostgresReplicationSubscriber>();
         services.AddSingleton<IDbChangeObserver, DbChangeObserver>();
         return services;
      }
   }
}