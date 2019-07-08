using DotNetify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace UnitTests
{
   [TestClass]
   public class DotNetifyConfigurationTest
   {
      private class MyContractResolver : VMContractResolver
      {
      }

      [TestMethod]
      public void UseJsonSerializerSettings_OverridesCurrentSettings()
      {
         var currentSettings = VMSerializer.SerializerSettings;

         var config = new DotNetifyConfiguration();

         Assert.IsInstanceOfType(VMSerializer.SerializerSettings.ContractResolver, typeof(VMContractResolver));

         config.UseJsonSerializerSettings(new JsonSerializerSettings
         {
            ContractResolver = new MyContractResolver()
         });

         Assert.IsInstanceOfType(VMSerializer.SerializerSettings.ContractResolver, typeof(MyContractResolver));

         VMSerializer.SerializerSettings = currentSettings;
      }

      [TestMethod]
      public void UseJsonSerializerSettings_OverridesCurrentSettings_WithAccessToDefaultSettings()
      {
         var currentResolver = VMSerializer.SerializerSettings.ContractResolver;
         var config = new DotNetifyConfiguration();

         config.UseJsonSerializerSettings(settings =>
         {
            Assert.AreSame(settings.ContractResolver, VMSerializer.SerializerSettings.ContractResolver);
            Assert.IsInstanceOfType(settings.ContractResolver, typeof(VMContractResolver));

            settings.ContractResolver = new MyContractResolver();
         });

         Assert.IsInstanceOfType(VMSerializer.SerializerSettings.ContractResolver, typeof(MyContractResolver));
         VMSerializer.SerializerSettings.ContractResolver = currentResolver;
      }
   }
}