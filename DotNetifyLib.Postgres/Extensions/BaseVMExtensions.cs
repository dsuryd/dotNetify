using System;
using System.Reflection;
using System.Reactive.Linq;
using System.Linq;

namespace DotNetify.Postgres
{
   public static class BaseVMExtensions
   {
      /// <summary>
      /// Observes database changes on the table associated with a view model's list property.
      /// </summary>
      /// <typeparam name="TTable">Type associated with a database table.</typeparam>
      /// <param name="baseVM">View model instance.</param>
      /// <param name="listPropName">Property name of the view model's list.</param>
      /// <param name="dbChangeObserver">Db change observer service.</param>
      public static void ObserveList<TTable>(this BaseVM baseVM, string listPropName, IDbChangeObserver dbChangeObserver) where TTable : new()
      {
         ObserveList<TTable, TTable>(baseVM, listPropName, dbChangeObserver, _ => _);
      }

      /// <summary>
      /// Observes database changes on the table associated with a view model's list property.
      /// </summary>
      /// <typeparam name="TTable">Type associated with a database table.</typeparam>
      /// <typeparam name="TList">Type associated with the view model's list.</typeparam>
      /// <param name="baseVM">View model instance.</param>
      /// <param name="listPropName">Property name of the view model's list.</param>
      /// <param name="dbChangeObserver">Db change observer service.</param>
      /// <param name="selector">Selector to transform TTable instance into TList instance.</param>
      public static void ObserveList<TTable, TList>(this BaseVM baseVM, string listPropName, IDbChangeObserver dbChangeObserver, Func<TTable, TList> selector) where TTable : new()
      {
         listPropName = listPropName ?? throw new ArgumentNullException(nameof(listPropName));

         PropertyInfo listProp = baseVM.GetType().GetProperty(listPropName);
         listProp = listProp ?? throw new DotNetifyPostgresException($"Property '{baseVM.GetType().Name}.{listPropName}' was not found.");

         string itemKeyPropName = listProp.GetCustomAttribute<ItemKeyAttribute>()?.ItemKey;
         if (itemKeyPropName == null)
            itemKeyPropName = baseVM.RuntimeProperties.FirstOrDefault(x => x.Name == $"{listPropName}_itemKey")?.Name;

         itemKeyPropName = itemKeyPropName ?? throw new DotNetifyPostgresException($"Property '{baseVM.GetType().Name}.{listPropName}' doesn't define an item key.");

         PropertyInfo itemKeyPropInfo = typeof(TTable).GetProperty(itemKeyPropName);

         bool handleDbChangeEvent(IDbChangeEvent<TTable> dbChangeEvent)
         {
            if (dbChangeEvent is DbInsertEvent<TTable>)
            {
               baseVM.AddList(listPropName, selector((dbChangeEvent as DbInsertEvent<TTable>).Row));
            }
            else if (dbChangeEvent is DbUpdateEvent<TTable>)
            {
               baseVM.UpdateList(listPropName, selector((dbChangeEvent as DbUpdateEvent<TTable>).NewRow));
            }
            else if (dbChangeEvent is DbDeleteEvent<TTable>)
            {
               var key = itemKeyPropInfo.GetValue((dbChangeEvent as DbDeleteEvent<TTable>).Row);
               baseVM.RemoveList(listPropName, key);
            }
            else
               return false;

            baseVM.PushUpdates();
            return true;
         }

         baseVM.AddInternalProperty<bool>($"{listPropName}_observe").SubscribeTo(dbChangeObserver.Observe<TTable>().Select(handleDbChangeEvent));
      }
   }
}