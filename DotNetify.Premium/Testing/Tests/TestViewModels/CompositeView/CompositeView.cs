﻿using DotNetify;
using System;

namespace DevApp.ViewModels
{
   public class CompositeViewVM : BaseVM
   {
      private readonly IMovieService _movieService;

      private event EventHandler<int> Selected;

      public CompositeViewVM(IMovieService movieService)
      {
         _movieService = movieService;
      }

      public override void OnSubVMCreated(BaseVM subVM)
      {
         if (subVM is FilterableMovieTableVM)
            InitMovieTableVM(subVM as FilterableMovieTableVM);
         else if (subVM is MovieDetailsVM)
            InitMovieDetailsVM(subVM as MovieDetailsVM);
      }

      private void InitMovieTableVM(FilterableMovieTableVM vm)
      {
         // Set the movie table data source to AFI Top 100 movies.
         vm.DataSource = () => _movieService.GetAFITop100();

         // When movie table selection changes, raise a private Selected event.
         vm.Selected += (sender, rank) => Selected?.Invoke(this, rank);
      }

      private void InitMovieDetailsVM(MovieDetailsVM vm)
      {
         // Set default details to the highest ranked movie.
         vm.SetByAFIRank(1);

         // When the Selected event occurs, update the movie details.
         Selected += (sender, rank) => vm.SetByAFIRank(rank);
      }
   }
}