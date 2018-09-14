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

  public class FromScratchScriptTagKO : BaseVM
  {
    public string Content => new Markdown("DotNetify.DevApp.Docs.Knockout.FromScratchScriptTag.md");
  }  
}