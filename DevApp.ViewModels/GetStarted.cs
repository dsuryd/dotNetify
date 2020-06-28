using System;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class GetStarted : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.GetStarted.md");
   }

  public class GetStartedKO : BaseVM
  {
    public string Content => 
      new Markdown("DotNetify.DevApp.Docs.Knockout.GetStarted.md").GetSection(null, "Client Setup") +
      new Markdown("DotNetify.DevApp.Docs.GetStarted.md").GetSection("NuGet Packages", "Client Setup") +
      new Markdown("DotNetify.DevApp.Docs.Knockout.GetStarted.md").GetSection("Client Setup") + 
      new Markdown("DotNetify.DevApp.Docs.GetStarted.md").GetSection(".NET Framework");
  }   

  public class GetStartedVue : BaseVM
  {
    public string Content => 
      new Markdown("DotNetify.DevApp.Docs.Vue.GetStarted.md").GetSection(null, "Client Setup") +
      new Markdown("DotNetify.DevApp.Docs.GetStarted.md").GetSection("NuGet Packages", "Client Setup") +
      new Markdown("DotNetify.DevApp.Docs.Vue.GetStarted.md").GetSection("Client Setup") + 
      new Markdown("DotNetify.DevApp.Docs.GetStarted.md").GetSection(".NET Framework");
  }     
}