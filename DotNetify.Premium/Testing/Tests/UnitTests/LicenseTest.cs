using Xunit;

namespace DotNetify.Testing.UnitTests
{
   public class LicenseTest
   {
      [Fact]
      public void CheckLicense_ReturnsValidLicense()
      {
         License.CheckUsage();
         var key = License.CheckUsage();

         Assert.NotNull(key);
         Assert.Equal(uint.MaxValue, key.Usage);
      }
   }
}