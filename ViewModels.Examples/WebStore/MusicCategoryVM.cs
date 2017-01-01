using System.Collections.Generic;
using System.Linq;
using DotNetify;
using DotNetify.Routing;

namespace ViewModels.WebStore
{
   /// <summary>
   /// Music category view model for the Web Store example.
   /// This implements IRoutable to be able to handle Routed event from the MusicStore view
   /// in order to determine the name of the music category to display.
   /// </summary>
   public class MusicCategoryVM : BaseVM, IRoutable
   {
      /// <summary>
      /// This class holds music information.
      /// </summary>
      public class Music
      {
         public Route Route { get; set; }
         public string Title { get; set; }
         public string Artist { get; set; }
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
      /// List of musics to display.
      /// </summary>
      public List<Music> Musics
      {
         get { return GetMusicsByCategory(CategoryName) ?? GetMusicsByCategory("Recommended"); }
      }

      /// <summary>
      /// Stores routing state.
      /// </summary>
      public RoutingState RoutingState { get; set; }

      /// <summary>
      /// Constructor.
      /// </summary>
      public MusicCategoryVM()
      {
         // Use OnRouted method extension to handle the Routed event which occurs when this view was 
         // loaded due to route action from another view.  In this case, it's from the MusicStore view
         // when a certain category route is activated.  The "from" path is given, so this view model
         // only needs to extract the category name from that path to determine what category to display.
         // The format of the "from" path is the URL pattern of the Category template defined in MusicStoreVM.
         this.OnRouted((sender, e) =>
         {
            CategoryName = e.From.Replace("category/", "").Replace('-', ' ');
            Changed(() => Musics);
         });
      }

      /// <summary>
      /// Gets list of musics by category.
      /// </summary>
      private List<Music> GetMusicsByCategory(string iCategory)
      {
         var musicRecords = iCategory == "Recommended" ?
            WebStoreModel.AllRecords.Where(i => i.Type.ToLower() == "music" && i.Recommended) :
            WebStoreModel.AllRecords.Where(i => i.Type.ToLower() == "music" && i.Category.ToLower() == iCategory.ToLower());

         var musics = new List<Music>();
         foreach (var music in musicRecords)
         {
            musics.Add(new Music
            {
               Route = this.Redirect("webstore/musics", "music/" + Utils.SafeUrl(music.Title)),
               ImageUrl = music.ImageUrl,
               Title = music.Title,
               Artist = music.Author
            });
         }

         CategoryName = iCategory == "Recommended" ? "Recommended For You" : musicRecords.Count() > 0 ? musicRecords.First().Category : "";

         return musics.Count > 0 ? musics : null;
      }
   }
}
