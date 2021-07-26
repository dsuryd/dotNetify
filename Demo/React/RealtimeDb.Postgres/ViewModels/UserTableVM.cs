using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DotNetify;
using DotNetify.Postgres;
using Microsoft.EntityFrameworkCore;

namespace RealtimeDb
{
   public class UserTableVM : BaseVM
   {
      private readonly IDbContextFactory<UserAccountDbContext> _dbContextFactory;

      [ItemKey(nameof(User.UserId))]
      public List<User> Users { get; set; }

      public UserTableVM(IDbContextFactory<UserAccountDbContext> dbContextFactory, IDbChangeObserver dbChangeObserver)
      {
         _dbContextFactory = dbContextFactory;

         using var dbContext = _dbContextFactory.CreateDbContext();
         Users = dbContext.Users.ToList();

         this.ObserveList<User>(nameof(Users), dbChangeObserver);
      }

      public void AddUser(string userName)
      {
         using var dbContext = _dbContextFactory.CreateDbContext();
         dbContext.Users.Add(new User { UserName = userName });
         dbContext.SaveChanges();
      }

      public void UpdateUser(User userInfo)
      {
         using var dbContext = _dbContextFactory.CreateDbContext();
         var user = dbContext.Users.Find(userInfo.UserId);
         if (user != null)
         {
            user.UserName = userInfo.UserName;
            dbContext.SaveChanges();
         }
      }

      public void RemoveUser(long userId)
      {
         using var dbContext = _dbContextFactory.CreateDbContext();
         var user = dbContext.Users.Find(userId);
         if (user != null)
         {
            dbContext.Users.Remove(user);
            dbContext.SaveChanges();
         }
      }
   }
}