using System.Collections.Generic;
using System.Linq;
using DotNetify;
using DotNetify.Routing;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates SPA routing with deep-linking.
   /// </summary>
   public class BookStoreVM : BaseVM, IRoutable
   {
      public RoutingState RoutingState { get; set; }

      public BookStoreVM()
      {
         // Register the route templates with RegisterRoutes method extension of the IRoutable.
         this.RegisterRoutes("BookStore", new List<RouteTemplate>
         {
            new RouteTemplate("BookStoreFront") {UrlPattern = "" },
            new RouteTemplate("Book") { UrlPattern = "book(/:title)" }
         });
      }
   }

   public class BookStoreFrontVM : BaseVM, IRoutable
   {
      // Normally services should come from dependency injection.
      private readonly WebStoreService _webStoreService = new WebStoreService();

      public RoutingState RoutingState { get; set; } = new RoutingState();
      public IEnumerable<object> Books => _webStoreService.GetAllBooks().Select( i => new { Info = i, Route = this.Redirect("BookStore", "book/" + i.UrlSafeTitle) });
   }

   public class BookDetailsVM : BaseVM, IRoutable
   {
      // Normally services should come from dependency injection.
      private readonly WebStoreService _webStoreService = new WebStoreService();

      public RoutingState RoutingState { get; set; }
      public WebStoreRecord Book { get; set; }

      public BookDetailsVM()
      {
         this.OnRouted((sender, e) =>
         {
            if (!string.IsNullOrEmpty(e.From))
            {
               var bookTitle = e.From.Replace("book/", "");
               Book = _webStoreService.GetBookByTitle(bookTitle);

               Changed(nameof(Book));
            }
         });
      }
   }
}
