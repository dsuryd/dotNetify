/*
Copyright 2021 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;

namespace DotNetify.Postgres
{
   /// <summary>
   /// Provides objects for observing data change events on database tables.
   /// </summary>
   public interface IDbChangeObserver
   {
      /// <summary>
      /// Provides an observable object to observe data change events on a specified table.
      /// </summary>
      /// <typeparam name="TTable">Table type.</typeparam>
      /// <returns>Observable object for the table.</returns>
      IObservable<IDbChangeEvent<TTable>> Observe<TTable>() where TTable : new();
   }

   public class DbChangeObserver : IDbChangeObserver
   {
      private IPostgresReplicationSubscriber _postgresReplication;
      private Dictionary<Type, Dictionary<string, PropertyInfo>> _tableProps = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

      public DbChangeObserver(IPostgresReplicationSubscriber postgresReplication)
      {
         _postgresReplication = postgresReplication;
      }

      public IObservable<IDbChangeEvent<TTable>> Observe<TTable>() where TTable : new()
      {
         if (!_tableProps.ContainsKey(typeof(TTable)))
         {
            // Cache the property info of the table type for fast lookup.
            var tableProps = typeof(TTable).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty).Select(prop =>
            {
               var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
               return KeyValuePair.Create(columnAttr != null ? columnAttr.Name : prop.Name, prop);
            }).ToDictionary(x => x.Key, x => x.Value);

            _tableProps.Add(typeof(TTable), tableProps);
         }

         return _postgresReplication.Transaction.Where(Filter<TTable>).SelectMany(Map<TTable>);
      }

      private TTable BuildRow<TTable>(Relation relation, object[] columnValues)
      {
         if (columnValues == null)
            return default;

         if (!_tableProps.TryGetValue(typeof(TTable), out Dictionary<string, PropertyInfo> tableProps))
            return default;

         var result = Activator.CreateInstance<TTable>();
         var columns = relation.ColumnNames.Zip(columnValues, (key, value) => KeyValuePair.Create(key, value)).ToDictionary(x => x.Key, x => x.Value);
         foreach (var column in columns)
         {
            try
            {
               if (tableProps.ContainsKey(column.Key))
               {
                  var value = column.Value;
                  if (value is string && tableProps[column.Key].PropertyType != typeof(string))
                     value = TypeDescriptor.GetConverter(tableProps[column.Key].PropertyType).ConvertFromString((string) value);
                  tableProps[column.Key].SetValue(result, value);
               }
            }
            catch (Exception ex)
            {
               System.Diagnostics.Debug.WriteLine(ex.Message);
            }
         }
         return result;
      }

      private bool Filter<TTable>(Transaction transaction)
      {
         return transaction.DataEvents.Any(x => x.Relation?.Name == GetTableName<TTable>());
      }

      private string GetTableName<TTable>()
      {
         var tableType = typeof(TTable);
         var tableAttribute = tableType.GetCustomAttribute<TableAttribute>();
         return tableAttribute?.Name ?? tableType.Name;
      }

      private IEnumerable<IDbChangeEvent<TTable>> Map<TTable>(Transaction transaction)
      {
         string tableName = GetTableName<TTable>();
         foreach (DataEvent dataEvent in transaction.DataEvents)
         {
            if (dataEvent is InsertEvent)
            {
               var insertEvent = dataEvent as InsertEvent;
               if (insertEvent.Relation?.Name == tableName)
                  yield return new DbInsertEvent<TTable>
                  {
                     Row = BuildRow<TTable>(insertEvent.Relation, insertEvent.ColumnValues)
                  };
            }
            else if (dataEvent is UpdateEvent)
            {
               var updateEvent = dataEvent as UpdateEvent;
               if (updateEvent.Relation?.Name == tableName)
                  yield return new DbUpdateEvent<TTable>
                  {
                     NewRow = BuildRow<TTable>(updateEvent.Relation, updateEvent.ColumnValues),
                     OldRow = BuildRow<TTable>(updateEvent.Relation, updateEvent.OldColumnValues)
                  };
            }
            else if (dataEvent is DeleteEvent)
            {
               var deleteEvent = dataEvent as DeleteEvent;
               if (deleteEvent.Relation?.Name == tableName)
                  yield return new DbDeleteEvent<TTable>
                  {
                     Row = BuildRow<TTable>(deleteEvent.Relation, deleteEvent.OldColumnValues ?? deleteEvent.Keys)
                  };
            }
         }
      }
   }
}