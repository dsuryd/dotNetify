using Microsoft.EntityFrameworkCore;

namespace RealtimeDb
{
   public class BusinessDbContext : DbContext
   {
      public DbSet<Business> Businesses { get; set; }

      public BusinessDbContext(DbContextOptions<BusinessDbContext> options) : base(options)
      {
      }
   }
}