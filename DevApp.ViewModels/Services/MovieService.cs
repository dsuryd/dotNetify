using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using DotNetify.Elements;

namespace DotNetify.DevApp
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
         Utils.GetResource("DotNetify.DevApp.Docs.Examples.AFITop100.json", GetType().Assembly).Result);

      public MovieRecord GetMovieByAFIRank(int rank) => GetAFITop100().FirstOrDefault(i => i.Rank == rank);
   }
}