using System;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class MicroFrontend : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.MicroFrontend.md");
   }
}