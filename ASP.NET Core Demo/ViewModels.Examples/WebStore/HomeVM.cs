using DotNetify;
using DotNetify.Routing;

namespace ViewModels.WebStore
{
   /// <summary>
   /// Home view model of the Web Store example, which displays a carousel of routes.
   /// This implements IRoutable to provide routing to BookStore and MusicStore views.
   /// </summary>
   public class HomeVM : BaseVM, IRoutable
   {
      /// <summary>
      /// Book route, which uses Redirect method extension of IRoutable.
      /// The method is used for routes whose templates are not defined in another view model.
      /// The first param is the redirect root path of the path given in the second param.
      /// At runtime, when the route is activated, dotNetify router will look for the view that
      /// matches this path: webstore/books, no matter how deeply nested it is.
      /// </summary>
      public Route Book { get { return this.Redirect("webstore", "books"); } }

      public Route Music { get { return this.Redirect("webstore", "musics"); } }

      /// <summary>
      /// Stores routing state. 
      /// </summary>
      public RoutingState RoutingState { get; set; }
   }
}
