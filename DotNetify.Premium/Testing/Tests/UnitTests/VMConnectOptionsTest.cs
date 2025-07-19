using Xunit;

namespace DotNetify.Testing.UnitTests
{
   public class TestVM : BaseVM
   {
      public string Prop1 { get; set; }
   }

   public class VMConnectOptionsTest
   {
      [Fact]
      public void VMConnectOptions_SetProperties_ReturnsJObject()
      {
         var sut = new VMConnectOptions();
         sut.VMArg
            .Set("Prop1", "Hello")
            .Set("namespace", "DotNetify.Testing.UnitTests");

         var hubEmulator = new HubEmulatorBuilder()
            .Register<TestVM>()
            .Build();

         var client = hubEmulator.CreateClient();
         var response = client.Connect(nameof(TestVM), sut).As<dynamic>();
         Assert.Equal("Hello", (string) response.Prop1);
      }
   }
}