using System;
using DotNetify;

namespace ViewModels.CompositeView
{
   public class AFITop100VM : BaseVM
   {
      private readonly MovieService _movieService;
      private event EventHandler<int> SelectedRank;

      /// <summary>
      /// Constructor.
      /// </summary>
      public AFITop100VM()
      {
         // Normally this will be constructor-injected.
         _movieService = new MovieService();
      }

      public override void OnSubVMCreated(BaseVM subVM)
      {
         if (subVM is FilterableMovieTableVM)
         {
            var vm = subVM as FilterableMovieTableVM;
            vm.DataSource = () => _movieService.GetAFITop100();
            vm.Selected += (sender, rank) => SelectedRank?.Invoke(this, rank);
         }
         else if (subVM is MovieDetailsVM)
         {
            var vm = subVM as MovieDetailsVM;
            vm.SetByAFIRank(1);
            SelectedRank += (sender, rank) => vm.SetByAFIRank(rank); 
         }
      }
   }
}
