using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevApp.ViewModels
{
   public static class Utils
   {
      public static async Task<string> GetResource(string resourceName, Assembly assembly = null)
      {
         assembly = assembly ?? Assembly.GetCallingAssembly();
         var resourceStream = assembly.GetManifestResourceStream(resourceName);
         if (resourceStream == null)
            throw new FileNotFoundException($"'{resourceName}' is not an embedded resource", resourceName);

         using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
         {
            return await reader.ReadToEndAsync();
         }
      }
   }
}