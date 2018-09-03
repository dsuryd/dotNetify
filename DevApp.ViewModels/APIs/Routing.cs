using System;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class Routing : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.Routing.md");
   }
}