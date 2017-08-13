using System.Security.Principal;
using DotNetify.Security;
using System.Threading;

namespace DotNetify
{
   internal class HubPrincipalAccessor : IPrincipalAccessor
   {
      private readonly static AsyncLocal<IPrincipal> _asyncLocal = new AsyncLocal<IPrincipal>();

      public IPrincipal Principal
      {
         get { return _asyncLocal.Value; }
         set { _asyncLocal.Value = value; }
      }
   }
}
