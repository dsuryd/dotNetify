using System;
using DotNetify;

namespace ViewModels.CompositeView
{
   public class MovieDetailsVM : BaseVM
   {
      private readonly MovieService _movieService;

      public MovieRecord Movie
      {
         get { return Get<MovieRecord>(); }
         set { Set(value); }
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      public MovieDetailsVM()
      {
         // Normally this will be constructor-injected.
         _movieService = new MovieService();
      }

      public void SetByAFIRank(int rank) => Movie = _movieService.GetMovieByAFIRank(rank);
   }
}
