using System;
using DotNetify;

namespace ViewModels.CompositeView
{
   public class AFITop100VM : BaseVM
   {
      private readonly MovieService _movieService;
      private readonly MovieTableVM _movieTableVM;

      /// <summary>
      /// Constructor.
      /// </summary>
      public AFITop100VM()
      {
         // Normally this will be constructor-injected.
         _movieService = new MovieService();

         _movieTableVM = new MovieTableVM();
         _movieTableVM.SetDataSource(() => _movieService.GetAFITop100());
      }

      public override BaseVM GetSubVM(string vmTypeName)
      {
         if (vmTypeName == nameof(MovieTableVM))
            return _movieTableVM;

         return base.GetSubVM(vmTypeName);
      }
   }
}
