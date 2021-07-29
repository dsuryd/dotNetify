using System.Collections.Generic;
using System.Linq;
using DotNetify;
using DotNetify.Postgres;
using Microsoft.EntityFrameworkCore;

namespace RealtimeDb.ViewModels
{
   public class AccountVM : BaseVM
   {
      private readonly IDbContextFactory<UserAccountDbContext> _dbContextFactory;

      [ItemKey(nameof(User.UserId))]
      public List<Account> Accounts { get; set; }

      public AccountVM(IDbContextFactory<UserAccountDbContext> dbContextFactory, IDbChangeObserver dbChangeObserver)
      {
         _dbContextFactory = dbContextFactory;

         using var dbContext = _dbContextFactory.CreateDbContext();
         Accounts = dbContext.Accounts.OrderBy(x => x.AccountId)
         .Include(x => x.User)
         .ToList();
      }
   }
}