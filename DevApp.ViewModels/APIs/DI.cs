using System;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class DI : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.DI.md");
   }
}