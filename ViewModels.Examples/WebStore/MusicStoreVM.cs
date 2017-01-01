using System.Collections.Generic;
using System.Linq;
using DotNetify;
using DotNetify.Routing;

namespace ViewModels.WebStore
{
   /// <summary>
   /// Music store front of the Web Store example.
   /// This implements IRoutable to provide routing to music category and music details views.
   /// </summary>
   public class MusicStoreVM : BaseVM, IRoutable
   {
      /// <summary>
      /// This class holds music category information.
      /// </summary>
      public class Category
      {
         public string Name { get; set; }
         public Route Route { get; set; }
      }

      /// <summary>
      /// List of music categories.
      /// </summary>
      public List<Category> Categories { get; set; }

      /// <summary>
      /// Stores routing state.
      /// </summary>
      public RoutingState RoutingState { get; set; }

      /// <summary>
      /// Constructor.
      /// </summary>
      public MusicStoreVM()
      {
         // Register the route templates with RegisterRoutes method extension of the IRoutable.
         // - The first param is the root path.
         // - Id uniquely identifies a route template.
         // - UrlPattern is the URL pattern that maps to a view. 
         //   A pattern may have parameters, which are identified with the colon prefix.
         //   The brackets indicate that there can be zero or one argument.
         // - ViewUrl is the URL of the view, which can be a simple HTML file or an ASP.NET Controller action.
         this.RegisterRoutes( "musics", new List<RouteTemplate>
         {
            new RouteTemplate { Id = "MusicsIndex", UrlPattern = "", Target = "MusicStoreContent", ViewUrl = "/Demo/WebStore/MusicCategory" },
            new RouteTemplate { Id = "Category", UrlPattern = "category(/:name)", Target = "MusicStoreContent", ViewUrl = "/Demo/WebStore/MusicCategory" },
            new RouteTemplate { Id = "Music", UrlPattern = "music(/:title)", Target = "MusicStoreContent", ViewUrl = "/Demo/WebStore/MusicDetails" }
         } );

         // Get all categories from the model to fill Categories property.
         var categoryNames = WebStoreModel.AllRecords
            .Where(i => i.Type.ToLower() == "music")
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
