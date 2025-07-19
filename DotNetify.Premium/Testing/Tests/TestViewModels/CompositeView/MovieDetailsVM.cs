﻿using DotNetify;

namespace DevApp.ViewModels
{
   public class MovieDetailsVM : BaseVM
   {
      private readonly IMovieService _movieService;

      public MovieRecord Movie
      {
         get { return Get<MovieRecord>(); }
         set { Set(value); }
      }

      public MovieDetailsVM(IMovieService movieService)
      {
         _movieService = movieService;
      }

      public void SetByAFIRank(int rank) => Movie = _movieService.GetMovieByAFIRank(rank);
   }
}