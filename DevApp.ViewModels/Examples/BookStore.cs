using System;
using DotNetify;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class BookStoreExample : BaseVM
   {
      public BookStoreExample()
      {
         var markdown = new Markdown("DotNetify.DevApp.Docs.Examples.BookStore.md");

         AddProperty("ViewSource", markdown.GetSection(null, "BookStoreVM.cs"));
         AddProperty("ViewModelSource", markdown.GetSection("BookStoreVM.cs"));
      }
   }

   public class BookStoreVM : BaseVM
   {
   }
}