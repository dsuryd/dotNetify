using System;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class GetStarted : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.GetStarted.md");
   }
}