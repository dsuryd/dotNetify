using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using DotNetify;
using DotNetify.Postgres;

namespace RealtimeDb.ViewModels
{
   public class ListenOnlyVM : BaseVM
   {
      private IDisposable _subs;

      [ItemKey(nameof(Business.Id))]
      public List<Business> Businesses { get; set; }

      public ListenOnlyVM(IDbChangeObserver dbChangeObserver)
      {
         Businesses = new List<Business>();
         
         _subs = dbChangeObserver.Observe<Business>().Subscribe(e =>
         {
            if (e is DbInsertEvent<Business>)
            {
               this.AddList(nameof(Businesses), (e as DbInsertEvent<Business>).Row);
            }
            else if (e is DbUpdateEvent<Business>)
            {
               this.UpdateList(nameof(Businesses), (e as DbUpdateEvent<Business>).NewRow);
            }
            else if (e is DbDeleteEvent<Business>)
            {
               var key = (e as DbDeleteEvent<Business>).Row.Id;
               this.RemoveList(nameof(Businesses), key);
            }

            PushUpdates();
         });
      }

      public override void Dispose() => _subs.Dispose();
   }
}