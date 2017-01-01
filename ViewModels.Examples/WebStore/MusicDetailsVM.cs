using System;
using System.Linq;
using DotNetify;
using DotNetify.Routing;

namespace ViewModels.WebStore
{
   /// <summary>
   /// Music details view model for the Web Store example.
   /// This implements IRoutable to be able to handle Routed event from the MusicStore view
   /// in order to determine the title of the music to display.
   /// </summary>
   public class MusicDetailsVM : BaseVM, IRoutable
   {
      // Display properties. 
      public string Title { get; set; }
      public string Artist { get; set; }
      public int[] Rating { get; set; }
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
      public MusicDetailsVM()
      {
         // Use OnRouted method extension to handle the Routed event which occurs when this view was 
         // loaded due to route action from another view.  In this case, it's from the MusicStore view
         // when a certain music route is activated.  The "from" path is given, so this view model
         // only needs to extract the music title from that path to determine which music info to display.
         // The format of the "from" path is the URL pattern of the Music template defined in MusicStoreVM.
         this.OnRouted((sender, e) => OnRoutedFrom(e.From));
      }

      /// <summary>
      /// Sets music information given its title.
      /// </summary>
      public void SetMusic(string iTitle)
      {
         var music = WebStoreModel.AllRecords.FirstOrDefault(i => Utils.SafeUrl(i.Title) == iTitle);
         if (music != null)
         {
            Title = music.Title;
            Artist = music.Author;
            Rating = new int[(int)music.Rating];
            ImageUrl = music.ImageUrl;
            ItemUrl = music.ItemUrl;
         }
      }

      /// <summary>
      /// Extracts from the routed "from" path the title of the music and whether the 
      /// review tab should be first to open.
      /// </summary>
      private void OnRoutedFrom(string iFromPath)
      {
         if (!String.IsNullOrEmpty(iFromPath))
         {
            var title = iFromPath.Replace("music/", "");
            SetMusic(title);
         }
      }
   }
}
