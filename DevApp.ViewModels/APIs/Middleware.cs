using System;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class Middleware : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.Middleware.md");
   }
}