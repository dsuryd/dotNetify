using System;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class Serverless : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.Serverless.md");
   }

   public class ServerlessVue : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.Serverless.md").GetSection(null, "Local View Model");
   }
}