using System;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class WebApiMode : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.WebApiMode.md");
   }
}