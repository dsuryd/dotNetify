using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace WebApplication.Core.React
{
   public static class EmbeddedResource
    {
      public static string GetEmbeddedResource(this object caller, string resourceName)
      {
         var assembly = caller.GetType().GetTypeInfo().Assembly;
         var name = assembly.GetManifestResourceNames().Where(i => i.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
         if (string.IsNullOrEmpty(name))
            throw new FileNotFoundException();

         using (var reader = new StreamReader(assembly.GetManifestResourceStream(name), Encoding.UTF8))
            return reader.ReadToEnd();
      }
    }
}
