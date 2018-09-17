using System;
using System.Linq;
using DotNetify;
using DotNetify.Routing;

namespace ViewModels.WebStore
{
   /// <summary>
   /// Book details view model for the Web Store example.
   /// This implements IRoutable to be able to handle Routed event from the BookStore view
   /// in order to determine the title of the book to display, and whether the review tab should open.
   /// </summary>
   public class BookDetailsVM : BaseVM, IRoutable
   {
      // Display properties. 
      public string Title { get; set; }
      public string Author { get; set; }
      public float Rating { get; set; }
      public string ImageUrl { get; set; }
      public string ItemUrl { get; set; }

      /// <summary>
      /// Whether to show detailed information tab or the review tab.
      /// </summary>
      public bool ShowDetails { get; set; }
      public bool ShowReviews { get { return !ShowDetails; } }

      /// <summary>
      /// Stores routing state.
      /// </summary>
      public RoutingState RoutingState { get; set; }

      /// <summary>
      /// Constructor.
      /// </summary>
      public BookDetailsVM()
      {
         // Use OnRouted method extension to handle the Routed event which occurs when this view was 
         // loaded due to route action from another view.  In this case, it's from the BookStore view
         // when a certain book route is activated.  The "from" path is given, so this view model
         // only needs to extract the book title from that path to determine which book info to display.
         // The format of the "from" path is the URL pattern of the Book template defined in BookStoreVM.
         this.OnRouted((sender, e) => OnRoutedFrom(e.From));

         ShowDetails = true;
      }

      /// <summary>
      /// Sets book information given its title.
      /// </summary>
      public void SetBook(string iTitle)
      {
         var book = WebStoreModel.AllRecords.FirstOrDefault(i => Utils.SafeUrl(i.Title) == iTitle);
         if (book != null)
         {
            Title = book.Title;
            Author = book.Author;
            Rating = book.Rating;
            ImageUrl = book.ImageUrl;
            ItemUrl = book.ItemUrl;
         }
      }

      /// <summary>
      /// Extracts from the routed "from" path the title of the book and whether the 
      /// review tab should be first to open.
      /// </summary>
      private void OnRoutedFrom(string iFromPath)
      {
         if (!String.IsNullOrEmpty(iFromPath))
         {
            var title = iFromPath.Replace("book/", "");
            if (title.Contains("/"))
            {
               if (title.ToLower().Contains("/review"))
                  ShowDetails = false;
               title = title.Remove(title.IndexOf("/"));
            }
            SetBook(title);
         }
      }
   }
}
