using System.Security.Principal;

namespace DotNetify.Security
{
   /// <summary>
   /// Provides current principal.
   /// </summary>
    public interface IPrincipalAccessor
    {
      IPrincipal Principal { get; }
    }
}
