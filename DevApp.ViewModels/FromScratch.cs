using System;
using System.Threading;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
  public class FromScratchWebPack : BaseVM
  {
    public string Content => new Markdown("DotNetify.DevApp.Docs.FromScratchWebPack.md");
  }

  public class FromScratchScriptTag : BaseVM
  {
    public string Content => new Markdown("DotNetify.DevApp.Docs.FromScratchScriptTag.md");
  }

  public class FromScratchWebPackKO : BaseVM
  {
    public string Content => new Markdown("DotNetify.DevApp.Docs.FromScratchWebPack.Knockout.md");
  }

  public class FromScratchScriptTagKO : BaseVM
  {
    public string Content => new Markdown("DotNetify.DevApp.Docs.FromScratchScriptTag.Knockout.md");
  }  
}