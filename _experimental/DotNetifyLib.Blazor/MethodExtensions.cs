using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;

namespace DotNetify.Blazor
{
   public static class MethodExtensions
   {
      public static IServiceCollection UseDotNetifyBlazor(this IServiceCollection services)
      {
         services.AddTransient(typeof(IVMProxy), typeof(VMProxy));
         return services;
      }

      internal static T As<T>(this object arg) => arg.As(s => JsonConvert.DeserializeObject<T>(s));

      internal static T As<T>(this object arg, JsonSerializerSettings settings) => arg.As(s => JsonConvert.DeserializeObject<T>(s, settings));

      internal static T As<T>(this object arg, params JsonConverter[] converters) => arg.As(s => JsonConvert.DeserializeObject<T>(s, converters));

      internal static T As<T>(this object arg, Func<string, T> deserialize)
      {
         if (typeof(T) == typeof(object))
            return (T) arg;

         try
         {
            return typeof(T) == typeof(string) ? (T) (object) $"{arg}" : deserialize($"{arg}");
         }
         catch (Exception ex)
         {
            throw new JsonSerializationException($"Cannot deserialize {arg} to {typeof(T)}", ex);
         }
      }

      /// <summary>
      /// Converts a boolean value to its string literal "true" or "false".
      /// </summary>
      public static string ToBoolString(this bool arg) => arg.ToString().ToLower();
   }
}