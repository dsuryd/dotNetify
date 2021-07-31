using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealtimeDb
{
   [Table("businesses")]
   public class Business
   {
      [Column("business_id")]
      [Key]
      public long Id { get; set; }

      [Column("business_name")]
      public string Name { get; set; }

      [Column("rating")]
      public int Rating { get; set; }
   }
}