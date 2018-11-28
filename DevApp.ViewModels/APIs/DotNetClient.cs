using System;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class DotNetClient : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.DotNetClient.md");
   }
}