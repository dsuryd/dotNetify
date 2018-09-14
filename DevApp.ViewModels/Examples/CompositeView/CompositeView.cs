using System;
using System.Reactive.Linq;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class CompositeViewExample : BaseVM
   {
      public CompositeViewExample()
      {
         var markdown = new Markdown("DotNetify.DevApp.Docs.Examples.CompositeView.md");

         AddProperty("ViewSource", markdown.GetSection(null, "CompositeViewVM.cs"))
            .SubscribeTo(AddInternalProperty<string>("Framework").Select(GetViewSource));

         AddProperty("ViewModelSource", markdown.GetSection("CompositeViewVM.cs"));
      }

      private string GetViewSource(string framework)
      {
         return framework == "Knockout" ?
            new Markdown("DotNetify.DevApp.Docs.Knockout.Examples.CompositeView.md") :
            new Markdown("DotNetify.DevApp.Docs.Examples.CompositeView.md").GetSection(null, "CompositeViewVM.cs");
      }
   }

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