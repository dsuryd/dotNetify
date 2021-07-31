using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DotNetify;
using DotNetify.Postgres;
using Microsoft.EntityFrameworkCore;

namespace RealtimeDb
{
   public class BusinessesVM : BaseVM
   {
      private readonly IDbContextFactory<BusinessDbContext> _dbContextFactory;

      [ItemKey(nameof(Business.Id))]
      public List<Business> Businesses { get; set; }

      public BusinessesVM(IDbContextFactory<BusinessDbContext> dbContextFactory, IDbChangeObserver dbChangeObserver)
      {
         _dbContextFactory = dbContextFactory;

         using var dbContext = _dbContextFactory.CreateDbContext();
         Businesses = dbContext.Businesses.OrderBy(x => x.Id).ToList();

         this.ObserveList<Business>(nameof(Businesses), dbChangeObserver);
      }

      public void Add(Business businessInfo)
      {
         using var dbContext = _dbContextFactory.CreateDbContext();
         dbContext.Businesses.Add(businessInfo);
         dbContext.SaveChanges();
      }

      public void Update(Business businessInfo)
      {
         using var dbContext = _dbContextFactory.CreateDbContext();
         var business = dbContext.Businesses.Find(businessInfo.Id);
         if (business != null)
         {
            business.Name = businessInfo.Name;
            business.Rating = businessInfo.Rating;
            dbContext.SaveChanges();
         }
      }

      public void Remove(Business businessInfo)
      {
         using var dbContext = _dbContextFactory.CreateDbContext();
         var business = dbContext.Businesses.Find(businessInfo.Id);
         if (business != null)
         {
            dbContext.Businesses.Remove(business);
            dbContext.SaveChanges();
         }
      }
   }
}