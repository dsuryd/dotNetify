using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class Overview : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.Overview.md");
   }
}