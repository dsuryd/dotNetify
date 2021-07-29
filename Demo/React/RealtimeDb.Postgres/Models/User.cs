using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealtimeDb
{
   [Table("users")]
   public class User
   {
      [Column("user_id")]
      [Key]
      public long UserId { get; set; }

      [Column("username")]
      public string UserName { get; set; }
   }
}