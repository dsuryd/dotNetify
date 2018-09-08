using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DotNetify.Routing;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class BookStoreExample : BaseVM
   {
      public BookStoreExample()
      {
         var markdown = new Markdown("DotNetify.DevApp.Docs.Examples.BookStore.md");

         AddProperty("ViewSource", markdown.GetSection(null, "BookStoreVM.cs"))
           .SubscribeTo(AddInternalProperty<string>("Framework").Select(GetViewSource));

         AddProperty("ViewModelSource", markdown.GetSection("BookStoreVM.cs"));
      }

      private string GetViewSource(string framework)
      {
          return framework == "Knockout" ?
              new Markdown("DotNetify.DevApp.Docs.Examples.Knockout.BookStore.md") :
              new Markdown("DotNetify.DevApp.Docs.Examples.BookStore.md").GetSection(null, "BookStoreVM.cs");
      }      
   }

   public class BookStoreVM : BaseVM, IRoutable
   {
      private readonly IWebStoreService _webStoreService;

      public RoutingState RoutingState { get; set; }

      public IEnumerable<object> Books => _webStoreService
         .GetAllBooks()
         .Select(i => new { Info = i, Route = this.GetRoute("Book", "book/" + i.UrlSafeTitle) });

      public BookStoreVM(IWebStoreService webStoreService)
      {
         _webStoreService = webStoreService;

         // Register the route templates with RegisterRoutes method extension of the IRoutable.
         this.RegisterRoutes("examples/bookstore", new List<RouteTemplate>
         {
            new RouteTemplate("BookDefault") { UrlPattern = "" },
            new RouteTemplate("Book") { UrlPattern = "book(/:title)" }
         });
      }
   }

   public class BookDetailsVM : BaseVM, IRoutable
   {
      private readonly IWebStoreService _webStoreService;

      public RoutingState RoutingState { get; set; }
      public WebStoreRecord Book { get; set; }

      public BookDetailsVM(IWebStoreService webStoreService)
      {
         _webStoreService = webStoreService;

         this.OnRouted((sender, e) =>
         {
            if (!string.IsNullOrEmpty(e.From))
            {
               // Extract the book title from the route path.
               var bookTitle = e.From.Replace("book/", "");

               Book = _webStoreService.GetBookByTitle(bookTitle);
               Changed(nameof(Book));
            }
         });
      }
   }
}