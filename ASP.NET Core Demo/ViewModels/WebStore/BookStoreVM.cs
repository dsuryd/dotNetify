using System.Collections.Generic;
using System.Linq;
using DotNetify;
using DotNetify.Routing;

namespace ViewModels.WebStore
{
   /// <summary>
   /// Book store front of the Web Store example.
   /// This implements IRoutable to provide routing to book category and book details views.
   /// </summary>
   public class BookStoreVM : BaseVM, IRoutable
   {
      /// <summary>
      /// This class holds book category information.
      /// </summary>
      public class Category
      {
         public string Name { get; set; }
         public Route Route { get; set; }
      }

      /// <summary>
      /// List of book categories.
      /// </summary>
      public List<Category> Categories { get; set; }

      /// <summary>
      /// Stores routing state.
      /// </summary>
      public RoutingState RoutingState { get; set; }

      /// <summary>
      /// Constructor.
      /// </summary>
      public BookStoreVM()
      {
         // Register the route templates with RegisterRoutes method extension of the IRoutable.
         // - The first param is the root path.
         // - Id uniquely identifies a route template.
         // - UrlPattern is the URL pattern that maps to a view. 
         //   A pattern may have parameters, which are identified with the colon prefix.
         //   The brackets indicate that there can be zero or one argument.
         // - ViewUrl is the URL of the view, which can be a simple HTML file or an ASP.NET Controller action.
         // - VMType is the view model type of the view.  This is only used if you want to do server-side routing.
         this.RegisterRoutes( "books", new List<RouteTemplate>
         {
            new RouteTemplate { Id = "BooksIndex", UrlPattern = "", Target = "BookStoreContent", ViewUrl = "/Demo/WebStore/BookCategory_cshtml" },
            new RouteTemplate { Id = "Category", UrlPattern = "category(/:name)", Target = "BookStoreContent", ViewUrl = "/Demo/WebStore/BookCategory_cshtml", VMType = typeof( BookCategoryVM ) },
            new RouteTemplate { Id = "Book", UrlPattern = "book(/:title)(/:tab)", Target = "BookStoreContent", ViewUrl = "/Demo/WebStore/BookDetails_cshtml", VMType = typeof( BookDetailsVM ) }
         } );

         // Get all categories from the model to fill Categories property.
         var categoryNames = WebStoreModel.AllRecords
            .Where(i => i.Type.ToLower() == "book")
            .GroupBy(i => i.Category)
            .Select(j => j.First())
            .Select(k => k.Category).ToList();

         Categories = new List<Category>();
         foreach (var name in categoryNames)
            // Use GetRoute method extension of the IRoutable to get the route objects to be bound to vmRoute binding types on the view.
            // For parameterized template, you will need to set the actual path to route to.
            Categories.Add(new Category { Name = name, Route = this.GetRoute("Category", "category/" + name.Replace(" ", "-").ToLower()) });
      }
   }
}
