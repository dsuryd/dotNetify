using System.Collections.Generic;
using Newtonsoft.Json;
using WebApplication.Core.React.Properties;

namespace ViewModels
{
   public class MovieRecord
   {
      public int Rank { get; set; }
      public string Movie { get; set; }
      public int Year { get; set; }
      public string Cast { get; set; }
      public string Director { get; set; }
   }

   public class MovieService
   {
      public IEnumerable<MovieRecord> GetAFITop100() => JsonConvert.DeserializeObject<List<MovieRecord>>(Resources.AFITop100_json);
   }
}
