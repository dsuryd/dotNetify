using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace SPA
{
   public interface IMovieService
   {
      IEnumerable<MovieRecord> GetAFITop100();

      MovieRecord GetMovieByAFIRank(int rank);
   }

   public class MovieRecord
   {
      public int Rank { get; set; }
      public string Movie { get; set; }
      public int Year { get; set; }
      public string Cast { get; set; }
      public string Director { get; set; }
   }

   public class MovieService : IMovieService
   {
      public IEnumerable<MovieRecord> GetAFITop100() => JsonConvert.DeserializeObject<List<MovieRecord>>(
        File.ReadAllText("server//ViewModels//CompositeView//AFITop100.json"));

      public MovieRecord GetMovieByAFIRank(int rank) => GetAFITop100().FirstOrDefault(i => i.Rank == rank);
   }
}