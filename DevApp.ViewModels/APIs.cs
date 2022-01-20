using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class Connection : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.Connection.md");
   }

   public class CRUD : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.CRUD.md");
   }

   public class DI : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.DI.md");
   }

   public class DotNetClient : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.DotNetClient.md");
   }

   public class Filter : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.Filter.md");
   }

   public class LocalMode : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.LocalMode.md");
   }

   public class LocalModeVue : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.LocalMode.md").GetSection(null, "Local VMContext");
   }

   public class Middleware : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.Middleware.md");
   }

   public class MinimalApi : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.MinimalApi.md");
   }

   public class Multicast : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.Multicast.md");
   }

   public class Routing : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.Routing.md");
   }

   public class RoutingKO : BaseVM
   {
      public string Content
      {
         get
         {
            var content = new Markdown("DotNetify.DevApp.Docs.APIs.Routing.md");
            var contentKO = new Markdown("DotNetify.DevApp.Docs.Knockout.APIs.Routing.md");
            return contentKO.GetSection(null, "Setting Up Route Links")
                + content.GetSection("Defining the Routes", "Setting Up Route Links")
                + contentKO.GetSection("Setting Up Route Links")
                + content.GetSection("Redirection", "Getting Initial State");
         }
      }
   }

   public class RoutingVue : BaseVM
   {
      public string Content
      {
         get
         {
            var content = new Markdown("DotNetify.DevApp.Docs.APIs.Routing.md");
            var contentKO = new Markdown("DotNetify.DevApp.Docs.Vue.APIs.Routing.md");
            return contentKO.GetSection(null, "Setting Up Route Links")
                + content.GetSection("Defining the Routes", "Setting Up Route Links")
                + contentKO.GetSection("Setting Up Route Links")
                + content.GetSection("Redirection", "Getting Initial State");
         }
      }
   }

   public class Security : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.Security.md");
   }

   public class SecurityKO : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.Security.md").GetSection(null, "Token");
   }

   public class SecurityVue : BaseVM
   {
      public string Content => ((string) new Markdown("DotNetify.DevApp.Docs.APIs.Security.md")).Replace("react", "vue");
   }

   public class WebApiMode : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.WebApiMode.md");
   }
}