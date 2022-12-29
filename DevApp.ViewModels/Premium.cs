using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class DotNetifyTesting : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.Premium.DotNetifyTesting.md");
   }

   public class DotNetifyLoadTester : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.Premium.DotNetifyLoadTester.md");
   }

   public class DotNetifyObserver : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.Premium.DotNetifyObserver.md");
   }

   public class DotNetifyResiliencyAddon : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.Premium.DotNetifyResiliencyAddon.md");
   }
}