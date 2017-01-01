using System.Collections.Generic;
using Newtonsoft.Json;

namespace ViewModels
{
   public class WebStoreRecord
   {
      public int Id { get; set; }
      public string Type { get; set; }
      public string Category { get; set; }
      public bool Recommended { get; set; }
      public string Title { get; set; }
      public string Author { get; set; }
      public float Rating { get; set; }
      public string ImageUrl { get; set; }
      public string ItemUrl { get; set; }

      public static List<WebStoreRecord> GetMockupData() => JsonConvert.DeserializeObject<List<WebStoreRecord>>(Properties.Resources.webstore_json);
   }

   public class WebStoreModel
   {
      public static List<WebStoreRecord> AllRecords => WebStoreRecord.GetMockupData();
   }

   public static class Utils
   {
      /// <summary>
      /// Converts a product title into a safe string for URL.
      /// </summary>
      public static string SafeUrl(string iTitle) => iTitle.ToLower().Replace("\'", "").Replace(".", "dot").Replace("#", "sharp").Replace(' ', '-');
   }
}
