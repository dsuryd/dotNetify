using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WebApplication.Core.React;

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
      public string UrlSafeTitle => ToUrlSafe(Title);

      public static string ToUrlSafe(string title) => title.ToLower().Replace("\'", "").Replace(".", "dot").Replace("#", "sharp").Replace(' ', '-');
   }

   public class WebStoreService 
   {
      public IEnumerable<WebStoreRecord> GetAllRecords() => JsonConvert.DeserializeObject<List<WebStoreRecord>>(this.GetEmbeddedResource("WebStore.json"));

      public IEnumerable<WebStoreRecord> GetAllBooks() => GetAllRecords().Where(i => i.Type == "Book");

      public WebStoreRecord GetBookByTitle( string title ) => GetAllBooks().FirstOrDefault(i => i.UrlSafeTitle == title);
   }
}
