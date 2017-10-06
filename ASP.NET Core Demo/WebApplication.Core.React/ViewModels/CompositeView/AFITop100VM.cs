using System;
using DotNetify;

namespace ViewModels.CompositeView
{
   public class AFITop100VM : BaseVM
   {
      private readonly MovieService _movieService;
      private event EventHandler<int> Selected;

      /// <summary>
      /// Constructor.
      /// </summary>
      public AFITop100VM()
      {
         // Normally this will be constructor-injected.
         _movieService = new MovieService();
      }

      /// <summary>
      /// This method is called when an instance of a view model inside this view model's scope is being created.
      /// It provides a chance for this view model to initialize them.
      /// </summary>
      /// <param name="subVM">Sub-view model instance.</param>
      public override void OnSubVMCreated(BaseVM subVM)
      {
         if (subVM is FilterableMovieTableVM)
            InitMovieTableVM(subVM as FilterableMovieTableVM);
         else if (subVM is MovieDetailsVM)
            InitMovieDetailsVM(subVM as MovieDetailsVM);
      }

      private void InitMovieTableVM( FilterableMovieTableVM vm)
      {
         // Set the movie table data source to AFI Top 100 movies.
         vm.DataSource = () => _movieService.GetAFITop100();

         // When movie table selection changes, raise a private Selected event.
         vm.Selected += (sender, rank) => Selected?.Invoke(this, rank);
      }

      private void InitMovieDetailsVM( MovieDetailsVM vm)
      {
         // Set default details to the highest ranked movie.
         vm.SetByAFIRank(1);

         // When the Selected event occurs, update the movie details.
         Selected += (sender, rank) => vm.SetByAFIRank(rank);
      }
   }
}
