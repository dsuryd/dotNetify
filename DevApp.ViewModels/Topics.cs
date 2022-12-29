using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class MicroFrontend : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.MicroFrontend.md");
   }

   public class Reactive : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.Reactive.md");
   }

   public class RealtimePostgres : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.RealtimePostgres.md");
   }

   public class Scaleout : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.Scaleout.md");
   }

   public class AWSIntegration : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.AWSIntegration.md");
   }
}