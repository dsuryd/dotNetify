using System;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class Security : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.Security.md");
   }

   public class SecurityKO : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.APIs.Security.md").GetSection(null, "Token");
   }   

   public class SecurityVue : BaseVM
   {
      public string Content => ((string) new Markdown("DotNetify.DevApp.Docs.APIs.Security.md")).Replace("react", "vue");
   }      
}