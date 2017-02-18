using System;
using DotNetify;

namespace ViewModels.CompositeView
{
   public class MovieDetailsVM : BaseVM
   {
      private MovieService _movieService;

      public MovieRecord Movie
      {
         get { return Get<MovieRecord>(); }
         set { Set(value); }
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      public MovieDetailsVM(MovieService moviceService)
      {
         _movieService = moviceService;
      }

      public void SetByAFIRank(int rank) => Movie = _movieService.GetMovieByAFIRank(rank);
   }
}
