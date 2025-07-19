﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DevApp.ViewModels
{
   public interface IWebStoreService
   {
      IEnumerable<WebStoreRecord> GetAllRecords();

      IEnumerable<WebStoreRecord> GetAllBooks();

      WebStoreRecord GetBookByTitle(string title);

      WebStoreRecord GetBookById(int id);
   }

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

      public static string ToUrlSafe(string title) => title.ToLower()
         .Replace("\'", "")
         .Replace(".", "dot")
         .Replace("#", "sharp")
         .Replace(' ', '-');
   }

   public class WebStoreService : IWebStoreService
   {
      public IEnumerable<WebStoreRecord> GetAllRecords() => JsonConvert.DeserializeObject<List<WebStoreRecord>>(
         Utils.GetResource("TestViewModels.webstore.json", GetType().Assembly).Result);

      public IEnumerable<WebStoreRecord> GetAllBooks() => GetAllRecords().Where(i => i.Type == "Book");

      public WebStoreRecord GetBookByTitle(string title) => GetAllBooks().FirstOrDefault(i => i.UrlSafeTitle == title);

      public WebStoreRecord GetBookById(int id) => GetAllBooks().FirstOrDefault(i => i.Id == id);
   }
}