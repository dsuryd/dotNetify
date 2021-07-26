using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealtimeDb
{
   [Table("accounts")]
   public class Account
   {
      [Column("account_id")]
      [Key]
      public long AccountId { get; set; }

      [Column("user_id")]
      public long UserId { get; set; }

      [Column("balance")]
      public decimal Balance { get; set; }

      [Column("created_on")]
      public DateTime CreatedOn { get; set; }
   }
}