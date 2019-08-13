using System;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class LocalMode : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.LocalMode.md");
   }

   public class LocalModeVue : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.LocalMode.md").GetSection(null, "Local View Model");
   }
}