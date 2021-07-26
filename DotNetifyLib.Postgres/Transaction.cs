using System.Collections.Generic;

namespace DotNetify.Postgres
{
   public class Transaction
   {
      public List<DataEvent> DataEvents { get; } = new List<DataEvent>();
   }

   public class Relation
   {
      public uint Id { get; set; }
      public string Name { get; set; }
      public string[] ColumnNames { get; set; }
   }

   public class DataEvent
   {
      public Relation Relation { get; set; }
   }

   public class InsertEvent : DataEvent
   {
      public object[] ColumnValues { get; set; }
   }

   public class UpdateEvent : DataEvent
   {
      public object[] ColumnValues { get; set; }
      public object[] OldColumnValues { get; set; }
   }

   public class DeleteEvent : DataEvent
   {
      public object[] Keys { get; set; }
      public object[] OldColumnValues { get; set; }
   }
}