using System.Collections.Generic;
using System.Linq;
using DotNetify;
using DotNetify.Routing;

namespace ViewModels.WebStore
{
   /// <summary>
   /// Book category view model for the Web Store example.
   /// This implements IRoutable to be able to handle Routed event from the BookStore view
   /// in order to determine the name of the book category to display.
   /// </summary>
   public class BookCategoryVM : BaseVM, IRoutable
   {
      /// <summary>
      /// This class holds book information.
      /// </summary>
      public class Book
      {
         public Route Route { get; set; }
         public string Title { get; set; }
         public string Author { get; set; }
         public string ImageUrl { get; set; }
      }

      /// <summary>
      /// Display text for the category name.
      /// </summary>
      public string CategoryName
      {
         get { return Get<string>() ?? "Recommended"; }
         set { Set(value); }
      }

      /// <summary>
      /// List of books to display.
      /// </summary>
      public List<Book> Books
      {
         get { return GetBooksByCategory(CategoryName) ?? GetBooksByCategory("Recommended"); }
      }

      /// <summary>
      /// Stores routing state.
      /// </summary>
      public RoutingState RoutingState { get; set; }

      /// <summary>
      /// Constructor.
      /// </summary>
      public BookCategoryVM()
      {
         // Use OnRouted method extension to handle the Routed event which occurs when this view was 
         // loaded due to route action from another view.  In this case, it's from the BookStore view
         // when a certain category route is activated.  The "from" path is given, so this view model
         // only needs to extract the category name from that path to determine what category to display.
         // The format of the "from" path is the URL pattern of the Category template defined in BookStoreVM.
         this.OnRouted((sender, e) =>
         {
            CategoryName = e.From.Replace("category/", "").Replace('-', ' ');
            Changed(() => Books);
         });
      }

      /// <summary>
      /// Gets list of books by category.
      /// </summary>
      private List<Book> GetBooksByCategory(string iCategory)
      {
         var bookRecords = iCategory == "Recommended" ?
            WebStoreModel.AllRecords.Where(i => i.Type.ToLower() == "book" && i.Recommended) :
            WebStoreModel.AllRecords.Where(i => i.Type.ToLower() == "book" && i.Category.ToLower() == iCategory.ToLower());

         var books = new List<Book>();
         foreach (var book in bookRecords)
         {
            books.Add(new Book
            {
               Route = this.Redirect("webstore/books", "book/" + Utils.SafeUrl(book.Title)),
               ImageUrl = book.ImageUrl,
               Title = book.Title,
               Author = book.Author
            });
         }

         CategoryName = iCategory == "Recommended" ? "Recommended For You" : bookRecords.Count() > 0 ? bookRecords.First().Category : "";

         return books.Count > 0 ? books : null;
      }
   }
}
