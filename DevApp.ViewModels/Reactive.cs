using System;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class Reactive : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.Reactive.md");
   }
}