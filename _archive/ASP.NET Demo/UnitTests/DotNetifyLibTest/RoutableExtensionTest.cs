using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetify;
using DotNetify.Routing;

namespace UnitTest.DotNetifyLibTest
{
   [TestClass]
   public class RoutableExtensionTest
   {
      private class TestNavBarVM : BaseVM, IRoutable
      {
         public RoutingState RoutingState { get; set; }
         public ActivatedEventArgs TestActivatedEventArgs { get; set; }

         public TestNavBarVM()
         {
            this.RegisterRoutes("index", new List<RouteTemplate>
            {
               new RouteTemplate { Id = "Books", UrlPattern = "books", Target = "NavContent", ViewUrl = "/BookStore_cshtml", VMType = typeof(TestBookStoreVM) },
            });
            this.OnActivated((sender, e) => TestActivatedEventArgs = e);
         }
      }

      private class TestBookStoreVM : BaseVM, IRoutable
      {
         public RoutingState RoutingState { get; set; }
         public ActivatedEventArgs TestActivatedEventArgs { get; set; }

         public TestBookStoreVM()
         {
            this.RegisterRoutes("books", new List<RouteTemplate>
            {
               new RouteTemplate { Id = "BooksIndex", UrlPattern = "", Target = "BookStoreContent", ViewUrl = "/BookCategory_cshtml" },
               new RouteTemplate { Id = "Category", UrlPattern = "category(/:name)", Target = "BookStoreContent", ViewUrl = "/BookCategory_cshtml", VMType = typeof( TestBookCategoryVM ) },
               new RouteTemplate { Id = "Book", UrlPattern = "book(/:title)(/:tab)", Target = "BookStoreContent", ViewUrl = "/BookDetails_cshtml", VMType = typeof( TestBookDetailsVM ) }
            });
            this.OnActivated((sender, e) => TestActivatedEventArgs = e);
         }
      }

      private class TestBookCategoryVM : BaseVM, IRoutable
      {
         public RoutingState RoutingState { get; set; }
         public RoutedEventArgs TestRoutedEventArgs { get; set; }

         public TestBookCategoryVM()
         {
            this.OnRouted((sender, e) => TestRoutedEventArgs = e);
         }
      }

      private class TestBookDetailsVM : BaseVM, IRoutable
      {
         public RoutingState RoutingState { get; set; }
         public RoutedEventArgs TestRoutedEventArgs { get; set; }

         public TestBookDetailsVM()
         {
            this.OnRouted((sender, e) => TestRoutedEventArgs = e);
         }
      }

      [TestMethod]
      public void Routing_GetRoute()
      {
         var navVM = new TestNavBarVM();
         var route = navVM.GetRoute("Books");
         Assert.IsNotNull(route);
         Assert.AreEqual("books", route.Path);

         var storeVM = new TestBookStoreVM();
         route = storeVM.GetRoute("Category");
         Assert.IsNotNull(route);
         Assert.AreEqual("category(/:name)", route.Path);

         route = storeVM.GetRoute("Book");
         Assert.IsNotNull(route);
         Assert.AreEqual("book(/:title)(/:tab)", route.Path);
      }

      [TestMethod]
      public void Routing_Redirect()
      {
         var storeVM = new TestBookStoreVM();
         var route = storeVM.Redirect("musics", "music/25");
         Assert.IsNotNull(route);
         Assert.AreEqual("musics", route.RedirectRoot);
         Assert.AreEqual("music/25", route.Path);
      }

      [TestMethod]
      public void Routing_RouteBookCategory()
      {
         var viewData = new RoutingViewData("/index/books/category/fiction", "/Index_cshtml", typeof(TestNavBarVM));

         IRoutable model;
         string viewId = String.Empty;

         viewId = RoutableExtension.Route(ref viewData, out model);
         Assert.IsNotNull(viewId);
         Assert.AreEqual("/Index_cshtml", viewId);
         Assert.IsNotNull(model is TestNavBarVM);

         var activatedEventArgs = (model as TestNavBarVM).TestActivatedEventArgs;
         Assert.IsNotNull(activatedEventArgs);
         Assert.AreEqual("books", activatedEventArgs.Active);

         viewId = RoutableExtension.Route(ref viewData, out model);
         Assert.IsNotNull(viewId);
         Assert.AreEqual("/BookStore_cshtml", viewId);
         Assert.IsNotNull(model is TestBookStoreVM);

         activatedEventArgs = (model as TestBookStoreVM).TestActivatedEventArgs;
         Assert.IsNotNull(activatedEventArgs);
         Assert.AreEqual("category/fiction", activatedEventArgs.Active);

         viewId = RoutableExtension.Route(ref viewData, out model);
         Assert.IsNotNull(viewId);
         Assert.AreEqual("/BookCategory_cshtml", viewId);
         Assert.IsNotNull(model is TestBookCategoryVM);

         var routedEventArgs = (model as TestBookCategoryVM).TestRoutedEventArgs;
         Assert.IsNotNull(routedEventArgs);
         Assert.AreEqual("category/fiction", routedEventArgs.From);
      }

      [TestMethod]
      public void Routing_RouteBookDetails()
      {
         var viewData = new RoutingViewData("/index/books/book/the-martian", "/Index_cshtml", typeof(TestNavBarVM));

         IRoutable model;
         string viewId = String.Empty;

         viewId = RoutableExtension.Route(ref viewData, out model);
         Assert.IsNotNull(viewId);
         Assert.AreEqual("/Index_cshtml", viewId);
         Assert.IsNotNull(model is TestNavBarVM);

         var activatedEventArgs = (model as TestNavBarVM).TestActivatedEventArgs;
         Assert.IsNotNull(activatedEventArgs);
         Assert.AreEqual("books", activatedEventArgs.Active);

         viewId = RoutableExtension.Route(ref viewData, out model);
         Assert.IsNotNull(viewId);
         Assert.AreEqual("/BookStore_cshtml", viewId);
         Assert.IsNotNull(model is TestBookStoreVM);

         activatedEventArgs = (model as TestBookStoreVM).TestActivatedEventArgs;
         Assert.IsNotNull(activatedEventArgs);
         Assert.AreEqual("book/the-martian", activatedEventArgs.Active);

         viewId = RoutableExtension.Route(ref viewData, out model);
         Assert.IsNotNull(viewId);
         Assert.AreEqual("/BookDetails_cshtml", viewId);
         Assert.IsNotNull(model is TestBookDetailsVM);

         var routedEventArgs = (model as TestBookDetailsVM).TestRoutedEventArgs;
         Assert.IsNotNull(routedEventArgs);
         Assert.AreEqual("book/the-martian", routedEventArgs.From);
      }
   }
}
