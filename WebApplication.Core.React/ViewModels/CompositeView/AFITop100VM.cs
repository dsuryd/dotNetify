using System;
using DotNetify;

namespace ViewModels.CompositeView
{
   public class AFITop100VM : BaseVM
   {
      private readonly MovieService _movieService;
      private readonly MovieTableVM _movieTableVM;
      private readonly MovieDetailsVM _movieDetailsVM;

      /// <summary>
      /// Constructor.
      /// </summary>
      public AFITop100VM()
      {
         // Normally this will be constructor-injected.
         _movieService = new MovieService();
         _movieTableVM = new MovieTableVM();
         _movieDetailsVM = new MovieDetailsVM(_movieService);

         _movieTableVM.SetDataSource(() => _movieService.GetAFITop100());
         _movieTableVM.Selected += (sender, rank) => _movieDetailsVM.SetByAFIRank(rank);
      }

      public override BaseVM GetSubVM(string vmTypeName)
      {
         if (vmTypeName == nameof(MovieTableVM))
            return _movieTableVM;
         else if (vmTypeName == nameof(MovieDetailsVM))
            return _movieDetailsVM;

         return base.GetSubVM(vmTypeName);
      }
   }
}
