using System.Collections.Generic;
using DotNetify;
using DotNetify.Routing;

namespace ViewModels.WebStore
{
   /// <summary>
   /// Navigation bar view model for the Web Store example. 
   /// This implements IRoutable to provide routing to BookStore and MusicStore sub-views.
   /// </summary>
   public class NavBarVM : BaseVM, IRoutable
   {
      /// <summary>
      /// This class holds navigation menu items.
      /// Activating a menu item routes to another view.
      /// </summary>
      public class MenuItem
      {
         public string Caption { get; set; }
         public Route Route { get; set; }
      }

      /// <summary>
      /// Navigation menu items.
      /// </summary>
      public List<MenuItem> MenuItems { get; set; }

      /// <summary>
      /// Stores routing state.
      /// </summary>
      public RoutingState RoutingState { get; set; }

      /// <summary>
      /// Constructor.
      /// </summary>
      public NavBarVM()
      {
         // Register the route templates with RegisterRoutes method extension of the IRoutable.
         // - The first param is the root path.
         // - Id uniquely identifies a route template.
         // - UrlPattern is the URL pattern that maps to a view. 
         //   For example: <domain>/webstore/books maps to the view of "Books" template.
         // - ViewUrl is the URL of the view, which can be a simple HTML file or an ASP.NET Controller action.
         // - VMType is the view model type of the view.  This is only used if you want to do server-side routing.
         this.RegisterRoutes("webstore", new List<RouteTemplate>
         {
            new RouteTemplate { Id = "Home", UrlPattern = "", Target = "NavContent", ViewUrl = "/Demo/WebStore/Home" },
            new RouteTemplate { Id = "Books", UrlPattern = "books", Target = "NavContent", ViewUrl = "/Demo/WebStore/BookStore_cshtml", VMType = typeof(BookStoreVM) },
            new RouteTemplate { Id = "Music", UrlPattern = "musics", Target = "NavContent", ViewUrl = "/Demo/WebStore/MusicStore" }
         });

         // Use GetRoute method extension to get the route object given a template ID.
         // Bind this object to vmRoute binding type on the view.
         MenuItems = new List<MenuItem> {
            new MenuItem { Caption = "Home", Route = this.GetRoute( "Home" ) },
            new MenuItem { Caption = "Books", Route = this.GetRoute( "Books" ) },
            new MenuItem { Caption = "Music", Route = this.GetRoute( "Music" ) }
         };
      }
   }
}
