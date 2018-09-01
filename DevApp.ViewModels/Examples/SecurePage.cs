using System;
using DotNetify;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class SecurePageExample : BaseVM
   {
      public SecurePageExample()
      {
         var markdown = new Markdown("DotNetify.DevApp.Docs.Examples.SecurePage.md");

         AddProperty("ViewSource", markdown.GetSection(null, "SecurePageVM.cs"));
         AddProperty("ViewModelSource", markdown.GetSection("SecurePageVM.cs"));
      }
   }

   public class SecurePageVM : BaseVM
   {
   }
}