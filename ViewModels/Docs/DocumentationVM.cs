using System.Collections.Generic;
using System.Linq;
using DotNetify;
using DotNetify.Routing;

namespace ViewModels
{
   public class DocumentationVM : BaseVM, IRoutable
   {
      public class SubSection
      {
         public string Title { get; set; }
         public string Description { get; set; }
         public string View { get; set; }
         public string ViewUrl { get; set; }
         public bool HasScript { get; set; }
         public Route Route { get; set; }
      }

      public class Section
      {
         public string Id { get; set; }
         public string Title { get; set; }
         public List<SubSection> SubSections { get; set; }
         public bool Collapse { get; set; }
      }

      public class Link
      {
         public Route Route { get; set; }
         public string Caption { get; set; }
      }

      public List<Section> Sections { get; set; }

      public Dictionary<string, bool> HasNotes { get; set; }

      public Link Next
      {
         get { return Get<Link>(); }
         set { Set(value); }
      }

      public Link Prev
      {
         get { return Get<Link>(); }
         set { Set(value); }
      }

      public bool HasNext
      {
         get { return Get<bool>(); }
         set { Set(value); }
      }

      public bool HasPrev
      {
         get { return Get<bool>(); }
         set { Set(value); }
      }

      /// <summary>
      /// Stores routing state.
      /// </summary>
      public RoutingState RoutingState { get; set; }

      public DocumentationVM()
      {
         var docs = new List<SubSection>
         {
            new SubSection { Title = "Overview", View="Overview" },
            new SubSection { Title = "Installing DotNetify", View="Installing" }
         };

         var basicExamples = new List<SubSection>
         {
               new SubSection { Title = "Hello World", Description = "Getting familiar with the basics", View="HelloWorld" },
               new SubSection { Title = "Control Types", Description = "Using various HTML control types", View="ControlTypes" },
               new SubSection { Title = "Simple List", Description = "Simple CRUD list with no Javascript", View="SimpleList" },
               new SubSection { Title = "Better List", Description = "Similar CRUD list with Javascript to optimize bandwidth usage", View="BetterList", HasScript = true },
         };

         var furtherExamples = new List<SubSection>
         {
               new SubSection { Title = "Grid View", Description = "More complex list with master-detail view, search, sorting and localization", View="GridView", HasScript = true },
               new SubSection { Title = "Tree View", Description = "Lazy loading technique on deep tree", View="TreeView", HasScript = true },
               new SubSection { Title = "Composite View", Description = "Compositing a view and enabling communication between the sub-views", View="CompositeView", HasScript = true },
               new SubSection { Title = "Live Chart", Description = "Working with real time push notification", View="LiveChart", HasScript = true },
               new SubSection { Title = "Dashboard", Description = "Putting it all together: modular and dynamically interacting views", View="Dashboard", HasScript = true },
               new SubSection { Title = "Web Store", Description = "Routing, deep-linking and server-side rendering on a single-page application", View="WebStore", ViewUrl = "/Webstore .container-fluid" }
         };

         var moreInfo = new List<SubSection>
         {
            new SubSection { Title = "Security", View="Security" },
            new SubSection { Title = "API Reference", View="Reference" },
         };


         var templates = new List<RouteTemplate>() {
            new RouteTemplate { Id = "Index", UrlPattern = "", Target = "Content", ViewUrl = "/Demo/Docs/Overview" }
         };

         foreach (var section in docs)
            templates.Add(new RouteTemplate { Id = section.View, UrlPattern = section.View, Target = "Content", ViewUrl = "/Demo/Docs/" + section.View });

         HasNotes = new Dictionary<string, bool>();
         foreach (var section in furtherExamples.Union(basicExamples))
         {
            templates.Add(new RouteTemplate
            {
               Id = section.View,
               UrlPattern = section.View,
               Target = "Content",
               ViewUrl = section.ViewUrl ?? "/Demo/" + section.View + " .container-fluid:first",
               JSModuleUrl = section.HasScript ? "/Scripts/Demo/" + section.View + ".js" : null
            });
            HasNotes.Add(section.View, true);
         }

         foreach (var section in moreInfo)
            templates.Add(new RouteTemplate { Id = section.View, UrlPattern = section.View, Target = "Content", ViewUrl = "/Demo/Docs/" + section.View });

         this.RegisterRoutes("index", templates);
         this.OnActivated((sender, e) => UpdateLinks(e.Active));

         Sections = new List<Section>()
         {
            new Section { Id="GetStarted", Title = "Getting Started", SubSections = docs },
            new Section { Id="BasicExamples", Title = "Basic Examples", SubSections = basicExamples },
            new Section { Id="FurtherExamples", Title = "Further Examples", SubSections = furtherExamples, Collapse = true },
            //new Section { Id="MoreInfo", Title = "More Info", SubSections = moreInfo,  Collapse = true }
         };

         foreach (var section in furtherExamples.Union(basicExamples).Union(moreInfo).Union(docs))
            section.Route = this.GetRoute(section.View);

         UpdateLinks("Overview");
      }

      public void UpdateLinks(string iActive)
      {
         var links = new List<SubSection>();
         foreach (var section in Sections)
            links = links.Union(section.SubSections).ToList();

         SubSection next = null, prev = null;
         var currentIdx = links.FindIndex(i => i.View == iActive);
         if (currentIdx < links.Count - 1)
            next = links[currentIdx + 1];
         if (currentIdx > 0)
            prev = links[currentIdx - 1];
         if (prev != null && prev.View == "Installing")
            prev = null;

         Next = next != null ? new Link { Route = new Route { TemplateId = next.View }, Caption = next.Title } : new Link { Route = this.GetRoute("Overview") };
         Prev = prev != null ? new Link { Route = new Route { TemplateId = prev.View }, Caption = prev.Title } : new Link { Route = this.GetRoute("Overview") };
         HasNext = next != null;
         HasPrev = prev != null;
      }
   }
}
