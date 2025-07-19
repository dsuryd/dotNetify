using Xunit;

namespace DotNetify.Testing.UnitTests
{
   public class FactDebugAttribute : FactAttribute
   {
      public FactDebugAttribute()
      {
#if !DEBUG
         Skip = "Only running in debug mode.";
#endif
      }
   }
}