using System;
using System.Threading;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
  public class FromScratchWebPack : BaseVM
  {
    public string Content => new Markdown("DotNetify.DevApp.Docs.FromScratchWebPack.md");
  }

    public class FromScratchWebPackKO : BaseVM
  {
    public string Content => new Markdown("DotNetify.DevApp.Docs.FromScratchWebPack.Knockout.md");
  }
}