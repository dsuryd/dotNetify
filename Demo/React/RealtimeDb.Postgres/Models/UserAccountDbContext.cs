using Microsoft.EntityFrameworkCore;

namespace RealtimeDb
{
   public class UserAccountDbContext : DbContext
   {
      public DbSet<User> Users { get; set; }
      public DbSet<Account> Accounts { get; set; }

      public UserAccountDbContext(DbContextOptions<UserAccountDbContext> options) : base(options)
      {
      }
   }
}