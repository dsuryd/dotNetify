using System.Security.Principal;
using DotNetify.Security;

namespace DotNetify
{
    internal class HubPrincipalAccessor : IPrincipalAccessor
    {
      public IPrincipal Principal { get; set; } = System.Threading.Thread.CurrentPrincipal;
    }
}
