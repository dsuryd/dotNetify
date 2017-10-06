using System.Collections.Generic;
using Newtonsoft.Json;

namespace ViewModels
{
   public class MovieRecord
   {
      public int Rank { get; set; }
      public string Movie { get; set; }
      public int Year { get; set; }
      public string Cast { get; set; }
      public string Director { get; set; }

      public static List<MovieRecord> GetData()
      {
         return JsonConvert.DeserializeObject<List<MovieRecord>>(Properties.Resources.AFITop100_json);
      }
   }

   public class AFITop100Model
   {
      private static List<MovieRecord> _allRecords = MovieRecord.GetData();
      public List<MovieRecord> AllRecords {  get { return _allRecords; } }
   }
}
